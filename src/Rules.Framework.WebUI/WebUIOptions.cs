namespace Rules.Framework.WebUI
{
    /// <summary>
    /// Options for the Rules Framework Web UI
    /// </summary>
    public sealed class WebUIOptions
    {
        /// <summary>
        /// Gets title to present on the Rules Framework Web UI page title. If not specified, will
        /// present "Rules Framework" as default.
        /// </summary>
        public string DocumentTitle { get; set; } = "Rules Framework";

        /// <summary>
        /// Gets the RQL terminal options.
        /// </summary>
        /// <value>The RQL terminal options.</value>
        public RqlTerminalOptions RqlTerminal { get; } = new RqlTerminalOptions();
    }
}