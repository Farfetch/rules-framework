namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class SwitchExpressionBuilder : ISwitchExpressionBuilder
    {
        private readonly IImplementationExpressionBuilder implementationExpressionBuilder;
        private readonly List<SwitchCase> switchCases;
        private Expression defaultExpression;

        public SwitchExpressionBuilder(IImplementationExpressionBuilder implementationExpressionBuilder)
        {
            this.defaultExpression = null;
            this.implementationExpressionBuilder = implementationExpressionBuilder;
            this.switchCases = new List<SwitchCase>();
        }

        public ISwitchExpressionBuilder Case(
            Expression caseExpression,
            Func<IImplementationExpressionBuilder, Expression> caseBodyExpressionBuilder)
        {
            if (caseExpression is null)
            {
                throw new ArgumentNullException(nameof(caseExpression));
            }

            return this.Case(new[] { caseExpression }, caseBodyExpressionBuilder);
        }

        public ISwitchExpressionBuilder Case(
            IEnumerable<Expression> caseExpressions,
            Func<IImplementationExpressionBuilder, Expression> caseBodyExpressionBuilder)
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

        public SwitchExpression CreateSwitchExpression(Expression switchValueExpression)
            => Expression.Switch(switchValueExpression, this.defaultExpression, this.switchCases.ToArray());

        public void Default(Func<IImplementationExpressionBuilder, Expression> defaultBodyExpressionBuilder)
        {
            if (defaultBodyExpressionBuilder is null)
            {
                throw new ArgumentNullException(nameof(defaultBodyExpressionBuilder));
            }

            defaultExpression = defaultBodyExpressionBuilder.Invoke(this.implementationExpressionBuilder);
        }
    }
}