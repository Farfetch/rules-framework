namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rules.Framework;
    using Rules.Framework.Builder.Generic.RulesBuilder;
    using Rules.Framework.Generic;
    using Rules.Framework.IntegrationTests.DataSource;

    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new RulesFromJsonFile();

        public static RulesFromJsonFile Load => instance;

        public async Task FromJsonFileAsync<TRuleset, TCondition>(IRulesEngine<TRuleset, TCondition> rulesEngine, string filePath, Type contentRuntimeType, bool serializedContent = true)
            where TRuleset : new()
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                var contents = await streamReader.ReadToEndAsync();
                var ruleDataModels = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(contents);
                var addedContentTypes = new HashSet<TRuleset>();

                foreach (var ruleDataModel in ruleDataModels)
                {
                    var contentType = GetRuleset<TRuleset>(ruleDataModel.Ruleset);
                    if (!addedContentTypes.Contains(contentType))
                    {
                        await rulesEngine.CreateRulesetAsync(contentType);
                        addedContentTypes.Add(contentType);
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

                    var ruleBuilder = Rule.Create<TRuleset, TCondition>(ruleDataModel.Name)
                        .InRuleset(contentType)
                        .SetContent(content)
                        .Since(ruleDataModel.DateBegin)
                        .Until(ruleDataModel.DateEnd);

                    if (ruleDataModel.RootCondition is { })
                    {
                        ruleBuilder.ApplyWhen(b => this.ConvertConditionNode(b, ruleDataModel.RootCondition));
                    }
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

        private static IFluentConditionNodeBuilder<TCondition> CreateValueConditionNode<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            var condition = RulesFromJsonFile.Parse<TCondition>(conditionNodeDataModel.Condition);
            var @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);

            switch (dataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.Decimal:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.String:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        conditionNodeDataModel.Operand);

                case DataTypes.Boolean:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType}.");
            }
        }

        private static IConditionNode CreateValueConditionNode<TCondition>(IRootConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            var dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            var condition = RulesFromJsonFile.Parse<TCondition>(conditionNodeDataModel.Condition);
            var @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);

            switch (dataType)
            {
                case DataTypes.Integer:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToInt32(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.Decimal:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToDecimal(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                case DataTypes.String:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        conditionNodeDataModel.Operand);

                case DataTypes.Boolean:
                    return conditionNodeBuilder.Value(
                        condition,
                        @operator,
                        Convert.ToBoolean(conditionNodeDataModel.Operand, CultureInfo.InvariantCulture));

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType}.");
            }
        }

        private static TRuleset GetRuleset<TRuleset>(string ruleset) where TRuleset : new()
            => RulesFromJsonFile.Parse<TRuleset>(ruleset);

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        private IConditionNode ConvertConditionNode<TCondition>(IRootConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
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

        private IFluentConditionNodeBuilder<TCondition> ConvertConditionNode<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
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

        private IFluentConditionNodeBuilder<TCondition> HandleChildConditionNodes<TCondition>(IFluentConditionNodeBuilder<TCondition> conditionNodeBuilder, ConditionNodeDataModel conditionNodeDataModel)
        {
            foreach (var child in conditionNodeDataModel.ChildConditionNodes)
            {
                this.ConvertConditionNode(conditionNodeBuilder, child);
            }

            return conditionNodeBuilder;
        }
    }
}