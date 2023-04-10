namespace Rules.Framework.Tests.Evaluation
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class EvaluationOptionsTests
    {
        public static IEnumerable<object[]> EqualsScenarios => OperatorEqualityScenarios.Concat(
            new[]
            {
                new object[] { new object(), false }
            })
            .ToList();

        public static IEnumerable<object[]> OperatorEqualityScenarios => new[]
        {
            new object[] { new EvaluationOptions { ExcludeRulesWithoutSearchConditions = true, MatchMode = MatchModes.Search}, false },
            new object[] { new EvaluationOptions { ExcludeRulesWithoutSearchConditions = true, MatchMode = MatchModes.Exact}, false },
            new object[] { new EvaluationOptions { ExcludeRulesWithoutSearchConditions = false, MatchMode = MatchModes.Search}, false },
            new object[] { new EvaluationOptions { ExcludeRulesWithoutSearchConditions = false, MatchMode = MatchModes.Exact}, true }
        };

        public static IEnumerable<object[]> OperatorInequalityScenarios => OperatorEqualityScenarios
            .Select(a => new object[] { a[0], !((bool)a[1]) })
            .ToList();

        [Theory]
        [MemberData(nameof(EqualsScenarios))]
        public void Equals_GivenObject_ReturnsComparisonResult(object compared, bool expetedResult)
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact
            };

            // Act
            var actualResult = evaluationOptions.Equals(compared);

            // Assert
            actualResult.Should().Be(expetedResult);
        }

        [Fact]
        public void GetHashCode_NoConditions_ReturnsHashCode()
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact
            };

            // Act
            var hashCode = evaluationOptions.GetHashCode();

            // Assert
            hashCode.Should().NotBe(0);
        }

        [Theory]
        [MemberData(nameof(OperatorEqualityScenarios))]
        public void OperatorEquality_GivenOtherInstance_ReturnsComparisonResult(object compared, bool expetedResult)
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact
            };
            var comparedEvaluationOptions = (EvaluationOptions)compared;

            // Act
            var actualResult = evaluationOptions == comparedEvaluationOptions;

            // Assert
            actualResult.Should().Be(expetedResult);
        }

        [Theory]
        [MemberData(nameof(OperatorInequalityScenarios))]
        public void OperatorInequality_GivenOtherInstance_ReturnsComparisonResult(object compared, bool expetedResult)
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact
            };
            var comparedEvaluationOptions = (EvaluationOptions)compared;

            // Act
            var actualResult = evaluationOptions != comparedEvaluationOptions;

            // Assert
            actualResult.Should().Be(expetedResult);
        }
    }
}