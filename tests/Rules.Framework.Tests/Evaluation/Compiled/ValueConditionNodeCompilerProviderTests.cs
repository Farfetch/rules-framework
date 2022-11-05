namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ValueConditionNodeCompilerProviderTests
    {
        public static IEnumerable<object[]> SuccessScenarios => new[]
        {
            new object[] { Multiplicities.OneToOne, typeof(OneToOneValueConditionNodeCompiler) },
            new object[] { Multiplicities.OneToMany, typeof(OneToManyValueConditionNodeCompiler) },
            new object[] { Multiplicities.ManyToOne, typeof(ManyToOneValueConditionNodeCompiler) },
            new object[] { Multiplicities.ManyToMany, typeof(ManyToManyValueConditionNodeCompiler) },
        };

        [Theory]
        [MemberData(nameof(SuccessScenarios))]
        public void GetValueConditionNodeCompiler_GivenValidMultiplicity_ReturnsMatchingCompiler(string multiplicity, Type compilerType)
        {
            // Arrange
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();

            ValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider
                = new ValueConditionNodeCompilerProvider(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            var valueConditionNodeCompiler = valueConditionNodeCompilerProvider.GetValueConditionNodeCompiler(multiplicity);

            // Assert
            valueConditionNodeCompiler.Should().NotBeNull().And.BeOfType(compilerType);
        }

        [Fact]
        public void GetValueConditionNodeCompiler_GivenUnknownMultiplicity_ThrowsInvalidOperationException()
        {
            // Arrange
            string multiplicity = "unknown-multiplicity";
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            IDataTypesConfigurationProvider dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();

            ValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider
                = new ValueConditionNodeCompilerProvider(conditionExpressionBuilderProvider, dataTypesConfigurationProvider);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => valueConditionNodeCompilerProvider.GetValueConditionNodeCompiler(multiplicity));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(multiplicity);
        }
    }
}
