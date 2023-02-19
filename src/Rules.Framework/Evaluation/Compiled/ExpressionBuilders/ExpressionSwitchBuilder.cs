namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionSwitchBuilder : IExpressionSwitchBuilder
    {
        private readonly IExpressionBlockBuilder implementationExpressionBuilder;
        private readonly List<SwitchCase> switchCases;
        private Expression defaultBody;

        public ExpressionSwitchBuilder(IExpressionBlockBuilder implementationExpressionBuilder)
        {
            this.defaultBody = null;
            this.implementationExpressionBuilder = implementationExpressionBuilder;
            this.switchCases = new List<SwitchCase>();
        }

        public Expression DefaultBody => this.defaultBody;

        public IEnumerable<SwitchCase> SwitchCases => this.switchCases.AsReadOnly();

        public IExpressionSwitchBuilder Case(
            Expression caseExpression,
            Func<IExpressionBlockBuilder, Expression> caseBodyExpressionBuilder)
        {
            if (caseExpression is null)
            {
                throw new ArgumentNullException(nameof(caseExpression));
            }

            return this.Case(new[] { caseExpression }, caseBodyExpressionBuilder);
        }

        public IExpressionSwitchBuilder Case(
            IEnumerable<Expression> caseExpressions,
            Func<IExpressionBlockBuilder, Expression> caseBodyExpressionBuilder)
        {
            if (caseExpressions is null)
            {
                throw new ArgumentNullException(nameof(caseExpressions));
            }

            if (caseBodyExpressionBuilder is null)
            {
                throw new ArgumentNullException(nameof(caseBodyExpressionBuilder));
            }

            var expression = caseBodyExpressionBuilder.Invoke(this.implementationExpressionBuilder);
            var switchCaseExpression = Expression.SwitchCase(expression, caseExpressions);
            this.switchCases.Add(switchCaseExpression);
            return this;
        }

        public void Default(Func<IExpressionBlockBuilder, Expression> defaultBodyExpressionBuilder)
        {
            if (defaultBodyExpressionBuilder is null)
            {
                throw new ArgumentNullException(nameof(defaultBodyExpressionBuilder));
            }

            defaultBody = defaultBodyExpressionBuilder.Invoke(this.implementationExpressionBuilder);
        }
    }
}