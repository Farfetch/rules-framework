namespace Rules.Framework.Rql
{
    using System;
    using System.Linq;
    using System.Text;
    using Rules.Framework.Rql.Ast;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;

    internal class ReverseRqlBuilder : IReverseRqlBuilder, IExpressionVisitor<string>, ISegmentVisitor<string>, IStatementVisitor<string>
    {
        private const char SPACE = ' ';

        public string BuildRql(IAstElement astElement)
        {
            if (astElement is null)
            {
                throw new ArgumentNullException(nameof(astElement));
            }

            return astElement switch
            {
                Expression expression => expression.Accept(this),
                Segment segment => segment.Accept(this),
                Statement statement => statement.Accept(this),
                _ => throw new NotSupportedException($"The given AST element is not supported: {astElement.GetType().FullName}."),
            };
        }

        public string BuildRql(Statement statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            return statement.Accept(this);
        }

        public string VisitActivationExpression(ActivationExpression activationExpression)
        {
            var ruleName = activationExpression.RuleName.Accept(this);
            var contentType = activationExpression.ContentType.Accept(this);
            return $"ACTIVATE {ruleName} {contentType}";
        }

        public string VisitAssignExpression(AssignmentExpression expression)
            => FormattableString.Invariant($"{expression.Left.Accept(this)} {expression.Assign.Lexeme} {expression.Right.Accept(this)}");

        public string VisitBlockStatement(BlockStatement blockStatement)
        {
            var stringBuilder = new StringBuilder(blockStatement.BeginBrace.Lexeme)
                .AppendLine();
            var statements = blockStatement.Statements;
            var statementsLength = statements.Length;
            for (int i = 0; i < statementsLength; i++)
            {
                var statementRql = statements[i].Accept(this);
                statementRql = statementRql.Replace("\n", $"\n{new string(SPACE, 4)}", StringComparison.Ordinal);
                stringBuilder.AppendLine(statementRql);
            }

            return stringBuilder.Append(blockStatement.EndBrace.Lexeme)
                .ToString();
        }

        public string VisitCallExpression(CallExpression callExpression)
        {
            var stringBuilder = new StringBuilder(callExpression.Name.Accept(this))
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

        public string VisitCardinalitySegment(CardinalitySegment expression)
            => $"{expression.CardinalityKeyword.Accept(this)} {expression.RuleKeyword.Accept(this)}";

        public string VisitComposedConditionSegment(ComposedConditionSegment expression)
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

        public string VisitConditionGroupingSegment(ConditionGroupingSegment expression)
            => $"({expression.RootCondition.Accept(this)})";

        public string VisitCreateExpression(CreateExpression createExpression)
        {
            var stringBuilder = new StringBuilder("CREATE RULE")
                .Append(SPACE)
                .Append(createExpression.RuleName.Accept(this))
                .Append(SPACE)
                .Append("AS")
                .Append(SPACE)
                .Append(createExpression.ContentType.Accept(this))
                .Append(SPACE)
                .Append("WITH CONTENT")
                .Append(SPACE)
                .Append(createExpression.Content.Accept(this))
                .Append(SPACE)
                .Append("BEGINS ON")
                .Append(SPACE)
                .Append(createExpression.DateBegin.Accept(this));

            if (createExpression.DateEnd != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("ENDS ON")
                    .Append(SPACE)
                    .Append(createExpression.DateEnd.Accept(this));
            }

            if (createExpression.Condition != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("APPLY WHEN")
                    .Append(SPACE)
                    .Append(createExpression.Condition.Accept(this));
            }

            if (createExpression.PriorityOption != null)
            {
                stringBuilder.Append(SPACE)
                    .Append("SET")
                    .Append(SPACE)
                    .Append(createExpression.PriorityOption.Accept(this));
            }

            return stringBuilder.ToString();
        }

        public string VisitDateEndSegment(DateEndSegment dateEndSegment) => dateEndSegment.Accept(this);

        public string VisitDeactivationExpression(DeactivationExpression deactivationExpression)
        {
            var ruleName = deactivationExpression.RuleName.Accept(this);
            var contentType = deactivationExpression.ContentType.Accept(this);
            return $"DEACTIVATE {ruleName} {contentType}";
        }

        public string VisitExpressionStatement(ExpressionStatement programmableStatement)
            => $"{programmableStatement.Expression.Accept(this)};";

        public string VisitIdentifierExpression(IdentifierExpression identifierExpression) => identifierExpression.Identifier.Lexeme;

        public string VisitIndexerGetExpression(IndexerGetExpression indexerGetExpression)
                            => $"{indexerGetExpression.Instance.Accept(this)}{indexerGetExpression.IndexLeftDelimeter.Lexeme}{indexerGetExpression.Index.Accept(this)}{indexerGetExpression.IndexRightDelimeter.Lexeme}";

        public string VisitIndexerSetExpression(IndexerSetExpression indexerSetExpression)
            => $"{indexerSetExpression.Instance.Accept(this)}{indexerSetExpression.IndexLeftDelimeter.Lexeme}{indexerSetExpression.Index.Accept(this)}{indexerSetExpression.IndexRightDelimeter.Lexeme} {indexerSetExpression.Assign.Lexeme} {indexerSetExpression.Value.Accept(this)}";

        public string VisitInputConditionSegment(InputConditionSegment inputConditionExpression)
        {
            var left = inputConditionExpression.Left.Accept(this);
            var @operator = inputConditionExpression.Operator.Lexeme;
            var right = inputConditionExpression.Right.Accept(this);
            return $"{left} {@operator} {right}";
        }

        public string VisitInputConditionsSegment(InputConditionsSegment inputConditionsExpression)
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

        public string VisitKeywordExpression(KeywordExpression keywordExpression) => keywordExpression.Keyword.Lexeme.ToUpperInvariant();

        public string VisitLiteralExpression(LiteralExpression literalExpression) => literalExpression.Type switch
        {
            LiteralType.String or LiteralType.Undefined => literalExpression.Token.Lexeme,
            LiteralType.Decimal or LiteralType.Integer or LiteralType.Bool => literalExpression.Value.ToString(),
            LiteralType.DateTime => $"${literalExpression.Value:yyyy-MM-ddTHH:mm:ssZ}$",
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

        public string VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            var stringBuilder = new StringBuilder(newArrayExpression.Array.Lexeme)
                .Append(SPACE)
                .Append(newArrayExpression.InitializerBeginToken.Lexeme);

            if (newArrayExpression.Size == Expression.None)
            {
                stringBuilder.Append(newArrayExpression.Size.Accept(this));
            }
            else
            {
                for (int i = 0; i < newArrayExpression.Values.Length; i++)
                {
                    stringBuilder.Append(SPACE)
                        .Append(newArrayExpression.Values[i].Accept(this));

                    if (i < newArrayExpression.Values.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
            }

            return stringBuilder.Append(SPACE)
                .Append(newArrayExpression.InitializerEndToken.Lexeme)
                .ToString();
        }

        public string VisitNewObjectExpression(NewObjectExpression newObjectExpression)
        {
            var stringBuilder = new StringBuilder(newObjectExpression.Object.Lexeme);

            if (newObjectExpression.PropertyAssignments.Length > 0)
            {
                stringBuilder.AppendLine()
                    .Append('{');
                for (int i = 0; i < newObjectExpression.PropertyAssignments.Length; i++)
                {
                    stringBuilder.AppendLine()
                        .Append(new string(' ', 4))
                        .Append(newObjectExpression.PropertyAssignments[i].Accept(this));

                    if (i < newObjectExpression.PropertyAssignments.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
                stringBuilder.AppendLine()
                    .Append('}');
            }

            return stringBuilder.ToString();
        }

        public string VisitNoneExpression(NoneExpression noneExpression) => string.Empty;

        public string VisitNoneSegment(NoneSegment noneSegment) => string.Empty;

        public string VisitNoneStatement(NoneStatement noneStatement) => string.Empty;

        public string VisitOperatorSegment(OperatorSegment operatorExpression) => operatorExpression.Token.Lexeme;

        public string VisitPlaceholderExpression(PlaceholderExpression placeholderExpression) => placeholderExpression.Token.Lexeme;

        public string VisitPriorityOptionSegment(PriorityOptionSegment priorityOptionExpression)
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

        public string VisitPropertyGetExpression(PropertyGetExpression propertyGetExpression)
            => FormattableString.Invariant($"{propertyGetExpression.Instance.Accept(this)}.{propertyGetExpression.Name.Accept(this)}");

        public string VisitPropertySetExpression(PropertySetExpression propertySetExpression)
            => $"{propertySetExpression.Instance.Accept(this)}.{propertySetExpression.Name.Accept(this)} {propertySetExpression.Assign.Lexeme} {propertySetExpression.Value.Accept(this)}";

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
                .Append("BEGINS ON")
                .Append(SPACE)
                .Append(dateBegin)
                .Append(SPACE)
                .Append("ENDS ON")
                .Append(SPACE)
                .Append(dateEnd);

            if (!string.IsNullOrWhiteSpace(inputConditions))
            {
                searchRqlBuilder.Append(SPACE)
                    .Append(inputConditions);
            }

            return searchRqlBuilder.ToString();
        }

        public string VisitUnaryExpression(UnaryExpression expression) => $"{expression.Operator.Lexeme}{expression.Right.Accept(this)}";

        public string VisitUpdatableAttributeSegment(UpdatableAttributeSegment updatableExpression) => updatableExpression.UpdatableAttribute.Accept(this);

        public string VisitUpdateExpression(UpdateExpression updateExpression)
        {
            var ruleName = updateExpression.RuleName.Accept(this);
            var contentType = updateExpression.ContentType.Accept(this);
            var stringBuilder = new StringBuilder("UPDATE RULE ")
                .Append(ruleName)
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE);

            for (int i = 0; i < updateExpression.UpdatableAttributes.Length; i++)
            {
                var updatableAttribute = updateExpression.UpdatableAttributes[i].Accept(this);
                stringBuilder.Append("SET")
                    .Append(SPACE)
                    .Append(updatableAttribute);

                if (i < updateExpression.UpdatableAttributes.Length - 1)
                {
                    stringBuilder.Append(',')
                        .Append(SPACE);
                }
            }

            return stringBuilder.Append(';')
                .ToString();
        }

        public string VisitValueConditionSegment(ValueConditionSegment valueConditionExpression)
            => $"{valueConditionExpression.Left.Accept(this)} {valueConditionExpression.Operator.Accept(this)} {valueConditionExpression.Right.Accept(this)}";

        public string VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
        {
            var stringBuilder = new StringBuilder(variableDeclarationStatement.Keyword.Lexeme)
                .Append(SPACE)
                .Append(variableDeclarationStatement.Name.Accept(this));

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

        public string VisitVariableExpression(VariableExpression variableExpression) => variableExpression.Name.Accept(this);
    }
}