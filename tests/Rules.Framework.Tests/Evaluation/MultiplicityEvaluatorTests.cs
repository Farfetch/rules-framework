namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MultiplicityEvaluatorTests
    {
        public static IEnumerable<object[]> SuccessCombinations => new[]
        {
            new object[] { 1, Operators.Equal, 2, Multiplicities.OneToOne },
            new object[] { 1, Operators.Equal, new[] { 1, 2 }, Multiplicities.OneToMany },
            new object[] { new[] { 1, 2 }, Operators.Equal, new[] { 1, 2 }, Multiplicities.ManyToMany },
            new object[] { new[] { 1, 2 }, Operators.Equal, 2 , Multiplicities.ManyToOne },
            new object[] { null, Operators.Equal, new[] { 1, 2 } , Multiplicities.OneToMany },
            new object[] { null, Operators.Equal, 2 , Multiplicities.OneToOne },
        };

        [Theory]
        [MemberData(nameof(SuccessCombinations))]
        public void EvaluateMultiplicity_GivenLeftOperandOperatorAndRightOperand_ReturnsMultiplicity(
            object leftOperand,
            Operators @operator,
            object rightOperand,
            string expectedMultiplicity)
        {
            // Arrange
            var multiplicityEvaluator = new MultiplicityEvaluator();

            // Act
            var multiplicity = multiplicityEvaluator.EvaluateMultiplicity(leftOperand, @operator, rightOperand);

            // Assert
            multiplicity.Should().NotBeNullOrWhiteSpace().And.Be(expectedMultiplicity);
        }

        [Fact]
        public void EvaluateMultiplicity_GivenLeftOperandUnknownOperatorAndRightOperand_ThrowsNotSupportedException()
        {
            // Arrange
            object leftOperand = null;
            const Operators @operator = 0;
            const int rightOperand = 1;
            var multiplicityEvaluator = new MultiplicityEvaluator();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => multiplicityEvaluator.EvaluateMultiplicity(leftOperand, @operator, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
        }
    }
}