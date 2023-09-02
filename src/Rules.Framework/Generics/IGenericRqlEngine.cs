namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGenericRqlEngine : IDisposable
    {
        Task<IEnumerable<IGenericRqlResult>> ExecuteAsync(string rql);
    }
}