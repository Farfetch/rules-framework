namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionConfiguration
    {
        public string ExpressionName { get; set; }

        public IEnumerable<Expression> Expressions { get; set; }

        public IDictionary<string, LabelTarget> LabelTargets { get; set; }

        public IDictionary<string, ParameterExpression> Parameters { get; set; }

        public object ReturnDefaultValue { get; set; }

        public LabelTarget ReturnLabelTarget { get; set; }

        public Type ReturnType { get; set; }

        public IDictionary<string, ParameterExpression> Variables { get; set; }
    }
}