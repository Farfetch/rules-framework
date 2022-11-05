namespace Rules.Framework.Tests.Evaluation
{
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class OperatorMetadataTests
    {
        [Fact]
        public void GetAllCombinations_NoConditions_ReturnsAllMultiplicitiesCombinedWithOperator()
        {
            // Arrange
            var expectedCombination = $"{Multiplicities.ManyToOne}-{Operators.Contains}";

            var operatorMetadata = new OperatorMetadata
            {
                Operator = Operators.Contains,
                SupportedMultiplicities = new[] { Multiplicities.ManyToOne }
            };

            // Act
            var combinations = operatorMetadata.GetAllCombinations();

            // Assert
            combinations.Should().NotBeNull().And.HaveCount(1);
            combinations.First().Should().Be(expectedCombination);
        }

        [Fact]
        public void HasSupportForOneMultiplicityAtLeft_WhenHasAtLeastOneToAnyMultiplicity_ReturnsTrue()
        {
            // Arrange
            var operatorMetadata = new OperatorMetadata
            {
                SupportedMultiplicities = new[] { Multiplicities.OneToMany }
            };

            // Act
            var result = operatorMetadata.HasSupportForOneMultiplicityAtLeft;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasSupportForOneMultiplicityAtLeft_WhenHasNoneOneToAnyMultiplicity_ReturnsFalse()
        {
            // Arrange
            var operatorMetadata = new OperatorMetadata
            {
                SupportedMultiplicities = new[] { Multiplicities.ManyToOne }
            };

            // Act
            var result = operatorMetadata.HasSupportForOneMultiplicityAtLeft;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasSupportForOneMultiplicityAtLeft_WhenSupportedMultiplicitiesIsNull_ReturnsFalse()
        {
            // Arrange
            var operatorMetadata = new OperatorMetadata
            {
                SupportedMultiplicities = null
            };

            // Act
            var result = operatorMetadata.HasSupportForOneMultiplicityAtLeft;

            // Assert
            result.Should().BeFalse();
        }
    }
}