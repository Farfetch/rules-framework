using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rules.Framework.Core;
using Rules.Framework.IntegrationTests.ContentTypes;

namespace Rules.Framework.IntegrationTests.Tests
{
    [TestClass]
    public class BodyMassIndexTests
    {
        [TestMethod]
        public async Task BodyMassIndex_NoConditions_ReturnsDefaultFormula()
        {
            // Arrange
            string expectedFormulaDescription = "Body Mass Index default formula";
            string expectedFormulaValue = "weight / (height ^ 2)";
            const IntegrationTestsContentTypes expectedContent = IntegrationTestsContentTypes.BodyMassIndexFormula;
            DateTime expectedMatchDate = new DateTime(2018, 06, 01);
            Condition<IntegrationTestsConditionTypes>[] expectedConditions = new Condition<IntegrationTestsConditionTypes>[0];

            IRulesDataSource<IntegrationTestsContentTypes, IntegrationTestsConditionTypes> rulesDataSource = await RulesFromJsonFile.Load
                .FromJsonFileAsync<IntegrationTestsContentTypes, IntegrationTestsConditionTypes>($@"{Environment.CurrentDirectory}/Tests/BodyMassIndexTests.datasource.json");

            RulesEngineBuilder rulesEngineBuilder = new RulesEngineBuilder();

            RulesEngine<IntegrationTestsContentTypes, IntegrationTestsConditionTypes> rulesEngine = rulesEngineBuilder.CreateRulesEngine()
                .WithContentType<IntegrationTestsContentTypes>()
                .WithConditionType<IntegrationTestsConditionTypes>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act
            Rule<IntegrationTestsContentTypes, IntegrationTestsConditionTypes> actual = await rulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            Formula actualFormula = actual.ContentContainer.GetContentAs<Formula>();
            actualFormula.Description.Should().Be(expectedFormulaDescription);
            actualFormula.Value.Should().Be(expectedFormulaValue);
        }
    }
}