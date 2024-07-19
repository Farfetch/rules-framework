namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.Builder.Generic;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.DataSource;

    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new RulesFromJsonFile();

        public static RulesFromJsonFile Load => instance;

        public async Task FromJsonFileAsync<TContentType, TConditionType>(IRulesEngine<TContentType, TConditionType> rulesEngine, string filePath, Type contentRuntimeType, bool serializedContent = true)
            where TContentType : new()
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                var contents = await streamReader.ReadToEndAsync();
                var ruleDataModels = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(contents);

                foreach (var ruleDataModel in ruleDataModels)
                {
                    var contentType = GetContentType<TContentType>(ruleDataModel.ContentTypeCode);

                    var ruleBuilder = Rule.New<TContentType, TConditionType>()
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

                    ruleBuilder.WithContent(contentType, content);
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

        private static IFluentComposedConditionNodeBuilder<TConditionType> CreateValueConditionNode<TConditionType>(IFluentComposedConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            var integrationTestsConditionType = RulesFromJsonFile.Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            var @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);

            switch (dataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.Decimal:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.String:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        conditionNodeDataModel.Operand);

                case DataTypes.Boolean:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType.ToString()}.");
            }
        }

        private static IConditionNode CreateValueConditionNode<TConditionType>(IRootConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            var integrationTestsConditionType = RulesFromJsonFile.Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            var @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);

            switch (dataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.Decimal:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.String:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        conditionNodeDataModel.Operand);

                case DataTypes.Boolean:
                    return conditionNodeBuilder.Value(
                        integrationTestsConditionType,
                        @operator,
                        Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType.ToString()}.");
            }
        }

        private static TContentType GetContentType<TContentType>(string contentTypeCode) where TContentType : new()
                    => RulesFromJsonFile.Parse<TContentType>(contentTypeCode);

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode ConvertConditionNode<TConditionType>(IRootConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var logicalOperator = RulesFromJsonFile.Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);

            switch (logicalOperator)
            {
                case LogicalOperators.And:
                    return conditionNodeBuilder.And(b => HandleChildConditionNodes(b, conditionNodeDataModel));

                case LogicalOperators.Or:
                    return conditionNodeBuilder.Or(b => HandleChildConditionNodes(b, conditionNodeDataModel));

                case LogicalOperators.Eval:
                    return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel);

                default:
                    throw new NotSupportedException($"The logical operator '{logicalOperator}' is not supported.");
            }
        }

        private IFluentComposedConditionNodeBuilder<TConditionType> ConvertConditionNode<TConditionType>(IFluentComposedConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var logicalOperator = RulesFromJsonFile.Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);

            switch (logicalOperator)
            {
                case LogicalOperators.And:
                    return conditionNodeBuilder.And(b => HandleChildConditionNodes(b, conditionNodeDataModel));

                case LogicalOperators.Or:
                    return conditionNodeBuilder.Or(b => HandleChildConditionNodes(b, conditionNodeDataModel));

                case LogicalOperators.Eval:
                    return CreateValueConditionNode(conditionNodeBuilder, conditionNodeDataModel);

                default:
                    throw new NotSupportedException($"The logical operator '{logicalOperator}' is not supported.");
            }
        }

        private IFluentComposedConditionNodeBuilder<TConditionType> HandleChildConditionNodes<TConditionType>(IFluentComposedConditionNodeBuilder<TConditionType> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            foreach (var child in conditionNodeDataModel.ChildConditionNodes)
            {
                this.ConvertConditionNode(conditionNodeBuilder, child);
            }

            return conditionNodeBuilder;
        }
    }
}