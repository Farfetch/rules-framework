namespace Rules.Framework.Tests.Core
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using System;
    using Xunit;

    public class ConditionNodePropertiesTests
    {
        [Fact]
        public void GetCompiledDelegateKey_GivenMultiplicityString_ReturnsString()
        {
            // Arrange
            string multiplicity = "test";
            string expected = "_compilation_compiled_test";

            // Act
            string actual = ConditionNodeProperties.GetCompiledDelegateKey(multiplicity);

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetCompiledDelegateKey_GivenEmptyMultiplicityString_ReturnsString()
        {
            // Arrange
            string multiplicity = "";

            // Act
            ArgumentException argumentException = Assert.Throws<ArgumentException>(() => ConditionNodeProperties.GetCompiledDelegateKey(multiplicity));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be("multiplicity");
        }
    }
}
