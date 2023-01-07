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

    public class NotContainsOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            var args = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), null),
                LeftHandOperand = Expression.Parameter(typeof(string), "leftHand"),
                RightHandOperand = Expression.Constant("test", typeof(string)),
            };

            var builder = Mock.Of<IImplementationExpressionBuilder>();

            var containsOneToOneConditionExpressionBuilder
                = new NotContainsOneToOneConditionExpressionBuilder();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => containsOneToOneConditionExpressionBuilder
                .BuildConditionExpression(builder, args));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.NotContains.ToString())
                .And
                .Contain(DataTypes.Integer.ToString());
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression()
        {
            // Arrange
            var containsOneToOneConditionExpressionBuilder
                = new NotContainsOneToOneConditionExpressionBuilder();

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

            notNullLeftHandValueResult1.Should().BeFalse();
            notNullLeftHandValueResult2.Should().BeTrue();
            nullLeftHandValueResult.Should().BeFalse();
        }
    }
}