namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal interface IValueConditionNodeExpressionBuilder
    {
        void Build(
            IImplementationExpressionBuilder builder,
            BuildValueConditionNodeExpressionArgs args);
    }
}