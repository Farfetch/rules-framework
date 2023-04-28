namespace Rules.Framework.Providers.SqlServer.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.SqlServer.Models;
    using Xunit;

    using Model = Rules.Framework.SqlServer.Models;

    [ExcludeFromCodeCoverage]
    public class SqlServerProviderRulesDataSourceTests
    {
        [Fact]
        public void SqlServerProviderRulesDataSource_AddRuleAsync_ReturnSuccess()
        {
            // Arrange
            var rule = new Rule<ContentType, ConditionType>();
            var ruleDataModel = new Model.Rule();

            var rulesFactory = new Mock<IRuleFactory<ContentType, ConditionType>>();
            rulesFactory.Setup(x => x.CreateRule(rule))
                .Returns(ruleDataModel);

            var rulesFrameworkDbContext = new Mock<IRulesFrameworkDbContext>();
            rulesFrameworkDbContext.Setup(x => x.SaveChanges())
                .Returns(1);

            var sqlServerProviderRulesDataSource = new SqlServerProviderRulesDataSource<ContentType, ConditionType>(rulesFrameworkDbContext.Object, rulesFactory.Object);

            // Act
            var result = sqlServerProviderRulesDataSource.AddRuleAsync(rule);

            // Assert
            result.Should().Be(Task.CompletedTask);
        }

        [Fact]
        public void SqlServerProviderRulesDataSource_GetRulesAsync_ReturnSuccess()
        {
            // Arrange
            var contentType = new ContentType();

            var dateBegin = new DateTime(2023, 4, 28);

            var dateEnd = new DateTime(2023, 4, 29);

            var rules = new DbContextOptionsBuilder<RulesFrameworkDbContext>();

            var rulesFactory = new Mock<IRuleFactory<ContentType, ConditionType>>();

            var ruleList = new List<Model.Rule>();

            var queryable = ruleList.AsQueryable();

            var dbSet = new Mock<DbSet<Model.Rule>>();
            dbSet.As<IQueryable<Rule<ContentType, ConditionType>>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<Rule<ContentType, ConditionType>>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<Rule<ContentType, ConditionType>>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<Model.Rule>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            //dbSet.Setup(d => d.Add(It.IsAny<Model.Rule>())).Callback<Rule<ContentType, ConditionType>>(ruleList.Add);

            var rulesFrameworkDbContext = new Mock<IRulesFrameworkDbContext>();
            rulesFrameworkDbContext.SetupGet(x => x.Rules)
                .Returns(dbSet.Object);

            var sqlServerProviderRulesDataSource = new SqlServerProviderRulesDataSource<ContentType, ConditionType>(rulesFrameworkDbContext.Object, rulesFactory.Object);

            // Act
            var result = sqlServerProviderRulesDataSource.GetRulesAsync(contentType, dateBegin, dateEnd);

            // Assert
            result.Should().NotBeNull();
        }
    }
}