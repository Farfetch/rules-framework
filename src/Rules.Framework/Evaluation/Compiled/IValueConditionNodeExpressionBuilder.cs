namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;

    internal interface IValueConditionNodeExpressionBuilder
    {
        void Build(
            IExpressionBlockBuilder builder,
            BuildValueConditionNodeExpressionArgs args);
    }
}