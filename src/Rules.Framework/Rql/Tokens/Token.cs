namespace Rules.Framework.Rql.Tokens
{
    using System;
    using Rules.Framework.Rql;

    internal class Token
    {
        private Token(string lexeme, bool isEscaped, object literal, RqlSourcePosition beginPosition, RqlSourcePosition endPosition, uint length, TokenType type)
        {
            this.Length = length;
            this.Lexeme = lexeme;
            this.IsEscaped = isEscaped;
            this.Literal = literal;
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
            this.Type = type;
        }

        public static Token None { get; } = new Token(lexeme: null, isEscaped: false, literal: null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 0, TokenType.None);

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public bool IsEscaped { get; }

        public uint Length { get; }

        public string Lexeme { get; }

        public object Literal { get; }

        public TokenType Type { get; }

        public string UnescapedLexeme => this.IsEscaped ? this.Lexeme[1..] : this.Lexeme;

        public static Token Create(string lexeme, bool isEscaped, object literal, RqlSourcePosition beginPosition, RqlSourcePosition endPosition, uint length, TokenType type)
        {
            if (lexeme is null)
            {
                throw new ArgumentNullException(nameof(lexeme), $"'{nameof(lexeme)}' cannot be null.");
            }

            return new Token(lexeme, isEscaped, literal, beginPosition, endPosition, length, type);
        }

        public override string ToString() => $"[{this.Type}] {this.Lexeme}: {this.Literal} @{this.BeginPosition},{this.EndPosition}";
    }
}