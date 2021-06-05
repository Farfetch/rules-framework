namespace Rules.Framework.Evaluation.ValueEvaluation.Dispatchers
{
    using Rules.Framework.Core;

    internal class OneToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public OneToOneConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            DataTypeConfiguration dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);
            object leftOperandConverted = ConvertToDataType(leftOperand, nameof(leftOperand), dataTypeConfiguration);
            object rightOperandConverted = ConvertToDataType(rightOperand, nameof(rightOperand), dataTypeConfiguration);

            return this.operatorEvalStrategyFactory.GetOneToOneOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}