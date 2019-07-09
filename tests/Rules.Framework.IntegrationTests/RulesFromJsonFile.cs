namespace Rules.Framework.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
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

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string contents = await streamReader.ReadToEndAsync();
                IEnumerable<RuleDataModel> ruleDataModels = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(contents));

                List<Rule<TContentType, TConditionType>> rules = new List<Rule<TContentType, TConditionType>>(ruleDataModels.Count());
                foreach (RuleDataModel ruleDataModel in ruleDataModels)
                {
                    TContentType contentType = GetContentType<TContentType>(ruleDataModel.ContentTypeCode);

                    Rule<TContentType, TConditionType> integrationTestsRule = new Rule<TContentType, TConditionType>
                    {
                        ContentContainer = serializedContent
                        ? new SerializedContentContainer<TContentType>(contentType, ruleDataModel.Content, serializationProvider)
                        : new ContentContainer<TContentType>(contentType, (t) => RulesFromJsonFile.Parse(ruleDataModel.Content, t)),
                        DateBegin = ruleDataModel.DateBegin,
                        DateEnd = ruleDataModel.DateEnd,
                        Name = ruleDataModel.Name,
                        Priority = ruleDataModel.Priority
                    };

                    if (ruleDataModel.RootCondition != null)
                    {
                        integrationTestsRule.RootCondition = this.ConvertConditionNode<TConditionType>(ruleDataModel.RootCondition);
                    }

                    rules.Add(integrationTestsRule);
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

        private IConditionNode<TConditionType> ConvertConditionNode<TConditionType>(ConditionNodeDataModel conditionNodeDataModel)
        {
            LogicalOperators logicalOperator = RulesFromJsonFile.Parse<LogicalOperators>(conditionNodeDataModel.LogicalOperator);
            if (logicalOperator == LogicalOperators.Eval)
            {
                return this.CreateValueConditionNode<TConditionType>(conditionNodeDataModel);
            }
            else
            {
                List<IConditionNode<TConditionType>> childConditionNodes =
                    new List<IConditionNode<TConditionType>>(conditionNodeDataModel.ChildConditionNodes.Count());
                foreach (ConditionNodeDataModel child in conditionNodeDataModel.ChildConditionNodes)
                {
                    childConditionNodes.Add(this.ConvertConditionNode<TConditionType>(child));
                }

                return new ComposedConditionNode<TConditionType>(logicalOperator, childConditionNodes);
            }
        }

        private IConditionNode<TConditionType> CreateValueConditionNode<TConditionType>(ConditionNodeDataModel conditionNodeDataModel)
        {
            DataTypes dataType = RulesFromJsonFile.Parse<DataTypes>(conditionNodeDataModel.DataType);
            TConditionType integrationTestsConditionType = RulesFromJsonFile.Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            Operators @operator = RulesFromJsonFile.Parse<Operators>(conditionNodeDataModel.Operator);
            switch (dataType)
            {
                case DataTypes.Integer:
                    return new IntegerConditionNode<TConditionType>(integrationTestsConditionType, @operator, Convert.ToInt32(conditionNodeDataModel.Operand));

                case DataTypes.Decimal:
                    return new DecimalConditionNode<TConditionType>(integrationTestsConditionType, @operator, Convert.ToDecimal(conditionNodeDataModel.Operand));

                case DataTypes.String:
                    return new StringConditionNode<TConditionType>(integrationTestsConditionType, @operator, conditionNodeDataModel.Operand);

                case DataTypes.Boolean:
                    return new BooleanConditionNode<TConditionType>(integrationTestsConditionType, @operator, Convert.ToBoolean(conditionNodeDataModel.Operand));

                default:
                    throw new NotSupportedException($"Unsupported data type: {dataType.ToString()}.");
            }
        }
    }
}