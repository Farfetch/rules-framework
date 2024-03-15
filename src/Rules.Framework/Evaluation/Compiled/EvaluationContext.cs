namespace Rules.Framework.Evaluation.Compiled
{
    using System.Collections.Generic;

    internal sealed class EvaluationContext<TConditionType>
    {
        public EvaluationContext(
            IDictionary<TConditionType, object> conditions,
            MatchModes matchMode,
            MissingConditionBehaviors missingConditionBehavior)
        {
            this.Conditions = conditions;
            this.MatchMode = matchMode;
            this.MissingConditionBehavior = missingConditionBehavior;
        }

        public IDictionary<TConditionType, object> Conditions { get; }

        public MatchModes MatchMode { get; }

        public MissingConditionBehaviors MissingConditionBehavior { get; }
    }
}