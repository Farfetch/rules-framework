namespace Rules.Framework.WebUI.Dto
{
    public class RqlRuntimeValueDto
    {
        public string DisplayValue { get; set; }

        public object RuntimeValue { get; set; }

        public RqlTypeDto Type { get; set; }
    }
}