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

    public class ContainsOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression()
        {
            // Arrange
            ParameterExpression leftHandExpression = Expression.Parameter(typeof(string), "leftHand");
            Expression rightHandExpression = Expression.Constant("quick", typeof(string));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null);

            ContainsOneToOneConditionExpressionBuilder containsOneToOneConditionExpressionBuilder
                = new ContainsOneToOneConditionExpressionBuilder();

            // Act
            Expression actualExpression = containsOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration);

            // Assert
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<string, bool>>(actualExpression, leftHandExpression).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke("The quick brown fox");
            var notNullLeftHandValueResult2 = compiledExpression.Invoke("The Quick brown fox");
            var nullLeftHandValueResult = compiledExpression.Invoke(null);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeFalse();
            nullLeftHandValueResult.Should().BeFalse();
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            Expression leftHandExpression = Expression.Constant(1, typeof(int));
            Expression rightHandExpression = Expression.Constant(2, typeof(int));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), null);

            ContainsOneToOneConditionExpressionBuilder containsOneToOneConditionExpressionBuilder
                = new ContainsOneToOneConditionExpressionBuilder();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => containsOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.Contains.ToString())
                .And
                .Contain(DataTypes.Integer.ToString());
        }
    }
}
