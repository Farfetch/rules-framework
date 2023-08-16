namespace Rules.Framework.WebUI.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Generics;
    using Rules.Framework.Rql;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.WebUI.Dto;

    internal static class RqlOutputDtoExtensions
    {
        public static IEnumerable<RqlOutputDto> ToRqlOutput(
            this IEnumerable<IGenericRqlResult> genericRqlResult,
            IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer) => genericRqlResult.Select(grr => grr switch
            {
                GenericNothingRqlResult => new RqlOutputDto { Rql = grr.Rql, Rules = null, Value = null },
                GenericRulesSetRqlResult grsrr => new RqlOutputDto { Rql = grsrr.Rql, Rules = grsrr.Lines.Select(l => l.Rule.ToRuleDto(ruleStatusDtoAnalyzer)), Value = null },
                GenericValueRqlResult gvrr => new RqlOutputDto { Rql = gvrr.Rql, Rules = null, Value = GetValue(gvrr) },
                _ => throw new NotSupportedException($"The RQL result of type '{grr.GetType().FullName}' is not supported."),
            });

        public static IEnumerable<RqlOutputDto> ToRqlOutput(
            this RqlException rqlException) => rqlException.Errors.Select(re => new RqlOutputDto
            {
                IsSuccess = false,
                Rql = re.Rql,
                Rules = null,
                Value = null,
                Error = new RqlErrorDto
                {
                    BeginPositionColumn = (int)re.BeginPosition.Column,
                    BeginPositionLine = (int)re.BeginPosition.Column,
                    EndPositionLine = (int)re.EndPosition.Line,
                    EndPositionColumn = (int)re.EndPosition.Column,
                    Message = re.Text,
                },
            });

        private static object GetValue(GenericValueRqlResult gvrr)
        {
            return gvrr.Value switch
            {
                RqlAny rqlAny => new RqlRuntimeValueDto { DisplayValue = rqlAny.ToString(), RuntimeValue = rqlAny.RuntimeValue, Type = rqlAny.Type.ToRqlTypeDto() },
                RqlArray rqlArray => new RqlRuntimeValueDto { DisplayValue = rqlArray.ToString(), RuntimeValue = rqlArray.RuntimeValue, Type = rqlArray.Type.ToRqlTypeDto() },
                RqlBool rqlBool => new RqlRuntimeValueDto { DisplayValue = rqlBool.ToString(), RuntimeValue = rqlBool.RuntimeValue, Type = rqlBool.Type.ToRqlTypeDto() },
                RqlDate rqlDate => new RqlRuntimeValueDto { DisplayValue = rqlDate.ToString(), RuntimeValue = rqlDate.RuntimeValue, Type = rqlDate.Type.ToRqlTypeDto() },
                RqlDecimal rqlDecimal => new RqlRuntimeValueDto { DisplayValue = rqlDecimal.ToString(), RuntimeValue = rqlDecimal.RuntimeValue, Type = rqlDecimal.Type.ToRqlTypeDto() },
                RqlInteger rqlInteger => new RqlRuntimeValueDto { DisplayValue = rqlInteger.ToString(), RuntimeValue = rqlInteger.RuntimeValue, Type = rqlInteger.Type.ToRqlTypeDto() },
                RqlNothing rqlNothing => new RqlRuntimeValueDto { DisplayValue = rqlNothing.ToString(), RuntimeValue = rqlNothing.RuntimeValue, Type = rqlNothing.Type.ToRqlTypeDto() },
                RqlObject rqlObject => new RqlRuntimeValueDto { DisplayValue = rqlObject.ToString(), RuntimeValue = rqlObject.RuntimeValue, Type = rqlObject.Type.ToRqlTypeDto() },
                RqlReadOnlyObject rqlReadOnlyObject => new RqlRuntimeValueDto { DisplayValue = rqlReadOnlyObject.ToString(), RuntimeValue = rqlReadOnlyObject.RuntimeValue, Type = rqlReadOnlyObject.Type.ToRqlTypeDto() },
                RqlString rqlString => new RqlRuntimeValueDto { DisplayValue = rqlString.ToString(), RuntimeValue = rqlString.RuntimeValue, Type = rqlString.Type.ToRqlTypeDto() },
                _ => gvrr.Value,
            };
        }

        private static RqlTypeDto ToRqlTypeDto(this RqlType rqlType)
            => new RqlTypeDto { Name = rqlType.Name };
    }
}