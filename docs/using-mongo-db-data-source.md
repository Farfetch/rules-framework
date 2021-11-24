# Using a MongoDB data source provider

With a MongoDB backed data source provider, you can create, manage and evaluate rules against a persistent database. MongoDB data source provider makes use of Mongo C# driver capability of deserializing BSON data into a dynamic object.

To make use of it, just set it as data source during rules engine construction:

```csharp
MongoClientSettings settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
IMongoClient mongoClient = new MongoClient(settings);
MongoDbProviderSettings mongoDbProviderSettings = new MongoDbProviderSettings
{
    DatabaseName = "sample-database",
    RulesCollectionName = "sample-collection"
};

RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .WithContentType<ContentTypes>()
    .WithConditionType<ConditionTypes>()
    .SetMongoDbDataSource(mongoClient, mongoDbProviderSettings)
    .Build();
```

If you choose to use MongoDB as your rules data source, make sure you follow these rules of thumb for rules "content":

- Use get/set properties with no behavior.
- Define a parameterless (default) constructor (it may be protected or private, but must be with no parameters).
- Keep behavior out of content objects.

It is really important to keep rules "content" objects as pure/anemic objects to avoid issues loading rules into memory.
