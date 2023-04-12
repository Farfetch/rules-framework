namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal interface IExpressionSwitchBuilder
    {
        Expression DefaultBody { get; }

        IEnumerable<SwitchCase> SwitchCases { get; }

        IExpressionSwitchBuilder Case(
            Expression caseExpression,
            Func<IExpressionBlockBuilder, Expression> caseBodyExpressionBuilder);

        IExpressionSwitchBuilder Case(
            IEnumerable<Expression> caseExpressions,
            Func<IExpressionBlockBuilder, Expression> caseBodyExpressionBuilder);

        void Default(Func<IExpressionBlockBuilder, Expression> defaultBodyExpressionBuilder);
    }
}