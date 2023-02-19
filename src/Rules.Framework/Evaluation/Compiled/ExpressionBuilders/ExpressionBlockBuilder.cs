namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class ExpressionBlockBuilder : IExpressionBlockBuilder
    {
        private readonly ExpressionConfiguration expressionConfiguration;
        private readonly List<Expression> expressions;
        private readonly IExpressionBuilderFactory factory;
        private readonly Dictionary<string, LabelTarget> labelTargets;
        private readonly Dictionary<string, ParameterExpression> variables;

        public ExpressionBlockBuilder(
            string scopeName,
            IExpressionBlockBuilder parent,
            IExpressionBuilderFactory factory,
            ExpressionConfiguration expressionConfiguration)
        {
            this.expressionConfiguration = expressionConfiguration;
            this.expressions = new List<Expression>();
            this.labelTargets = new Dictionary<string, LabelTarget>(StringComparer.Ordinal);
            this.ScopeName = scopeName;
            this.Parent = parent;
            this.factory = factory;
            this.variables = new Dictionary<string, ParameterExpression>(StringComparer.Ordinal);
        }

        public IReadOnlyList<Expression> Expressions => this.expressions;

        public IReadOnlyDictionary<string, LabelTarget> LabelTargets => this.labelTargets;

        public IExpressionBlockBuilder Parent { get; }

        public string ScopeName { get; }

        public IReadOnlyDictionary<string, ParameterExpression> Variables => this.variables;

        public void AddExpression(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            this.expressions.Add(expression);
        }

        public Expression AndAlso(Expression left, Expression right)
            => Expression.AndAlso(left, right);

        public Expression AndAlso(IEnumerable<Expression> expressions)
        {
            if (expressions is null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            if (!expressions.Any())
            {
                throw new ArgumentException(
                    "A empty enumeration of expressions was provided and it should contain at least one expression",
                    nameof(expressions));
            }

            return expressions.Aggregate((left, right) => this.AndAlso(left, right));
        }

        public void Assign(Expression left, Expression right)
        {
            var expression = Expression.Assign(left, right);
            this.AddExpression(expression);
        }

        public Expression Block(Action<IExpressionBlockBuilder> implementationBuilder)
            => this.Block(string.Empty, implementationBuilder);

        public Expression Block(string scopeName, Action<IExpressionBlockBuilder> implementationBuilder)
        {
            if (implementationBuilder is null)
            {
                throw new ArgumentNullException(nameof(implementationBuilder));
            }

            var actualScopeName = scopeName ?? string.Empty;
            var expressionBlockBuilder = this.factory.CreateExpressionBlockBuilder(actualScopeName, this, this.expressionConfiguration);
            implementationBuilder.Invoke(expressionBlockBuilder);

            if (expressionBlockBuilder.Expressions.Count == 0)
            {
                throw new InvalidOperationException($"No body block expressions were added for '{expressionBlockBuilder.ScopeName}'.");
            }

            var expression = Expression.Block(expressions: expressionBlockBuilder.Expressions);

            return expression;
        }

        public Expression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
            => Expression.Call(instance, method, arguments);

        public Expression Call(Expression instance, MethodInfo method)
            => Expression.Call(instance, method);

        public Expression Constant<T>(T value)
            => this.Constant(value, typeof(T));

        public Expression Constant(object value, Type type)
            => Expression.Constant(value, type);

        public Expression ConvertChecked(Expression expression, Type type)
            => Expression.ConvertChecked(expression, type);

        public Expression ConvertChecked<T>(Expression expression)
            => this.ConvertChecked(expression, typeof(T));

        public LabelTarget CreateLabelTarget(string name)
        {
            var labelTarget = NewLabelTarget(name);
            this.labelTargets.Add(name, labelTarget);
            return labelTarget;

            LabelTarget NewLabelTarget(string name)
            {
                if (this.Parent is null)
                {
                    if (this.labelTargets.ContainsKey(name))
                    {
                        throw new InvalidOperationException($"A label target for name '{name}' was already added.");
                    }

                    return Expression.Label(name);
                }

                if (this.labelTargets.ContainsKey(name))
                {
                    throw new InvalidOperationException($"A label target for name '{name}' under scope '{this.ScopeName}' was already added.");
                }

                string prefixedName = $"{this.ScopeName}_{name}";
                return this.Parent.CreateLabelTarget(prefixedName);
            }
        }

        public ParameterExpression CreateVariable(string name, Type type)
        {
            var variableExpression = NewVariable(name, type);
            this.variables.Add(name, variableExpression);
            return variableExpression;

            ParameterExpression NewVariable(string name, Type type)
            {
                if (this.Parent is null)
                {
                    if (this.variables.ContainsKey(name))
                    {
                        throw new InvalidOperationException($"A variable for name '{name}' was already added.");
                    }

                    return Expression.Variable(type, name);
                }

                if (this.variables.ContainsKey(name))
                {
                    throw new InvalidOperationException($"A variable for name '{name}' under scope '{this.ScopeName}' was already added.");
                }

                string prefixedName = $"{this.ScopeName}_{name}";
                return this.Parent.CreateVariable(prefixedName, type);
            }
        }

        public ParameterExpression CreateVariable<T>(string name)
            => this.CreateVariable(name, typeof(T));

        public Expression Empty()
            => Expression.Empty();

        public Expression Equal(Expression left, Expression right)
            => Expression.Equal(left, right);

        public LabelTarget GetLabelTarget(string name)
        {
            if (this.labelTargets.TryGetValue(name, out var labelTarget))
            {
                return labelTarget;
            }

            throw new KeyNotFoundException($"A label target with name '{name}' was not found.");
        }

        public ParameterExpression GetParameter(string name)
        {
            if (this.expressionConfiguration.Parameters.TryGetValue(name, out var parameter))
            {
                return parameter;
            }

            throw new KeyNotFoundException($"A parameter with name '{name}' was not found.");
        }

        public ParameterExpression GetVariable(string name)
        {
            if (this.variables.TryGetValue(name, out var variable))
            {
                return variable;
            }

            throw new KeyNotFoundException($"A variable with name '{name}' was not found.");
        }

        public void Goto(LabelTarget labelTarget)
        {
            var expression = Expression.Goto(labelTarget);
            this.AddExpression(expression);
        }

        public Expression GreaterThan(Expression left, Expression right)
            => Expression.GreaterThan(left, right);

        public Expression GreaterThanOrEqual(Expression left, Expression right)
            => Expression.GreaterThanOrEqual(left, right);

        public void If(
            Func<IExpressionBlockBuilder, Expression> testExpressionBuilder,
            Func<IExpressionBlockBuilder, Expression> thenExpressionBuilder)
            => this.If(testExpressionBuilder, thenExpressionBuilder, elseExpressionBuilder: null);

        public void If(
            Func<IExpressionBlockBuilder, Expression> testExpressionBuilder,
            Func<IExpressionBlockBuilder, Expression> thenExpressionBuilder,
            Func<IExpressionBlockBuilder, Expression> elseExpressionBuilder)
        {
            if (testExpressionBuilder is null)
            {
                throw new ArgumentNullException(nameof(testExpressionBuilder));
            }

            if (thenExpressionBuilder is null)
            {
                throw new ArgumentNullException(nameof(thenExpressionBuilder));
            }

            var testExpression = testExpressionBuilder.Invoke(this);
            var thenExpression = thenExpressionBuilder.Invoke(this);

            Expression expression;
            if (elseExpressionBuilder is not null)
            {
                var elseExpression = elseExpressionBuilder.Invoke(this);
                expression = Expression.IfThenElse(testExpression, thenExpression, elseExpression);
            }
            else
            {
                expression = Expression.IfThen(testExpression, thenExpression);
            }

            this.AddExpression(expression);
        }

        public void Label(LabelTarget labelTarget)
        {
            var expression = Expression.Label(labelTarget);
            this.AddExpression(expression);
        }

        public Expression LessThan(Expression left, Expression right)
            => Expression.LessThan(left, right);

        public Expression LessThanOrEqual(Expression left, Expression right)
            => Expression.LessThanOrEqual(left, right);

        public Expression Not(Expression expression)
            => Expression.Not(expression);

        public Expression NotEqual(Expression left, Expression right)
            => Expression.NotEqual(left, right);

        public Expression OrElse(Expression left, Expression right)
            => Expression.OrElse(left, right);

        public Expression OrElse(IEnumerable<Expression> expressions)
        {
            if (expressions is null)
            {
                throw new ArgumentNullException(nameof(expressions));
            }

            if (!expressions.Any())
            {
                throw new ArgumentException(
                    "A empty enumeration of expressions was provided and it should contain at least one expression",
                    nameof(expressions));
            }

            return expressions.Aggregate((left, right) => this.OrElse(left, right));
        }

        public void Return(Expression returnValueExpression)
        {
            if (returnValueExpression is null)
            {
                throw new ArgumentNullException(nameof(returnValueExpression));
            }

            var returnExpression = Expression.Return(this.expressionConfiguration.ReturnLabelTarget, returnValueExpression);
            this.AddExpression(returnExpression);
        }

        public void Switch(
            Expression switchValueExpression,
            Action<IExpressionSwitchBuilder> expressionSwitchBuilderAction)
        {
            if (switchValueExpression is null)
            {
                throw new ArgumentNullException(nameof(switchValueExpression));
            }

            if (expressionSwitchBuilderAction is null)
            {
                throw new ArgumentNullException(nameof(expressionSwitchBuilderAction));
            }

            var expressionSwitchBuilder = this.factory.CreateExpressionSwitchBuilder(this);
            expressionSwitchBuilderAction.Invoke(expressionSwitchBuilder);
            var expression = Expression.Switch(
                switchValueExpression,
                expressionSwitchBuilder.DefaultBody,
                expressionSwitchBuilder.SwitchCases.ToArray());
            this.AddExpression(expression);
        }
    }
}