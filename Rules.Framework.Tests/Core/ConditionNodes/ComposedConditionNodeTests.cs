using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rules.Framework.Core;
using Rules.Framework.Core.ConditionNodes;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests.Core.ConditionNodes
{
    [TestClass]
    public class ComposedConditionNodeTests
    {
        [TestMethod]
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
            Assert.AreEqual(expectedLogicalOperator, actualLogicalOperator);
            Assert.AreSame(expectedChildConditionNodes, actualChildConditionNodes);
        }
    }
}