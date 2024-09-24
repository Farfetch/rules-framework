namespace Rules.Framework.Providers.InMemory.IntegrationTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Rules.Framework;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

    public class BaseScenarioTests
    {
        protected BaseScenarioTests()
        {
        }

        internal ConditionNodeDataModel CreateConditionNodeDataModel(dynamic conditionNode)
        {
            var logicalOperator = this.Parse<LogicalOperators>((string)conditionNode.LogicalOperator);

            if (logicalOperator == LogicalOperators.Eval)
            {
                var dataType = this.Parse<DataTypes>((string)conditionNode.DataType);
                object operand = dataType switch
                {
                    DataTypes.Boolean => Convert.ToBoolean((string)conditionNode.Operand, CultureInfo.InvariantCulture),
                    DataTypes.Integer => Convert.ToInt32((string)conditionNode.Operand, CultureInfo.InvariantCulture),
                    DataTypes.Decimal => Convert.ToDecimal((string)conditionNode.Operand, CultureInfo.InvariantCulture),
                    DataTypes.String => (string)conditionNode.Operand,
                    DataTypes.ArrayInteger => ((IEnumerable<string>)conditionNode.Operand).Select(x => Convert.ToInt32(x, CultureInfo.InvariantCulture)),
                    DataTypes.ArrayDecimal => ((IEnumerable<string>)conditionNode.Operand).Select(x => Convert.ToDecimal(x, CultureInfo.InvariantCulture)),
                    DataTypes.ArrayBoolean => ((IEnumerable<string>)conditionNode.Operand).Select(x => Convert.ToBoolean(x, CultureInfo.InvariantCulture)),
                    DataTypes.ArrayString => (IEnumerable<string>)conditionNode.Operand,
                    _ => null,
                };
                return new ValueConditionNodeDataModel
                {
                    ConditionType = conditionNode.ConditionType,
                    DataType = dataType,
                    LogicalOperator = logicalOperator,
                    Operator = this.Parse<Operators>((string)conditionNode.Operator),
                    Operand = operand,
                    Properties = new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize),
                };
            }
            else
            {
                var childConditionNodes = conditionNode.ChildConditionNodes as IEnumerable<dynamic>;

                var conditionNodeDataModels = new ConditionNodeDataModel[childConditionNodes.Count()];
                var i = 0;
                foreach (var child in childConditionNodes)
                {
                    conditionNodeDataModels[i++] = this.CreateConditionNodeDataModel(child);
                }

                return new ComposedConditionNodeDataModel
                {
                    ChildConditionNodes = conditionNodeDataModels,
                    LogicalOperator = logicalOperator,
                    Properties = new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize),
                };
            }
        }

        internal IRulesDataSource CreateRulesDataSourceTest(IInMemoryRulesStorage inMemoryRulesStorage)
        {
            var ruleFactory = new RuleFactory();
            return new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);
        }

        internal void LoadInMemoryStorage<TContent>(
            string dataSourceFilePath,
            IInMemoryRulesStorage inMemoryRulesStorage,
            Func<dynamic, TContent> contentConvertFunc)
        {
            var rulesFile = File.OpenRead(dataSourceFilePath);

            IEnumerable<RuleDataModel> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                var json = streamReader.ReadToEnd();

                var array = JsonConvert.DeserializeObject(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }) as IEnumerable<dynamic>;

                rules = array.Select(t =>
                {
                    return new RuleDataModel
                    {
                        Content = contentConvertFunc.Invoke(t.Content),
                        ContentType = t.ContentTypeCode,
                        DateBegin = t.DateBegin,
                        DateEnd = t.DateEnd,
                        Name = t.Name,
                        Priority = t.Priority,
                        Active = t.Active ?? true,
                        RootCondition = this.CreateConditionNodeDataModel(t.RootCondition)
                    };
                }).ToList();
            }

            foreach (var rule in rules)
            {
                inMemoryRulesStorage.AddRule(rule);
            }
        }

        protected T Parse<T>(string value)
                            => (T)Parse(value, typeof(T));

        protected object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
}