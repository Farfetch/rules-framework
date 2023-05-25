namespace Rules.Framework.Tests.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesSourceTests
    {
        [Fact]
        public async Task AddRuleAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            AddRuleArgs<ContentType, ConditionType> addRuleArgs = new()
            {
                Rule = rule,
            };

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware<ContentType, ConditionType>>();

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            // Assert
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task AddRuleAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            AddRuleArgs<ContentType, ConditionType> addRuleArgs = new()
            {
                Rule = rule,
            };

            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            // Assert
            middleware1.AddRuleCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task AddRuleAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            AddRuleArgs<ContentType, ConditionType> addRuleArgs = new()
            {
                Rule = rule,
            };

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            // Assert
            middleware1.AddRuleCalls.Should().Be(1);
            middleware2.AddRuleCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.AddRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task GetRulesAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs<ContentType> getRulesArgs = CreateGetRulesArgs();

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware<ContentType, ConditionType>>();

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

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
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs<ContentType> getRulesArgs = CreateGetRulesArgs();

            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

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
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesArgs<ContentType> getRulesArgs = CreateGetRulesArgs();

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesAsync(It.IsIn(getRulesArgs.ContentType), It.IsIn(getRulesArgs.DateBegin), It.IsIn(getRulesArgs.DateEnd)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesAsync(getRulesArgs).ConfigureAwait(false);

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
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs<ContentType> getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware<ContentType, ConditionType>>();

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.BeEquivalentTo(expected);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task GetRulesFilteredAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs<ContentType> getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesFilteredCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task GetRulesFilteredAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();
            var expected = new[] { rule };

            GetRulesFilteredArgs<ContentType> getRulesFilteredArgs = CreateGetRulesFilteredArgs();

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)))
                .ReturnsAsync(expected);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            var actual = await rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNullOrEmpty()
                .And.Contain(expected);
            middleware1.GetRulesFilteredCalls.Should().Be(1);
            middleware2.GetRulesFilteredCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.GetRulesByAsync(It.Is<RulesFilterArgs<ContentType>>(
                    (val) => string.Equals(val.Name, getRulesFilteredArgs.Name) && val.ContentType == getRulesFilteredArgs.ContentType && val.Priority == getRulesFilteredArgs.Priority)), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_NoMiddlewares_CallsRulesDataSource()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            UpdateRuleArgs<ContentType, ConditionType> updateRuleArgs = new()
            {
                Rule = rule,
            };

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares
                = Enumerable.Empty<IRulesSourceMiddleware<ContentType, ConditionType>>();

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);

            // Assert
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_OneMiddleware_CallsMiddlewareAndRulesDataSourceAfter()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            UpdateRuleArgs<ContentType, ConditionType> updateRuleArgs = new()
            {
                Rule = rule,
            };

            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), new List<string>());

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);

            // Assert
            middleware1.UpdateRulesCalls.Should().Be(1);
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        [Fact]
        public async Task UpdateRuleAsync_TwoMiddlewares_CallsFirstMiddlewareSecondMiddlewareAfterAndRulesDataSourceByLast()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = CreateRule();

            UpdateRuleArgs<ContentType, ConditionType> updateRuleArgs = new()
            {
                Rule = rule,
            };

            List<string> middlewareMessages = new();
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware1 = new(nameof(middleware1), middlewareMessages);
            StubRulesSourceMiddleware<ContentType, ConditionType> middleware2 = new(nameof(middleware2), middlewareMessages);

            IEnumerable<IRulesSourceMiddleware<ContentType, ConditionType>> rulesSourceMiddlewares = new[] { middleware1, middleware2 };

            IRulesDataSource<ContentType, ConditionType> rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))))
                .Returns(Task.CompletedTask);

            RulesSource<ContentType, ConditionType> rulesSource = new(rulesDataSource, rulesSourceMiddlewares);

            // Act
            await rulesSource.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);

            // Assert
            middleware1.UpdateRulesCalls.Should().Be(1);
            middleware2.UpdateRulesCalls.Should().Be(1);
            middlewareMessages.Should().ContainInOrder("Enter middleware1.", "Enter middleware2.", "Exit middleware2.", "Exit middleware1.");
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.Is<Rule<ContentType, ConditionType>>((val, _) => object.ReferenceEquals(val, rule))), Times.Once());
        }

        private static GetRulesArgs<ContentType> CreateGetRulesArgs() => new()
        {
            ContentType = ContentType.Type1,
            DateBegin = DateTime.Parse("2022-01-01Z"),
            DateEnd = DateTime.Parse("2023-01-01Z"),
        };

        private static GetRulesFilteredArgs<ContentType> CreateGetRulesFilteredArgs() => new()
        {
            ContentType = ContentType.Type1,
            Name = "test",
            Priority = 1,
        };

        private static Rule<ContentType, ConditionType> CreateRule() =>
            RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("Test rule")
                .WithDateBegin(DateTime.Parse("2022-11-27Z"))
                .WithContent(ContentType.Type1, "test")
                .Build()
            .Rule;
    }
}