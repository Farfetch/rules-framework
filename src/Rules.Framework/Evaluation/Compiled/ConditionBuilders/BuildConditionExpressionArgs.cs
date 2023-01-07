namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;

    internal struct BuildConditionExpressionArgs
    {
        public DataTypeConfiguration DataTypeConfiguration { get; set; }

        public Expression LeftHandOperand { get; set; }

        public Expression RightHandOperand { get; set; }
    }
}