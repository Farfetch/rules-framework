namespace Rules.Framework.Rql.Ast.Segments
{
    internal interface ISegmentVisitor<out T>
    {
        T VisitCardinalitySegment(CardinalitySegment cardinalitySegment);

        T VisitDatesIntervalSegment(DatesIntervalSegment datesIntervalSegment);

        T VisitInputConditionSegment(InputConditionSegment inputConditionSegment);

        T VisitInputConditionsSegment(InputConditionsSegment inputConditionsSegment);

        T VisitMatchDateSegment(MatchDateSegment matchDateSegment);

        T VisitNoneSegment(NoneSegment noneSegment);

        T VisitOperatorSegment(OperatorSegment operatorSegment);

        T VisitRulesetSegment(RulesetSegment rulesetSegment);
    }
}