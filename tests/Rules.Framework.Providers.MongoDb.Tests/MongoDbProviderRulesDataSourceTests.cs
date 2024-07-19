namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Moq;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Xunit;

    public class MongoDbProviderRulesDataSourceTests
    {
        [Fact]
        public async Task CreateContentTypeAsync_GivenContentTypeName_InsertsContentTypeOnCollection()
        {
            // Arrange
            var contentType = nameof(RulesetNames.RulesetSample);
            RulesetDataModel actual = null;
            var contentTypesCollection = Mock.Of<IMongoCollection<RulesetDataModel>>();
            Mock.Get(contentTypesCollection)
                .Setup(x => x.InsertOneAsync(It.IsAny<RulesetDataModel>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Callback<RulesetDataModel, InsertOneOptions, CancellationToken>((ct, _, _) => actual = ct);

            var mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<RulesetDataModel>(It.IsAny<string>(), null))
                .Returns(contentTypesCollection);

            var mongoClient = Mock.Of<IMongoClient>();
            Mock.Get(mongoClient)
                .Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabase);

            var mongoDbProviderSettings = new MongoDbProviderSettings
            {
                DatabaseName = "TestDatabaseName",
                RulesCollectionName = "TestCollectionName"
            };

            var ruleFactory = Mock.Of<IRuleFactory>();

            var mongoDbProviderRulesDataSource = new MongoDbProviderRulesDataSource(
                mongoClient,
                mongoDbProviderSettings,
                ruleFactory);

            // Act
            await mongoDbProviderRulesDataSource.CreateRulesetAsync(contentType);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(contentType);
            actual.Id.Should().NotBeEmpty();
            actual.Creation.Should().BeWithin(TimeSpan.FromSeconds(5)).Before(DateTime.UtcNow);
        }

        [Fact]
        public async Task GetContentTypesAsync_NoConditions_ReturnsCollectionOfContentTypes()
        {
            // Arrange
            var contentTypeDataModels = new[]
            {
                new RulesetDataModel
                {
                    Creation = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Name = nameof(RulesetNames.RulesetSample),
                },
            };

            var fetchedRulesCursor = Mock.Of<IAsyncCursor<RulesetDataModel>>();
            Mock.Get(fetchedRulesCursor)
                .SetupSequence(x => x.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            Mock.Get(fetchedRulesCursor)
                .SetupGet(x => x.Current)
                .Returns(contentTypeDataModels);
            Mock.Get(fetchedRulesCursor)
                .Setup(x => x.Dispose());

            var contentTypesCollection = Mock.Of<IMongoCollection<RulesetDataModel>>();
            Mock.Get(contentTypesCollection)
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<RulesetDataModel>>(), It.IsAny<FindOptions<RulesetDataModel, RulesetDataModel>>(), default))
                .ReturnsAsync(fetchedRulesCursor);

            var mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<RulesetDataModel>(It.IsAny<string>(), null))
                .Returns(contentTypesCollection);

            var mongoClient = Mock.Of<IMongoClient>();
            Mock.Get(mongoClient)
                .Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabase);

            var mongoDbProviderSettings = new MongoDbProviderSettings
            {
                DatabaseName = "TestDatabaseName",
                RulesCollectionName = "TestCollectionName"
            };

            var ruleFactory = Mock.Of<IRuleFactory>();

            var mongoDbProviderRulesDataSource = new MongoDbProviderRulesDataSource(
                mongoClient,
                mongoDbProviderSettings,
                ruleFactory);

            // Act
            var actual = await mongoDbProviderRulesDataSource.GetRulesetsAsync();

            // Assert
            actual.Should().NotBeNull()
                .And.HaveCount(1)
                .And.Contain(r => string.Equals(r.Name, nameof(RulesetNames.RulesetSample), StringComparison.Ordinal));
        }

        [Fact]
        public async Task GetRulesAsync_GivenContentTypeAndDatesInterval_ReturnsCollectionOfRules()
        {
            // Arrange
            var contentType = RulesetNames.RulesetSample.ToString();
            var dateBegin = new DateTime(2020, 03, 01);
            var dateEnd = new DateTime(2020, 04, 01);

            var ruleDataModels = new List<RuleDataModel>
            {
                new()
                {
                    Name = "Rule 1"
                },
                new()
                {
                    Name = "Rule 2"
                }
            };

            var fetchedRulesCursor = Mock.Of<IAsyncCursor<RuleDataModel>>();
            Mock.Get(fetchedRulesCursor)
                .SetupSequence(x => x.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            Mock.Get(fetchedRulesCursor)
                .SetupGet(x => x.Current)
                .Returns(ruleDataModels);
            Mock.Get(fetchedRulesCursor)
                .Setup(x => x.Dispose());

            var rulesCollection = Mock.Of<IMongoCollection<RuleDataModel>>();
            Mock.Get(rulesCollection)
                .Setup(x => x.FindAsync<RuleDataModel>(It.IsAny<FilterDefinition<RuleDataModel>>(), null, default))
                .ReturnsAsync(fetchedRulesCursor);

            var mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<RuleDataModel>(It.IsAny<string>(), null))
                .Returns(rulesCollection);

            var mongoClient = Mock.Of<IMongoClient>();
            Mock.Get(mongoClient)
                .Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabase);

            var mongoDbProviderSettings = new MongoDbProviderSettings
            {
                DatabaseName = "TestDatabaseName",
                RulesCollectionName = "TestCollectionName"
            };

            var ruleFactory = Mock.Of<IRuleFactory>();
            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(It.IsAny<RuleDataModel>()))
                .Returns<RuleDataModel>(x => Rule.Create(x.Name)
                    .OnRuleset("test ruleset")
                    .SetContent(new object())
                    .Since(dateBegin)
                    .Build()
                    .Rule);

            var mongoDbProviderRulesDataSource = new MongoDbProviderRulesDataSource(
                mongoClient,
                mongoDbProviderSettings,
                ruleFactory);

            // Act
            var rules = await mongoDbProviderRulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd);

            // Assert
            rules.Should().NotBeNull()
                .And.HaveCount(2);
        }
    }
}