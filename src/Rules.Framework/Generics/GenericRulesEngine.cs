namespace Rules.Framework.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Extensions;

    internal sealed class GenericRulesEngine<TContentType, TConditionType> : IGenericRulesEngine
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;

        public GenericRulesEngine(IRulesEngine<TContentType, TConditionType> rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        public IEnumerable<GenericContentType> GetContentTypes()
        {
            if (!typeof(TContentType).IsEnum)
            {
                throw new ArgumentException("Method only works if TContentType is a enum");
            }

            return Enum.GetValues(typeof(TContentType))
               .Cast<TContentType>()
               .Select(t => new GenericContentType
               {
                   Identifier = Enum.Parse(typeof(TContentType), t.ToString()).ToString()
               });
        }


        public PriorityCriterias GetPriorityCriteria()
        {
            return this.rulesEngine.GetPriorityCriteria();
        }

        public async Task<IEnumerable<GenericRule>> SearchAsync(SearchArgs<GenericContentType, GenericConditionType> genericSearchArgs)
        {
            var searchArgs = genericSearchArgs.ToSearchArgs<TContentType, TConditionType>();

            var result = await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);

            return result.Select(rule => rule.ToGenericRule()).ToList();
        }
    }
}