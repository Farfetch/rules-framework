namespace Rules.Framework.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.InMemory.DataModel;

    internal sealed class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> CreateRule(RuleDataModel<TContentType, TConditionType> ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            var contentContainer = new ContentContainer<TContentType>(ruleDataModel.ContentType, (_) => ruleDataModel.Content);
            var ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithCondition(cnb => ruleDataModel.RootCondition is { } ? ConvertConditionNode(cnb, ruleDataModel.RootCondition) : null)
                .WithContentContainer(contentContainer)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}", ruleBuilderResult.Errors);
            }

            ruleBuilderResult.Rule.Priority = ruleDataModel.Priority;

            if (ruleBuilderResult.Rule.Priority <= 0)
            {
                throw new InvalidRuleException(
                    $"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}",
                    new[] { $"Loaded rule priority number is invalid: {ruleBuilderResult.Rule.Priority}." });
            }

            return ruleBuilderResult.Rule;
        }

        public RuleDataModel<TContentType, TConditionType> CreateRule(Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var content = rule.ContentContainer.GetContentAs<dynamic>();

            var ruleDataModel = new RuleDataModel<TContentType, TConditionType>
            {
                Content = content,
                ContentType = rule.ContentContainer.ContentType,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null,
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> ConvertConditionNode(
            IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel<TConditionType> conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel<TConditionType>);
            }

            var composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel<TConditionType>;

            var composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                .WithLogicalOperator(composedConditionNodeDataModel.LogicalOperator);

            foreach (ConditionNodeDataModel<TConditionType> child in composedConditionNodeDataModel.ChildConditionNodes)
            {
                composedConditionNodeBuilder.AddCondition(cnb => ConvertConditionNode(cnb, child));
            }

            return composedConditionNodeBuilder.Build();
        }

        private static ValueConditionNodeDataModel<TConditionType> ConvertValueConditionNode(ValueConditionNode<TConditionType> valueConditionNode) => new ValueConditionNodeDataModel<TConditionType>
        {
            ConditionType = valueConditionNode.ConditionType,
            LogicalOperator = LogicalOperators.Eval,
            DataType = valueConditionNode.DataType,
            Operand = valueConditionNode.Operand,
            Operator = valueConditionNode.Operator,
        };

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ValueConditionNodeDataModel<TConditionType> conditionNodeDataModel)
        {
            return conditionNodeDataModel.DataType switch
            {
                DataTypes.Integer => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                    .OfDataType<int>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),
                DataTypes.Decimal => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                   .OfDataType<decimal>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.String => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                   .OfDataType<string>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.Boolean => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                   .OfDataType<bool>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),

                DataTypes.ArrayInteger => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                    .OfDataType<IEnumerable<int>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<int>)
                    .Build(),
                DataTypes.ArrayDecimal => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                    .OfDataType<IEnumerable<decimal>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<decimal>)
                    .Build(),
                DataTypes.ArrayString => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                    .OfDataType<IEnumerable<string>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<string>)
                    .Build(),
                DataTypes.ArrayBoolean => conditionNodeBuilder.AsValued(conditionNodeDataModel.ConditionType)
                    .OfDataType<IEnumerable<bool>>()
                    .WithComparisonOperator(conditionNodeDataModel.Operator)
                    .SetOperand(conditionNodeDataModel.Operand as IEnumerable<bool>)
                    .Build(),
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };
        }

        private ConditionNodeDataModel<TConditionType> ConvertComposedConditionNode(ComposedConditionNode<TConditionType> composedConditionNode)
        {
            var conditionNodeDataModels = new List<ConditionNodeDataModel<TConditionType>>(composedConditionNode.ChildConditionNodes.Count());

            foreach (IConditionNode<TConditionType> child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels.Add(ConvertConditionNode(child));
            }

            return new ComposedConditionNodeDataModel<TConditionType>
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
            };
        }

        private ConditionNodeDataModel<TConditionType> ConvertConditionNode(IConditionNode<TConditionType> conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode(conditionNode as ValueConditionNode<TConditionType>);
            }

            return ConvertComposedConditionNode(conditionNode as ComposedConditionNode<TConditionType>);
        }
    }
}