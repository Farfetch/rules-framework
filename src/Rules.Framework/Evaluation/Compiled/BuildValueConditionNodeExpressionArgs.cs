namespace Rules.Framework.Evaluation.Compiled
{
    using System.Linq.Expressions;
    using Rules.Framework.Core;

    internal struct BuildValueConditionNodeExpressionArgs
    {
        public DataTypeConfiguration DataTypeConfiguration { get; set; }

        public ParameterExpression LeftOperandVariableExpression { get; set; }

        public Operators Operator { get; set; }

        public ParameterExpression ResultVariableExpression { get; set; }

        public ParameterExpression RightOperandVariableExpression { get; set; }
    }
}