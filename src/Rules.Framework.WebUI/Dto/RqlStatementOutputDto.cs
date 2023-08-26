namespace Rules.Framework.WebUI.Dto
{
    using System.Collections.Generic;

    internal class RqlStatementOutputDto
    {
        public RqlErrorDto Error { get; set; }

        public bool IsSuccess { get; set; }

        public string Rql { get; set; }

        public IEnumerable<RuleDto> Rules { get; set; }

        public object Value { get; set; }
    }
}