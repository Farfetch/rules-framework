namespace Rules.Framework.WebUI
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Options for the Rules Framework Web UI
    /// </summary>
    public sealed class WebUIOptions
    {
        /// <summary>
        /// Gets title for the Rules Framework Web UI
        /// </summary>
        public string DocumentTitle { get; } = "Rules Framework";

        /// <summary>
        /// Gets additional content to place in the head of the Rules Framework Web UI
        /// </summary>
        public string HeadContent { get; } = "";

        /// <summary>
        /// Gets a Stream function for retrieving the Rules Framework Web UI
        /// </summary>
        public Func<Stream> IndexStream { get; } = () => typeof(WebUIOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream("Rules.Framework.WebUI.index.html");

        /// <summary>
        /// Gets or set route prefix for accessing the Rules Framework Web UI
        /// </summary>
        public string RoutePrefix { get; set; } = "rules";
    }
}