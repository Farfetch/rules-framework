namespace Rules.Framework.Rql
{
    using System;
    using System.Linq;
    using System.Text;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;

    internal class ReverseRqlBuilder : IReverseRqlBuilder, IExpressionVisitor<string>, IStatementVisitor<string>
    {
        private const char SPACE = ' ';

        public string BuildRql(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Accept(this);
        }

        public string BuildRql(Statement statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            return statement.Accept(this);
        }

        public string VisitActivationStatement(ActivationStatement activationStatement)
        {
            var ruleName = activationStatement.RuleName.Accept(this);
            var contentType = activationStatement.ContentType.Accept(this);
            return $"ACTIVATE {ruleName} {contentType}";
        }

        public string VisitCallExpression(CallExpression callExpression)
        {
            var stringBuilder = new StringBuilder(callExpression.Identifier.Lexeme.ToUpperInvariant())
                .Append('(');

            int argumentsLength = callExpression.Arguments.Length;
            for (int i = 0; i < argumentsLength; i++)
            {
                var argument = callExpression.Arguments[i].Accept(this);
                stringBuilder.Append(argument);

                if (i < argumentsLength - 1)
                {
                    stringBuilder.Append(',');
                }
            }

            return stringBuilder.Append(')')
                .ToString();
        }

        public string VisitCardinalityExpression(CardinalityExpression expression)
            => $"{expression.CardinalityKeyword.Accept(this)} {expression.RuleKeyword.Accept(this)}";

        public string VisitComposedConditionExpression(ComposedConditionExpression expression)
        {
            var logicalOperator = expression.LogicalOperator.Accept(this);
            var stringBuilder = new StringBuilder();
            var first = true;
            foreach (var childCondition in expression.ChildConditions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.Append(SPACE)
                        .Append(logicalOperator)
                        .Append(SPACE);
                }

                stringBuilder.Append(childCondition.Accept(this));
            }

            return stringBuilder.ToString();
        }

        public string VisitConditionGroupingExpression(ConditionGroupingExpression expression)
            => $"({expression.RootCondition.Accept(this)})";

        public string VisitCreateStatement(CreateStatement createStatement)
        {
            var stringBuilder = new StringBuilder("CREATE RULE")
                .Append(SPACE)
                .Append(createStatement.RuleName.Accept(this))
                .Append(SPACE)
                .Append("AS")
                .Append(SPACE)
                .Append(createStatement.ContentType.Accept(this))
                .Append(SPACE)
                .Append("WITH CONTENT")
                .Append(SPACE)
                .Append(createStatement.Content.Accept(this))
                .Append(SPACE)
                .Append("BEGINS ON")
                .Append(SPACE)
                .Append(createStatement.DateBegin.Accept(this));

            if (createStatement.DateEnd != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("ENDS ON")
                    .Append(SPACE)
                    .Append(createStatement.DateEnd.Accept(this));
            }

            if (createStatement.Condition != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("APPLY WHEN")
                    .Append(SPACE)
                    .Append(createStatement.Condition.Accept(this));
            }

            if (createStatement.PriorityOption != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("SET")
                    .Append(SPACE)
                    .Append(createStatement.PriorityOption.Accept(this));
            }

            return stringBuilder.ToString();
        }

        public string VisitDeactivationStatement(DeactivationStatement deactivationStatement)
        {
            var ruleName = deactivationStatement.RuleName.Accept(this);
            var contentType = deactivationStatement.ContentType.Accept(this);
            return $"DEACTIVATE {ruleName} {contentType}";
        }

        public string VisitDefinitionStatement(RuleDefinitionStatement definitionStatement) => $"{definitionStatement.Definition.Accept(this)};";

        public string VisitInputConditionExpression(InputConditionExpression inputConditionExpression)
        {
            var left = inputConditionExpression.Left.Accept(this);
            var @operator = inputConditionExpression.Operator.Lexeme;
            var right = inputConditionExpression.Right.Accept(this);
            return $"{left} {@operator} {right}";
        }

        public string VisitInputConditionsExpression(InputConditionsExpression inputConditionsExpression)
        {
            var inputConditionsRqlBuilder = new StringBuilder();
            if (inputConditionsExpression.InputConditions.Any())
            {
                inputConditionsRqlBuilder.Append("WITH {");

                var notFirst = false;
                foreach (var inputConditionExpression in inputConditionsExpression.InputConditions)
                {
                    if (notFirst)
                    {
                        inputConditionsRqlBuilder.Append(',');
                    }
                    else
                    {
                        notFirst = true;
                    }

                    var inputCondition = inputConditionExpression.Accept(this);
                    inputConditionsRqlBuilder.Append(SPACE)
                        .Append(inputCondition);
                }

                inputConditionsRqlBuilder.Append(SPACE)
                    .Append('}');
            }

            return inputConditionsRqlBuilder.ToString();
        }

        public string VisitKeywordExpression(KeywordExpression keywordExpression) => keywordExpression.Keyword.Lexeme;

        public string VisitLiteralExpression(LiteralExpression literalExpression) => literalExpression.Type switch
        {
            LiteralType.String => literalExpression.Token.Lexeme,
            LiteralType.Decimal or LiteralType.Integer or LiteralType.Bool => literalExpression.Value.ToString(),
            LiteralType.DateTime => $"\"{literalExpression.Value:yyyy-MM-ddTHH:mm:ssZ}\"",
            _ => throw new NotSupportedException($"The literal type '{literalExpression.Type}' is not supported."),
        };

        public string VisitMatchExpression(MatchExpression matchExpression)
        {
            var cardinality = matchExpression.Cardinality.Accept(this);
            var contentType = matchExpression.ContentType.Accept(this);
            var matchDate = matchExpression.MatchDate.Accept(this);
            var inputConditions = matchExpression.InputConditions.Accept(this);

            var matchRqlBuilder = new StringBuilder("MATCH")
                .Append(SPACE)
                .Append(cardinality)
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE)
                .Append("ON")
                .Append(SPACE)
                .Append(matchDate);

            if (!string.IsNullOrWhiteSpace(inputConditions))
            {
                matchRqlBuilder.Append(SPACE)
                    .Append(inputConditions);
            }

            return matchRqlBuilder.ToString();
        }

        public string VisitNoneExpression(NoneExpression noneExpression) => string.Empty;

        public string VisitNoneStatement(NoneStatement noneStatement) => string.Empty;

        public string VisitOperatorExpression(OperatorExpression operatorExpression) => operatorExpression.Token.Lexeme;

        public string VisitPlaceholderExpression(PlaceholderExpression placeholderExpression) => placeholderExpression.Token.Lexeme;

        public string VisitPriorityOptionExpression(PriorityOptionExpression priorityOptionExpression)
        {
            var stringBuilder = new StringBuilder("PRIORITY ");
            var priorityOption = priorityOptionExpression.PriorityOption.Accept(this);

            switch (priorityOption)
            {
                case "TOP":
                case "BOTTOM":
                    stringBuilder.Append(priorityOption);
                    break;

                case "NAME":
                    var ruleName = (string)priorityOptionExpression.Argument.Accept(this);
                    stringBuilder.Append(SPACE)
                        .Append("RULE NAME")
                        .Append(SPACE)
                        .Append(ruleName);
                    break;

                case "NUMBER":
                    var priorityValue = priorityOptionExpression.Argument.Accept(this);
                    stringBuilder.Append(SPACE)
                        .Append("NUMBER")
                        .Append(SPACE)
                        .Append(priorityValue);
                    break;

                default:
                    throw new NotSupportedException($"The priority option '{priorityOption}' is not supported.");
            }

            return stringBuilder.ToString();
        }

        public string VisitProgrammableSubLanguageStatement(ProgrammableSubLanguageStatement programmableStatement)
            => $"{programmableStatement.Expression.Accept(this)};";

        public string VisitQueryStatement(RuleQueryStatement matchStatement) => $"{matchStatement.Query.Accept(this)};";

        public string VisitSearchExpression(SearchExpression searchExpression)
        {
            var contentType = searchExpression.ContentType.Accept(this);
            var dateBegin = searchExpression.DateBegin.Accept(this);
            var dateEnd = searchExpression.DateEnd.Accept(this);
            var inputConditions = searchExpression.InputConditions.Accept(this);

            var searchRqlBuilder = new StringBuilder("SEARCH RULES")
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE)
                .Append("ON RANGE")
                .Append(SPACE)
                .Append(dateBegin)
                .Append(SPACE)
                .Append("TO")
                .Append(SPACE)
                .Append(dateEnd);

            if (!string.IsNullOrWhiteSpace(inputConditions))
            {
                searchRqlBuilder.Append(SPACE)
                    .Append(inputConditions);
            }

            return searchRqlBuilder.ToString();
        }

        public string VisitUpdatableAttributeExpression(UpdatableAttributeExpression updatableExpression) => updatableExpression.UpdatableAttribute.Accept(this);

        public string VisitUpdateStatement(UpdateStatement updateStatement)
        {
            var ruleName = updateStatement.RuleName.Accept(this);
            var contentType = updateStatement.ContentType.Accept(this);
            var stringBuilder = new StringBuilder("UPDATE RULE ")
                .Append(ruleName)
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE);

            for (int i = 0; i < updateStatement.UpdatableAttributes.Length; i++)
            {
                var updatableAttribute = updateStatement.UpdatableAttributes[i].Accept(this);
                stringBuilder.Append("SET")
                    .Append(SPACE)
                    .Append(updatableAttribute);

                if (i < updateStatement.UpdatableAttributes.Length - 1)
                {
                    stringBuilder.Append(',')
                        .Append(SPACE);
                }
            }

            return stringBuilder.Append(';')
                .ToString();
        }

        public string VisitValueConditionExpression(ValueConditionExpression valueConditionExpression)
            => $"{valueConditionExpression.Left.Accept(this)} {valueConditionExpression.Operator.Accept(this)} {valueConditionExpression.Right.Accept(this)}";

        public string VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
        {
            var stringBuilder = new StringBuilder(variableDeclarationStatement.Keyword.Lexeme)
                .Append(SPACE)
                .Append(variableDeclarationStatement.Name.Lexeme);

            if (variableDeclarationStatement.Assignable != Expression.None)
            {
                stringBuilder.Append(SPACE)
                    .Append('=')
                    .Append(SPACE)
                    .Append(variableDeclarationStatement.Assignable.Accept(this));
            }

            return stringBuilder.Append(';')
                .ToString();
        }

        public string VisitVariableExpression(VariableExpression variableExpression) => variableExpression.Token.Lexeme;
    }
}