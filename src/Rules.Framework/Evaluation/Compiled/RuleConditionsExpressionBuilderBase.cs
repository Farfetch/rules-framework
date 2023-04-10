namespace Rules.Framework.Evaluation.Compiled
{
    using System.Reflection;

    internal class RuleConditionsExpressionBuilderBase
    {
        protected static readonly MethodInfo multiplicityEvaluateMethod = typeof(MultiplicityEvaluator)
            .GetMethod(nameof(MultiplicityEvaluator.Evaluate));

        protected RuleConditionsExpressionBuilderBase()
        {
        }
    }
}