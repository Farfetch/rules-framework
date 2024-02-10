namespace Rules.Framework.Rql.Tokens
{
    internal enum TokenType
    {
        None = 0,

        #region Keywords

        [AllowAsIdentifier(RequireEscaping = true)]
        ACTIVATE,

        [AllowAsIdentifier]
        ALL,

        [AllowAsIdentifier]
        AND,

        [AllowAsIdentifier]
        APPLY,

        [AllowAsIdentifier(RequireEscaping = true)]
        ARRAY,

        [AllowAsIdentifier]
        AS,

        [AllowAsIdentifier]
        BOTTOM,

        [AllowAsIdentifier]
        CONTENT,

        [AllowAsIdentifier(RequireEscaping = true)]
        CREATE,

        [AllowAsIdentifier]
        DEACTIVATE,

        [AllowAsIdentifier(RequireEscaping = true)]
        ELSE,

        [AllowAsIdentifier(RequireEscaping = true)]
        FOR,

        [AllowAsIdentifier(RequireEscaping = true)]
        FOREACH,

        [AllowAsIdentifier(RequireEscaping = true)]
        IF,

        [AllowAsIdentifier]
        IS,

        [AllowAsIdentifier(RequireEscaping = true)]
        MATCH,

        [AllowAsIdentifier]
        NAME,

        [AllowAsIdentifier(RequireEscaping = true)]
        NOTHING,

        [AllowAsIdentifier]
        NUMBER,

        [AllowAsIdentifier(RequireEscaping = true)]
        OBJECT,

        [AllowAsIdentifier]
        ON,

        [AllowAsIdentifier]
        ONE,

        [AllowAsIdentifier]
        OR,

        [AllowAsIdentifier]
        PRIORITY,

        [AllowAsIdentifier]
        RULE,

        [AllowAsIdentifier]
        RULES,

        [AllowAsIdentifier(RequireEscaping = true)]
        SEARCH,

        [AllowAsIdentifier]
        SET,

        [AllowAsIdentifier]
        SINCE,

        [AllowAsIdentifier]
        TO,

        [AllowAsIdentifier]
        TOP,

        [AllowAsIdentifier]
        UNTIL,

        [AllowAsIdentifier(RequireEscaping = true)]
        UPDATE,

        [AllowAsIdentifier(RequireEscaping = true)]
        VAR,

        [AllowAsIdentifier]
        WHEN,

        [AllowAsIdentifier]
        WITH,

        #endregion Keywords

        #region Literals

        BOOL,
        DATE,
        DECIMAL,

        [AllowAsIdentifier]
        IDENTIFIER,

        INT,
        PLACEHOLDER,
        STRING,

        #endregion Literals

        #region Operators

        ASSIGN,
        EQUAL,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        IN,
        MINUS,
        STAR,
        NOT_EQUAL,
        NOT,
        PLUS,
        SLASH,

        #endregion Operators

        #region Tokens

        BRACE_LEFT,
        BRACE_RIGHT,
        BRACKET_LEFT,
        BRACKET_RIGHT,
        COMMA,
        DOT,
        ESCAPE,
        SEMICOLON,
        STRAIGHT_BRACKET_LEFT,
        STRAIGHT_BRACKET_RIGHT,
        EOF,

        #endregion Tokens
    }
}