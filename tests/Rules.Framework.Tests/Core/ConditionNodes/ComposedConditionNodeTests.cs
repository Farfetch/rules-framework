namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
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
    }
}