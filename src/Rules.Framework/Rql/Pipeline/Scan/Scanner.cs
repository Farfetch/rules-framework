namespace Rules.Framework.Rql.Pipeline.Scan
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Tokens;

    internal class Scanner
    {
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            { "ACTIVATE", TokenType.ACTIVATE },
            { "ALL", TokenType.ALL },
            { "AND", TokenType.AND },
            { "APPLY", TokenType.APPLY },
            { "AS", TokenType.AS },
            { "ARRAY", TokenType.ARRAY },
            { "BEGINS", TokenType.BEGINS },
            { "BOTTOM", TokenType.BOTTOM },
            { "CONTENT", TokenType.CONTENT },
            { "CREATE", TokenType.CREATE },
            { "DEACTIVATE", TokenType.DEACTIVATE },
            { "ELSE", TokenType.ELSE },
            { "ENDS", TokenType.ENDS },
            { "FALSE", TokenType.BOOL },
            { "FOR", TokenType.FOR },
            { "FOREACH", TokenType.FOREACH },
            { "IF", TokenType.IF },
            { "IN", TokenType.IN },
            { "IS", TokenType.IS },
            { "MATCH", TokenType.MATCH },
            { "NAME", TokenType.NAME },
            { "NOTHING", TokenType.NOTHING },
            { "NUMBER", TokenType.NUMBER },
            { "OBJECT", TokenType.OBJECT },
            { "ON", TokenType.ON },
            { "ONE", TokenType.ONE },
            { "OR", TokenType.OR },
            { "PRIORITY", TokenType.PRIORITY },
            { "RANGE", TokenType.RANGE },
            { "RULE", TokenType.RULE },
            { "RULES", TokenType.RULES },
            { "SEARCH", TokenType.SEARCH },
            { "SET", TokenType.SET },
            { "TO", TokenType.TO },
            { "TOP", TokenType.TOP },
            { "TRUE", TokenType.BOOL },
            { "UPDATE", TokenType.UPDATE },
            { "VAR", TokenType.VAR },
            { "WHEN", TokenType.WHEN },
            { "WITH", TokenType.WITH },
        };

        public Scanner()
        {
        }

        public ScanResult ScanTokens(string source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var tokens = new List<Token>();
            using var messageContainer = new MessageContainer();
            if (!string.IsNullOrWhiteSpace(source))
            {
                var scanContext = new ScanContext(source);
                while (scanContext.MoveNext())
                {
                    using (scanContext.BeginTokenCandidate())
                    {
                        var token = ScanNextToken(scanContext);
                        if (token != Token.None)
                        {
                            tokens.Add(token);
                        }

                        if (scanContext.TokenCandidate.HasError)
                        {
                            messageContainer.Error(
                                scanContext.TokenCandidate.Message,
                                scanContext.TokenCandidate.BeginPosition,
                                scanContext.TokenCandidate.EndPosition);
                        }
                    }
                }

                using (scanContext.BeginTokenCandidate())
                {
                    CreateToken(scanContext, string.Empty, TokenType.EOF, literal: null);
                }
            }

            var messages = messageContainer.Messages;
            if (messageContainer.ErrorsCount > 0)
            {
                return ScanResult.CreateError(messages);
            }

            return ScanResult.CreateSuccess(tokens, messages);
        }

        private static void ConsumeAlphaNumeric(ScanContext scanContext)
        {
            while (IsAlphaNumeric(scanContext.GetNextChar()) && scanContext.MoveNext()) { }
        }

        private static Token CreateToken(ScanContext scanContext, TokenType tokenType)
        {
            string lexeme = scanContext.ExtractLexeme();
            return CreateToken(scanContext, lexeme, tokenType, literal: null);
        }

        private static Token CreateToken(ScanContext scanContext, string lexeme, TokenType tokenType, object literal)
        {
            var isEscaped = lexeme.Length > 0 && IsEscape(lexeme[0]);
            return Token.Create(
                lexeme,
                isEscaped,
                literal,
                scanContext.TokenCandidate.BeginPosition,
                scanContext.TokenCandidate.EndPosition,
                scanContext.TokenCandidate.Length,
                tokenType);
        }

        private static Token HandleDate(ScanContext scanContext)
        {
            while (scanContext.GetNextChar() != '$' && scanContext.MoveNext())
            {
            }

            if (scanContext.IsEof())
            {
                scanContext.TokenCandidate.MarkAsError("Unterminated date.");
                return Token.None;
            }

            _ = scanContext.MoveNext();

            // Trim the surrounding dollar symbols.
            var lexeme = scanContext.ExtractLexeme();
            var value = Regex.Unescape(lexeme[1..^1]);
            return CreateToken(scanContext, lexeme, TokenType.DATE, value);
        }

        private static Token HandleIdentifier(ScanContext scanContext)
        {
            ConsumeAlphaNumeric(scanContext);
            var lexeme = scanContext.ExtractLexeme();
            var lexemeUpper = lexeme.ToUpperInvariant();
            if (!keywords.TryGetValue(lexemeUpper, out TokenType type))
            {
                return CreateToken(scanContext, lexeme, TokenType.IDENTIFIER, lexeme);
            }

            if (type == TokenType.BOOL)
            {
                return CreateToken(scanContext, lexemeUpper, type, bool.Parse(lexeme));
            }

            return CreateToken(scanContext, type);
        }

        private static Token HandleNumber(ScanContext scanContext)
        {
            string lexeme;
            ConsumeDigits(scanContext);

            if (scanContext.GetNextChar() == '.' && scanContext.MoveNext() && IsNumeric(scanContext.GetNextChar()))
            {
                ConsumeDigits(scanContext);
                lexeme = scanContext.ExtractLexeme();
                return CreateToken(scanContext, lexeme, TokenType.DECIMAL, decimal.Parse(lexeme, CultureInfo.InvariantCulture));
            }

            lexeme = scanContext.ExtractLexeme();
            return CreateToken(scanContext, lexeme, TokenType.INT, int.Parse(lexeme, CultureInfo.InvariantCulture));

            static void ConsumeDigits(ScanContext scanContext)
            {
                while (IsNumeric(scanContext.GetNextChar()) && scanContext.MoveNext()) { }
            }
        }

        private static Token HandlePlaceholder(ScanContext scanContext)
        {
            ConsumeAlphaNumeric(scanContext);
            var lexeme = scanContext.ExtractLexeme();
            var literal = lexeme[1..];
            return CreateToken(scanContext, lexeme, TokenType.PLACEHOLDER, literal);
        }

        private static Token HandleString(ScanContext scanContext)
        {
            while (scanContext.GetNextChar() != '"' && scanContext.MoveNext())
            {
                // Support escaping double quotes.
                if (scanContext.GetCurrentChar() == '\\' && scanContext.GetNextChar() == '"')
                {
                    _ = scanContext.MoveNext();
                }
            }

            if (scanContext.IsEof())
            {
                scanContext.TokenCandidate.MarkAsError("Unterminated string");
                return Token.None;
            }

            // The closing ".
            _ = scanContext.MoveNext();

            // Trim the surrounding quotes.
            var lexeme = scanContext.ExtractLexeme();
            var value = Regex.Unescape(lexeme[1..^1]);
            return CreateToken(scanContext, lexeme, TokenType.STRING, value);
        }

        private static bool IsAlpha(char @char) => @char >= 'A' && @char <= 'Z' || @char >= 'a' && @char <= 'z' || @char == '_';

        private static bool IsAlphaNumeric(char @char) => IsAlpha(@char) || IsNumeric(@char);

        private static bool IsEscape(char @char) => @char == '#';

        private static bool IsNumeric(char @char) => @char >= '0' && @char <= '9';

        private static Token ScanNextToken(ScanContext scanContext)
        {
            var @char = scanContext.GetCurrentChar();
            switch (@char)
            {
                case '(':
                    return CreateToken(scanContext, TokenType.BRACKET_LEFT);

                case ')':
                    return CreateToken(scanContext, TokenType.BRACKET_RIGHT);

                case '{':
                    return CreateToken(scanContext, TokenType.BRACE_LEFT);

                case '}':
                    return CreateToken(scanContext, TokenType.BRACE_RIGHT);

                case ';':
                    return CreateToken(scanContext, TokenType.SEMICOLON);

                case ',':
                    return CreateToken(scanContext, TokenType.COMMA);

                case '.':
                    return CreateToken(scanContext, TokenType.DOT);

                case '+':
                    return CreateToken(scanContext, TokenType.PLUS);

                case '-':
                    return CreateToken(scanContext, TokenType.MINUS);

                case '[':
                    return CreateToken(scanContext, TokenType.STRAIGHT_BRACKET_LEFT);

                case ']':
                    return CreateToken(scanContext, TokenType.STRAIGHT_BRACKET_RIGHT);

                case '/':
                    return CreateToken(scanContext, TokenType.DIVIDE);

                case '*':
                    return CreateToken(scanContext, TokenType.MULTIPLY);

                case '!':
                    if (scanContext.MoveNextConditionally('='))
                    {
                        return CreateToken(scanContext, TokenType.NOT_EQUAL);
                    }

                    scanContext.TokenCandidate.MarkAsError("Expected '=' after '!'");
                    return Token.None;

                case '=':
                    if (scanContext.MoveNextConditionally('='))
                    {
                        return CreateToken(scanContext, TokenType.EQUAL);
                    }

                    return CreateToken(scanContext, TokenType.ASSIGN);

                case '>':
                    if (scanContext.MoveNextConditionally('='))
                    {
                        return CreateToken(scanContext, TokenType.GREATER_THAN_OR_EQUAL);
                    }

                    return CreateToken(scanContext, TokenType.GREATER_THAN);

                case '<':
                    if (scanContext.MoveNextConditionally('='))
                    {
                        return CreateToken(scanContext, TokenType.GREATER_THAN_OR_EQUAL);
                    }

                    if (scanContext.MoveNextConditionally('>'))
                    {
                        return CreateToken(scanContext, TokenType.NOT_EQUAL);
                    }

                    return CreateToken(scanContext, TokenType.GREATER_THAN);

                case '$':
                    return HandleDate(scanContext);

                case ' ':
                case '\r':
                case '\t':
                case '\n':
                    // Ignore whitespace.
                    return Token.None;

                case '@':
                    return HandlePlaceholder(scanContext);

                case '"':
                    return HandleString(scanContext);

                default:
                    if (IsNumeric(@char))
                    {
                        return HandleNumber(scanContext);
                    }

                    if (IsAlpha(@char))
                    {
                        return HandleIdentifier(scanContext);
                    }

                    if (IsEscape(@char))
                    {
                        if (!scanContext.MoveNext())
                        {
                            scanContext.TokenCandidate.MarkAsError($"Expected char after '{@char}'");
                            return Token.None;
                        }

                        return HandleIdentifier(scanContext);
                    }

                    scanContext.TokenCandidate.MarkAsError($"Invalid char '{@char}'");
                    return Token.None;
            }
        }
    }
}