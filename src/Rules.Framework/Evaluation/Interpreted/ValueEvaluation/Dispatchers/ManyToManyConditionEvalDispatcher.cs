<<<<<<<< HEAD:src/Rules.Framework/Evaluation/Classic/ValueEvaluation/Dispatchers/ManyToManyConditionEvalDispatcher.cs
namespace Rules.Framework.Evaluation.Classic.ValueEvaluation.Dispatchers
========
namespace Rules.Framework.Evaluation.Interpreted.ValueEvaluation.Dispatchers
>>>>>>>> master:src/Rules.Framework/Evaluation/Interpreted/ValueEvaluation/Dispatchers/ManyToManyConditionEvalDispatcher.cs
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal sealed class ManyToManyConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
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
            IEnumerable<object> leftOperandConverted = leftOperandAux.Select(x => ConvertToDataType(x, nameof(leftOperand), dataTypeConfiguration));
            IEnumerable<object> rightOperandAux = ConvertToTypedEnumerable(rightOperand, nameof(rightOperand));
            IEnumerable<object> rightOperandConverted = rightOperandAux.Select(x => ConvertToDataType(x, nameof(rightOperand), dataTypeConfiguration));

            return this.operatorEvalStrategyFactory.GetManyToManyOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}