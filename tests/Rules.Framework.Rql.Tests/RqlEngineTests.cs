namespace Rules.Framework.Rql.Tests
{
    using System.Globalization;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tests.Stubs;
    using Rules.Framework.Rql.Tests.TestStubs;
    using Rules.Framework.Rql.Tokens;
    using Xunit;

    public class RqlEngineTests
    {
        private readonly IInterpreter interpreter;
        private readonly IParser parser;
        private readonly RqlEngine<ContentType, ConditionType> rqlEngine;
        private readonly ITokenScanner tokenScanner;

        public RqlEngineTests()
        {
            this.tokenScanner = Mock.Of<ITokenScanner>();
            this.parser = Mock.Of<IParser>();
            this.interpreter = Mock.Of<IInterpreter>();
            var rqlEngineArgs = new RqlEngineArgs
            {
                Interpreter = interpreter,
                Parser = parser,
                TokenScanner = tokenScanner,
            };

            this.rqlEngine = new RqlEngine<ContentType, ConditionType>(rqlEngineArgs);
        }

        [Fact]
        public void Dispose_NoConditions_ExecutesDisposal()
        {
            // Act
            this.rqlEngine.Dispose();
        }

        /// <summary>
        /// Case 1 - RQL source is given with 2 match expression statements. The first statement
        /// produces a result with 'nothing' as output. The second statement produces a result with
        /// 1 rule as output.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase1_InterpretsAndReturnsResult()
        {
            // Arrange
            var rql = "MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$;\\nMATCH ONE RULE FOR \"Other\\nTest\" ON $2024-01-01Z$;";
            var tokens = new[]
            {
                CreateToken("MATCH", null, TokenType.MATCH),
                CreateToken("ONE", null, TokenType.ONE),
                CreateToken("RULE", null, TokenType.RULE),
                CreateToken("FOR", null, TokenType.FOR),
                CreateToken("\"Test\"", "Test", TokenType.STRING),
                CreateToken("ON", null, TokenType.ON),
                CreateToken("$2023-01-01Z$", DateTime.Parse("2023-01-01Z", CultureInfo.InvariantCulture), TokenType.DATE),
                CreateToken(";", null, TokenType.SEMICOLON),
                CreateToken("MATCH", null, TokenType.MATCH),
                CreateToken("ONE", null, TokenType.ONE),
                CreateToken("RULE", null, TokenType.RULE),
                CreateToken("FOR", null, TokenType.FOR),
                CreateToken("\"Other\\nTest\"", "Other\nTest", TokenType.STRING),
                CreateToken("ON", null, TokenType.ON),
                CreateToken("$2024-01-01Z$", DateTime.Parse("2024-01-01Z", CultureInfo.InvariantCulture), TokenType.DATE),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                    MatchExpression.Create(
                        CardinalitySegment.Create(
                            KeywordExpression.Create(tokens[1]), // ONE
                            KeywordExpression.Create(tokens[2])), // RULE
                        LiteralExpression.Create(LiteralType.String, tokens[4], tokens[4].Literal), // Test
                        LiteralExpression.Create(LiteralType.DateTime, tokens[6], tokens[6].Literal), // 2023-01-01Z
                        Segment.None)),
                ExpressionStatement.Create(
                    MatchExpression.Create(
                        CardinalitySegment.Create(
                            KeywordExpression.Create(tokens[9]), // ONE
                            KeywordExpression.Create(tokens[10])), // RULE
                        LiteralExpression.Create(LiteralType.String, tokens[12], tokens[12].Literal), // Other\nTest
                        LiteralExpression.Create(LiteralType.DateTime, tokens[14], tokens[14].Literal), // 2024-01-01Z
                        Segment.None)),
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var rqlRule = new RqlRule<ContentType, ConditionType>();
            var rqlArray = new RqlArray(1);
            rqlArray.SetAtIndex(0, rqlRule);
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new NothingStatementResult("MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$;"));
            interpretResult.AddStatementResult(new ExpressionStatementResult("MATCH ONE RULE FOR \"Other\\nTest\" ON $2024-01-01Z$;", rqlArray));

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var results = await this.rqlEngine.ExecuteAsync(rql);

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(2);
            var result1 = results.FirstOrDefault();
            result1.Should().NotBeNull()
                .And.BeOfType<NothingResult>();
            result1.As<NothingResult>()
                .Rql.Should().Be("MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$;");
            var result2 = results.LastOrDefault();
            result2.Should().NotBeNull()
                .And.BeOfType<RulesSetResult<ContentType, ConditionType>>();
            result2.As<RulesSetResult<ContentType, ConditionType>>()
                .Rql.Should().Be("MATCH ONE RULE FOR \"Other\\nTest\" ON $2024-01-01Z$;");
            result2.As<RulesSetResult<ContentType, ConditionType>>()
                .Lines.Should().HaveCount(1)
                .And.Contain(line => line.LineNumber == 1 && object.Equals(line.Rule, rqlRule));
        }

        /// <summary>
        /// Case 2 - RQL source is given with 1 new array expression statement which produces as
        /// result an empty array with size 3.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase2_InterpretsAndReturnsResult()
        {
            // Arrange
            var rql = "ARRAY[3];";
            var tokens = new[]
            {
                CreateToken("ARRAY", null, TokenType.ARRAY),
                CreateToken("[", null, TokenType.STRAIGHT_BRACKET_LEFT),
                CreateToken("3", 3, TokenType.INT),
                CreateToken("]", null, TokenType.STRAIGHT_BRACKET_RIGHT),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                    NewArrayExpression.Create(
                        tokens[0], // ARRAY
                        tokens[1], // [
                        LiteralExpression.Create(LiteralType.Integer, tokens[2], tokens[2].Literal), // 3
                        Array.Empty<Expression>(),
                        tokens[3])), // ]
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var rqlArray = new RqlArray(3);
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new ExpressionStatementResult(rql, rqlArray));

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var results = await this.rqlEngine.ExecuteAsync(rql);

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            var result1 = results.FirstOrDefault();
            result1.Should().NotBeNull()
                .And.BeOfType<ValueResult>();
            result1.As<ValueResult>()
                .Rql.Should().Be(rql);
            result1.As<ValueResult>()
                .Value.Should().Be(rqlArray);
            result1.As<ValueResult>().Value.As<RqlArray>()
                .Size.Value.Should().Be(3);
        }

        /// <summary>
        /// Case 3 - RQL source is given with 1 new array expression statement which produces as
        /// result an empty array with size 0.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase3_InterpretsAndReturnsResult()
        {
            // Arrange
            var rql = "ARRAY[0];";
            var tokens = new[]
            {
                CreateToken("ARRAY", null, TokenType.ARRAY),
                CreateToken("[", null, TokenType.STRAIGHT_BRACKET_LEFT),
                CreateToken("0", 0, TokenType.INT),
                CreateToken("]", null, TokenType.STRAIGHT_BRACKET_RIGHT),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                    NewArrayExpression.Create(
                        tokens[0], // ARRAY
                        tokens[1], // [
                        LiteralExpression.Create(LiteralType.Integer, tokens[2], tokens[2].Literal), // 0
                        Array.Empty<Expression>(),
                        tokens[3])), // ]
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var rqlArray = new RqlArray(0);
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new ExpressionStatementResult(rql, rqlArray));

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var results = await this.rqlEngine.ExecuteAsync(rql);

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            var result1 = results.FirstOrDefault();
            result1.Should().NotBeNull()
                .And.BeOfType<ValueResult>();
            result1.As<ValueResult>()
                .Rql.Should().Be(rql);
            result1.As<ValueResult>()
                .Value.Should().Be(rqlArray);
            result1.As<ValueResult>().Value.As<RqlArray>()
                .Size.Value.Should().Be(0);
        }

        /// <summary>
        /// Case 4 - RQL source is given with 1 string expression statement which produces as result
        /// a string.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase4_InterpretsAndReturnsResult()
        {
            // Arrange
            var rql = "\"test string\";";
            var tokens = new[]
            {
                CreateToken("\"test string\"", "test string", TokenType.STRING),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                        LiteralExpression.Create(LiteralType.Integer, tokens[0], tokens[0].Literal)), // "test string"
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var rqlString = new RqlString("test string");
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new ExpressionStatementResult(rql, rqlString));

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var results = await this.rqlEngine.ExecuteAsync(rql);

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            results.Should().NotBeNullOrEmpty()
                .And.HaveCount(1);
            var result1 = results.FirstOrDefault();
            result1.Should().NotBeNull()
                .And.BeOfType<ValueResult>();
            result1.As<ValueResult>()
                .Rql.Should().Be(rql);
            result1.As<ValueResult>()
                .Value.Should().Be(rqlString);
            result1.As<ValueResult>().Value.As<RqlString>()
                .Value.Should().Be("test string");
        }

        /// <summary>
        /// Case 5 - RQL source is given and fails to scan, throwing a RqlException.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase5HavingErrorsOnScanner_ThrowsRqlException()
        {
            // Arrange
            var rql = "\"test string\";";
            var messages = new List<Message>
            {
                Message.Create("Sample scan error", RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 10), MessageSeverity.Error),
            };
            var scanResult = ScanResult.CreateError(messages);

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);

            // Act
            var rqlException = await Assert.ThrowsAsync<RqlException>(async () => await this.rqlEngine.ExecuteAsync(rql));

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner));

            rqlException.Message.Should().Be("Errors have occurred processing provided RQL source - Sample scan error for source <unavailable> @{1:1}-{1:10}");
            rqlException.Errors.Should().HaveCount(1);
            var rqlError = rqlException.Errors.First();
            rqlError.Text.Should().Be("Sample scan error");
            rqlError.BeginPosition.Should().Be(RqlSourcePosition.From(1, 1));
            rqlError.EndPosition.Should().Be(RqlSourcePosition.From(1, 10));
            rqlError.Rql.Should().Be("<unavailable>");
        }

        /// <summary>
        /// Case 6 - RQL source is given and fails to parse, throwing a RqlException.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase6HavingErrorsOnParser_ThrowsRqlException()
        {
            // Arrange
            var rql = "\"test string\";";
            var tokens = new[]
            {
                CreateToken("\"test string\"", "test string", TokenType.STRING),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var messages = new List<Message>
            {
                Message.Create("Sample parse error", RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 10), MessageSeverity.Error),
            };
            var parseResult = ParseResult.CreateError(messages);

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);

            // Act
            var rqlException = await Assert.ThrowsAsync<RqlException>(async () => await this.rqlEngine.ExecuteAsync(rql));

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser));

            rqlException.Message.Should().Be("Errors have occurred processing provided RQL source - Sample parse error for source <unavailable> @{1:1}-{1:10}");
            rqlException.Errors.Should().HaveCount(1);
            var rqlError = rqlException.Errors.First();
            rqlError.Text.Should().Be("Sample parse error");
            rqlError.BeginPosition.Should().Be(RqlSourcePosition.From(1, 1));
            rqlError.EndPosition.Should().Be(RqlSourcePosition.From(1, 10));
            rqlError.Rql.Should().Be("<unavailable>");
        }

        /// <summary>
        /// Case 7 - RQL source is given and fails to be interpreted, throwing a RqlException.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase7HavingErrorsOnInterpreter_ThrowsRqlException()
        {
            // Arrange
            var rql = "\"test string\";";
            var tokens = new[]
            {
                CreateToken("\"test string\"", "test string", TokenType.STRING),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                        LiteralExpression.Create(LiteralType.Integer, tokens[0], tokens[0].Literal)), // "test string"
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new ErrorStatementResult("Sample interpret error", rql, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 10)));

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var rqlException = await Assert.ThrowsAsync<RqlException>(async () => await this.rqlEngine.ExecuteAsync(rql));

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            rqlException.Message.Should().Be("Errors have occurred processing provided RQL source - Sample interpret error for source \"test string\"; @{1:1}-{1:10}");
            rqlException.Errors.Should().HaveCount(1);
            var rqlError = rqlException.Errors.First();
            rqlError.Text.Should().Be("Sample interpret error");
            rqlError.BeginPosition.Should().Be(RqlSourcePosition.From(1, 1));
            rqlError.EndPosition.Should().Be(RqlSourcePosition.From(1, 10));
            rqlError.Rql.Should().Be(rql);
        }

        /// <summary>
        /// Case 8 - RQL source is given which produces a unknown result type, throwing a NotSupportedException.
        /// </summary>
        [Fact]
        public async Task ExecuteAsync_GivenRqlSourceCase8HavingUnknownResultType_ThrowsNotSupportedException()
        {
            // Arrange
            var rql = "\"test string\";";
            var tokens = new[]
            {
                CreateToken("\"test string\"", "test string", TokenType.STRING),
                CreateToken(";", null, TokenType.SEMICOLON),
            }.ToList().AsReadOnly();
            var scanResult = ScanResult.CreateSuccess(tokens, new List<Message>());
            var statements = new[]
            {
                ExpressionStatement.Create(
                        LiteralExpression.Create(LiteralType.Integer, tokens[0], tokens[0].Literal)), // "test string"
            }.ToList().AsReadOnly();
            var parseResult = ParseResult.CreateSuccess(statements, new List<Message>());
            var interpretResult = new InterpretResult();
            interpretResult.AddStatementResult(new StubResult());

            Mock.Get(this.tokenScanner)
                .Setup(x => x.ScanTokens(It.Is(rql, StringComparer.Ordinal)))
                .Returns(scanResult);
            Mock.Get(this.parser)
                .Setup(x => x.Parse(It.Is<IReadOnlyList<Token>>(tks => object.Equals(tks, tokens))))
                .Returns(parseResult);
            Mock.Get(this.interpreter)
                .Setup(x => x.InterpretAsync(It.Is<IReadOnlyList<Statement>>(stmts => object.Equals(stmts, statements))))
                .Returns(Task.FromResult(interpretResult));

            // Act
            var notSupportedException = await Assert.ThrowsAsync<NotSupportedException>(async () => await this.rqlEngine.ExecuteAsync(rql));

            // Assert
            Mock.VerifyAll(
                Mock.Get(this.tokenScanner),
                Mock.Get(this.parser),
                Mock.Get(this.interpreter));

            notSupportedException.Message.Should().Be($"Result of type '{typeof(StubResult).FullName}' is not supported.");
        }

        private static Token CreateToken(string lexeme, object? literal, TokenType type)
            => Token.Create(lexeme, false, literal, RqlSourcePosition.Empty, RqlSourcePosition.Empty, (uint)lexeme.Length, type);
    }
}