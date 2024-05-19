namespace Rules.Framework.WebUI.Dto
{
    internal class RqlErrorDto
    {
        public int BeginPositionColumn { get; set; }

        public int BeginPositionLine { get; set; }

        public int EndPositionLine { get; set; }

        public int EndPositionColumn { get; set; }

        public string Message { get; set; }
    }
}