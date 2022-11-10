// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to
// this project. Project-level suppressions either have no target or are given a specific target and
// scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.InvalidRulesEngineOptionsException")]
[assembly: SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.Core.DataTypes")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.PriorityCriterias")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.MissingConditionBehaviors")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.Core.Operators")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.Core.LogicalOperators")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.Core.DataTypes")]
[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>", Scope = "member", Target = "~P:Rules.Framework.Core.ConditionNodes.IValueConditionNode`1.Operator")]
[assembly: SuppressMessage("Microsoft.Performace", "CA1801", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.InvalidRulesEngineOptionsException")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>", Scope = "member", Target = "~M:Rules.Framework.RulesEngine`2.AddRuleAsync(Rules.Framework.Core.Rule{`0,`1})~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "<Pending>", Scope = "type", Target = "~T:Rules.Framework.PriorityOptions")]
[assembly: SuppressMessage("Major Bug", "S2259:Null pointers should not be dereferenced", Justification = "That is validated somes lines above, and if it is null, a RuleOperationResult with error is returned right away.", Scope = "member", Target = "~M:Rules.Framework.RulesEngine`2.UpdateRuleInternalAsync(Rules.Framework.Core.Rule{`0,`1})~System.Threading.Tasks.Task{Rules.Framework.RuleOperationResult}")]
[assembly: SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Generic purpose", Scope = "type", Target = "~T:Rules.Framework.Generic.GenericConditionNode`1")]