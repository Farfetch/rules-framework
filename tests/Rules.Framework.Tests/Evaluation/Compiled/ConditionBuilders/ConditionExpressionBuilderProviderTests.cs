namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ConditionExpressionBuilderProviderTests
    {
        public static IEnumerable<object[]> Scenarios => OperatorsMetadata.All
            .SelectMany(c => c.SupportedMultiplicities.Select(m => new object[] { c.Operator, m }) )
            .ToList();

        [Theory]
        [MemberData(nameof(Scenarios))]
        public void GetConditionExpressionBuilderFor_GivenOperatorAndSupportedMultiplicity_ReturnsConditionExpressionBuilder(Operators @operator, string multiplicity)
        {
            // Arrange
            ConditionExpressionBuilderProvider conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();

            // Act
            IConditionExpressionBuilder conditionExpressionBuilder = conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(@operator, multiplicity);

            // Assert
            conditionExpressionBuilder.Should().NotBeNull();
        }

        [Fact]
        public void GetConditionExpressionBuilderFor_GivenOperatorAndNotSupportedMultiplicity_ThrowsNotSupportedException()
        {
            // Arrange
            Operators @operator = Operators.In;
            string multiplicity = Multiplicities.OneToOne;

            ConditionExpressionBuilderProvider conditionExpressionBuilderProvider = new ConditionExpressionBuilderProvider();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => conditionExpressionBuilderProvider.GetConditionExpressionBuilderFor(@operator, multiplicity));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(@operator.ToString()).And.Contain(multiplicity);
        }
    }
}
