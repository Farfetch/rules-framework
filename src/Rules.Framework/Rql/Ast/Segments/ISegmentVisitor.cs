namespace Rules.Framework.Rql.Ast.Segments
{
    internal interface ISegmentVisitor<T>
    {
        T VisitCardinalitySegment(CardinalitySegment cardinalitySegment);

        T VisitComposedConditionSegment(ComposedConditionSegment composedConditionSegment);

        T VisitConditionGroupingSegment(ConditionGroupingSegment conditionGroupingSegment);

        T VisitDateEndSegment(DateEndSegment dateEndSegment);

        T VisitInputConditionSegment(InputConditionSegment inputConditionSegment);

        T VisitInputConditionsSegment(InputConditionsSegment inputConditionsSegment);

        T VisitNoneSegment(NoneSegment noneSegment);

        T VisitOperatorSegment(OperatorSegment operatorSegment);

        T VisitPriorityOptionSegment(PriorityOptionSegment priorityOptionSegment);

        T VisitUpdatableAttributeSegment(UpdatableAttributeSegment updatableSegment);

        T VisitValueConditionSegment(ValueConditionSegment valueConditionSegment);
    }
}