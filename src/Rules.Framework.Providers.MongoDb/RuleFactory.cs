using System;
using System.Globalization;
using Rules.Framework.Builder;
using Rules.Framework.Core;
using Rules.Framework.Providers.MongoDb.DataModel;
using Rules.Framework.Serialization;

namespace Rules.Framework.Providers.MongoDb
{
    internal class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        private readonly IContentSerializationProvider<TContentType> contentSerializationProvider;

        public RuleFactory(IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            this.contentSerializationProvider = contentSerializationProvider;
        }

        public Rule<TContentType, TConditionType> CreateRule(RuleDataModel ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            TContentType contentType = Parse<TContentType>(ruleDataModel.ContentType);

            RuleBuilderResult<TContentType, TConditionType> ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithPriority(ruleDataModel.Priority)
                .WithCondition(cnb => this.ConvertConditionNode(cnb, ruleDataModel.RootCondition))
                .WithSerializedContent(contentType, (object)ruleDataModel.Content, this.contentSerializationProvider)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule ID: {ruleDataModel.Id.ToString()}", ruleBuilderResult.Errors);
            }

            return ruleBuilderResult.Rule;
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ValueConditionNodeDataModel conditionNodeDataModel)
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            switch (conditionNodeDataModel.DataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.AsValued(conditionType)
                        .OfDataType<int>()
                        .WithComparisonOperator(conditionNodeDataModel.Operator)
                        .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                case DataTypes.Decimal:
                    return conditionNodeBuilder.AsValued(conditionType)
                        .OfDataType<decimal>()
                        .WithComparisonOperator(conditionNodeDataModel.Operator)
                        .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                case DataTypes.String:
                    return conditionNodeBuilder.AsValued(conditionType)
                        .OfDataType<string>()
                        .WithComparisonOperator(conditionNodeDataModel.Operator)
                        .SetOperand(Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                case DataTypes.Boolean:
                    return conditionNodeBuilder.AsValued(conditionType)
                        .OfDataType<bool>()
                        .WithComparisonOperator(conditionNodeDataModel.Operator)
                        .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                default:
                    throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType.ToString()}.");
            }
        }

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode<TConditionType> ConvertConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel);
            }
            else
            {
                ComposedConditionNodeDataModel composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

                IComposedConditionNodeBuilder<TConditionType> composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                    .WithLogicalOperator(composedConditionNodeDataModel.LogicalOperator);

                foreach (ConditionNodeDataModel child in composedConditionNodeDataModel.ChildConditionNodes)
                {
                    composedConditionNodeBuilder.AddCondition(cnb => this.ConvertConditionNode(cnb, child));
                }

                return composedConditionNodeBuilder.Build();
            }
        }
    }
}