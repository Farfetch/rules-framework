namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal class ManyToManyConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public ManyToManyConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            DataTypeConfiguration dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);

            IEnumerable<object> leftOperandAux = ConvertToTypedEnumerable(leftOperand, nameof(leftOperand));
            IEnumerable<object> leftOperandConverted = leftOperandAux.Select(x => ConvertToDataType(x, dataTypeConfiguration));
            IEnumerable<object> rightOperandAux = ConvertToTypedEnumerable(rightOperand, nameof(rightOperand));
            IEnumerable<object> rightOperandConverted = rightOperandAux.Select(x => ConvertToDataType(x, dataTypeConfiguration));

            return this.operatorEvalStrategyFactory.GetManyToManyOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}