namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class ExpressionConfiguration
    {
        public string ExpressionName { get; set; }

        public IEnumerable<Expression> Expressions { get; set; }

        public IReadOnlyDictionary<string, LabelTarget> LabelTargets { get; set; }

        public IReadOnlyDictionary<string, ParameterExpression> Parameters { get; set; }

        public object ReturnDefaultValue { get; set; }

        public LabelTarget ReturnLabelTarget { get; set; }

        public Type ReturnType { get; set; }

        public IReadOnlyDictionary<string, ParameterExpression> Variables { get; set; }
    }
}