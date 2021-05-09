# Using a in-memory data source provider

With a in-memory data source provider, you can define rules and manage them in-memory, and you can also manage their lifetime in memory.

There are 2 configuration possibilities:

1. Setting the in-memory data source attached to rules engine lifetime.

    ```csharp
    RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
        .WithContentType<ContentTypes>()
        .WithConditionType<ConditionTypes>()
        .SetInMemoryDataSource()
        .Build();
    ```

2. Registering in-memory data source on dependency injection and using service provider-

    ```csharp
        // Registering in-memory data source on dependency injection.
        (...)
        services.AddInMemoryRulesDataSource<ContentTypes, ConditionTypes>(ServiceLifetime.Singleton);
        (...)

        // Creating the rules engine specifying as parameter the service provider.
    RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
        .WithContentType<ContentTypes>()
        .WithConditionType<ConditionTypes>()
        .SetInMemoryDataSource(serviceProvider)
        .Build();
    ```