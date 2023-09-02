namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Extensions;
    using Rules.Framework.Rql;

    internal class GenericRqlEngine<TContentType, TConditionType> : IGenericRqlEngine
    {
        private bool disposedValue;
        private IRqlEngine<TContentType, TConditionType> rqlEngine;

        public GenericRqlEngine(IRqlEngine<TContentType, TConditionType> rqlEngine)
        {
            this.rqlEngine = rqlEngine;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<IGenericRqlResult>> ExecuteAsync(string rql)
        {
            var rqlResult = await this.rqlEngine.ExecuteAsync(rql).ConfigureAwait(false);
            return ConvertToGenericRqlResult(rqlResult);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.rqlEngine.Dispose();
                    this.rqlEngine = null!;
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