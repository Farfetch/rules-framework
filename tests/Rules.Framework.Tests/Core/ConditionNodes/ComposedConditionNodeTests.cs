namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ComposedConditionNodeTests
    {
        [Fact]
        public void ComposedConditionNode_Init_GivenSetupWithChildConditionsAndLogicalOperator_ReturnsSettedValues()
        {
            // Arrange
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            IEnumerable<IConditionNode<ConditionType>> expectedChildConditionNodes = new IConditionNode<ConditionType>[]
            {
                new Mock<IConditionNode<ConditionType>>().Object,
                new Mock<IConditionNode<ConditionType>>().Object
            };

            ComposedConditionNode<ConditionType> sut = new ComposedConditionNode<ConditionType>(expectedLogicalOperator, expectedChildConditionNodes);

            // Act
            LogicalOperators actualLogicalOperator = sut.LogicalOperator;
            IEnumerable<IConditionNode<ConditionType>> actualChildConditionNodes = sut.ChildConditionNodes;

            // Assert
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualChildConditionNodes.Should().NotBeNull().And.BeSameAs(expectedChildConditionNodes);
        }

        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            IConditionNode<ConditionType> conditionNode1 = Mock.Of<IConditionNode<ConditionType>>();
            IConditionNode<ConditionType> conditionNode2 = Mock.Of<IConditionNode<ConditionType>>();
            Mock.Get(conditionNode1)
                .Setup(x => x.Clone())
                .Returns(conditionNode1);
            Mock.Get(conditionNode2)
                .Setup(x => x.Clone())
                .Returns(conditionNode2);

            IEnumerable <IConditionNode<ConditionType>> expectedChildConditionNodes = new IConditionNode<ConditionType>[] { conditionNode1, conditionNode2 };

            ComposedConditionNode<ConditionType> sut = new ComposedConditionNode<ConditionType>(expectedLogicalOperator, expectedChildConditionNodes);
            sut.Properties["test"] = "test";

            // Act
            IConditionNode<ConditionType> actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ComposedConditionNode<ConditionType>>();
            ComposedConditionNode<ConditionType> valueConditionNode = actual.As<ComposedConditionNode<ConditionType>>();
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.ChildConditionNodes.Should().BeEquivalentTo(expectedChildConditionNodes);
            valueConditionNode.Properties.Should().BeEmpty();
        }
    }
}