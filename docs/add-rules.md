# Add rules

This page focus on how to create and add rules using Rules.Framework. It disposes of a rule build fluent API, to provide a near-natural language experience for whomever is creating rule programatically.

## The `RuleBuilder` API

The `RuleBuilder` API currently is the only way to build new rules for adding to Rules Engine. It has a fluent API in order to make rule building a development experience easy and comprehensive.

At this point, you should have defined your content type and condition type, and built your rules engine instance. If you haven't done so, please take a look at [README](../README.md#Getting%20started) first.

To start building a rule, begin by invoking:

```csharp
RuleBuilder.NewRule<TContentType, TConditionType>()
```

### Rule name

And then define a name for it:

```csharp
WithName(string name)
```

A rule name is its' unique identifier, so you must pay attention to create your rules without repeating names over the time.

### Set dates interval

After a name, you should assign it with a dates interval to define from when to when the rule is applicable. Although the begin date is mandatory, the end date is not, so you have 2 options at your choosing for which fits your scenario:

```csharp
WithDatesInterval(DateTime dateBegin, DateTime dateEnd)
```

or

```csharp
WithDateBegin(DateTime dateBegin)
```

Having the last always date end setted to `null` (it means rule applies to *ad eternum*).

### Set additional conditions

After a dates interval, we should set additional conditions to restrict at your discretion how it should be matched when you try to evaluate it using Rules Engine. You should start adding conditions by specifying a root condition and specifying additional ones on a tree format.

```csharp
WithCondition(Func<IConditionNodeBuilder, IConditionNode> conditionFunc)
```

You have at your disposal 2 types of condition nodes:

- Valued condition nodes

    ```csharp
    AsValued(TConditionType conditionType)
    ```

- Composed condition nodes

    ```csharp
    AsComposed()
    ```

#### Valued condition nodes

Valued condition nodes allow you to define conditions evaluated against values supplied to Rules Engine when matching or searching rules. This type of condition node considers the value supplied to Rules Engine as a left hand operand, defines a operator and a right hand operator. Consider the example:

> Age > 18

Valued condition nodes allow to capture conditions like the sample in the following way:

```csharp
WithCondition(cnb => 
    cnb.AsValued(ConditionTypes.Age)
        .OfDataType<int>()
        .WithComparisonOperator(Operators.GreaterThan)
        .SetOperand(18)
        .Build()
)
```

The `AsValued(TConditionType conditionType)` accepts a value for the condition type defined by you. You must then specify the data type of the operands associated to your condition type. Supported data types are:

- int
- bool
- string
- decimal

Next step is to define a comparison operator using method `WithComparisonOperator(Operators @operator)`. The following ones are supported:

- Equal (`=`)
- NotEqual (`!=` or `<>`)
- GreaterThan (`>`)
- GreaterThanOrEqual (`>=`)
- LesserThan (`<`)
- LesserThanOrEqual (`<=`)
- Contains

Not all comparison operators are supported for all data types, some combinations of them are not valid. This will be validated by rule builder and a validation error will be returned if a invalid combination is attempted. Check the following grid to see combinations are valid or not:

|Operator/Data Type|int|decimal|bool|string|
|-|-|-|-|-|
|Equal (`=`)|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|NotEqual (`!=` or `<>`)|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|:heavy_check_mark:|
|GreaterThan (`>`)|:heavy_check_mark:|:heavy_check_mark:|:x:|:x:|
|GreaterThanOrEqual (`>=`)|:heavy_check_mark:|:heavy_check_mark:|:x:|:x:|
|LesserThan (`<`)|:heavy_check_mark:|:heavy_check_mark:|:x:|:x:|
|LesserThanOrEqual (`<=`)|:heavy_check_mark:|:heavy_check_mark:|:x:|:x:|
|Contains|:x:|:x:|:x:|:heavy_check_mark:|

At last, you should complete valued condition node build with a right hand operand with method `SetOperator(TDataType operand)`. For the case of sample above, `operand` would be set with value 18.

#### Composed condition nodes

Composed condition nodes allow you to make compositions (in tree) of valued condition nodes, also specifying a logical operator to apply between them (And/Or). This type of condition node actualy lets you build complex decision trees tailored to your needs. Taking for instance a little more complex example:

