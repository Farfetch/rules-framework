namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GreaterThanOrEqualOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            ParameterExpression leftHandExpression = Expression.Parameter(typeof(int), "leftHand");
            Expression rightHandExpression = Expression.Constant(1, typeof(int));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0);

            GreaterThanOrEqualOneToOneConditionExpressionBuilder greaterThanOrEqualOneToOneConditionExpressionBuilder
                = new GreaterThanOrEqualOneToOneConditionExpressionBuilder();

            // Act
            Expression actualExpression = greaterThanOrEqualOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration);

            // Assert
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<int, bool>>(actualExpression, leftHandExpression).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke(2);
            var notNullLeftHandValueResult2 = compiledExpression.Invoke(0);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ThrowsNotSupportedException()
        {
            // Arrange
            ParameterExpression leftHandExpression = Expression.Parameter(typeof(string), "leftHand");
            Expression rightHandExpression = Expression.Constant("test", typeof(string));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null);

            GreaterThanOrEqualOneToOneConditionExpressionBuilder greaterThanOrEqualOneToOneConditionExpressionBuilder
                = new GreaterThanOrEqualOneToOneConditionExpressionBuilder();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => greaterThanOrEqualOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should()
                .Contain(DataTypes.String.ToString())
                .And
                .Contain(Operators.GreaterThan.ToString());
        }
    }
}
