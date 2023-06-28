namespace Rules.Framework.RqlReplTester
{
    using System.Text;
    using Newtonsoft.Json;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark3;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Runtime.Types;

    internal class Program
    {
        private static readonly string tab = new string(' ', 4);

        private static void HandleObjectResult(ValueResult result)
        {
            Console.WriteLine();
            var rawValue = result.Value switch
            {
                RqlAny rqlAny when rqlAny.UnderlyingType == RqlTypes.Object => rqlAny.ToString() ?? string.Empty,
                RqlAny rqlAny => rqlAny.ToString() ?? string.Empty,
                _ => result.Value.ToString(),
            };
            var value = rawValue!.Replace("\n", $"\n{tab}");
            Console.WriteLine($"{tab}{value}");
        }

        private static void HandleRulesSetResult(RulesSetResult<ContentTypes, ConditionTypes> result)
        {
            Console.WriteLine();
            if (result.Lines.Any())
            {
                Console.WriteLine($"{tab}{result.Rql}");
                Console.WriteLine($"{tab}{new string('-', Math.Min(result.Rql.Length, Console.WindowWidth - 5))}");
                if (result.NumberOfRules > 0)
                {
                    Console.WriteLine($"{tab} {result.NumberOfRules} rules were returned.");
                }
                else
                {
                    Console.WriteLine($"{tab} {result.Lines.Count} rules were returned.");
                }

                Console.WriteLine();
                Console.WriteLine($"{tab} | # | Priority | Status   | Range                     | Rule");
                Console.WriteLine($"{tab}{new string('-', Console.WindowWidth - 5)}");

                foreach (var line in result.Lines)
                {
                    var rule = line.Rule.Value;
                    var lineNumber = line.LineNumber.ToString();
                    var priority = rule.Priority.ToString();
                    var active = rule.Active ? "Active" : "Inactive";
                    var dateBegin = rule.DateBegin.Date.ToString("yyyy-MM-ddZ");
                    var dateEnd = rule.DateEnd?.Date.ToString("yyyy-MM-ddZ") ?? "(no end)";
                    var ruleName = rule.Name;
                    var content = JsonConvert.SerializeObject(rule.ContentContainer.GetContentAs<object>());

                    Console.WriteLine($"{tab} | {lineNumber} | {priority,-8} | {active,-8} | {dateBegin,-11} - {dateEnd,-11} | {ruleName}: {content}");
                }
            }
            else if (result.NumberOfRules > 0)
            {
                Console.WriteLine($"{tab}{result.Rql}");
                Console.WriteLine($"{tab}{new string('-', result.Rql.Length)}");
                Console.WriteLine($"{tab} {result.NumberOfRules} rules were affected.");
            }
            else
            {
                Console.WriteLine($"{tab}{result.Rql}");
                Console.WriteLine($"{tab}{new string('-', result.Rql.Length)}");
                Console.WriteLine($"{tab} (empty)");
            }
        }

        private static async Task Main()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource()
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario8Data());
            var rqlClient = rulesEngine.GetRqlClient();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input.ToUpperInvariant() == "EXIT")
                {
                    break;
                }

                try
                {
                    var results = await rqlClient.ExecuteAsync(input);
                    foreach (var result in results)
                    {
                        switch (result)
                        {
                            case RulesSetResult<ContentTypes, ConditionTypes> rulesResultSet:
                                HandleRulesSetResult(rulesResultSet);
                                break;

                            case NothingResult:
                                // Nothing to be done.
                                break;

                            case ValueResult valueResult:
                                HandleObjectResult(valueResult);
                                break;

                            default:
                                throw new NotSupportedException($"Result type is not supported: '{result.GetType().FullName}'");
                        }
                    }
                }
                catch (RqlException rqlException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{rqlException.Message} Errors:");

                    foreach (var rqlError in rqlException.Errors)
                    {
                        var errorMessageBuilder = new StringBuilder(" - ")
                            .Append(rqlError.Text)
                            .Append(" @")
                            .Append(rqlError.BeginPosition)
                            .Append('-')
                            .Append(rqlError.EndPosition);
                        Console.WriteLine(errorMessageBuilder.ToString());
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.WriteLine();
            }
        }
    }
}