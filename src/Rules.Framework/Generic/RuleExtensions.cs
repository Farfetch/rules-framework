namespace Rules.Framework.Generic
{
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Generic.ConditionNodes;

    internal static class RuleExtensions
    {
        public static IConditionNode<TCondition> ToGenericConditionNode<TCondition>(this IConditionNode rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = (ValueConditionNode)rootCondition;

                return new ValueConditionNode<TCondition>(condition);
            }

            var composedConditionNode = (ComposedConditionNode)rootCondition;
            return new ComposedConditionNode<TCondition>(composedConditionNode);
        }

        public static Rule<TRuleset, TCondition> ToGenericRule<TRuleset, TCondition>(this Rule rule) => new(rule);
    }
}