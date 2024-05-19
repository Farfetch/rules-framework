namespace Rules.Framework.Rql.Tests.Runtime
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public class RqlRuntimeTests
    {
        public static IEnumerable<object?[]> ApplyBinary_ErrorCases() => new[]
        {
            // RqlOperators.Minus
            new object?[] { new RqlInteger(1), RqlOperators.Minus, new RqlDecimal(2.0m), "Expected right operand of type integer but found decimal." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Minus, new RqlInteger(3), "Expected right operand of type decimal but found integer." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Minus, new RqlBool(true), "Expected right operand of type decimal but found bool." },
            new object?[] { new RqlInteger(9), RqlOperators.Minus, new RqlBool(true), "Expected right operand of type integer but found bool." },
            new object?[] { new RqlString("abc"), RqlOperators.Minus, new RqlInteger(1), "Cannot subtract operand of type string." },

            // RqlOperators.Plus
            new object?[] { new RqlInteger(1), RqlOperators.Plus, new RqlDecimal(2.0m), "Expected right operand of type integer but found decimal." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Plus, new RqlInteger(3), "Expected right operand of type decimal but found integer." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Plus, new RqlBool(true), "Expected right operand of type decimal but found bool." },
            new object?[] { new RqlInteger(9), RqlOperators.Plus, new RqlBool(true), "Expected right operand of type integer but found bool." },
            new object?[] { new RqlString("abc"), RqlOperators.Plus, new RqlInteger(1), "Cannot sum operand of type string." },

            // RqlOperators.Slash
            new object?[] { new RqlInteger(1), RqlOperators.Slash, new RqlDecimal(2.0m), "Expected right operand of type integer but found decimal." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Slash, new RqlInteger(3), "Expected right operand of type decimal but found integer." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Slash, new RqlBool(true), "Expected right operand of type decimal but found bool." },
            new object?[] { new RqlInteger(9), RqlOperators.Slash, new RqlBool(true), "Expected right operand of type integer but found bool." },
            new object?[] { new RqlString("abc"), RqlOperators.Slash, new RqlInteger(1), "Cannot divide operand of type string." },

            // RqlOperators.Star
            new object?[] { new RqlInteger(1), RqlOperators.Star, new RqlDecimal(2.0m), "Expected right operand of type integer but found decimal." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Star, new RqlInteger(3), "Expected right operand of type decimal but found integer." },
            new object?[] { new RqlDecimal(1.5m), RqlOperators.Star, new RqlBool(true), "Expected right operand of type decimal but found bool." },
            new object?[] { new RqlInteger(9), RqlOperators.Star, new RqlBool(true), "Expected right operand of type integer but found bool." },
            new object?[] { new RqlString("abc"), RqlOperators.Star, new RqlInteger(1), "Cannot multiply operand of type string." },
        };

        public static IEnumerable<object?[]> ApplyBinary_SuccessCases() => new[]
        {
            // RqlOperators.Minus
            new object?[] { new RqlInteger(5), RqlOperators.Minus, new RqlInteger(4), new RqlInteger(1) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Minus, new RqlDecimal(2.3m), new RqlDecimal(2.8m) },
            new object?[] { new RqlAny(new RqlInteger(5)), RqlOperators.Minus, new RqlInteger(4), new RqlInteger(1) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Minus, new RqlAny(new RqlDecimal(2.3m)), new RqlDecimal(2.8m) },

            // RqlOperators.Plus
            new object?[] { new RqlInteger(2), RqlOperators.Plus, new RqlInteger(4), new RqlInteger(6) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Plus, new RqlDecimal(14.3m), new RqlDecimal(19.4m) },
            new object?[] { new RqlAny(new RqlInteger(2)), RqlOperators.Plus, new RqlInteger(4), new RqlInteger(6) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Plus, new RqlAny(new RqlDecimal(14.3m)), new RqlDecimal(19.4m) },

            // RqlOperators.Slash
            new object?[] { new RqlInteger(6), RqlOperators.Slash, new RqlInteger(2), new RqlInteger(3) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Slash, new RqlDecimal(2m), new RqlDecimal(2.55m) },
            new object?[] { new RqlAny(new RqlInteger(6)), RqlOperators.Slash, new RqlInteger(2), new RqlInteger(3) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Slash, new RqlAny(new RqlDecimal(2m)), new RqlDecimal(2.55m) },

            // RqlOperators.Star
            new object?[] { new RqlInteger(6), RqlOperators.Star, new RqlInteger(2), new RqlInteger(12) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Star, new RqlDecimal(2m), new RqlDecimal(10.2m) },
            new object?[] { new RqlAny(new RqlInteger(6)), RqlOperators.Star, new RqlInteger(2), new RqlInteger(12) },
            new object?[] { new RqlDecimal(5.1m), RqlOperators.Star, new RqlAny(new RqlDecimal(2m)), new RqlDecimal(10.2m) },
            new object?[] { new RqlInteger(1), RqlOperators.None, new RqlInteger(1), new RqlNothing() },
        };

        public static IEnumerable<object?[]> ApplyUnary_ErrorCases() => new[]
        {
            new object?[] { new RqlInteger(10), RqlOperators.Plus, "Unary operator Plus is not supported for value '<integer> 10'." },
            new object?[] { new RqlString("abc"), RqlOperators.Minus, "Unary operator Minus is not supported for value '<string> \"abc\"'." },
        };

        public static IEnumerable<object?[]> ApplyUnary_SuccessCases() => new[]
        {
            new object?[] { new RqlInteger(10), RqlOperators.Minus, new RqlInteger(-10) },
            new object?[] { new RqlDecimal(34.7m), RqlOperators.Minus, new RqlDecimal(-34.7m) },
            new object?[] { new RqlAny(new RqlInteger(10)), RqlOperators.Minus, new RqlInteger(-10) },
            new object?[] { new RqlAny(new RqlDecimal(34.7m)), RqlOperators.Minus, new RqlDecimal(-34.7m) },
        };

        [Theory]
        [MemberData(nameof(ApplyBinary_ErrorCases))]
        public void ApplyBinary_ErrorConditions_ThrowsRuntimeException(object left, object @operator, object right, string expectedErrorMessage)
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var runtimeException = Assert.Throws<RuntimeException>(() => rqlRuntime.ApplyBinary((IRuntimeValue)left, (RqlOperators)@operator, (IRuntimeValue)right));

            // Assert
            runtimeException.Message.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [MemberData(nameof(ApplyBinary_SuccessCases))]
        public void ApplyBinary_SuccessConditions_ReturnsBinaryResult(object left, object @operator, object right, object expected)
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = rqlRuntime.ApplyBinary((IRuntimeValue)left, (RqlOperators)@operator, (IRuntimeValue)right);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(ApplyUnary_ErrorCases))]
        public void ApplyUnary_ErrorConditions_ThrowsRuntimeException(object operand, object @operator, string expectedErrorMessage)
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var runtimeException = Assert.Throws<RuntimeException>(() => rqlRuntime.ApplyUnary((IRuntimeValue)operand, (RqlOperators)@operator));

            // Assert
            runtimeException.Message.Should().Be(expectedErrorMessage);
        }

        [Theory]
        [MemberData(nameof(ApplyUnary_SuccessCases))]
        public void ApplyUnary_SuccessConditions_ReturnsUnaryResult(object operand, object @operator, object expected)
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = rqlRuntime.ApplyUnary((IRuntimeValue)operand, (RqlOperators)@operator);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Create_GivenRulesEngine_ReturnsNewRqlRuntime()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();

            // Act
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Assert
            rqlRuntime.Should().NotBeNull();
        }

        [Fact]
        public async Task MatchRulesAsync_GivenAllMatchCardinalityWithResult_ReturnsRqlArrayWithTwoRules()
        {
            // Arrange
            const MatchCardinality matchCardinality = MatchCardinality.All;
            const ContentType contentType = ContentType.Type1;
            var matchDate = new RqlDate(DateTime.Parse("2024-04-13Z"));
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "PT")
            };
            var matchRulesArgs = new MatchRulesArgs<ContentType, ConditionType>
            {
                Conditions = conditions,
                ContentType = contentType,
                MatchCardinality = matchCardinality,
                MatchDate = matchDate,
            };

            var expectedRule1 = BuildRule("Rule 1", DateTime.Parse("2024-01-01Z"), DateTime.Parse("2025-01-01Z"), new object(), contentType);
            var expectedRule2 = BuildRule("Rule 2", DateTime.Parse("2024-01-01Z"), DateTime.Parse("2025-01-01Z"), new object(), contentType);
            var expectedRules = new[] { expectedRule1, expectedRule2 };
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            Mock.Get(rulesEngine)
                .Setup(x => x.MatchManyAsync(contentType, matchDate.Value, It.Is<IEnumerable<Condition<ConditionType>>>(c => c.SequenceEqual(conditions))))
                .ReturnsAsync(expectedRules);
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = await rqlRuntime.MatchRulesAsync(matchRulesArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Size.Value.Should().Be(2);
            actual.Value[0].Unwrap().Should().BeOfType<RqlRule<ContentType, ConditionType>>()
                .Subject.Value.Should().BeSameAs(expectedRule1);
            actual.Value[1].Unwrap().Should().BeOfType<RqlRule<ContentType, ConditionType>>()
                .Subject.Value.Should().BeSameAs(expectedRule2);
        }

        [Fact]
        public async Task MatchRulesAsync_GivenNoneMatchCardinality_ThrowsArgumentException()
        {
            // Arrange
            const MatchCardinality matchCardinality = MatchCardinality.None;
            const ContentType contentType = ContentType.Type1;
            var matchDate = new RqlDate(DateTime.Parse("2024-04-13Z"));
            var conditions = Array.Empty<Condition<ConditionType>>();
            var matchRulesArgs = new MatchRulesArgs<ContentType, ConditionType>
            {
                Conditions = conditions,
                ContentType = contentType,
                MatchCardinality = matchCardinality,
                MatchDate = matchDate,
            };

            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = await Assert.ThrowsAsync<ArgumentException>(async () => await rqlRuntime.MatchRulesAsync(matchRulesArgs));

            // Assert
            actual.Should().NotBeNull();
            actual.ParamName.Should().Be(nameof(matchRulesArgs));
            actual.Message.Should().StartWith("A valid match cardinality must be provided.");
        }

        [Fact]
        public async Task MatchRulesAsync_GivenOneMatchCardinalityWithoutResult_ReturnsEmptyRqlArray()
        {
            // Arrange
            const MatchCardinality matchCardinality = MatchCardinality.One;
            const ContentType contentType = ContentType.Type1;
            var matchDate = new RqlDate(DateTime.Parse("2024-04-13Z"));
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "PT")
            };
            var matchRulesArgs = new MatchRulesArgs<ContentType, ConditionType>
            {
                Conditions = conditions,
                ContentType = contentType,
                MatchCardinality = matchCardinality,
                MatchDate = matchDate,
            };

            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            Mock.Get(rulesEngine)
                .Setup(x => x.MatchOneAsync(contentType, matchDate.Value, It.Is<IEnumerable<Condition<ConditionType>>>(c => c.SequenceEqual(conditions))))
                .Returns(Task.FromResult<Rule<ContentType, ConditionType>>(null!));
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = await rqlRuntime.MatchRulesAsync(matchRulesArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Size.Value.Should().Be(0);
        }

        [Fact]
        public async Task MatchRulesAsync_GivenOneMatchCardinalityWithResult_ReturnsRqlArrayWithOneRule()
        {
            // Arrange
            const MatchCardinality matchCardinality = MatchCardinality.One;
            const ContentType contentType = ContentType.Type1;
            var matchDate = new RqlDate(DateTime.Parse("2024-04-13Z"));
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "PT")
            };
            var matchRulesArgs = new MatchRulesArgs<ContentType, ConditionType>
            {
                Conditions = conditions,
                ContentType = contentType,
                MatchCardinality = matchCardinality,
                MatchDate = matchDate,
            };

            var expectedRule = BuildRule("Rule 1", DateTime.Parse("2024-01-01Z"), DateTime.Parse("2025-01-01Z"), new object(), contentType);
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            Mock.Get(rulesEngine)
                .Setup(x => x.MatchOneAsync(contentType, matchDate.Value, It.Is<IEnumerable<Condition<ConditionType>>>(c => c.SequenceEqual(conditions))))
                .ReturnsAsync(expectedRule);
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = await rqlRuntime.MatchRulesAsync(matchRulesArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Size.Value.Should().Be(1);
            actual.Value[0].Unwrap().Should().BeOfType<RqlRule<ContentType, ConditionType>>()
                .Subject.Value.Should().BeSameAs(expectedRule);
        }

        [Fact]
        public async Task MatchSearchRulesAsync_GivenSearchArgs_ReturnsRqlArrayWithTwoRules()
        {
            // Arrange
            const ContentType contentType = ContentType.Type1;
            var dateBegin = new RqlDate(DateTime.Parse("2020-01-01Z"));
            var dateEnd = new RqlDate(DateTime.Parse("2030-01-01Z"));
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "PT")
            };
            var searchRulesArgs = new SearchRulesArgs<ContentType, ConditionType>
            {
                Conditions = conditions,
                ContentType = contentType,
                DateBegin = dateBegin,
                DateEnd = dateEnd,
            };

            var expectedRule1 = BuildRule("Rule 1", DateTime.Parse("2024-01-01Z"), DateTime.Parse("2025-01-01Z"), new object(), contentType);
            var expectedRule2 = BuildRule("Rule 2", DateTime.Parse("2024-01-01Z"), DateTime.Parse("2025-01-01Z"), new object(), contentType);
            var expectedRules = new[] { expectedRule1, expectedRule2 };
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            Mock.Get(rulesEngine)
                .Setup(x => x.SearchAsync(It.Is<SearchArgs<ContentType, ConditionType>>(c => c.ExcludeRulesWithoutSearchConditions == true
                    && c.Conditions.Equals(searchRulesArgs.Conditions)
                    && c.ContentType.Equals(searchRulesArgs.ContentType)
                    && c.DateBegin.Equals(searchRulesArgs.DateBegin.Value)
                    && c.DateEnd.Equals(searchRulesArgs.DateEnd.Value))))
                .ReturnsAsync(expectedRules);
            var rqlRuntime = RqlRuntime<ContentType, ConditionType>.Create(rulesEngine);

            // Act
            var actual = await rqlRuntime.SearchRulesAsync(searchRulesArgs);

            // Assert
            actual.Should().NotBeNull();
            actual.Size.Value.Should().Be(2);
            actual.Value[0].Unwrap().Should().BeOfType<RqlRule<ContentType, ConditionType>>()
                .Subject.Value.Should().BeSameAs(expectedRule1);
            actual.Value[1].Unwrap().Should().BeOfType<RqlRule<ContentType, ConditionType>>()
                .Subject.Value.Should().BeSameAs(expectedRule2);
        }

        private static Rule<ContentType, ConditionType> BuildRule(string name, DateTime dateBegin, DateTime? dateEnd, object content, ContentType contentType)
        {
            return RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName(name)
                .WithDatesInterval(dateBegin, dateEnd.GetValueOrDefault())
                .WithContent(contentType, content)
                .Build().Rule;
        }
    }
}