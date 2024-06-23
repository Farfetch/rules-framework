namespace Rules.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Generics;

    internal static class RuleExtensions
    {
        public static IConditionNode<TConditionType> ToConcreteConditionNode<TConditionType>(this IConditionNode<string> rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = (ValueConditionNode<string>)rootCondition;

                return new ValueConditionNode<TConditionType>(
                    condition.DataType,
                    condition.ConditionType!.ToConcreteConditionType<TConditionType>(),
                    condition.Operator,
                    condition.Operand,
                    condition.Properties);
            }

            var composedConditionNode = (ComposedConditionNode<string>)rootCondition;

            var childConditionNodes = new List<IConditionNode<TConditionType>>(composedConditionNode.ChildConditionNodes.Count());

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                childConditionNodes.Add(child.ToConcreteConditionNode<TConditionType>());
            }

            return new ComposedConditionNode<TConditionType>(composedConditionNode.LogicalOperator, childConditionNodes);
        }

        public static TContentType ToConcreteConditionType<TContentType>(this string contentType)
        {
            return (TContentType)Enum.Parse(typeof(TContentType), contentType);
        }

        public static TConditionType ToConcreteContentType<TConditionType>(this string conditionType)
        {
            return (TConditionType)Enum.Parse(typeof(TConditionType), conditionType);
        }

        public static Rule<TContentType, TConditionType> ToConcreteRule<TContentType, TConditionType>(this Rule<string, string> rule)
        {
            return new Rule<TContentType, TConditionType>
            {
                RootCondition = rule.RootCondition?.ToConcreteConditionNode<TConditionType>()!,
                ContentContainer = new ContentContainer<TContentType>(
                    rule.ContentContainer.ContentType.ToConcreteContentType<TContentType>(),
                    (_) => rule.ContentContainer.GetContentAs<dynamic>()),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                Active = rule.Active,
            };
        }

        public static IConditionNode<string> ToGenericConditionNode<TConditionType>(this IConditionNode<TConditionType> rootCondition)
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = (ValueConditionNode<TConditionType>)rootCondition;

                return new ValueConditionNode<string>(
                    condition.DataType,
                    condition.ConditionType!.ToString(),
                    condition.Operator,
                    condition.Operand,
                    condition.Properties);
            }

            var composedConditionNode = (ComposedConditionNode<TConditionType>)rootCondition;

            var childConditionNodes = new List<IConditionNode<string>>(composedConditionNode.ChildConditionNodes.Count());

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                childConditionNodes.Add(child.ToGenericConditionNode());
            }

            return new ComposedConditionNode<string>(composedConditionNode.LogicalOperator, childConditionNodes);
        }

        public static Rule<string, string> ToGenericRule<TContentType, TConditionType>(this Rule<TContentType, TConditionType> rule)
        {
            return new Rule<string, string>
            {
                RootCondition = rule.RootCondition?.ToGenericConditionNode()!,
                ContentContainer = new ContentContainer<string>(
                    rule.ContentContainer.ContentType!.ToString(),
                    (_) => rule.ContentContainer.GetContentAs<dynamic>()),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                Active = rule.Active,
            };
        }
    }
}