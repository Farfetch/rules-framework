namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Extensions;
    using Rules.Framework.Rql;

    internal class GenericRqlClient<TContentType, TConditionType> : IGenericRqlClient
    {
        private readonly IRqlClient<TContentType, TConditionType> rqlClient;
        private bool disposedValue;

        public GenericRqlClient(IRqlClient<TContentType, TConditionType> rqlClient)
        {
            this.rqlClient = rqlClient;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<IGenericRqlResult>> ExecuteAsync(string rql)
        {
            var rqlResult = await this.rqlClient.ExecuteAsync(rql).ConfigureAwait(false);
            return ConvertToGenericRqlResult(rqlResult);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.rqlClient.Dispose();
                }

                disposedValue = true;
            }
        }

        private static IEnumerable<IGenericRqlResult> ConvertToGenericRqlResult(IEnumerable<IResult> rqlResult)
        {
            var rqlResultArray = rqlResult.ToArray();
            var genericRqlResultArray = new IGenericRqlResult[rqlResultArray.Length];
            for (int i = 0; i < genericRqlResultArray.Length; i++)
            {
                genericRqlResultArray[i] = rqlResultArray[i] switch
                {
                    NothingResult nothingResult => new GenericNothingRqlResult(nothingResult.Rql),
                    RulesSetResult<TContentType, TConditionType> rulesSetResult => ConvertToGenericRulesSetRqlResult(rulesSetResult),
                    ValueResult valueResult => new GenericValueRqlResult(valueResult.Rql, valueResult.Value),
                    _ => throw new NotSupportedException($"The RQL result of type '{rqlResultArray[i].GetType().FullName}' is not supported.")
                };
            }

            return genericRqlResultArray;
        }

        private static GenericRulesSetRqlResult ConvertToGenericRulesSetRqlResult(RulesSetResult<TContentType, TConditionType> rulesSetResult)
        {
            var sourceLines = rulesSetResult.Lines;
            var destinationLines = new List<GenericRulesSetRqlResultLine>(sourceLines.Count);
            for (int i = 0; i < sourceLines.Count; i++)
            {
                var rule = sourceLines[i].Rule.Value.ToGenericRule();
                var rulesSetResultLine = new GenericRulesSetRqlResultLine(i + 1, rule);
                destinationLines.Add(rulesSetResultLine);
            }

            return new GenericRulesSetRqlResult(rulesSetResult.Rql, sourceLines.Count, destinationLines);
        }
    }
}