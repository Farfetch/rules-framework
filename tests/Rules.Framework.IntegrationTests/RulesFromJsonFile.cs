namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.DataSource;
    using Rules.Framework.Serialization;

    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new RulesFromJsonFile();

        public static RulesFromJsonFile Load => instance;

        public async Task<IRulesDataSource<TContentType, TConditionType>> FromJsonFileAsync<TContentType, TConditionType>(string filePath, bool serializedContent = true)
            where TContentType : new()
        {
            JsonContentSerializationProvider<TContentType> serializationProvider = new JsonContentSerializationProvider<TContentType>();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                string contents = await streamReader.ReadToEndAsync();
                var ruleDataModels = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(contents));

                var rules = new List<Rule<TContentType, TConditionType>>(ruleDataModels.Count());
                foreach (var ruleDataModel in ruleDataModels)
                {
                    var contentType = GetContentType<TContentType>(ruleDataModel.ContentTypeCode);

                    var ruleBuilder = RuleBuilder.NewRule<TContentType, TConditionType>()
                        .WithName(ruleDataModel.Name)
                        .WithDatesInterval(ruleDataModel.DateBegin, ruleDataModel.DateEnd);

                    if (ruleDataModel.RootCondition is { })
                    {
                        ruleBuilder.WithCondition(b => this.ConvertConditionNode(b, ruleDataModel.RootCondition));
                    }

                    if (serializedContent)
                    {
                        ruleBuilder.WithSerializedContent(contentType, ruleDataModel.Content, serializationProvider);
                    }
                    else
                    {
                        ruleBuilder.WithContentContainer(new ContentContainer<TContentType>(contentType, (t) => RulesFromJsonFile.Parse(ruleDataModel.Content, t)));
                    }

                    var ruleBuilderResult = ruleBuilder.Build();

                    if (ruleBuilderResult.IsSuccess)
                    {
                        var rule = ruleBuilderResult.Rule;
                        rule.Priority = ruleDataModel.Priority;
                        rules.Add(rule);
                    }
                    else
                    {
                        throw new InvalidRuleException($"Loaded invalid rule from file. Rule name: {ruleDataModel.Name}");
                    }
                }

                return new InMemoryRulesDataSource<TContentType, TConditionType>(rules);
            }
        }

        private static TContentType GetContentType<TContentType>(short contentTypeCode) where TContentType : new()
            => RulesFromJsonFile.Parse<TContentType>(contentTypeCode.ToString());

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type);

        private IConditionNode<TConditionType> ConvertConditionNode<TConditionType>(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var logicalOperator = RulesFromJsonFile.Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);

            if (logicalOperator == LogicalOperators.Eval)
            {
                return this.CreateValueConditionNode<TConditionType>(conditionNodeBuilder, conditionNodeDataModel);
            }
            else
            {
                var composedConditionNodeBuilder = conditionNodeBuilder.AsComposed()
                    .WithLogicalOperator(logicalOperator);

                foreach (ConditionNodeDataModel child in conditionNodeDataModel.ChildConditionNodes)
                {
                    composedConditionNodeBuilder.AddCondition(cnb => this.ConvertConditionNode(cnb, child));
                }

                return composedConditionNodeBuilder.Build();
            }
        }

        private IConditionNode<TConditionType> CreateValueConditionNode<TConditionType>(IConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            var integrationTestsConditionType = RulesFromJsonFile.Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            var @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);

            switch (dataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.AsValued(integrationTestsConditionType)
                        .OfDataType<int>()
                        .WithComparisonOperator(@operator)
                        .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand))
                        .Build();

                case DataTypes.Decimal:
                    return conditionNodeBuilder.AsValued(integrationTestsConditionType)
                        .OfDataType<decimal>()
                        .WithComparisonOperator(@operator)
                        .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand))
                        .Build();

                case DataTypes.String:
                    return conditionNodeBuilder.AsValued(integrationTestsConditionType)
                        .OfDataType<string>()
                        .WithComparisonOperator(@operator)
                        .SetOperand(conditionNodeDataModel.Operand)
                        .Build();

                case DataTypes.Boolean:
                    return conditionNodeBuilder.AsValued(integrationTestsConditionType)
                        .OfDataType<bool>()
                        .WithComparisonOperator(@operator)
                        .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand))
                        .Build();

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType.ToString()}.");
            }
        }
    }
}