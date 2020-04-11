namespace Rules.Framework.IntegrationTests.Tests.Scenario2
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Xunit;

    public class CarInsuranceAdvisorTests
    {
        [Fact]
        public async Task BodyMassIndex_NoConditions_ReturnsDefaultFormula()
        {
            // Arrange
            CarInsuranceAdvices expected = CarInsuranceAdvices.RefusePaymentPerFranchise;
            const ContentTypes expectedContent = ContentTypes.CarInsuranceAdvice;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<ConditionTypes>[] expectedConditions = new Condition<ConditionTypes>[]
            {
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCosts,
                    Value = 800.00000m
                },
                new Condition<ConditionTypes>
                {
                    Type = ConditionTypes.RepairCostsCommercialValueRate,
                    Value = 23.45602m
                }
            };

            IRulesDataSource<ContentTypes, ConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<ContentTypes, ConditionTypes>($@"{Environment.CurrentDirectory}/Tests/Scenario2/CarInsuranceAdvisorTests.datasource.json", serializedContent: false);

            RulesEngine<ContentTypes, ConditionTypes> rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Configure(reo =>
                {
                    reo.PriotityCriteria = PriorityCriterias.BottommostRuleWins;
                })
                .Build();

            // Act
            Rule<ContentTypes, ConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            CarInsuranceAdvices actualContent = actual.ContentContainer.GetContentAs<CarInsuranceAdvices>();
            actualContent.Should().Be(expected);
        }
    }
}