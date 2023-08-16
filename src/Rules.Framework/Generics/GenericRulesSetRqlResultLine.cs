namespace Rules.Framework.Generics
{
    public class GenericRulesSetRqlResultLine
    {
        public GenericRulesSetRqlResultLine(int lineNumber, GenericRule rule)
        {
            this.LineNumber = lineNumber;
            this.Rule = rule;
        }

        public int LineNumber { get; }

        public GenericRule Rule { get; }
    }
}