> Age > 18 **And** Gender = 'male'

It would be captured in the following way:

```csharp
WithCondition(cnb => 
    cnb.AsComposed()
        .WithLogicalOperator(LogicalOperators.And)
        .AddCondition(x1 =>
            x1.AsValued(ConditionTypes.Age)
                .OfDataType<int>()
                .WithComparisonOperator(Operators.GreaterThan)
                .SetOperand(18)
                .Build()
        )
        .AddCondition(x2 =>
            x2.AsValued(ConditionTypes.Gender)
                .OfDataType<string>()
                .WithComparisonOperator(Operators.Equal)
                .SetOperand("male")
                .Build()
        )
        .Build()
)
```

To build a composed condition node, you have to set the logical operator using `WithLogicalOperator(LogicalOperators logicalOperator)` method. Supported logical operators are:

- And (`&&`)
- Or (`||`)

Then you'd add as much condition nodes as you like to the composed condition node, keeping in mind that the logical operator will apply to all equally. If you use a `LogicalOperators.And` operator, all defined condition nodes under it would need to evaluate as true, but on the other hand, using `LogicalOperators.Or`, it would only need one of the conditions to evaluate as true for the whole composed condition node evaluate as true.

### Content

For last, you can set the rule content using the method:

```csharp
WithContentContainer(ContentContainer<TContentType> contentContainer)
```

This method allows you to set what value the rule is actually keeping so that when you match it, you can retrieve its' value.

The `ContentContainer<TContentType` abstraction exists so that rules content can be kept in-memory still in a serialized form, which you'll only need to concern about if you implement a new `IRulesDataSource<TContentType, TConditionType>` implementation. For most use cases, it should suffice simply doing `new ContentContainer<ContentTypes>(<your content type here>, (t) => <your content value/object here>)`.

### Completing the rule

By last, call `Build()` on your rule to finalize building it. Be aware that this method returns `RuleBuilderResult<TContentType, TConditionType>`, which may be a success or failure result. If result is success, it will contain the built rule. In case it is a failure, it will contain the errors occurred that led to fail to build the rule.

## Adding a new rule to Rules Engine

Up until now, all we've dealt with was how to build a single rule considering the own rules' logic. From now on, we have to consider the implications of adding a rules to a data set and ensure consistency in the end. When addind a rule, it must be ensured that:

- Rule name is unique (per each content type)
- Priority is unique (per each content type)
- Priority values are continguous

The method `AddRuleAsync(Rule<TContentType, TConditionType> rule, RuleAddPriorityOption ruleAddPriorityOption)` ensures those conditions are respected when adding a rule, and thus ensuring rules evaluation works as supposed when requesting rule matches. So, considering a already built rule and Rules Engine, adding a rule is simple as:

```csharp
RuleOperationResult ruleOperationResult = await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);
```

The parameter `ruleAddPriorityOption` can be configured with one of the following options:

- Add at top of rules, and by "top" we mean the least priority number.

    ```csharp
    RuleAddPriorityOption.AtTop
    ```

- Add at bottom of rules, where we mean the number next to the largest priority number already existent.

    ```csharp
    RuleAddPriorityOption.AtBottom
    ```

- Add at a rules' name priority, which causes that rule and subsequent rules with larger priority to be pushed down by one priority value.

    ```csharp
    RuleAddPriorityOption.ByRuleName(string ruleName)
    ```

    If a rule name that does not exist is specified, a failure result will be returned as result of the adding operation.

- Add at a specific priority number, which causes any rule existing on specified priority number and subsequent with larger priority to be pushed down by one priority value.

    ```csharp
    RuleAddPriorityOption.ByPriorityNumber(int priority)
    ```

    If a priority number lower than existent rules is specified, Rules Engine will fix it to be the same as the lowest existent priority (e.g. if -2 is specified, and lowest existent rule priority is 1, then Rules Engine will fix value to 1). On the other hand, if a priority number higher than existent rules plus 1 is specified, Rules Engine will fix it to be the same as the highest existent priority plus 1 (e.g. it 30 is specified and highest existent rule priority is 20, the Rules Engine will fix value to 21).

Be sure to check on the returned `ruleOperationResult` if the operation was a success, as it may contain any errors occurred while adding rule if operation was a failure.
