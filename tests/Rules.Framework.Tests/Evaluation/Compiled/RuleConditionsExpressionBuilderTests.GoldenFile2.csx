private Func<object, Operators, object, string> Evaluate1;
private Func<IDictionary<string, object>, string, object> GetValueOrDefault1;

internal bool Main(EvaluationContext evaluationContext)
{
    bool cnd0Result;
    object cnd0LeftOperand;
    object cnd0RightOperand;
    string cnd0Multiplicity;
    bool cnd1Result;
    object cnd1LeftOperand;
    object cnd1RightOperand;
    string cnd1Multiplicity;

    cnd0LeftOperand = GetValueOrDefault1.Invoke(evaluationContext.get_Conditions(), "NumberOfSales");
    cnd0RightOperand = 100;

    if (cnd0LeftOperand == null)
    {
        if (evaluationContext.get_MissingConditionBehavior() == MissingConditionBehaviors.Discard)
        {
            cnd0Result = false;
            goto cnd0LabelEndValueConditionNode;
        }

        if (evaluationContext.get_MatchMode() == MatchModes.Search)
        {
            cnd0Result = true;
            goto cnd0LabelEndValueConditionNode;
        }
    }
    cnd0Multiplicity = Evaluate1.Invoke(cnd0LeftOperand, Operators.Equal, cnd0RightOperand);

    switch (cnd0Multiplicity)
    {
        case "one-to-one":
            cnd0Result = true;
            break;
        default:
            break;
    }
cnd0LabelEndValueConditionNode:

    cnd1LeftOperand = GetValueOrDefault1.Invoke(evaluationContext.get_Conditions(), "IsoCountryCode");
    cnd1RightOperand = "GB";

    if (cnd1LeftOperand == null)
    {
        if (evaluationContext.get_MissingConditionBehavior() == MissingConditionBehaviors.Discard)
        {
            cnd1Result = false;
            goto cnd1LabelEndValueConditionNode;
        }

        if (evaluationContext.get_MatchMode() == MatchModes.Search)
        {
            cnd1Result = true;
            goto cnd1LabelEndValueConditionNode;
        }
    }
    cnd1Multiplicity = Evaluate1.Invoke(cnd1LeftOperand, Operators.Equal, cnd1RightOperand);

    switch (cnd1Multiplicity)
    {
        case "one-to-one":
            cnd1Result = true;
            break;
        default:
            break;
    }
cnd1LabelEndValueConditionNode:
    bool Result = cnd0Result || cnd1Result;
    return Result;

}