namespace Rules.Framework.Evaluation.Compiled.ConditionBuilders
{
    using Rules.Framework.Core;

    internal interface IConditionExpressionBuilderProvider
    {
        IConditionExpressionBuilder GetConditionExpressionBuilderFor(Operators @operator, string multiplicity);
    }
}
