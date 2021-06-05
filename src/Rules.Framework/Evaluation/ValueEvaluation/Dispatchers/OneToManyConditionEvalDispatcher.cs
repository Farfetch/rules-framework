namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal class OneToManyConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public OneToManyConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            DataTypeConfiguration dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);

            object leftOperandConverted = ConvertToDataType(leftOperand, dataTypeConfiguration);
            IEnumerable<object> rightOperandAux = ConvertToTypedEnumerable(rightOperand, nameof(rightOperand));
            IEnumerable<object> rightOperandConverted = rightOperandAux.Select(x => ConvertToDataType(x, dataTypeConfiguration));

            return this.operatorEvalStrategyFactory.GetOneToManyOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}