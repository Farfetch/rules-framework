namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal interface IExpressionBuilderFactory
    {
        IExpressionBlockBuilder CreateExpressionBlockBuilder(
            string scopeName,
            IExpressionBlockBuilder parent,
            ExpressionConfiguration expressionConfiguration);

        IExpressionParametersBuilder CreateExpressionBuilder(
            ExpressionConfiguration expressionConfiguration);
        IExpressionParametersConfiguration CreateExpressionParametersConfiguration();
        IExpressionSwitchBuilder CreateExpressionSwitchBuilder(
            IExpressionBlockBuilder parent);
    }
}