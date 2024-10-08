namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class GreaterThanOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            var greaterThanOneToOneConditionExpressionBuilder
                = new GreaterThanOneToOneConditionExpressionBuilder();

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftHand", typeof(int));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0),
                        LeftHandOperand = builder.GetParameter("leftHand"),
                        RightHandOperand = builder.Constant(1),
                    };
                    var conditionExpression = greaterThanOneToOneConditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<int, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke(2);
            var notNullLeftHandValueResult2 = compiledExpression.Invoke(0);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ThrowsNotSupportedException()
        {
            // Arrange
            var args = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null),
                LeftHandOperand = Expression.Parameter(typeof(string), "leftHand"),
                RightHandOperand = Expression.Constant("test", typeof(string)),
            };

            var builder = Mock.Of<IExpressionBlockBuilder>();

            var greaterThanOneToOneConditionExpressionBuilder
                = new GreaterThanOneToOneConditionExpressionBuilder();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => greaterThanOneToOneConditionExpressionBuilder
                .BuildConditionExpression(builder, args));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should()
                .Contain(DataTypes.String.ToString())
                .And
                .Contain(Operators.GreaterThan.ToString());
        }
    }
}