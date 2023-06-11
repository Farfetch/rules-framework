namespace Rules.Framework.Rql.Tokens
{
    internal enum TokenType
    {
        None = 0,

        #region Keywords

        ACTIVATE,
        ALL,
        AND,
        APPLY,
        AS,
        BOTTOM,
        CONTENT,
        CREATE,
        DEACTIVATE,
        ENDS,
        FOR,
        IS,
        MATCH,
        NAME,
        NUMBER,
        ON,
        ONE,
        OR,
        PRIORITY,
        RANGE,
        RULE,
        RULES,
        SEARCH,
        SET,
        STARTS,
        TO,
        TOP,
        UPDATE,
        WHEN,
        WITH,

        #endregion Keywords

        #region Literals

        STRING,
        INT,
        DECIMAL,
        BOOL,
        IDENTIFIER,
        PLACEHOLDER,

        #endregion Literals

        #region Operators

        EQUAL,
        GREATER_THAN,
        GREATER_THAN_OR_EQUAL,
        LESS_THAN,
        LESS_THAN_OR_EQUAL,
        IN,
        NOT_EQUAL,
        NOT_IN,

        #endregion Operators

        #region Tokens

        BRACE_LEFT,
        BRACE_RIGHT,
        PARENTHESIS_LEFT,
        PARENTHESIS_RIGHT,
        SEMICOLON,
        COMMA,
        EOF,

        #endregion Tokens
    }
}