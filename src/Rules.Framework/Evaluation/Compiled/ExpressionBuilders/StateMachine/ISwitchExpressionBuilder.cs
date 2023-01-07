namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal interface ISwitchExpressionBuilder
    {
        ISwitchExpressionBuilder Case(
            Expression caseExpression,
            Func<IImplementationExpressionBuilder, Expression> caseBodyExpressionBuilder);

        ISwitchExpressionBuilder Case(
            IEnumerable<Expression> caseExpressions,
            Func<IImplementationExpressionBuilder, Expression> caseBodyExpressionBuilder);

        void Default(Func<IImplementationExpressionBuilder, Expression> defaultBodyExpressionBuilder);
    }
}