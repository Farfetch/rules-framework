namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal interface IImplementationExpressionBuilder
    {
        string ScopeName { get; }

        void AddExpression(Expression expression);

        Expression AndAlso(Expression left, Expression right);

        Expression AndAlso(IEnumerable<Expression> expressions);

        void Assign(Expression left, Expression right);

        Expression Block(Action<IImplementationExpressionBuilder> blcokImplementationBuilderAction);

        Expression Block(string scopeName, Action<IImplementationExpressionBuilder> blockImplementationBuilderAction);

        Expression Call(Expression instance, MethodInfo method);

        Expression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments);

        Expression Constant<T>(T value);

        Expression Constant(object value, Type type);

        Expression ConvertChecked<T>(Expression expression);

        Expression ConvertChecked(Expression expression, Type type);

        LabelTarget CreateLabelTarget(string name);

        ParameterExpression CreateVariable<T>(string name);

        ParameterExpression CreateVariable(string name, Type type);

        Expression Empty();

        Expression Equal(Expression left, Expression right);

        LabelTarget GetLabelTarget(string name);

        ParameterExpression GetParameter(string name);

        ParameterExpression GetVariable(string name);

        void Goto(LabelTarget labelTarget);

        Expression GreaterThan(Expression left, Expression right);

        Expression GreaterThanOrEqual(Expression left, Expression right);

        void If(
            Func<IImplementationExpressionBuilder, Expression> testExpressionBuilderFunc,
            Func<IImplementationExpressionBuilder, Expression> thenExpressionBuilderFunc);

        void If(
            Func<IImplementationExpressionBuilder, Expression> testExpressionBuilderFunc,
            Func<IImplementationExpressionBuilder, Expression> thenExpressionBuilderFunc,
            Func<IImplementationExpressionBuilder, Expression> elseExpressionBuilderFunc);

        void Label(LabelTarget labelTarget);

        Expression LessThan(Expression left, Expression right);

        Expression LessThanOrEqual(Expression left, Expression right);

        Expression Not(Expression expression);

        Expression NotEqual(Expression left, Expression right);

        Expression OrElse(Expression left, Expression right);

        Expression OrElse(IEnumerable<Expression> expressions);

        void Return(Expression returnValueExpression);

        void Switch(
            Expression switchExpressionValue,
            Action<ISwitchExpressionBuilder> switchExpressionBuilderAction);
    }
}