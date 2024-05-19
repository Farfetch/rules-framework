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
        public static RqlOutputDto ToRqlOutput(
            this IEnumerable<IResult> genericRqlResult,
            IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer,
            string standardOutput)
        {
            var rqlStatementOutputs = genericRqlResult.Select(grr => grr switch
            {
                NothingResult => new RqlStatementOutputDto { Rql = grr.Rql, Rules = null, Value = null },
                RulesSetResult<string, string> grsrr => new RqlStatementOutputDto { Rql = grsrr.Rql, Rules = grsrr.Lines.Select(l => l.Rule.Value.ToRuleDto(ruleStatusDtoAnalyzer)), Value = null },
                ValueResult gvrr => new RqlStatementOutputDto { Rql = gvrr.Rql, Rules = null, Value = GetValue(gvrr) },
                _ => throw new NotSupportedException($"The RQL result of type '{grr.GetType().FullName}' is not supported."),
            });

            return new RqlOutputDto
            {
                StandardOutput = standardOutput,
                StatementResults = rqlStatementOutputs,
            };
        }

        public static RqlOutputDto ToRqlOutput(
            this RqlException rqlException)
        {
            var rqlStatementOutputs = rqlException.Errors.Select(re => new RqlStatementOutputDto
            {
                IsSuccess = false,
                Rql = re.Rql,
                Rules = null,
                Value = null,
                Error = new RqlErrorDto
                {
                    BeginPositionColumn = (int)re.BeginPosition.Column,
                    BeginPositionLine = (int)re.BeginPosition.Line,
                    EndPositionLine = (int)re.EndPosition.Line,
                    EndPositionColumn = (int)re.EndPosition.Column,
                    Message = re.Text,
                },
            });

            return new RqlOutputDto
            {
                StandardOutput = null!,
                StatementResults = rqlStatementOutputs,
            };
        }

        private static object GetValue(ValueResult vr)
        {
            return vr.Value switch
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
                _ => vr.Value,
            };
        }

        private static RqlTypeDto ToRqlTypeDto(this RqlType rqlType)
            => new RqlTypeDto { Name = rqlType.Name };
    }
}