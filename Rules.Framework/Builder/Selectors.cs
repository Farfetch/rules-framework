using System;
using Rules.Framework.Evaluation;
using Rules.Framework.Evaluation.ValueEvaluation;

namespace Rules.Framework.Builder
{
    internal class Selectors
    {
        internal class ConditionTypeSelector<TContentType> : IConditionTypeSelector<TContentType>
        {
            public IRulesDataSourceSelector<TContentType, TConditionType> WithConditionType<TConditionType>()
                => new RulesDataSourceSelector<TContentType, TConditionType>();
        }

        internal class ContentTypeSelector : IContentTypeSelector
        {
            public IConditionTypeSelector<TContentType> WithContentType<TContentType>() => new ConditionTypeSelector<TContentType>();
        }

        internal class RulesDataSourceSelector<TContentType, TConditionType> : IRulesDataSourceSelector<TContentType, TConditionType>
        {
            public RulesEngine<TContentType, TConditionType> SetDataSource(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
            {
                if (rulesDataSource == null)
                {
                    throw new ArgumentNullException(nameof(rulesDataSource));
                }

                IOperatorEvalStrategyFactory operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();
                DeferredEval deferredEval = new DeferredEval(operatorEvalStrategyFactory);
                IConditionsEvalEngine<TConditionType> conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval);
                return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, rulesDataSource);
            }
        }
    }
}