namespace Rules.Framework.Rql.IntegrationTests.GrammarCheck
{
    using System.Reflection;
    using System.Text;
    using FluentAssertions;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;
    using Xunit;
    using Xunit.Abstractions;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class GrammarCheckTests
    {
        private static readonly string[] checksFiles =
        [
            "Rules.Framework.Rql.IntegrationTests.GrammarCheck.CheckFiles.MatchExpressionChecks.yaml",
            "Rules.Framework.Rql.IntegrationTests.GrammarCheck.CheckFiles.BasicLanguageChecks.yaml",
            "Rules.Framework.Rql.IntegrationTests.GrammarCheck.CheckFiles.SearchExpressionChecks.yaml",
        ];

        private readonly IParser parser;
        private readonly ITestOutputHelper testOutputHelper;
        private readonly ITokenScanner tokenScanner;

        public GrammarCheckTests(ITestOutputHelper testOutputHelper)
        {
            this.tokenScanner = new TokenScanner();
            this.parser = new Parser(new ParseStrategyPool());
            this.testOutputHelper = testOutputHelper;
        }

        public static IEnumerable<object[]> GetTestCases()
        {
            foreach (var checksFile in checksFiles)
            {
                var checksStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(checksFile);
                using (var checksStreamReader = new StreamReader(checksStream!))
                {
                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var checks = deserializer.Deserialize<GrammarChecks>(checksStreamReader);

                    foreach (var checkLine in checks.Checks)
                    {
                        yield return new object[] { checkLine.Rql, checkLine.ExpectsSuccess, checkLine.ExpectedMessages };
                    }
                }
            }

            yield break;
        }

        [Theory]
        [MemberData(nameof(GetTestCases))]
        public void CheckRqlGrammar(string rqlSource, bool expectsSuccess, IEnumerable<string> expectedMessages)
        {
            // Arrange
            var testOutputMessage = new StringBuilder()
                .Append("RQL: ")
                .AppendLine(rqlSource)
                .Append("Is success expected? -> ")
                .Append(expectsSuccess);

            if (expectedMessages.Any())
            {
                testOutputMessage.AppendLine()
                    .AppendLine("Expected messages:");
                foreach (var message in expectedMessages)
                {
                    testOutputMessage.Append("  - ")
                        .AppendLine(message);
                }
            }

            this.testOutputHelper.WriteLine(testOutputMessage.ToString());

            // Act
            var isSuccess = this.TryScanAndParse(rqlSource, out var actualMessages);

            testOutputMessage.Clear()
                .Append("Outcome: ")
                .Append(isSuccess);

            if (actualMessages.Any())
            {
                testOutputMessage.AppendLine()
                    .AppendLine("Actual messages:");
                foreach (var message in actualMessages)
                {
                    testOutputMessage.Append("  - ")
                        .AppendLine(message);
                }
            }

            this.testOutputHelper.WriteLine(testOutputMessage.ToString());

            // Assert
            isSuccess.Should().Be(expectsSuccess);
            if (expectedMessages.Any())
            {
                actualMessages.Should().Contain(expectedMessages);
            }
            else
            {
                actualMessages.Should().BeEmpty();
            }
        }

        private bool TryScanAndParse(string rqlSource, out IEnumerable<string> errorMessages)
        {
            var scanResult = this.tokenScanner.ScanTokens(rqlSource);
            if (!scanResult.Success)
            {
                errorMessages = scanResult.Messages.Select(x => x.Text).ToArray();
                return false;
            }

            var parseResult = this.parser.Parse(scanResult.Tokens);
            if (!parseResult.Success)
            {
                errorMessages = parseResult.Messages.Select(x => x.Text).ToArray();
                return false;
            }

            errorMessages = Array.Empty<string>();
            return true;
        }
    }
}