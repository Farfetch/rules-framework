namespace Rules.Framework.Rql.Pipeline.Scan
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Tokens;

    internal class Scanner : IScanner
    {
        private const char DecimalSeparator = '.';

        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            { nameof(TokenType.ACTIVATE), TokenType.ACTIVATE },
            { nameof(TokenType.ALL), TokenType.ALL },
            { nameof(TokenType.AND), TokenType.AND },
            { nameof(TokenType.APPLY), TokenType.APPLY },
            { nameof(TokenType.AS), TokenType.AS },
            { nameof(TokenType.ARRAY), TokenType.ARRAY },
            { nameof(TokenType.BOTTOM), TokenType.BOTTOM },
            { nameof(TokenType.CONTENT), TokenType.CONTENT },
            { nameof(TokenType.CREATE), TokenType.CREATE },
            { nameof(TokenType.DEACTIVATE), TokenType.DEACTIVATE },
            { nameof(TokenType.ELSE), TokenType.ELSE },
            { "FALSE", TokenType.BOOL },
            { nameof(TokenType.FOR), TokenType.FOR },
            { nameof(TokenType.FOREACH), TokenType.FOREACH },
            { nameof(TokenType.IF), TokenType.IF },
            { nameof(TokenType.IN), TokenType.IN },
            { nameof(TokenType.IS), TokenType.IS },
            { nameof(TokenType.MATCH), TokenType.MATCH },
            { nameof(TokenType.NAME), TokenType.NAME },
            { nameof(TokenType.NOT), TokenType.NOT },
            { nameof(TokenType.NOTHING), TokenType.NOTHING },
            { nameof(TokenType.NUMBER), TokenType.NUMBER },
            { nameof(TokenType.OBJECT), TokenType.OBJECT },
            { nameof(TokenType.ON), TokenType.ON },
            { nameof(TokenType.ONE), TokenType.ONE },
            { nameof(TokenType.OR), TokenType.OR },
            { nameof(TokenType.PRIORITY), TokenType.PRIORITY },
            { nameof(TokenType.RULE), TokenType.RULE },
            { nameof(TokenType.RULES), TokenType.RULES },
            { nameof(TokenType.SEARCH), TokenType.SEARCH },
            { nameof(TokenType.SET), TokenType.SET },
            { nameof(TokenType.SINCE), TokenType.SINCE },
            { nameof(TokenType.TO), TokenType.TO },
            { nameof(TokenType.TOP), TokenType.TOP },
            { "TRUE", TokenType.BOOL },
            { nameof(TokenType.UNTIL), TokenType.UNTIL },
            { nameof(TokenType.UPDATE), TokenType.UPDATE },
            { nameof(TokenType.VAR), TokenType.VAR },
            { nameof(TokenType.WHEN), TokenType.WHEN },
            { nameof(TokenType.WITH), TokenType.WITH },
        };

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
                do
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
                } while (scanContext.MoveNext());

                using (scanContext.BeginTokenCandidate())
                {
                    CreateToken(scanContext, string.Empty, TokenType.EOF, literal: null!);
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
            while (IsAlphaNumeric(scanContext.GetNextChar()))
            {
                if (!scanContext.MoveNext())
                {
                    break;
                }
            }
        }

        private static Token CreateToken(ScanContext scanContext, TokenType tokenType)
        {
            string lexeme = scanContext.ExtractLexeme();
            return CreateToken(scanContext, lexeme, tokenType, literal: null!);
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
            string lexeme;
            while (scanContext.GetNextChar() != '$' && scanContext.MoveNext())
            {
                continue;
            }

            if (scanContext.IsEof())
            {
                lexeme = scanContext.ExtractLexeme();
                scanContext.TokenCandidate.MarkAsError($"Unterminated date '{lexeme}'.");
                return Token.None;
            }

            _ = scanContext.MoveNext();

            // Trim the surrounding dollar symbols.
            lexeme = scanContext.ExtractLexeme();
            var value = Regex.Unescape(lexeme.Substring(1, lexeme.Length - 2));
            if (!DateTime.TryParse(value, out _))
            {
                scanContext.TokenCandidate.MarkAsError($"Invalid date '{lexeme}'.");
                return Token.None;
            }

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

            if (scanContext.GetNextChar() == DecimalSeparator && scanContext.MoveNext() && IsNumeric(scanContext.GetNextChar()))
            {
                ConsumeDigits(scanContext);
                lexeme = scanContext.ExtractLexeme();
                return CreateToken(scanContext, lexeme, TokenType.DECIMAL, decimal.Parse(lexeme, CultureInfo.InvariantCulture));
            }

            if (ConsumeRemainingTokenCharacters(scanContext))
            {
                lexeme = scanContext.ExtractLexeme();
                scanContext.TokenCandidate.MarkAsError($"Invalid number '{lexeme}'.");
                return Token.None;
            }

            lexeme = scanContext.ExtractLexeme();
            return CreateToken(scanContext, lexeme, TokenType.INT, int.Parse(lexeme, CultureInfo.InvariantCulture));

            static void ConsumeDigits(ScanContext scanContext)
            {
                while (IsNumeric(scanContext.GetNextChar()) && scanContext.MoveNext())
                {
                    continue;
                }
            }

            static bool ConsumeRemainingTokenCharacters(ScanContext scanContext)
            {
                var consumed = false;
                while ((IsAlphaNumeric(scanContext.GetNextChar()) || scanContext.GetNextChar() == DecimalSeparator) && scanContext.MoveNext())
                {
                    if (!consumed)
                    {
                        consumed = true;
                    }
                }

                return consumed;
            }
        }

        private static Token HandlePlaceholder(ScanContext scanContext)
        {
            ConsumeAlphaNumeric(scanContext);
            var lexeme = scanContext.ExtractLexeme();
            var literal = lexeme.Substring(1, lexeme.Length - 1);
            return CreateToken(scanContext, lexeme, TokenType.PLACEHOLDER, literal);
        }

        private static Token HandleString(ScanContext scanContext)
        {
            string lexeme;
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
                lexeme = scanContext.ExtractLexeme();
                scanContext.TokenCandidate.MarkAsError($"Unterminated string '{lexeme}'.");
                return Token.None;
            }

            // The closing ".
            _ = scanContext.MoveNext();

            // Trim the surrounding quotes.
            lexeme = scanContext.ExtractLexeme();
            var value = Regex.Unescape(lexeme.Substring(1, lexeme.Length - 2));
            return CreateToken(scanContext, lexeme, TokenType.STRING, value);
        }

        private static bool IsAlpha(char @char) => @char >= 'A' && @char <= 'Z' || @char >= 'a' && @char <= 'z' || @char == '_';

        private static bool IsAlphaNumeric(char @char) => IsAlpha(@char) || IsNumeric(@char);

        private static bool IsEscape(char @char) => @char == '#';

        private static bool IsNumeric(char @char) => @char >= '0' && @char <= '9';

        private static bool IsWhiteSpace(char @char) => @char == ' ' || @char == '\r' || @char == '\t' || @char == '\n';

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
                    return CreateToken(scanContext, TokenType.SLASH);

                case '*':
                    return CreateToken(scanContext, TokenType.STAR);

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
                        return CreateToken(scanContext, TokenType.LESS_THAN_OR_EQUAL);
                    }

                    if (scanContext.MoveNextConditionally('>'))
                    {
                        return CreateToken(scanContext, TokenType.NOT_EQUAL);
                    }

                    return CreateToken(scanContext, TokenType.LESS_THAN);

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