namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal class ValueConditionNodeCompilerBase
    {

        protected static readonly MethodInfo changeTypeMethodInfo = typeof(Convert)
                .GetMethod(
                    nameof(Convert.ChangeType),
                    new[] { typeof(object), typeof(Type), typeof(IFormatProvider) });
        protected static readonly Type systemType = typeof(Type);
        protected static readonly Type formatProviderType = typeof(IFormatProvider);
        protected static readonly Type objectType = typeof(object);
        protected readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;

        protected ValueConditionNodeCompilerBase(IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
        }

        protected static object ConvertToDataType(object operand, string paramName, Type dataType, object dataDefault)
        {
            try
            {
                return Convert.ChangeType(operand
                    ?? dataDefault, dataType, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException ice)
            {
                throw new ArgumentException($"Parameter value or contained value is not convertible to {dataType.Name}.", paramName, ice);
            }
        }

        protected static Expression CreateGetConditionExpression<TConditionType>(TConditionType conditionType, ParameterExpression parameterExpression, DataTypeConfiguration dataTypeConfiguration)
        {
            var returnTargetExpression = Expression.Label(typeof(object));
            var outVariableExpression = Expression.Variable(typeof(object), "conditionValue");
            var dictionaryTryGetValueMethodInfo = typeof(IDictionary<TConditionType, object>).GetMethod("TryGetValue");
            var conditionTypeConstantExpression = Expression.Constant(conditionType, typeof(TConditionType));
            var testExpression = Expression.Call(parameterExpression, dictionaryTryGetValueMethodInfo, conditionTypeConstantExpression, outVariableExpression);
            var defaultValueExpression = Expression.Constant(dataTypeConfiguration.Default, typeof(object));
            var ifThenExpression = Expression.IfThen(testExpression, Expression.Return(returnTargetExpression, outVariableExpression));
            return Expression.Block(new[] { outVariableExpression }, new Expression[] { ifThenExpression, Expression.Label(returnTargetExpression, defaultValueExpression) });
        }

        protected static Expression CreateConvertedArrayExpression(object operand, Type dataType)
        {
            IEnumerable<object> operandAsTypedEnumerable = ConvertToTypedEnumerable(operand, nameof(operand), dataType);
            IEnumerable<Expression> arrayInitializerExpressions = operandAsTypedEnumerable.Select(o => Expression.Constant(o, dataType));
            return Expression.NewArrayInit(dataType, arrayInitializerExpressions);
        }

        protected static Expression CreateConvertedObjectExpression(Expression operandExpression, Type dataType, object dataTypeDefault)
        {
            var testExpression = Expression.NotEqual(operandExpression, Expression.Constant(null));
            var returnTargetLabelExpression = Expression.Label(typeof(object));
            var defaultValueExpression = Expression.Constant(dataTypeDefault, objectType);
            var ifThenElseExpression = Expression.IfThenElse(
                testExpression,
                Expression.Return(returnTargetLabelExpression, operandExpression),
                Expression.Return(returnTargetLabelExpression, defaultValueExpression));
            var nullCoalesceBlock = Expression.Block(ifThenElseExpression, Expression.Label(returnTargetLabelExpression, defaultValueExpression));

            var operandConvertExpression = Expression.Call(
                changeTypeMethodInfo,
                nullCoalesceBlock,
                Expression.Constant(dataType, systemType),
                Expression.Constant(CultureInfo.InvariantCulture, formatProviderType));
            return Expression.ConvertChecked(operandConvertExpression, dataType);
        }

        protected static Expression CreateConvertedObjectExpression(object operand, Type dataType, object dataTypeDefault)
        {
            var rightOperandConverted = ConvertToDataType(operand, "operand", dataType, dataTypeDefault);
            return Expression.ConvertChecked(Expression.Constant(rightOperandConverted), dataType);
        }

        protected static IEnumerable<object> ConvertToTypedEnumerable(object operand, string paramName, Type dataType)
        {
            if (operand is IEnumerable enumerable)
            {
                return enumerable.Cast<object>().Select(o => ConvertToDataType(o, paramName, dataType, null));
            }

            throw new ArgumentException($"Parameter must be of type {nameof(IEnumerable)}.", paramName);
        }

        protected DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);
    }
}
