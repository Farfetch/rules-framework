namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;

    internal sealed class EvaluationContext
    {
        public EvaluationContext(
            IDictionary<string, object> conditions,
            MatchModes matchMode,
            MissingConditionBehaviors missingConditionBehavior)
        {
            this.Conditions = conditions;
            this.MatchMode = matchMode;
            this.MissingConditionBehavior = missingConditionBehavior;
        }

        public IDictionary<string, object> Conditions { get; }

        public MatchModes MatchMode { get; }

        public MissingConditionBehaviors MissingConditionBehavior { get; }
    }
}