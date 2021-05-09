# Getting started

1. Define your content types (suggestion: use an enum).

```csharp
internal enum ContentTypes
{
    BodyMassIndexFormula = 1
}
```

2. Define your condition types (suggestion: use an enum).

```csharp
internal enum ConditionTypes
{
    MassUnitOfMeasure = 1,
    HeightUnitOfMeasure = 2
}
```

3. Build a rules engine.

```csharp
RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
    .WithContentType<ContentTypes>() // Your content types.
    .WithConditionType<ConditionTypes>() // Your condition types.
    .SetDataSource(rulesDataSource) // A rules data source instance implemented by you OR using one of the available providers.
    .Build();
```

## Evaluating rules

1. Set the conditions to supply to the engine.

```csharp
Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[]
{
    new Condition<ConditionTypes>
    {
        Type = ConditionTypes.MassUnitOfMeasure,
        Value = "pounds"
    },
    new Condition<ConditionTypes>
    {
        Type = ConditionTypes.HeightUnitOfMeasure,
        Value = "inches"
    }
};
```

2. Set the date at which you want the rules set to evalutated.

```csharp
DateTime date = new DateTime(2019, 1, 1);
```

3. Set the content you want to fetch.

```csharp
ContentTypes contentType = ContentTypes.BodyMassIndexFormula;
```

4. Evalutate rule match.

```csharp
Rule<ContentTypes, ConditionTypes> ruleMatch = await rulesEngine.MatchOneAsync(contentType, date, conditions);
```