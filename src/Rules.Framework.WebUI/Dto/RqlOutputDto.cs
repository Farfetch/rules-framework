namespace Rules.Framework.WebUI.Dto
{
    using System.Collections.Generic;

    internal class RqlOutputDto
    {
        public string StandardOutput { get; set; }

        public IEnumerable<RqlStatementOutputDto> StatementResults { get; set; }
    }
}