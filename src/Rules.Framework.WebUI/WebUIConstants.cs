namespace Rules.Framework.WebUI
{
    internal static class WebUIConstants
    {
        public const string IsUniqueInstanceStorageKey = "Is-Unique-Instance";

        public const string OptionAutoCreateRulesetsDescription = "Behavior of the rules engine when inserting a new rule" +
            " whose ruleset does not exist. If set to true, the ruleset will be created before inserting the rule, otherwise rule" +
            " insertion will fail.";

        public const string OptionDataTypeDefaultsDescription = "The default of each data type to be used when a specific" +
            " condition is not given on 'conditions' parameter when matching (one or all) rules of a particular ruleset.";

        public const string OptionMissingConditionBehaviorDescription = "Behavior of the rules engine when matching (one or all)" +
            " rules of a particular ruleset and a specific condition is not given on 'conditions' parameter.";

        public const string OptionMissingConditionBehaviorDiscardDescription = "When a condition is missing, instructs the rules" +
            " engine to discard the rule under evaluation (rule is considered not a match).";

        public const string OptionMissingConditionBehaviorUseDataTypeDefaultDescription = "When a condition is missing, instructs" +
            " the rules engine to use the configured data type default for rule's condition.";

        public const string OptionPriorityCriteriaBottommostRuleWinsDescription = "Rules with the highest priority number have greater" +
            " priority than the ones with lowest.";

        public const string OptionPriorityCriteriaBottommostRuleWinsName = "Bottommost rule wins";

        public const string OptionPriorityCriteriaDescription = "Sets the way the rules engine interprets the 'Priority' for each rule.";

        public const string OptionPriorityCriteriaTopmostRuleWinsDescription = "Rules with the lowest priority number have greater" +
            " priority than the ones with highest.";

        public const string OptionPriorityCriteriaTopmostRuleWinsName = "Topmost rule wins";

        public const string SelectedInstanceStorageKey = "Selected-Instance-ID";

        public const string SelectedRulesetsStorageKey = "Selected-Ruleset-IDs";
    }
}