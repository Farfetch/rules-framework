namespace Rules.Framework.Tests.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory;
    using Rules.Framework.Providers.InMemory.DataModel;
    using Rules.Framework.Tests.Providers.InMemory.TestStubs;
    using Xunit;

    public class InMemoryProviderRulesDataSourceTests
    {
        public static IEnumerable<object[]> CtorArguments { get; } = new[]
        {
            new object[] { null, null },
            new object[] { Mock.Of<IInMemoryRulesStorage<ContentType, ConditionType>>(), null }
        };

        [Fact]
        public async Task AddRuleAsync_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = null;

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage<ContentType, ConditionType>>();
            var ruleFactory = Mock.Of<IRuleFactory<ContentType, ConditionType>>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.AddRule(It.IsAny<RuleDataModel<ContentType, ConditionType>>()))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<ContentType, ConditionType>(inMemoryRulesStorage, ruleFactory);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await inMemoryProviderRulesDataSource.AddRuleAsync(rule).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public async Task AddRuleAsync_GivenRule_ConvertsToRuleDataModelAndAddsToDataSource()
        {
            // Arrange
            var rule = new Rule<ContentType, ConditionType>();
            var ruleDataModel = new RuleDataModel<ContentType, ConditionType>();

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage<ContentType, ConditionType>>();
            var ruleFactory = Mock.Of<IRuleFactory<ContentType, ConditionType>>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Returns(ruleDataModel)
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.AddRule(ruleDataModel))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<ContentType, ConditionType>(inMemoryRulesStorage, ruleFactory);

            // Act
            await inMemoryProviderRulesDataSource.AddRuleAsync(rule).ConfigureAwait(false);

            // Assert
            Mock.VerifyAll(Mock.Get(inMemoryRulesStorage), Mock.Get(ruleFactory));
        }

        [Theory]
        [MemberData(nameof(CtorArguments))]
        public void Ctor_GivenNullParameters_ThrowsArgumentNullException(object param1, object param2)
        {
            // Arrange
            var inMemoryRulesStorage = param1 as IInMemoryRulesStorage<ContentType, ConditionType>;
            var ruleFactory = param2 as IRuleFactory<ContentType, ConditionType>;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new InMemoryProviderRulesDataSource<ContentType, ConditionType>(inMemoryRulesStorage, ruleFactory));

            //Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().BeOneOf(nameof(inMemoryRulesStorage), nameof(ruleFactory));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = null;

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage<ContentType, ConditionType>>();
            var ruleFactory = Mock.Of<IRuleFactory<ContentType, ConditionType>>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.UpdateRule(It.IsAny<RuleDataModel<ContentType, ConditionType>>()))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<ContentType, ConditionType>(inMemoryRulesStorage, ruleFactory);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await inMemoryProviderRulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false)
            ).ConfigureAwait(false);

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRule_ConvertsToRuleDataModelAndUpdatesOnDataSource()
        {
            // Arrange
            var rule = new Rule<ContentType, ConditionType>();
            var ruleDataModel = new RuleDataModel<ContentType, ConditionType>();

            var inMemoryRulesStorage = Mock.Of<IInMemoryRulesStorage<ContentType, ConditionType>>();
            var ruleFactory = Mock.Of<IRuleFactory<ContentType, ConditionType>>();

            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(rule))
                .Returns(ruleDataModel)
                .Verifiable();

            Mock.Get(inMemoryRulesStorage)
                .Setup(x => x.UpdateRule(ruleDataModel))
                .Verifiable();

            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource<ContentType, ConditionType>(inMemoryRulesStorage, ruleFactory);

            // Act
            await inMemoryProviderRulesDataSource.UpdateRuleAsync(rule).ConfigureAwait(false);

            // Assert
            Mock.VerifyAll(Mock.Get(inMemoryRulesStorage), Mock.Get(ruleFactory));
        }
    }
}