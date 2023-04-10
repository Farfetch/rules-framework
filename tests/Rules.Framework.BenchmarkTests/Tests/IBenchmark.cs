namespace Rules.Framework.BenchmarkTests.Tests
{
    using System.Threading.Tasks;

    internal interface IBenchmark
    {
        Task RunAsync();

        Task SetUpAsync();

        Task TearDownAsync();
    }
}