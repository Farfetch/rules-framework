# Update rules

This page focus on how to update existent rules using Rules.Framework. Rule update operations are very limited, and that is due to protecting the rules historic data and prevent unnintended effects.

Rules update allows you to:

- Change a rule's priority - Rules Engine will re-organize rules according to chosen value. If a value greater than existent rules is chosen, Rules Engine will fix it to the largest value on rules. If a value lower than existent rules is chosen, Rules Engine will fix it to the lowest value on rules.
- Change a rule's date end (e.g. a rule that used to be applicable with no limit - *ad eternum* - and now must be replaced with another one)

Assuming you have a existent rule instance and a built Rules Engine, you can update by simply:

```csharp
rule.Priority = 10;
rule.Date = new DateTime(2021, 05, 10);

RuleOperationResult ruleOperationResult = await rulesEngine.UpdateRuleAsync(rule);
```

Be sure to check on the returned `ruleOperationResult` if the operation was a success, as it may contain any errors occurred while updating rule if operation was a failure.
