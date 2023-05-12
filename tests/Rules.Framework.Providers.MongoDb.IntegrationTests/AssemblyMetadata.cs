using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: ExcludeFromCodeCoverage]
[assembly: AssemblyTrait("Category", "Integration")]
[assembly: CollectionBehavior(DisableTestParallelization = true)]