namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Extensions;

    public class GenericRulesEngineAdapter<TContentType, TConditionType> : IGenericRulesEngineAdapter
    {
        private readonly RulesEngine<TContentType, TConditionType> rulesEngine;

        public GenericRulesEngineAdapter(RulesEngine<TContentType, TConditionType> rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        /// <exception cref="System.ArgumentException">
        /// Method only works if TContentType is a enum
        /// </exception>
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
                   Code = Enum.Parse(typeof(TContentType), t.ToString()).ToString(),
                   Name = t.ToString()
               });
        }

        /// <summary>
        /// Gets the priority criterias.
        /// </summary>
        /// <returns>Rules engine priority criterias</returns>
        public PriorityCriterias GetPriorityCriterias()
        {
            return this.rulesEngine.GetPriorityCriterias();
        }

        /// <summary>
        /// Searches for generic rules that match on supplied <paramref name="genericSearchArgs"/>.
        /// </summary>
        /// <param name="genericSearchArgs"></param>
        /// <returns>the set of generic rules matching the conditions.</returns>
        public async Task<IEnumerable<GenericRule>> SearchAsync(SearchArgs<GenericContentType, GenericConditionType> genericSearchArgs)
        {
            var searchArgs = genericSearchArgs.ToSearchArgs<TContentType, TConditionType>();

            var result = await this.rulesEngine.SearchAsync(searchArgs);

            return result.Select(rule => rule.ToGenericRule());
        }
    }
}