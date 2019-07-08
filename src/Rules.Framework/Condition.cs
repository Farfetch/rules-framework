namespace Rules.Framework
{
    public class Condition<TConditionType>
    {
        public TConditionType Type { get; set; }

        public object Value { get; set; }
    }
}