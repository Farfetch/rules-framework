namespace Rules.Framework.RqlReplTester
{
    using System.Text;
    using Newtonsoft.Json;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark3;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Rules.Framework.Rql;

    internal class Program
    {
        private static readonly string tab = new string(' ', 4);

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
                    var results = await rqlClient.ExecuteQueryAsync(input);
                    Console.WriteLine("RESULT:");
                    foreach (var resultSet in results)
                    {
                        if (resultSet.Lines.Any())
                        {
                            Console.WriteLine($"{tab}{resultSet.RqlStatement}");
                            Console.WriteLine($"{tab}{new string('-', Math.Min(resultSet.RqlStatement.Length, Console.WindowWidth - 4))}");
                            if (resultSet.AffectedRules > 0)
                            {
                                Console.WriteLine($"{tab} {resultSet.AffectedRules} rules were affected.");
                            }
                            else
                            {
                                Console.WriteLine($"{tab} {resultSet.Lines.Count} rules were returned.");
                            }

                            Console.WriteLine();
                            Console.WriteLine($"{tab} | # | Priority | Status   | Range                     | Rule");
                            Console.WriteLine($"{tab} ------------------------------------------------------------");

                            foreach (var line in resultSet.Lines)
                            {
                                var lineNumber = line.LineNumber.ToString();
                                var priority = line.Rule.Priority.ToString();
                                var active = line.Rule.Active ? "Active" : "Inactive";
                                var dateBegin = line.Rule.DateBegin.Date.ToString("yyyy-MM-ddZ");
                                var dateEnd = line.Rule.DateEnd?.Date.ToString("yyyy-MM-ddZ") ?? "(no end)";
                                var ruleName = line.Rule.Name;
                                var content = JsonConvert.SerializeObject(line.Rule.ContentContainer.GetContentAs<object>());

                                Console.WriteLine($"{tab} | {lineNumber} | {priority,-8} | {active,-8} | {dateBegin,-11} - {dateEnd,-11} | {ruleName}: {content}");
                            }
                        }
                        else if (resultSet.AffectedRules > 0)
                        {
                            Console.WriteLine($"{tab}{resultSet.RqlStatement}");
                            Console.WriteLine($"{tab}{new string('-', resultSet.RqlStatement.Length)}");
                            Console.WriteLine($"{tab} {resultSet.AffectedRules} rules were affected.");
                        }
                        else
                        {
                            Console.WriteLine($"{tab}{resultSet.RqlStatement}");
                            Console.WriteLine($"{tab}{new string('-', resultSet.RqlStatement.Length)}");
                            Console.WriteLine($"{tab} (empty)");
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