namespace Rules.Framework.Rql
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRqlClient<TContentType, TConditionType>
    {
        Task<IEnumerable<ResultSet<TContentType, TConditionType>>> ExecuteQueryAsync(string rql);
    }
}