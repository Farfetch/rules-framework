namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework;

    internal interface IConditionExpressionBuilderProvider
    {
        IConditionExpressionBuilder GetConditionExpressionBuilderFor(Operators @operator, string multiplicity);
    }
}
