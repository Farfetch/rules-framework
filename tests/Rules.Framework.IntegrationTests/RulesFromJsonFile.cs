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

namespace Rules.Framework.IntegrationTests
{
    internal class RulesFromJsonFile
    {
        private static readonly RulesFromJsonFile instance = new RulesFromJsonFile();

        public static RulesFromJsonFile Load => instance;

        public async Task<IRulesDataSource<TContentType, TConditionType>> FromJsonFileAsync<TContentType, TConditionType>(string filePath)
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
                        ContentContainer = new SerializedContentContainer<TContentType>(contentType, ruleDataModel.Content, serializationProvider),
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
        {
            Type type = typeof(TContentType);

            return type.IsEnum ? (TContentType)Enum.Parse(type, contentTypeCode.ToString()) : (TContentType)Convert.ChangeType(contentTypeCode, type);
        }

        private IConditionNode<TConditionType> ConvertConditionNode<TConditionType>(ConditionNodeDataModel conditionNodeDataModel)
        {
            LogicalOperators logicalOperator = (LogicalOperators)conditionNodeDataModel.LogicalOperatorCode;
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
            DataTypes dataType = (DataTypes)conditionNodeDataModel.DataTypeCode;
            TConditionType integrationTestsConditionType = (TConditionType)Convert.ChangeType(conditionNodeDataModel.ConditionTypeCode, typeof(TConditionType));
            Operators @operator = (Operators)conditionNodeDataModel.OperatorCode;
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