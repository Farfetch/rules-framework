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

    public class CaseInsensitiveEndsWithOneToOneConditionExpressionBuilderTests
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

            var caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                = new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                .BuildConditionExpression(builder, args));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.CaseInsensitiveEndsWith.ToString())
                .And
                .Contain(DataTypes.Integer.ToString());
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression()
        {
            // Arrange
            var caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                = new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder();

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter<string>("leftHand");
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null),
                        LeftHandOperand = builder.GetParameter("leftHand"),
                        RightHandOperand = builder.Constant("fox"),
                    };
                    var conditionExpression = caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<string, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke("The quick brown fox");
            var notNullLeftHandValueResult2 = compiledExpression.Invoke("The quick brown Fox");
            var nullLeftHandValueResult = compiledExpression.Invoke(null);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeTrue();
            nullLeftHandValueResult.Should().BeFalse();
        }
    }
}