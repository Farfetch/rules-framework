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
            var contentType = nameof(ContentType.ContentTypeSample);
            ContentTypeDataModel actual = null;
            var contentTypesCollection = Mock.Of<IMongoCollection<ContentTypeDataModel>>();
            Mock.Get(contentTypesCollection)
                .Setup(x => x.InsertOneAsync(It.IsAny<ContentTypeDataModel>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Callback<ContentTypeDataModel, InsertOneOptions, CancellationToken>((ct, opt, t) => actual = ct);

            var mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<ContentTypeDataModel>(It.IsAny<string>(), null))
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
            await mongoDbProviderRulesDataSource.CreateContentTypeAsync(contentType);

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
            var contentTypeDataModels = new[] { nameof(ContentType.ContentTypeSample), };

            var fetchedRulesCursor = Mock.Of<IAsyncCursor<string>>();
            Mock.Get(fetchedRulesCursor)
                .SetupSequence(x => x.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            Mock.Get(fetchedRulesCursor)
                .SetupGet(x => x.Current)
                .Returns(contentTypeDataModels);
            Mock.Get(fetchedRulesCursor)
                .Setup(x => x.Dispose());

            var contentTypesCollection = Mock.Of<IMongoCollection<ContentTypeDataModel>>();
            Mock.Get(contentTypesCollection)
                .Setup(x => x.FindAsync<string>(It.IsAny<FilterDefinition<ContentTypeDataModel>>(), It.IsAny<FindOptions<ContentTypeDataModel, string>>(), default))
                .ReturnsAsync(fetchedRulesCursor);

            var mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<ContentTypeDataModel>(It.IsAny<string>(), null))
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
            var actual = await mongoDbProviderRulesDataSource.GetContentTypesAsync();

            // Assert
            actual.Should().NotBeNull()
                .And.HaveCount(1)
                .And.Contain(nameof(ContentType.ContentTypeSample));
        }

        [Fact]
        public async Task GetRulesAsync_GivenContentTypeAndDatesInterval_ReturnsCollectionOfRules()
        {
            // Arrange
            var contentType = ContentType.ContentTypeSample.ToString();
            var dateBegin = new DateTime(2020, 03, 01);
            var dateEnd = new DateTime(2020, 04, 01);

            var ruleDataModels = new List<RuleDataModel>
            {
                new RuleDataModel
                {
                    Name = "Rule 1"
                },
                new RuleDataModel
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
                .Returns<RuleDataModel>(x => Rule.New().WithName(x.Name).Build().Rule);

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