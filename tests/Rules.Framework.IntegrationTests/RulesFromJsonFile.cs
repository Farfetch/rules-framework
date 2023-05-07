namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.DataSource;

    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new RulesFromJsonFile();

        public static RulesFromJsonFile Load => instance;

        public async Task FromJsonFileAsync<TContentType, TConditionType>(RulesEngine<TContentType, TConditionType> rulesEngine, string filePath, Type contentRuntimeType, bool serializedContent = true)
            where TContentType : new()
        {
            var serializationProvider = new JsonContentSerializationProvider<TContentType>();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                var contents = await streamReader.ReadToEndAsync();
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

                    object content;
                    if (serializedContent)
                    {
                        content = JsonConvert.DeserializeObject(ruleDataModel.Content, contentRuntimeType);
                    }
                    else
                    {
                        content = RulesFromJsonFile.Parse(ruleDataModel.Content, contentRuntimeType);
                    }

                    ruleBuilder.WithContentContainer(new ContentContainer<TContentType>(contentType, (t) => content));
                    var ruleBuilderResult = ruleBuilder.Build();

                    if (ruleBuilderResult.IsSuccess)
                    {
                        var rule = ruleBuilderResult.Rule;
                        await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.ByPriorityNumber(ruleDataModel.Priority));
                    }
                    else
                    {
                        throw new InvalidRuleException($"Loaded invalid rule from file. Rule name: {ruleDataModel.Name}");
                    }
                }
            }
        }

        private static TContentType GetContentType<TContentType>(short contentTypeCode) where TContentType : new()
            => RulesFromJsonFile.Parse<TContentType>(contentTypeCode.ToString());

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

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

                foreach (var child in conditionNodeDataModel.ChildConditionNodes)
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
                        .SetOperand(Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                case DataTypes.Decimal:
                    return conditionNodeBuilder.AsValued(integrationTestsConditionType)
                        .OfDataType<decimal>()
                        .WithComparisonOperator(@operator)
                        .SetOperand(Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
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
                        .SetOperand(Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture))
                        .Build();

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType.ToString()}.");
            }
        }
    }
}