namespace Rules.Framework.Providers.InMemory.IntegrationTests.Scenarios
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory.DataModel;

    public class BaseScenarioTests
    {
        protected BaseScenarioTests()
        {
        }

        internal ConditionNodeDataModel<TConditionType> CreateConditionNodeDataModel<TConditionType>(dynamic conditionNode)
        {
            LogicalOperators logicalOperator = this.Parse<LogicalOperators>((string)conditionNode.LogicalOperator);

            if (logicalOperator == LogicalOperators.Eval)
            {
                return new ValueConditionNodeDataModel<TConditionType>
                {
                    ConditionType = this.Parse<TConditionType>((string)conditionNode.ConditionType),
                    DataType = this.Parse<DataTypes>((string)conditionNode.DataType),
                    LogicalOperator = logicalOperator,
                    Operator = this.Parse<Operators>((string)conditionNode.Operator),
                    Operand = (string)conditionNode.Operand
                };
            }
            else
            {
                IEnumerable<dynamic> childConditionNodes = conditionNode.ChildConditionNodes as IEnumerable<dynamic>;

                List<ConditionNodeDataModel<TConditionType>> conditionNodeDataModels = new List<ConditionNodeDataModel<TConditionType>>(childConditionNodes.Count());
                foreach (dynamic child in childConditionNodes)
                {
                    conditionNodeDataModels.Add(this.CreateConditionNodeDataModel<TConditionType>(child));
                }

                return new ComposedConditionNodeDataModel<TConditionType>
                {
                    ChildConditionNodes = conditionNodeDataModels,
                    LogicalOperator = logicalOperator
                };
            }
        }

        internal IRulesDataSource<TContentType, TConditionType> CreateRulesDataSourceTest<TContentType, TConditionType>(InMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage)
        {
            IRuleFactory<TContentType, TConditionType> ruleFactory = new RuleFactory<TContentType, TConditionType>();
            return new InMemoryProviderRulesDataSource<TContentType, TConditionType>(inMemoryRulesStorage, ruleFactory);
        }

        internal void LoadInMemoryStorage<TContentType, TConditionType, TContent>(
            string dataSourceFilePath,
            IInMemoryRulesStorage<TContentType, TConditionType> inMemoryRulesStorage,
            Func<dynamic, TContent> contentConvertFunc)
        {
            var rulesFile = File.OpenRead(dataSourceFilePath);

            IEnumerable<RuleDataModel<TContentType, TConditionType>> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                var json = streamReader.ReadToEnd();

                IEnumerable<dynamic> array = JsonConvert.DeserializeObject(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }) as IEnumerable<dynamic>;

                rules = array.Select(t =>
                {
                    return new RuleDataModel<TContentType, TConditionType>
                    {
                        Content = contentConvertFunc.Invoke(t.Content),
                        ContentType = (TContentType)t.ContentTypeCode,
                        DateBegin = t.DateBegin,
                        DateEnd = t.DateEnd,
                        Name = t.Name,
                        Priority = t.Priority,
                        Active = t.Active ?? true,
                        RootCondition = this.CreateConditionNodeDataModel<TConditionType>(t.RootCondition)
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