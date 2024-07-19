namespace Rules.Framework.Generic
{
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Core;
    using Rules.Framework.Generic.ConditionNodes;

    internal static class RuleExtensions
    {
        public static IConditionNode<TConditionType> ToGenericConditionNode<TConditionType>(this IConditionNode rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = (ValueConditionNode)rootCondition;

                return new ValueConditionNode<TConditionType>(condition);
            }

            var composedConditionNode = (ComposedConditionNode)rootCondition;
            return new ComposedConditionNode<TConditionType>(composedConditionNode);
        }

        public static Rule<TContentType, TConditionType> ToGenericRule<TContentType, TConditionType>(this Rule rule) => new(rule);
    }
}