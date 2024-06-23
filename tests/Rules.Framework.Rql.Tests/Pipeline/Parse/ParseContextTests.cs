namespace Rules.Framework.Rql.Tests.Pipeline.Parse
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Tokens;
    using Xunit;

    public class ParseContextTests
    {
        private readonly IReadOnlyList<Token> _tokens;

        public ParseContextTests()
        {
            this._tokens = new List<Token>
            {
                Token.Create("test1", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 10), 10, TokenType.STRING),
                Token.Create("test2", false, null, RqlSourcePosition.From(1, 11), RqlSourcePosition.From(1, 20), 10, TokenType.STRING),
                Token.Create("test3", false, null, RqlSourcePosition.From(1, 21), RqlSourcePosition.From(1, 30), 10, TokenType.EOF),
            };
        }

        [Fact]
        public void EnterPanicMode_WhenInPanicModeAlready_ThrowsInvalidOperationException()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            var token = this._tokens.First();
            parseContext.EnterPanicMode("Panic is installed", token);

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => parseContext.EnterPanicMode("More panic", token));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Parse operation is already in panic mode.");
        }

        [Fact]
        public void EnterPanicMode_WhenNotInPanicModeAlready_SetsPanicModeInfo()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            var token = this._tokens.First();

            // Act
            parseContext.EnterPanicMode("Panic is installed", token);

            // Assert
            parseContext.PanicMode.Should().BeTrue();
            parseContext.PanicModeInfo.Should().NotBeNull();
            parseContext.PanicModeInfo.Message.Should().Be("Panic is installed");
            parseContext.PanicModeInfo.CauseToken.Should().Be(token);
        }

        [Fact]
        public void ExitPanicMode_WhenInPanicMode_ClearsPanicModeInfo()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            var token = this._tokens.First();
            parseContext.EnterPanicMode("Panic is installed", token);

            // Act
            parseContext.ExitPanicMode();

            // Assert
            parseContext.PanicMode.Should().BeFalse();
            parseContext.PanicModeInfo.Should().Be(PanicModeInfo.None);
        }

        [Fact]
        public void ExitPanicMode_WhenNotInPanicMode_ThrowsInvalidOperationException()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => parseContext.ExitPanicMode());

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Parse operation is not in panic mode.");
        }

        [Theory]
        [InlineData(1, "test1")]
        [InlineData(2, "test2")]
        [InlineData(3, "test3")]
        [InlineData(4, "test3")]
        public void GetCurrentToken_Conditions_ReturnsToken(int numberOfMoves, string expected)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.GetCurrentToken();

            // Assert
            actual.Should().NotBeNull();
            actual.Lexeme.Should().Be(expected);
        }

        [Fact]
        public void GetCurrentToken_NeverCalledMoveNext_ThrowsInvalidOperationException()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => parseContext.GetCurrentToken());

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Must invoke MoveNext() first.");
        }

        [Theory]
        [InlineData(1, "test2")]
        [InlineData(2, "test3")]
        [InlineData(3, "test3")]
        [InlineData(4, "test3")]
        public void GetNextToken_Conditions_ReturnsToken(int numberOfMoves, string expected)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.GetNextToken();

            // Assert
            actual.Should().NotBeNull();
            actual.Lexeme.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        public void IsEof_Conditions_ReturnsBoolean(int numberOfMoves, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.IsEof();

            // Assert
            actual.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, 0, TokenType.STRING, true)]
        [InlineData(1, 1, TokenType.STRING, true)]
        [InlineData(1, 2, TokenType.STRING, false)]
        [InlineData(1, 2, TokenType.EOF, true)]
        [InlineData(1, 3, TokenType.EOF, true)]
        public void IsMatchAtOffsetFromCurrent_Conditions_ReturnsBoolean(int numberOfMoves, int offset, object tokenType, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.IsMatchAtOffsetFromCurrent(offset, (TokenType)tokenType);

            // Assert
            actual.Should().Be(expectedResult);
        }

        [Fact]
        public void IsMatchAtOffsetFromCurrent_InvalidOffset_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);

            // Act
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => parseContext.IsMatchAtOffsetFromCurrent(0, TokenType.VAR));

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Contain("Offset must be zero or greater.");
        }

        [Theory]
        [InlineData(1, TokenType.STRING, true)]
        [InlineData(3, TokenType.STRING, false)]
        [InlineData(1, TokenType.EOF, false)]
        public void IsMatchCurrentToken_Conditions_ReturnsBoolean(int numberOfMoves, object tokenType, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.IsMatchCurrentToken((TokenType)tokenType);

            // Assert
            actual.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, TokenType.STRING, true)]
        [InlineData(2, TokenType.STRING, false)]
        [InlineData(1, TokenType.EOF, false)]
        public void IsMatchNextToken_Conditions_ReturnsBoolean(int numberOfMoves, object tokenType, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var actual = parseContext.IsMatchNextToken((TokenType)tokenType);

            // Assert
            actual.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        public void MoveNext_Conditions_ReturnsBoolean(int numberOfMoves, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);

            // Act
            bool? result = null;
            for (int i = 0; i < numberOfMoves; i++)
            {
                result = parseContext.MoveNext();
            }

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, TokenType.STRING, true)]
        [InlineData(2, TokenType.STRING, true)]
        [InlineData(3, TokenType.EOF, false)]
        [InlineData(1, TokenType.NUMBER, false)]
        public void MoveNextIfCurrentToken_Conditions_ReturnsBoolean(int numberOfMoves, object tokenType, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var result = parseContext.MoveNextIfCurrentToken((TokenType)tokenType);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, TokenType.STRING, true)]
        [InlineData(2, TokenType.STRING, false)]
        [InlineData(2, TokenType.EOF, true)]
        [InlineData(1, TokenType.NUMBER, false)]
        public void MoveNextIfNextToken_Conditions_ReturnsBoolean(int numberOfMoves, object tokenType, bool expectedResult)
        {
            // Arrange
            var parseContext = new ParseContext(this._tokens);
            for (int i = 0; i < numberOfMoves; i++)
            {
                parseContext.MoveNext();
            }

            // Act
            var result = parseContext.MoveNextIfNextToken((TokenType)tokenType);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}