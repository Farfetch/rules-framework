namespace Rules.Framework.Tests.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesSourceTests
    {
        [Fact]
        public async Task AddRuleAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule rule = CreateRule();

            AddRuleArgs addRuleArgs = new()
            {
                Rule = rule,
            };

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware>();

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs);

            // Assert
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task AddRuleAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule rule = CreateRule();

            AddRuleArgs addRuleArgs = new()
            {
                Rule = rule,
            };

            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs);

            // Assert
            middleware1.AddRuleCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task AddRuleAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule rule = CreateRule();

            AddRuleArgs addRuleArgs = new()
            {
                Rule = rule,
            };

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs);

            // Assert
            middleware1.AddRuleCalls.Should().Be(1);
            middleware2.AddRuleCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task GetRulesAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs getRulesArgs = CreateGetRulesArgs();

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware>();

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expected);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)), Times.Once());
        }

        [Fact]
        public async Task GetRulesAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs getRulesArgs = CreateGetRulesArgs();

            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)), Times.Once());
        }

        [Fact]
        public async Task GetRulesAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs getRulesArgs = CreateGetRulesArgs();

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesCalls.Should().Be(1);
            middleware2.GetRulesCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)), Times.Once());
        }

        [Fact]
        public async Task GetRulesFilteredAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware>();

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expected);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task GetRulesFilteredAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesFilteredCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task GetRulesFilteredAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesFilteredCalls.Should().Be(1);
            middleware2.GetRulesFilteredCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule rule = CreateRule();

            UpdateRuleArgs updateRuleArgs = new()
            {
                Rule = rule,
            };

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware>();

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs);

            // Assert
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule rule = CreateRule();

            UpdateRuleArgs updateRuleArgs = new()
            {
                Rule = rule,
            };

            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs);

            // Assert
            middleware1.UpdateRulesCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule rule = CreateRule();

            UpdateRuleArgs updateRuleArgs = new()
            {
                Rule = rule,
            };

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs);

            // Assert
            middleware1.UpdateRulesCalls.Should().Be(1);
            middleware2.UpdateRulesCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        private static GetRulesArgs CreateGetRulesArgs() => new()
        {
            ContentType = ContentType.Type1.ToString(),
            DateBegin = DateTime.Parse("2022-01-01Z"),
            DateEnd = DateTime.Parse("2023-01-01Z"),
        };

        private static GetRulesFilteredArgs CreateGetRulesFilteredArgs() => new()
        {
            ContentType = ContentType.Type1.ToString(),
            Name = "test",
            Priority = 1,
        };

        private static Rule CreateRule() =>
            Rule.New()
                .WithName("Test rule")
                .WithDateBegin(DateTime.Parse("2022-11-27Z"))
                .WithContent(ContentType.Type1.ToString(), "test")
                .Build()
            .Rule;
    }
}