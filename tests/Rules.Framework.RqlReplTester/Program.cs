namespace Rules.Framework.RqlReplTester
{
    using System.Text;
    using Newtonsoft.Json;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark3;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Rules.Framework.Rql;

    internal class Program
    {
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
                            Console.WriteLine($"\t{resultSet.RqlStatement}");
                            Console.WriteLine("\t | # | Priority | Rule");
                            Console.WriteLine("\t----------");

                            foreach (var line in resultSet.Lines)
                            {
                                var content = line.Rule.ContentContainer.GetContentAs<object>();
                                Console.WriteLine($"\t | {line.LineNumber} | {line.Rule.Priority,-8} | {line.Rule.Name}: {JsonConvert.SerializeObject(content)}");
                            }
                        }
                        else if (resultSet.AffectedRules > 0)
                        {
                            Console.WriteLine($"\t{resultSet.RqlStatement}");
                            Console.WriteLine("\t----------");
                            Console.WriteLine($"\t {resultSet.AffectedRules} rules were affected.");
                        }
                        else
                        {
                            Console.WriteLine($"\t{resultSet.RqlStatement}");
                            Console.WriteLine("\t----------");
                            Console.WriteLine("\t (empty)");
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
                            .Append(" @{")
                            .Append(rqlError.BeginPosition)
                            .Append(':')
                            .Append(rqlError.EndPosition)
                            .Append('}');
                        Console.WriteLine(errorMessageBuilder.ToString());
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.WriteLine();
            }
        }
    }
}