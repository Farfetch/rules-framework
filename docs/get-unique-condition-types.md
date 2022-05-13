# Get Unique Condition Types

This page focus on how to get the condition types  associated with rules of a specific content type using Rules.Framework.

## The `GetUniqueConditionsTypes` Method

Get the unique condition types associated with rules of a specific content type and period.

```csharp
GetUniqueConditionTypesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
```
A set of rules is requested to rules data source and all conditions are evaluated against them to provide a set of matches. All rules matching supplied conditions are returned.

```csharp
GetConditionTypes(IEnumerable<Rule<TContentType, TConditionType>> matchedRules)
```
If no conditions are found an empty list is returned