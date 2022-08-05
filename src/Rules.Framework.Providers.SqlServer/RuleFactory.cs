namespace Rules.Framework.Providers.SqlServer
{
    using System;
    using System.Globalization;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.SqlServer.Models;

    using Core = Rules.Framework.Core;
    using Model = Rules.Framework.SqlServer.Models;

    public class RuleFactory<TContentType, TConditionType> : IRuleFactory<TContentType, TConditionType>
    {
        public Core.Rule<TContentType, TConditionType> CreateRule(Model.Rule ruleDataModel)
        {
            if (ruleDataModel is null)
            {
                throw new ArgumentNullException(nameof(ruleDataModel));
            }

            TContentType contentType = Parse<TContentType>(ruleDataModel.ContentTypeCode.ToString());

            RuleBuilderResult<TContentType, TConditionType> ruleBuilderResult = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleDataModel.Name)
                .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd)
                .WithCondition(cnb => ruleDataModel.ConditionNode is { } ? this.ConvertConditionNode(cnb, ruleDataModel.ConditionNode) : null) //TODO: replace ruleDataModel.ConditionNode by ruleDataModel.RootCondition
                .WithSerializedContent(contentType, (object)ruleDataModel.Content, this.contentSerializationProvider)
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

        public Model.Rule CreateRule(Core.Rule<TContentType, TConditionType> rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            dynamic content = rule.ContentContainer.GetContentAs<dynamic>();

            Model.Rule ruleDataModel = new Model.Rule
            {
                Content = content,
                //ContentTypeCode = Convert.ToString(rule.ContentContainer.ContentType, CultureInfo.InvariantCulture),
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                //RootCondition = rule.RootCondition is { } ? this.ConvertConditionNode(rule.RootCondition) : null
            };

            return ruleDataModel;
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNode conditionNodeDataModel) //TODO replace conditionNode by ValueConditionNodeDataModel
        {
            TConditionType conditionType = Parse<TConditionType>(conditionNodeDataModel.ConditionTypeCode);
            return conditionNodeDataModel.DataType switch
            {
                DataTypes.Integer => conditionNodeBuilder.AsValued(conditionType)
                    .OfDataType<int>()
                    .WithComparisonOperator(conditionNodeDataModel.OperatorCode)
                    .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),
                DataTypes.Decimal => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<decimal>()
                   .WithComparisonOperator(conditionNodeDataModel.OperatorCode)
                   .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.String => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<string>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToString(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                   .Build(),
                DataTypes.Boolean => conditionNodeBuilder.AsValued(conditionType)
                   .OfDataType<bool>()
                   .WithComparisonOperator(conditionNodeDataModel.Operator)
                   .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                    .Build(),
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType}."),
            };
        }

        //TODO: move to a common place
        private static T Parse<T>(string value)
           => (T)Parse(value, typeof(T));

        //TODO: move to a common place
        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode<TConditionType> ConvertConditionNode(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNode conditionNodeDataModel) //TODO: replace conditionNode by ConditionNodeDataModel
        {
            if (conditionNodeDataModel.LogicalOperatorCode == (int)LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel as ValueConditionNodeDataModel); //TODO: replace conditionNode by ValueConditionNodeDataModel
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