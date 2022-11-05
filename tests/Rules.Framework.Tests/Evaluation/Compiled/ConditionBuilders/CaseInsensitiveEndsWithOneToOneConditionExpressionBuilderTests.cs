namespace Rules.Framework.Tests.Evaluation.Compiled.ConditionBuilders
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled.ConditionBuilders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class CaseInsensitiveEndsWithOneToOneConditionExpressionBuilderTests
    {
        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForString_ReturnsConditionExpression()
        {
            // Arrange

            ParameterExpression leftHandExpression = Expression.Parameter(typeof(string), "leftHand");
            Expression rightHandExpression = Expression.Constant("fox", typeof(string));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null);
            
            CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                = new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder();

            // Act
            Expression actualExpression = caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration);

            // Assert
            actualExpression.Should().NotBeNull();

            var compiledExpression = Expression.Lambda<Func<string, bool>>(actualExpression, leftHandExpression).Compile(true);
            var notNullLeftHandValueResult1 = compiledExpression.Invoke("The quick brown fox");
            var notNullLeftHandValueResult2 = compiledExpression.Invoke("The quick brown Fox");
            var nullLeftHandValueResult = compiledExpression.Invoke(null);

            notNullLeftHandValueResult1.Should().BeTrue();
            notNullLeftHandValueResult2.Should().BeTrue();
            nullLeftHandValueResult.Should().BeFalse();
        }

        [Fact]
        public void BuildConditionExpression_GivenLeftExpressionRightExpressionAndDataTypeConfigurationForInt_ReturnsConditionExpression()
        {
            // Arrange
            Expression leftHandExpression = Expression.Constant(1, typeof(int));
            Expression rightHandExpression = Expression.Constant(2, typeof(int));
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), null);

            CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                = new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => caseInsensitiveEndsWithOneToOneConditionExpressionBuilder
                .BuildConditionExpression(leftHandExpression, rightHandExpression, dataTypeConfiguration));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message
                .Should()
                .Contain(Operators.CaseInsensitiveEndsWith.ToString())
                .And
                .Contain(DataTypes.Integer.ToString());
        }
    }
}
