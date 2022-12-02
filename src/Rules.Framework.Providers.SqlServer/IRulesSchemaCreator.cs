namespace Rules.Framework.Providers.SqlServer
{
    using System.Threading.Tasks;

    public interface IRulesSchemaCreator
    {
        Task CreateOrUpdateSchemaAsync(string databaseName, string schemaName);

        Task CreateOrUpdateSchemaAsync(string databaseName);
    }
}