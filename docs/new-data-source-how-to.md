# My database has no provider available

In case you need to support other databases, you'll have to implement your own data source provider logic. You should implement `IRulesDataSource<TContentType, TConditionType>` interface and use it on rules engine builder.

```csharp
public interface IRulesDataSource<TContentType, TConditionType>
{
    Task AddRuleAsync(Rule<TContentType, TConditionType> rule);

    Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd);

    Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs);

    Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule);
}
```

You'll have to map each condition type defined by you to a data type on your rules data source. The ones available are:

- Integer
- Decimal
- String
- Boolean

For your guidance, check Mongo DB implementation: [MongoDbProviderRulesDataSource.cs](src/Rules.Framework.Providers.MongoDb/MongoDbProviderRulesDataSource.cs)