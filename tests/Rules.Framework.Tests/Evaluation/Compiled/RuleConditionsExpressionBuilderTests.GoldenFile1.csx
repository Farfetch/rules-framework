private Func<object, Operators, object, string> Evaluate1;
private Func<IDictionary<ConditionType, object>, ConditionType, object> GetValueOrDefault1;

internal bool Main(EvaluationContext<ConditionType> evaluationContext)
{
    bool _C0_result;
    object _C0_leftOperand;
    object _C0_rightOperand;
    string _C0_multiplicity;
    bool _C1_result;
    object _C1_leftOperand;
    object _C1_rightOperand;
    string _C1_multiplicity;

    _C0_leftOperand = GetValueOrDefault1.Invoke(evaluationContext.get_Conditions(), ConditionType.NumberOfSales);
    _C0_rightOperand = 100;

    if (_C0_leftOperand == null)
    {
        if (evaluationContext.get_MissingConditionBehavior() == MissingConditionBehaviors.Discard)
        {
            _C0_result = false;
            goto _C0_Label_EndValueConditionNode;
        }

        if (evaluationContext.get_MatchMode() == MatchModes.Search)
        {
            _C0_result = true;
            goto _C0_Label_EndValueConditionNode;
        }
    }
    _C0_multiplicity = Evaluate1.Invoke(_C0_leftOperand, Operators.Equal, _C0_rightOperand);

    switch (_C0_multiplicity)
    {
        case "one-to-one":
            _C0_result = true;
            break;
        default:
            break;
    }
_C0_Label_EndValueConditionNode:

    _C1_leftOperand = GetValueOrDefault1.Invoke(evaluationContext.get_Conditions(), ConditionType.IsoCountryCode);
    _C1_rightOperand = "GB";

    if (_C1_leftOperand == null)
    {
        if (evaluationContext.get_MissingConditionBehavior() == MissingConditionBehaviors.Discard)
        {
            _C1_result = false;
            goto _C1_Label_EndValueConditionNode;
        }

        if (evaluationContext.get_MatchMode() == MatchModes.Search)
        {
            _C1_result = true;
            goto _C1_Label_EndValueConditionNode;
        }
    }
    _C1_multiplicity = Evaluate1.Invoke(_C1_leftOperand, Operators.Equal, _C1_rightOperand);

    switch (_C1_multiplicity)
    {
        case "one-to-one":
            _C1_result = true;
            break;
        default:
            break;
    }
_C1_Label_EndValueConditionNode:
    bool result = _C0_result && _C1_result;
    return result;

}