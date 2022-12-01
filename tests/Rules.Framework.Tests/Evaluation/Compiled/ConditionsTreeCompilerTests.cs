namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Tests.TestStubs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ConditionsTreeCompilerTests
    {
        [Fact]
        public void Compile_GivenComposedConditionNodeWith2ChildValueConditionNodes_CompilesConditionAndStoresExpressionAsProperty()
        {
            // Arrange
            ValueConditionNode<ConditionType> valueConditionNode1
                = new ValueConditionNode<ConditionType>(DataTypes.Integer, ConditionType.NumberOfSales, Operators.Equal, 100);
            ValueConditionNode<ConditionType> valueConditionNode2
                = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "GB");

            ComposedConditionNode<ConditionType> composedConditionNode
                = new ComposedConditionNode<ConditionType>(LogicalOperators.And, new[] { valueConditionNode1, valueConditionNode2 });
            Func<IDictionary<ConditionType, object>, bool> expectedCompiledFunc = (c) => true;

            IValueConditionNodeCompiler valueConditionNodeCompiler = Mock.Of<IValueConditionNodeCompiler>();
            Mock.Get(valueConditionNodeCompiler)
                .Setup(x => x.Compile(It.IsAny<ValueConditionNode<ConditionType>>(), It.IsAny<ParameterExpression>()))
                .Returns(expectedCompiledFunc);

            IValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider = Mock.Of<IValueConditionNodeCompilerProvider>();
            Mock.Get(valueConditionNodeCompilerProvider)
                .Setup(x => x.GetValueConditionNodeCompiler(Multiplicities.OneToOne))
                .Returns(valueConditionNodeCompiler);

            ConditionsTreeCompiler<ConditionType> conditionsTreeCompiler = new ConditionsTreeCompiler<ConditionType>(valueConditionNodeCompilerProvider);

            // Act
            conditionsTreeCompiler.Compile(composedConditionNode);

            // Assert
            valueConditionNode1.Properties.TryGetValue(ConditionNodeProperties.GetCompiledDelegateKey(Multiplicities.OneToOne), out var compiledExpression1);
            valueConditionNode2.Properties.TryGetValue(ConditionNodeProperties.GetCompiledDelegateKey(Multiplicities.OneToOne), out var compiledExpression2);
            compiledExpression1.Should()
                .NotBeNull()
                .And
                .BeOfType<Func<IDictionary<ConditionType, object>, bool>>()
                .And
                .BeSameAs(expectedCompiledFunc);
            compiledExpression2.Should()
                .NotBeNull()
                .And
                .BeOfType<Func<IDictionary<ConditionType, object>, bool>>()
                .And
                .BeSameAs(expectedCompiledFunc);
        }

        [Fact]
        public void Compile_GivenUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            StubConditionNode<ConditionType> stubConditionNode = new StubConditionNode<ConditionType>();

            IValueConditionNodeCompilerProvider valueConditionNodeCompilerProvider = Mock.Of<IValueConditionNodeCompilerProvider>();

            ConditionsTreeCompiler<ConditionType> conditionsTreeCompiler = new ConditionsTreeCompiler<ConditionType>(valueConditionNodeCompilerProvider);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => conditionsTreeCompiler.Compile(stubConditionNode));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(StubConditionNode<ConditionType>));
        }
    }
}
