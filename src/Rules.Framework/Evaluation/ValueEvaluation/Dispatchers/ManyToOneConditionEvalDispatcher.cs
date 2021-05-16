namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal class ManyToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public ManyToOneConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            DataTypeConfiguration dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);

            IEnumerable<object> leftOperandAux = leftOperand as IEnumerable<object>;
            IEnumerable<object> leftOperandConverted = leftOperandAux.Select(x => ConvertToDataType(x, dataTypeConfiguration));
            object rightOperandConverted = ConvertToDataType(leftOperand, dataTypeConfiguration);

            return this.operatorEvalStrategyFactory.GetManyToOneOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}