namespace Rules.Framework.Evaluation.Compiled
{
    internal interface IValueConditionNodeExpressionBuilderProvider
    {
        IValueConditionNodeExpressionBuilder GetExpressionBuilder(string multiplicity);
    }
}