namespace Rules.Framework.WebUI
{
    /// <summary>
    /// Options for the Web UI RQL terminal.
    /// </summary>
    public class RqlTerminalOptions
    {
        /// <summary>
        /// Gets or sets the maximum output lines kept during a RQL terminal session. When the
        /// output history surpasses the configured number of lines, older lines are discarded.
        /// </summary>
        /// <value>The maximum output lines.</value>
        /// <remarks>assumes a maximum output lines of 200 by default.</remarks>
        public int MaxOutputLines { get; set; } = 200;
    }
}