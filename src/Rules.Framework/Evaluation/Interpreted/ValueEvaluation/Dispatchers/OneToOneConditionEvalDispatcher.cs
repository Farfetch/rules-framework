<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/OneToOneConditionEvalDispatcher.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/OneToOneConditionEvalDispatcher.cs
{
    using Rules.Framework.Core;

    internal sealed class OneToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
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