<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/ManyToOneConditionEvalDispatcher.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/ManyToOneConditionEvalDispatcher.cs
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal sealed class ManyToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
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

            IEnumerable<object> leftOperandAux = ConvertToTypedEnumerable(leftOperand, nameof(leftOperand));
            IEnumerable<object> leftOperandConverted = leftOperandAux.Select(x => ConvertToDataType(x, nameof(leftOperand), dataTypeConfiguration));
            object rightOperandConverted = ConvertToDataType(rightOperand, nameof(rightOperand), dataTypeConfiguration);

            return this.operatorEvalStrategyFactory.GetManyToOneOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}