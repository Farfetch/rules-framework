namespace Rules.Framework.Rql.Tests
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Ast;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;
    using Xunit;

    public class ReverseRqlBuilderTests
    {
        [Theory]
        [InlineData("expression")]
        [InlineData("segment")]
        [InlineData("statement")]
        public void BuildRql_GivenKnownAstElement_ReturnsRqlRepresentation(string astElementType)
        {
            // Arrange
            IAstElement astElement;
            switch (astElementType)
            {
                case "expression":
                    var expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
                    Mock.Get(expression)
                        .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                        .Returns("test");
                    astElement = expression;
                    break;

                case "segment":
                    var segment = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
                    Mock.Get(segment)
                        .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                        .Returns("test");
                    astElement = segment;
                    break;

                case "statement":
                    var statement = CreateMock<Statement>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
                    Mock.Get(statement)
                        .Setup(x => x.Accept(It.IsAny<IStatementVisitor<string>>()))
                        .Returns("test");
                    astElement = statement;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var builder = new ReverseRqlBuilder();

            // Act
            var actual = builder.BuildRql(astElement);

            // Assert
            actual.Should().Be("test");
        }

        [Fact]
        public void BuildRql_GivenNullAstElement_ThrowsNotSupportedException()
        {
            // Arrange
            var builder = new ReverseRqlBuilder();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => builder.BuildRql(null));

            // Assert
            actual.ParamName.Should().Be("astElement");
        }

        [Fact]
        public void BuildRql_GivenUnknownAstElement_ThrowsNotSupportedException()
        {
            // Arrange
            var unknownAstElement = CreateMock<IAstElement>();

            var builder = new ReverseRqlBuilder();

            // Act
            var actual = Assert.Throws<NotSupportedException>(() => builder.BuildRql(unknownAstElement));

            // Assert
            actual.Message.Should().Contain("The given AST element is not supported:");
        }

        [Fact]
        public void VisitAssignmentExpression_GivenAssignmentExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var left = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(left)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("var1");
            var right = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(right)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("array { 1, 2, 3 }");
            var @operator = Token.Create("=", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.EQUAL);
            var assignmentExpression = new AssignmentExpression(left, @operator, right);

            var builder = new ReverseRqlBuilder();

            // Act
            var actual = builder.VisitAssignmentExpression(assignmentExpression);

            // Assert
            actual.Should().Be("var1 = array { 1, 2, 3 }");
        }

        [Fact]
        public void VisitBinaryExpression_GivenBinaryExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var left = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(left)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("1");
            var right = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(right)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("{ 1, 2, 3 }");
            var @operator = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(@operator)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("in");
            var binaryExpression = new BinaryExpression(left, @operator, right);

            var builder = new ReverseRqlBuilder();

            // Act
            var actual = builder.VisitBinaryExpression(binaryExpression);

            // Assert
            actual.Should().Be("1 in { 1, 2, 3 }");
        }

        [Fact]
        public void VisitCardinalitySegment_GivenCardinalitySegment_ReturnsRqlRepresentation()
        {
            // Arrange
            var cardinalityKeyword = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(cardinalityKeyword)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("one");
            var ruleKeyword = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(ruleKeyword)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("rule");
            var cardinalitySegment = CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitCardinalitySegment(cardinalitySegment);

            // Assert
            actual.Should().Be("one rule");
        }

        [Fact]
        public void VisitExpressionStatement_GivenExpressionStatement_ReturnsRqlRepresentation()
        {
            // Arrange
            var expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$");
            var expressionStatement = ExpressionStatement.Create(expression);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitExpressionStatement(expressionStatement);

            // Assert
            actual.Should().Be("MATCH ONE RULE FOR \"Test\" ON $2023-01-01Z$;");
        }

        [Fact]
        public void VisitIdentifierExpression_GivenIdentifierExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var identifierToken = Token.Create("abc", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 3, TokenType.IDENTIFIER);
            var identifierExpression = new IdentifierExpression(identifierToken);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitIdentifierExpression(identifierExpression);

            // Assert
            actual.Should().Be("abc");
        }

        [Fact]
        public void VisitInputConditionSegment_GivenInputConditionSegment_ReturnsRqlRepresentation()
        {
            // Arrange
            var leftExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(leftExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("@TestCondition");
            var operatorToken = Token.Create("is", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 2, TokenType.IS);
            var rightExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(rightExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("true");
            var inputConditionSegment = new InputConditionSegment(leftExpression, operatorToken, rightExpression);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitInputConditionSegment(inputConditionSegment);

            // Assert
            actual.Should().Be("@TestCondition is true");
        }

        [Fact]
        public void VisitInputConditionsSegment_GivenInputConditionsSegmentWithConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var inputConditionSegment1 = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditionSegment1)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("@TestCondition1 is true");
            var inputConditionSegment2 = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditionSegment2)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("@TestCondition2 is 30");
            var inputConditionsSegment = new InputConditionsSegment(new[] { inputConditionSegment1, inputConditionSegment2 });

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitInputConditionsSegment(inputConditionsSegment);

            // Assert
            actual.Should().Be("WITH { @TestCondition1 is true, @TestCondition2 is 30 }");
        }

        [Fact]
        public void VisitInputConditionsSegment_GivenInputConditionsSegmentWithoutConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var inputConditionsSegment = new InputConditionsSegment(new Segment[0]);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitInputConditionsSegment(inputConditionsSegment);

            // Assert
            actual.Should().Be(string.Empty);
        }

        [Fact]
        public void VisitKeywordExpression_GivenKeywordExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var keywordToken = Token.Create("CREATE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.CREATE);
            var keyworkExpression = KeywordExpression.Create(keywordToken);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitKeywordExpression(keyworkExpression);

            // Assert
            actual.Should().Be("CREATE");
        }

        [Theory]
        [InlineData(LiteralType.Bool, true, "TRUE")]
        [InlineData(LiteralType.Decimal, 10.35, "10.35")]
        [InlineData(LiteralType.Integer, 3, "3")]
        [InlineData(LiteralType.String, "test", "test")]
        [InlineData(LiteralType.DateTime, "2024-01-05T22:36:05Z", "$2024-01-05T22:36:05Z$")]
        [InlineData(LiteralType.Undefined, "abc", "abc")]
        public void VisitLiteralExpression_GivenLiteralExpression_ReturnsRqlRepresentation(object literalType, object value, string expected)
        {
            // Arrange
            var value1 = (LiteralType)literalType == LiteralType.DateTime ? DateTime.Parse(value.ToString()) : value;
            var token = Token.Create(value1.ToString(), false, value1, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.CREATE);
            var literalExpression = LiteralExpression.Create((LiteralType)literalType, token, value1);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitLiteralExpression(literalExpression);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void VisitMatchExpression_GivenMatchExpressionWithConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var cardinalitySegment = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(cardinalitySegment)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("ONE RULE");
            var contentTypeExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(contentTypeExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"Test\"");
            var matchDateExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(matchDateExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2024-03-24$");
            var inputConditionsSegment = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditionsSegment)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("WITH { @TestCondition1 is true }");
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitMatchExpression(matchExpression);

            // Assert
            actual.Should().Be("MATCH ONE RULE FOR \"Test\" ON $2024-03-24$ WITH { @TestCondition1 is true }");
        }

        [Fact]
        public void VisitMatchExpression_GivenMatchExpressionWithoutConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var cardinalitySegment = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(cardinalitySegment)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("ONE RULE");
            var contentTypeExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(contentTypeExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"Test\"");
            var matchDateExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(matchDateExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2024-03-24$");
            var inputConditionsSegment = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditionsSegment)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns(string.Empty);
            var matchExpression = MatchExpression.Create(cardinalitySegment, contentTypeExpression, matchDateExpression, inputConditionsSegment);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitMatchExpression(matchExpression);

            // Assert
            actual.Should().Be("MATCH ONE RULE FOR \"Test\" ON $2024-03-24$");
        }

        [Fact]
        public void VisitNewArrayExpression_GivenNewArrayExpressionWithElementsInitializer_ReturnsRqlRepresentation()
        {
            // Arrange
            var arrayToken = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var initializerBeginToken = Token.Create("{", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_LEFT);
            var sizeExpression = Expression.None;
            var value1Expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(value1Expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"abc\"");
            var value2Expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(value2Expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("123");
            var values = new[]
            {
                value1Expression, value2Expression
            };
            var initializerEndToken = Token.Create("}", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_RIGHT);

            var newArrayExpression = NewArrayExpression.Create(arrayToken, initializerBeginToken, sizeExpression, values, initializerEndToken);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitNewArrayExpression(newArrayExpression);

            // Assert
            actual.Should().Be("ARRAY { \"abc\", 123 }");
        }

        [Fact]
        public void VisitNewArrayExpression_GivenNewArrayExpressionWithSizeInitializer_ReturnsRqlRepresentation()
        {
            // Arrange
            var arrayToken = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var initializerBeginToken = Token.Create("[", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.STRAIGHT_BRACKET_LEFT);
            var sizeExpression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(sizeExpression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("3");
            var value1Expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(value1Expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"test\"");
            var values = new[]
            {
                value1Expression
            };
            var initializerEndToken = Token.Create("]", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.STRAIGHT_BRACKET_RIGHT);

            var newArrayExpression = NewArrayExpression.Create(arrayToken, initializerBeginToken, sizeExpression, values, initializerEndToken);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitNewArrayExpression(newArrayExpression);

            // Assert
            actual.Should().Be("ARRAY [3]");
        }

        [Fact]
        public void VisitNewObjectExpression_GivenNewObjectExpressionWithInitializer_ReturnsRqlRepresentation()
        {
            // Arrange
            var objectToken = Token.Create("OBJECT", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.OBJECT);
            var propertyAssignment1Expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(propertyAssignment1Expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"Name\" = \"John Doe\"");
            var propertyAssignment2Expression = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(propertyAssignment2Expression)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"Age\" = 1");
            var values = new[] { propertyAssignment1Expression, propertyAssignment2Expression };

            var newObjectExpression = new NewObjectExpression(objectToken, values);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitNewObjectExpression(newObjectExpression);

            // Assert
            actual.Should().Be($"OBJECT{Environment.NewLine}{{{Environment.NewLine}    \"Name\" = \"John Doe\",{Environment.NewLine}    \"Age\" = 1{Environment.NewLine}}}");
        }

        [Fact]
        public void VisitNewObjectExpression_GivenNewObjectExpressionWithoutInitializer_ReturnsRqlRepresentation()
        {
            // Arrange
            var objectToken = Token.Create("OBJECT", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.OBJECT);
            var values = new Expression[0];

            var newObjectExpression = new NewObjectExpression(objectToken, values);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitNewObjectExpression(newObjectExpression);

            // Assert
            actual.Should().Be("OBJECT");
        }

        [Fact]
        public void VisitNoneExpression_GivenNoneExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var noneExpression = new NoneExpression();

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.BuildRql(noneExpression);

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public void VisitNoneSegment_GivenNoneSegment_ReturnsRqlRepresentation()
        {
            // Arrange
            var noneSegment = new NoneSegment();

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.BuildRql(noneSegment);

            // Assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public void VisitNoneStatement_GivenNoneStatement_ReturnsRqlRepresentation()
        {
            // Arrange
            var noneStatement = new NoneStatement();

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.BuildRql(noneStatement);

            // Assert
            actual.Should().BeEmpty();
        }

        [Theory]
        [InlineData(new[] { "NOT", "IN" }, new object[] { TokenType.NOT, TokenType.IN }, "NOT IN")]
        [InlineData(new[] { "=" }, new object[] { TokenType.EQUAL }, "=")]
        public void VisitOperatorSegment_GivenOperatorSegment_ReturnsRqlRepresentation(string[] operatorTokens, object[] tokenTypes, string expected)
        {
            // Act
            var tokens = new Token[operatorTokens.Length];
            for (int i = 0; i < operatorTokens.Length; i++)
            {
                tokens[i] = Token.Create(
                    operatorTokens[i],
                    false,
                    null,
                    RqlSourcePosition.Empty,
                    RqlSourcePosition.Empty,
                    (uint)operatorTokens[i].Length,
                    (TokenType)tokenTypes[i]);
            }

            var operatorSegment = new OperatorSegment(tokens);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitOperatorSegment(operatorSegment);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void VisitPlaceholderExpression_GivenPlaceholderExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var placeholderToken = Token.Create("@test", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.PLACEHOLDER);

            var placeholderExpression = new PlaceholderExpression(placeholderToken);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitPlaceholderExpression(placeholderExpression);

            // Assert
            actual.Should().Be("@test");
        }

        [Fact]
        public void VisitSearchExpression_GivenSearchExpressionWithInputConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var contentType = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(contentType)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"test content type\"");

            var dateBegin = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(dateBegin)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2023-01-01$");

            var dateEnd = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(dateEnd)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2024-01-01$");

            var inputConditions = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditions)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns("WITH { @TestCondition1 is \"abc\" }");

            var searchExpression = new SearchExpression(contentType, dateBegin, dateEnd, inputConditions);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitSearchExpression(searchExpression);

            // Assert
            actual.Should().Be("SEARCH RULES FOR \"test content type\" SINCE $2023-01-01$ UNTIL $2024-01-01$ WITH { @TestCondition1 is \"abc\" }");
        }

        [Fact]
        public void VisitSearchExpression_GivenSearchExpressionWithoutInputConditions_ReturnsRqlRepresentation()
        {
            // Arrange
            var contentType = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(contentType)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("\"test content type\"");

            var dateBegin = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(dateBegin)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2023-01-01$");

            var dateEnd = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(dateEnd)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("$2024-01-01$");

            var inputConditions = CreateMock<Segment>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(inputConditions)
                .Setup(x => x.Accept(It.IsAny<ISegmentVisitor<string>>()))
                .Returns(string.Empty);

            var searchExpression = new SearchExpression(contentType, dateBegin, dateEnd, inputConditions);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitSearchExpression(searchExpression);

            // Assert
            actual.Should().Be("SEARCH RULES FOR \"test content type\" SINCE $2023-01-01$ UNTIL $2024-01-01$");
        }

        [Fact]
        public void VisitUnaryExpression_GivenUnaryExpression_ReturnsRqlRepresentation()
        {
            // Arrange
            var unaryOperator = Token.Create("-", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.MINUS);
            var right = CreateMock<Expression>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            Mock.Get(right)
                .Setup(x => x.Accept(It.IsAny<IExpressionVisitor<string>>()))
                .Returns("123");

            var unaryExpression = new UnaryExpression(unaryOperator, right);

            var reverseRqlBuilder = new ReverseRqlBuilder();

            // Act
            var actual = reverseRqlBuilder.VisitUnaryExpression(unaryExpression);

            // Assert
            actual.Should().Be("-123");
        }

        private T CreateMock<T>(params object[] args)
            where T : class
        {
            var mock = new Mock<T>(args);
            return mock.Object;
        }
    }
}