namespace Rules.Framework.Rql.Tests.Pipeline.Parse
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Xunit;

    public class ParseStrategyPoolTests
    {
        [Fact]
        public void GetExpressionParseStrategy_GivenNotPooledStrategy_CreatesNewStrategyInstanceAndReturns()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();
            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetExpressionParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrAfter(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }

        [Fact]
        public void GetExpressionParseStrategy_GivenPooledStrategy_ReturnsPooledStrategy()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();

            // Simulate parse strategy already pooled
            parseStrategyPool.GetExpressionParseStrategy<StubParseStrategy>();

            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetExpressionParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrBefore(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }

        [Fact]
        public void GetSegmentParseStrategy_GivenNotPooledStrategy_CreatesNewStrategyInstanceAndReturns()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();
            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetSegmentParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrAfter(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }

        [Fact]
        public void GetSegmentParseStrategy_GivenPooledStrategy_ReturnsPooledStrategy()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();

            // Simulate parse strategy already pooled
            parseStrategyPool.GetSegmentParseStrategy<StubParseStrategy>();

            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetSegmentParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrBefore(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }

        [Fact]
        public void GetStatementParseStrategy_GivenNotPooledStrategy_CreatesNewStrategyInstanceAndReturns()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();
            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetStatementParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrAfter(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }

        [Fact]
        public void GetStatementParseStrategy_GivenPooledStrategy_ReturnsPooledStrategy()
        {
            // Arrange
            var parseStrategyPool = new ParseStrategyPool();

            // Simulate parse strategy already pooled
            parseStrategyPool.GetStatementParseStrategy<StubParseStrategy>();

            var before = DateTime.UtcNow;

            // Act
            var actual = parseStrategyPool.GetStatementParseStrategy<StubParseStrategy>();

            // Assert
            actual.Should().NotBeNull();
            actual.CreationDateTime.Should().BeOnOrBefore(before);
            actual.ParseStrategyProvider.Should().BeSameAs(parseStrategyPool);
        }
    }
}