namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ContainsManyToOneConditionExpressionBuilderTests
    {
        private readonly ContainsManyToOneConditionExpressionBuilder conditionExpressionBuilder;

        public ContainsManyToOneConditionExpressionBuilderTests()
        {
            this.conditionExpressionBuilder = new ContainsManyToOneConditionExpressionBuilder();
        }

        public static IEnumerable<object[]> NotSupportedExceptionCases => new[]
        {
            new object[] { DataTypes.ArrayBoolean, typeof(IEnumerable<bool>), new[] { true, false } },
            new object[] { DataTypes.ArrayDecimal, typeof(IEnumerable<decimal>), new[] { 1.1m, 2.6m } },
            new object[] { DataTypes.ArrayInteger, typeof(IEnumerable<int>), new[] { 1, 2 } },
            new object[] { DataTypes.ArrayString, typeof(IEnumerable<string>), new[] { "A", "B" } },
        };

        public static IEnumerable<object[]> ValidCases => new[]
        {
            new object[]{ DataTypes.Boolean, typeof(IEnumerable<bool>), typeof(bool), new[] { true, }, new[] { false, }, true },
            new object[]{ DataTypes.Decimal, typeof(IEnumerable<decimal>), typeof(decimal), new[] { 10.5m, 3.6m, 1.9m, }, new[] { 2.4m, 5.6m, 7.0m, }, 10.5m },
            new object[]{ DataTypes.Integer, typeof(IEnumerable<int>), typeof(int), new[] { 1, 2, 3, 4, 5, }, new[] { 10, 11, 12, 13, 14, }, 3 },
            new object[]{ DataTypes.String, typeof(IEnumerable<string>), typeof(string), new[] { "A", "B", "C", "D", }, new[] { "E", "F", "G", "H", }, "C" },
        };

        [Theory]
        [MemberData(nameof(NotSupportedExceptionCases))]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression(
            DataTypes dataType,
            Type type,
            object leftOperand)
        {
            // Arrange
            var args = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = DataTypeConfiguration.Create(dataType, type, null),
                LeftHandOperand = Expression.Constant(leftOperand),
                RightHandOperand = Expression.Constant(2),
            };

            var builder = Mock.Of<IExpressionBlockBuilder>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => this.conditionExpressionBuilder
                .BuildConditionExpression(builder, args));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.Contains.ToString())
                .And
                .Contain(dataType.ToString());
        }

        [Theory]
        [MemberData(nameof(ValidCases))]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression(
            DataTypes dataType,
            Type leftOperandType,
            Type rightOperandType,
            object leftOperandMatch,
            object leftOperandNonMatch,
            object rightOperand)
        {
            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestCondition")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftHand", typeof(object));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var args = new BuildConditionExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(dataType, rightOperandType, null),
                        LeftHandOperand = builder.ConvertChecked(builder.GetParameter("leftHand"), leftOperandType),
                        RightHandOperand = builder.ConvertChecked(builder.Constant(rightOperand), rightOperandType),
                    };
                    var conditionExpression = this.conditionExpressionBuilder
                        .BuildConditionExpression(builder, args);

                    builder.Return(conditionExpression);
                })
                .Build();

            // Assert
            var actualExpression = expressionResult.Implementation;
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<object, bool>>(actualExpression, expressionResult.Parameters).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke(leftOperandMatch);
            var notNullLeftHandValueResult2 = compiledExpression.Invoke(leftOperandNonMatch);
            var nullLeftHandValueResult = compiledExpression.Invoke(null);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
            nullLeftHandValueResult.Should().BeFalse();
        }
    }
}