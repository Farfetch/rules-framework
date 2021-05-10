# Search rules

Here it is described how to search rules base using the Rules Engine. The Rules Engine exposes a method to search rules:

```csharp
SearchRulesAsync(SearchArgs<TContentType, TConditionType> searchArgs)
```

The `searchArgs` parameter allows you to define the following options:

- [mandatory] `ContentType` filters by a specific content type.
- [mandatory] `DateBegin` filters the rules base for rules with a match starting on this value.
- [mandatory] `DateEnd` filters the rules base for rules with a match until this value. This value cannot be lesser than `DateBegin` - a `ArgumentException` will be thrown if this validation fails.
- `Conditions` filters rules by specified conditions. It may be empty.
- `ExcludeRulesWithoutSearchConditions` allows to exclude rules that do not contain search conditions. This option defaults to `false`.

NOTE: if `DateBegin` equals to `DateEnd`, rules engine will assume DateEnd as `DateBegin` plus 1 day.
