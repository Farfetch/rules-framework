namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRqlEngine<TContentType, TConditionType> : IDisposable
    {
        Task<IEnumerable<IResult>> ExecuteAsync(string rql);
    }
}