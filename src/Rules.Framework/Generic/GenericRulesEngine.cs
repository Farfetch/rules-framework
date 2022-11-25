namespace Rules.Framework.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Extensions;

    /// <summary>
    /// Exposes generic rules engine logic to provide rule matches to requests.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    /// TODO: Gather to discuss better name for this property
    public class GenericRulesEngine<TContentType, TConditionType> : IGenericRulesEngine
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="GenericRulesEngine{TContentType,TConditionType}"/> class.
        /// </summary>
        public GenericRulesEngine(IRulesEngine<TContentType, TConditionType> rulesEngine)
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
                   Name = Enum.Parse(typeof(TContentType), t.ToString()).ToString()
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