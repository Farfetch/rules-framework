namespace Rules.Framework.InMemory.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using global::Rules.Framework.InMemory.Sample.Engine;
    using global::Rules.Framework.InMemory.Sample.Enums;
    using global::Rules.Framework.InMemory.Sample.Helper;
    using global::Rules.Framework.InMemory.Sample.Rules;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var rulesService = new RulesService(new List<IRuleSpecificationsRegistrar>()
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

                    var conditions = new Dictionary<ConditionNames, object> {
                        { ConditionNames.IsPrimeNumber, value.IsPrime() },
                        { ConditionNames.CanNumberBeDividedBy3, value.CanNumberBeDividedBy3() },
                        { ConditionNames.SumAll, value.SumAll() },
                        { ConditionNames.RoyalNumber, value }
                    };

                    var result = await rulesService
                        .MatchOneAsync<string>(RulesetNames.TestNumber, targetDate, conditions);

                    Console.WriteLine($"The result is: {result}");
                }
            }
        }
    }
}