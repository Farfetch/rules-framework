namespace Rules.Framework.Rql
{
    internal class AssistSuggestion : IAssistSuggestion
    {
        private AssistSuggestion()
        {
        }

        public string Lexeme { get; private set; }

        public static AssistSuggestion New(string lexeme) => new AssistSuggestion
        {
            Lexeme = lexeme,
        };
    }
}