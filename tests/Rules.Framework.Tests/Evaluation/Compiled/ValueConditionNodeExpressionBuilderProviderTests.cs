namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Xunit;

    public class ValueConditionNodeExpressionBuilderProviderTests
    {
        public static IEnumerable<object[]> SuccessScenarios => new[]
        {
            new object[] { Multiplicities.OneToOne, typeof(OneToOneValueConditionNodeExpressionBuilder) },
            new object[] { Multiplicities.OneToMany, typeof(OneToManyValueConditionNodeExpressionBuilder) },
            new object[] { Multiplicities.ManyToOne, typeof(ManyToOneValueConditionNodeExpressionBuilder) },
            new object[] { Multiplicities.ManyToMany, typeof(ManyToManyValueConditionNodeExpressionBuilder) },
        };

        [Fact]
        public void GetExpressionBuilder_GivenUnknownMultiplicity_ThrowsInvalidOperationException()
        {
            // Arrange
            var multiplicity = "unknown-multiplicity";
            var conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();

            var valueConditionNodeExpressionBuilderProvider
                = new ValueConditionNodeExpressionBuilderProvider(conditionExpressionBuilderProvider);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => valueConditionNodeExpressionBuilderProvider.GetExpressionBuilder(multiplicity));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(multiplicity);
        }

        [Theory]
        [MemberData(nameof(SuccessScenarios))]
        public void GetExpressionBuilder_GivenValidMultiplicity_ReturnsMatchingBuilder(string multiplicity, Type compilerType)
        {
            // Arrange
            var conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();

            var valueConditionNodeExpressionBuilderProvider
                = new ValueConditionNodeExpressionBuilderProvider(conditionExpressionBuilderProvider);

            // Act
            var valueConditionNodeExpressionBuilder = valueConditionNodeExpressionBuilderProvider.GetExpressionBuilder(multiplicity);

            // Assert
            valueConditionNodeExpressionBuilder.Should().NotBeNull().And.BeOfType(compilerType);
        }
    }
}