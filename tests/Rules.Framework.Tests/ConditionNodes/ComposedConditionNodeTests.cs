namespace Rules.Framework.Tests.ConditionNodes
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Xunit;

    public class ComposedConditionNodeTests
    {
        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            var expectedLogicalOperator = LogicalOperators.Eval;
            var conditionNode1 = Mock.Of<IConditionNode>();
            var conditionNode2 = Mock.Of<IConditionNode>();
            Mock.Get(conditionNode1)
                .Setup(x => x.Clone())
                .Returns(conditionNode1);
            Mock.Get(conditionNode2)
                .Setup(x => x.Clone())
                .Returns(conditionNode2);

            var expectedChildConditionNodes = new[] { conditionNode1, conditionNode2 };

            var sut = new ComposedConditionNode(expectedLogicalOperator, expectedChildConditionNodes);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ComposedConditionNode>();
            var valueConditionNode = actual.As<ComposedConditionNode>();
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.ChildConditionNodes.Should().BeEquivalentTo(expectedChildConditionNodes);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void ComposedConditionNode_Init_GivenSetupWithChildConditionsAndLogicalOperator_ReturnsSettedValues()
        {
            // Arrange
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedChildConditionNodes = new[]
            {
                new Mock<IConditionNode>().Object,
                new Mock<IConditionNode>().Object
            };

            var sut = new ComposedConditionNode(expectedLogicalOperator, expectedChildConditionNodes);

            // Act
            var actualLogicalOperator = sut.LogicalOperator;
            var actualChildConditionNodes = sut.ChildConditionNodes;

            // Assert
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualChildConditionNodes.Should().NotBeNull().And.BeSameAs(expectedChildConditionNodes);
        }
    }
}