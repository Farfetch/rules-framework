# Rules Framework

Rules.Framework is a generic framework that allows defining and evaluating rules for complex business scenarios.

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8b48f4541fba4d4b8bad2e9a8563ede3)](https://app.codacy.com/gh/Farfetch/rules-framework?utm_source=github.com&utm_medium=referral&utm_content=Farfetch/rules-framework&utm_campaign=Badge_Grade_Settings)
[![.NET build](https://github.com/luispfgarces/rules-framework/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/luispfgarces/rules-framework/actions/workflows/dotnet-build.yml)

## Packages

|Name                             |nuget.org|fuget.org|
|---------------------------------|----|---------|
|Rules.Framework|[![Nuget Package](https://img.shields.io/nuget/v/Rules.Framework.svg?logo=nuget)](https://www.nuget.org/packages/Rules.Framework/)|[![Rules.Framework on fuget.org](https://www.fuget.org/packages/Rules.Framework/badge.svg)](https://www.fuget.org/packages/Rules.Framework)|
|Rules.Framework.Providers.MongoDb|[![Nuget Package](https://img.shields.io/nuget/v/Rules.Framework.Providers.MongoDb?logo=nuget)](https://www.nuget.org/packages/Rules.Framework.Providers.MongoDb/)|[![Rules.Framework.Providers.MongoDb on fuget.org](https://www.fuget.org/packages/Rules.Framework.Providers.MongoDb/badge.svg)](https://www.fuget.org/packages/Rules.Framework.Providers.MongoDb)|

## What is a rule?

A rule is a data structure limited in time (`date begin` and `date end`), whose content is categorized by a `content type`. Its applicability is constrained by `conditions`, and a `priority` value is used as untie criteria when there are multiple matches.

## Why use rules?

By using rules, one is able to support a multiplicity of business scenarios through rule configurations, instead of heavy code development efforts. Rules enable a fast response to change and a better control of the business logic by the product teams.

## Basic usage

To set up a rules engine, define the content and condition types to be used.

```csharp
enum AnimalContentType { Sound, IsDomestic, IsDangerous }
enum AnimalConditionType { Animal, Breed, Family }
```

Use the `RulesEngineBuilder` to build the rules engine.

```csharp
var rulesEngine = RulesEngineBuilder
    .CreateRulesEngine()
    .WithContentType<AnimalContentType>()
    .WithConditionType<AnimalConditionType>()
    .SetInMemoryDataSource()
    .Configure(c => c.PriorityCriteria = PriorityCriterias.TopmostRuleWins)
    .Build();
```

To define and add a rule, use the `RuleBuilder` and the `AddRuleAsync()`.
```csharp
var ruleForDogSound = RuleBuilder
    .NewRule<AnimalContentType, AnimalConditionType>()
    .WithName("Rule for the sound of the dog.")
    .WithContent(AnimalContentType.Sound, "Bark")
    .WithCondition(c => 
        c.AsValued(AnimalConditionType.Animal)
        .OfDataType<string>().WithComparisonOperator(Operators.Equal)
        .SetOperand("Dog")
        .Build())
    .WithDateBegin(new DateTime(2020, 01, 01))
    .Build();

rulesEngine.AddRuleAsync(ruleForDogSound.Rule, RuleAddPriorityOption.ByPriorityNumber(1));
```


You can then get a matchingRule by creating the necessary condition(s) and by using the `MatchOneAsync()`.

```csharp
var matchConditions = new[]
{
    new Condition<AnimalConditionType> { Type = AnimalConditionType.Animal, Value = "Dog" },
};

var matchingRule = rulesEngine.MatchOneAsync(AnimalContentType.Sound, new DateTime(2021, 04, 13), matchConditions);
```

## Complex scenarios

To understand how the Rules.Framework can be used in various business scenarios please check the [Documentation](#documentation).

You can also check the scenarios and samples available in the source-code.

## Features

The following listing presents features implemented and features to be implemented:
- [x] Rules evaluation (match one)
- [x] Rules evaluation (match many)
- [x] Rules search
- [x] Rules content serialization
- [x] Rules management (Create, Read, Update)
- [x] In-memory data source support
- [x] MongoDB data source support
- [ ] SQL Server data source support
- [ ] Rules data source caching
- [x] Rules WebUI for visualization

## Documentation

1.  [Introduction](docs/introduction.md)
2.  [Getting started](docs/getting-started.md)
3.  [Add rules](docs/add-rules.md)
4.  [Update rules](docs/update-rules.md)
5.  [Search rules](docs/search-rules.md)
6.  [Get Unique Condition Types](get-unique-condition-types.md)
7.  [In-memory data source provider](docs/using-in-memory-data-source.md)
8.  [MongoDB data source provider](docs/using-mongo-db-data-source.md)
9.  [New data source provider - How To](docs/new-data-source-how-to.md)

## Contributing

Contributions are more than welcome! Submit comments, issues or pull requests, we promise to keep an eye on them :)

Head over to [CONTRIBUTING](CONTRIBUTING.md) for further details.

## License

[MIT License](LICENSE.md)