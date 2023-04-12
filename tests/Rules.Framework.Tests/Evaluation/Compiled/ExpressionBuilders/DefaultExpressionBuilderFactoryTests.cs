namespace Rules.Framework.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class DefaultExpressionBuilderFactoryTests
    {
        [Fact]
        public void CreateExpressionBlockBuilder_GivenNotNullExpressionConfigurationWithNullScopeAndParent_ReturnsNewExpressionBlockBuilder()
        {
            // Arrange
            var scope = "TestScope";
            var parent = Mock.Of<IExpressionBlockBuilder>();
            var expressionConfiguration = new ExpressionConfiguration();

            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var actual = defaultExpressionBuilderFactory.CreateExpressionBlockBuilder(scope, parent, expressionConfiguration);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionBlockBuilder>();
        }

        [Fact]
        public void CreateExpressionBlockBuilder_GivenNotNullExpressionConfigurationWithScopeAndParent_ReturnsNewExpressionBlockBuilder()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();

            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var actual = defaultExpressionBuilderFactory.CreateExpressionBlockBuilder(null, null, expressionConfiguration);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionBlockBuilder>();
        }

        [Fact]
        public void CreateExpressionBlockBuilder_GivenNullExpressionConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var action = FluentActions.Invoking(() => defaultExpressionBuilderFactory.CreateExpressionBlockBuilder(null, null, null));

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("expressionConfiguration");
        }

        [Fact]
        public void CreateExpressionBuilder_GivenNotNullExpressionConfiguration_ReturnsNewExpressionBuilder()
        {
            // Arrange
            var expressionConfiguration = new ExpressionConfiguration();

            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var actual = defaultExpressionBuilderFactory.CreateExpressionBuilder(expressionConfiguration);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionBuilder>();
        }

        [Fact]
        public void CreateExpressionBuilder_GivenNullExpressionConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var action = FluentActions.Invoking(() => defaultExpressionBuilderFactory.CreateExpressionBuilder(null));

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("expressionConfiguration");
        }

        [Fact]
        public void CreateExpressionParametersConfiguration_NoConditions_ReturnsExpressionParametersConfiguration()
        {
            // Arrange
            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var actual = defaultExpressionBuilderFactory.CreateExpressionParametersConfiguration();

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionParametersConfiguration>();
        }

        [Fact]
        public void CreateSwitchExpressionBuilder_GivenNotNullParent_ReturnsNewExpressionSwitchBuilder()
        {
            // Arrange
            var parent = Mock.Of<IExpressionBlockBuilder>();

            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var actual = defaultExpressionBuilderFactory.CreateExpressionSwitchBuilder(parent);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionSwitchBuilder>();
        }

        [Fact]
        public void CreateSwitchExpressionBuilder_GivenNullParent_ThrowsArgumentNullException()
        {
            // Arrange
            var defaultExpressionBuilderFactory = new DefaultExpressionBuilderFactory();

            // Act
            var action = FluentActions.Invoking(() => defaultExpressionBuilderFactory.CreateExpressionSwitchBuilder(null));

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("parent");
        }
    }
}