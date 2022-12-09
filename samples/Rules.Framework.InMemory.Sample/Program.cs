namespace Rules.Framework.InMemory.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using global::Rules.Framework.InMemory.Sample.Engine;
    using global::Rules.Framework.InMemory.Sample.Enums;
    using global::Rules.Framework.InMemory.Sample.Helper;
    using global::Rules.Framework.InMemory.Sample.Rules;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var rulesService = new RulesService(new List<IContentTypes>()
            {
                new TestNumberRules()
            });

            var targetDate = DateTime.UtcNow;

            while (true)
            {
                Console.WriteLine("Type a number and see the rule and value configured it for or use 0 to exit");
                var input = Console.ReadLine().ToLower(CultureInfo.InvariantCulture);

                if (int.TryParse(input, out var value))
                {
                    if (value == 0)
                    {
                        break;
                    }

                    var conditions = new Dictionary<ConditionTypes, object> {
                        { ConditionTypes.IsPrimeNumber, value.IsPrime() },
                        { ConditionTypes.CanNumberBeDividedBy3, value.CanNumberBeDividedBy3() },
                        { ConditionTypes.SumAll, value.SumAll() },
                        { ConditionTypes.RoyalNumber, value }
                    };

                    var result = rulesService
                        .MatchOneAsync<string>(ContentTypes.TestNumber, targetDate, conditions)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    Console.WriteLine($"The result is: {result}");
                }
            }
        }
    }
}