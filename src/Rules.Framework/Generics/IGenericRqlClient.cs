namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGenericRqlClient : IDisposable
    {
        Task<IEnumerable<IGenericRqlResult>> ExecuteAsync(string rql);
    }
}