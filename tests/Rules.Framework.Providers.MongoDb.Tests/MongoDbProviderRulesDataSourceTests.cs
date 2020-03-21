namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Xunit;

    public class MongoDbProviderRulesDataSourceTests
    {
        [Fact]
        public async Task GetRulesAsync_GivenContentTypeAndDatesInterval_ReturnsCollectionOfRules()
        {
            // Arrange
            ContentType contentType = ContentType.ContentTypeSample;
            DateTime dateBegin = new DateTime(2020, 03, 01);
            DateTime dateEnd = new DateTime(2020, 04, 01);

            List<RuleDataModel> ruleDataModels = new List<RuleDataModel>
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

            IAsyncCursor<RuleDataModel> fetchedRulesCursor = Mock.Of<IAsyncCursor<RuleDataModel>>();
            Mock.Get(fetchedRulesCursor)
                .SetupSequence(x => x.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);
            Mock.Get(fetchedRulesCursor)
                .SetupGet(x => x.Current)
                .Returns(ruleDataModels);
            Mock.Get(fetchedRulesCursor)
                .Setup(x => x.Dispose());

            IMongoCollection<RuleDataModel> rulesCollection = Mock.Of<IMongoCollection<RuleDataModel>>();
            Mock.Get(rulesCollection)
                .Setup(x => x.FindAsync<RuleDataModel>(It.IsAny<FilterDefinition<RuleDataModel>>(), null, default))
                .ReturnsAsync(fetchedRulesCursor);

            IMongoDatabase mongoDatabase = Mock.Of<IMongoDatabase>();
            Mock.Get(mongoDatabase)
                .Setup(x => x.GetCollection<RuleDataModel>(It.IsAny<string>(), null))
                .Returns(rulesCollection);

            IMongoClient mongoClient = Mock.Of<IMongoClient>();
            Mock.Get(mongoClient)
                .Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabase);

            MongoDbProviderSettings mongoDbProviderSettings = new MongoDbProviderSettings
            {
                DatabaseName = "TestDatabaseName",
                RulesCollectionName = "TestCollectionName"
            };

            IRuleFactory<ContentType, ConditionType> ruleFactory = Mock.Of<IRuleFactory<ContentType, ConditionType>>();
            Mock.Get(ruleFactory)
                .Setup(x => x.CreateRule(It.IsAny<RuleDataModel>()))
                .Returns<RuleDataModel>(x => RuleBuilder.NewRule<ContentType, ConditionType>().WithName(x.Name).Build());

            MongoDbProviderRulesDataSource<ContentType, ConditionType> mongoDbProviderRulesDataSource = new MongoDbProviderRulesDataSource<ContentType, ConditionType>(
                mongoClient,
                mongoDbProviderSettings,
                ruleFactory);

            // Act
            IEnumerable<Rule<ContentType, ConditionType>> rules = await mongoDbProviderRulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd);

            // Assert
            rules.Should().NotBeNull()
                .And.HaveCount(2);
        }
    }
}