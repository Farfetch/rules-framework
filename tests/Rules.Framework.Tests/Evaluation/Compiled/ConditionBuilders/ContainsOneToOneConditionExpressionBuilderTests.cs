namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;
    using Xunit;

    public class ContainsOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            var args = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), null),
                LeftHandOperand = Expression.Constant(1),
                RightHandOperand = Expression.Constant(2),
            };

            var builder = Mock.Of<IImplementationExpressionBuilder>();

            var containsOneToOneConditionExpressionBuilder
                = new ContainsOneToOneConditionExpressionBuilder();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => containsOneToOneConditionExpressionBuilder
                .BuildConditionExpression(builder, args));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.Contains.ToString())
                .And
                .Contain(DataTypes.Integer.ToString());
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression()
        {
            // Arrange
            var containsOneToOneConditionExpressionBuilder
                = new ContainsOneToOneConditionExpressionBuilder();

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftHand", typeof(string));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null),
                        LeftHandOperand = builder.GetParameter("leftHand"),
                        RightHandOperand = builder.Constant("quick"),
                    };
                    var conditionExpression = containsOneToOneConditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<string, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke("The quick brown fox");
            var notNullLeftHandValueResult2 = compiledExpression.Invoke("The Quick brown fox");
            var nullLeftHandValueResult = compiledExpression.Invoke(null);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
            nullLeftHandValueResult.Should().BeFalse();
        }
    }
}