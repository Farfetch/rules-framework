namespace Rules.Framework.Rql.Ast.Segments
{
    internal interface ISegmentVisitor<out T>
    {
        T VisitCardinalitySegment(CardinalitySegment cardinalitySegment);

        T VisitInputConditionSegment(InputConditionSegment inputConditionSegment);

        T VisitInputConditionsSegment(InputConditionsSegment inputConditionsSegment);

        T VisitNoneSegment(NoneSegment noneSegment);

        T VisitOperatorSegment(OperatorSegment operatorSegment);
    }
}