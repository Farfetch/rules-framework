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
        ARRAY,
        AS,
        ASSIGN,
        BEGINS,
        BOTTOM,
        CONTENT,
        CREATE,
        DEACTIVATE,
        ELSE,
        ENDS,
        FOR,
        FOREACH,
        IF,
        IS,
        MATCH,
        NAME,
        NOTHING,
        NUMBER,
        OBJECT,
        ON,
        ONE,
        OR,
        PRIORITY,
        RANGE,
        RULE,
        RULES,
        SEARCH,
        SET,
        TO,
        TOP,
        UPDATE,
        VAR,
        WHEN,
        WITH,

        #endregion Keywords

        #region Literals

        BOOL,
        DATE,
        DECIMAL,
        IDENTIFIER,
        INT,
        PLACEHOLDER,
        STRING,

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
        BRACKET_LEFT,
        BRACKET_RIGHT,
        COMMA,
        DOT,
        ESCAPE,
        MINUS,
        SEMICOLON,
        STRAIGHT_BRACKET_LEFT,
        STRAIGHT_BRACKET_RIGHT,
        EOF,

        #endregion Tokens
    }
}