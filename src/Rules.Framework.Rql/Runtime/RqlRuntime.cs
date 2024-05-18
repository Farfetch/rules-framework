namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;

    internal class RqlRuntime<TContentType, TConditionType> : IRuntime<TContentType, TConditionType>
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;

        private RqlRuntime(IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        public static IRuntime<TContentType, TConditionType> Create(
            IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            return new RqlRuntime<TContentType, TConditionType>(rulesEngine);
        }

        public IRuntimeValue ApplyBinary(IRuntimeValue leftOperand, RqlOperators rqlOperator, IRuntimeValue rightOperand)
        {
            leftOperand = EnsureUnwrapped(leftOperand);
            rightOperand = EnsureUnwrapped(rightOperand);
            switch (rqlOperator)
            {
                case RqlOperators.Slash:
                    return Divide(leftOperand, rightOperand);

                case RqlOperators.Minus:
                    return Subtract(leftOperand, rightOperand);

                case RqlOperators.Star:
                    return Multiply(leftOperand, rightOperand);

                case RqlOperators.Plus:
                    return Sum(leftOperand, rightOperand);

                default:
                    return new RqlNothing();
            }
        }

        public IRuntimeValue ApplyUnary(IRuntimeValue value, RqlOperators rqlOperator)
        {
            value = EnsureUnwrapped(value);
            if (rqlOperator == RqlOperators.Minus)
            {
                if (value is RqlInteger rqlInteger)
                {
                    return new RqlInteger(-rqlInteger.Value);
                }

                if (value is RqlDecimal rqlDecimal)
                {
                    return new RqlDecimal(-rqlDecimal.Value);
                }
            }

            throw new RuntimeException($"Unary operator {rqlOperator} is not supported for value '{value}'.");
        }

        public async ValueTask<RqlArray> MatchRulesAsync(MatchRulesArgs<TContentType, TConditionType> matchRulesArgs)
        {
            if (matchRulesArgs.MatchCardinality == MatchCardinality.None)
            {
                throw new ArgumentException("A valid match cardinality must be provided.", nameof(matchRulesArgs));
            }

            if (matchRulesArgs.MatchCardinality == MatchCardinality.One)
            {
                var rule = await this.rulesEngine.MatchOneAsync(matchRulesArgs.ContentType, matchRulesArgs.MatchDate.Value, matchRulesArgs.Conditions).ConfigureAwait(false);
                if (rule != null)
                {
                    var rqlArrayOne = new RqlArray(1);
                    rqlArrayOne.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
                    return rqlArrayOne;
                }

                return new RqlArray(0);
            }

            var rules = await this.rulesEngine.MatchManyAsync(matchRulesArgs.ContentType, matchRulesArgs.MatchDate.Value, matchRulesArgs.Conditions).ConfigureAwait(false);
            var rqlArrayAll = new RqlArray(rules.Count());
            var i = 0;
            foreach (var rule in rules)
            {
                rqlArrayAll.SetAtIndex(i++, new RqlRule<TContentType, TConditionType>(rule));
            }

            return rqlArrayAll;
        }

        public async ValueTask<RqlArray> SearchRulesAsync(SearchRulesArgs<TContentType, TConditionType> searchRulesArgs)
        {
            var searchArgs = new SearchArgs<TContentType, TConditionType>(
                searchRulesArgs.ContentType,
                searchRulesArgs.DateBegin.Value,
                searchRulesArgs.DateEnd.Value)
            {
                Conditions = searchRulesArgs.Conditions,
                ExcludeRulesWithoutSearchConditions = true,
            };

            var rules = await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);
            var rqlArray = new RqlArray(rules.Count());
            var i = 0;
            foreach (var rule in rules)
            {
                rqlArray.SetAtIndex(i++, new RqlRule<TContentType, TConditionType>(rule));
            }

            return rqlArray;
        }

        private static IRuntimeValue Divide(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value / right.Value),
            RqlInteger when rightOperand is RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value / right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot divide operand of type {leftOperand.Type.Name}."),
        };

        private static IRuntimeValue EnsureUnwrapped(IRuntimeValue runtimeValue)
            => runtimeValue.Type == RqlTypes.Any ? ((RqlAny)runtimeValue).Unwrap() : runtimeValue;

        private static IRuntimeValue Multiply(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value * right.Value),
            RqlInteger when rightOperand is RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value * right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot multiply operand of type {leftOperand.Type.Name}."),
        };

        private static IRuntimeValue Subtract(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value - right.Value),
            RqlInteger when rightOperand is RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value - right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot subtract operand of type {leftOperand.Type.Name}."),
        };

        private static IRuntimeValue Sum(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value + right.Value),
            RqlInteger when rightOperand is RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value + right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot sum operand of type {leftOperand.Type.Name}."),
        };
    }
}