namespace Rules.Framework.WebUI.ViewModels
{
    internal sealed class TerminalOutputLineViewModel
    {
        public string Text { get; set; } = null!;
        public TerminalOutputLineTypes Type { get; set; } = TerminalOutputLineTypes.PlainText;
    }
}