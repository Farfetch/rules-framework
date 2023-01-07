namespace Rules.Framework.Evaluation.Compiled.ExpressionBuilders
{
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal static class ExpressionBuilder
    {
        public static INamedExpressionBuilder NewExpression(string name)
        {
            var expressionConfiguration = new ExpressionConfiguration
            {
                ExpressionName = name,
            };

            return new NamedExpressionBuilder(expressionConfiguration);
        }
    }
}