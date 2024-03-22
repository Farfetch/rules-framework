// See https://aka.ms/new-console-template for more information
using Rules.Framework.Providers.SqlServer.Sample.Bootstrapper;

Console.WriteLine("Rules Framework DB will be created...");

await BootstrapperFixtureTemplate.InitializeSqlServerAsync();

Console.WriteLine("Rules Framework has been created!");