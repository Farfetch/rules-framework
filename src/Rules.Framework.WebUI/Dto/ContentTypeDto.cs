namespace Rules.Framework.WebUI.Dto
{
    internal sealed class ContentTypeDto
    {
        public int Index { get; internal set; }
        public int ActiveRulesCount { get; internal set; }
        public string Name { get; internal set; }
        public int RulesCount { get; internal set; }
    }
}