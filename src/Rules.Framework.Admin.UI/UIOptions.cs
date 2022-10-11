namespace Rules.Framework.Admin.UI
{
    using System;
    using System.IO;
    using System.Reflection;

    public class UIOptions
    {
        /// <summary>
        /// Gets or sets a title for the ui page
        /// </summary>
        public string DocumentTitle { get; } = "Rules Framework UI";

        /// <summary>
        /// Gets or sets additional content to place in the head of the ui page
        /// </summary>
        public string HeadContent { get; } = "";

        /// <summary>
        /// Gets or sets a Stream function for retrieving the ui page
        /// </summary>
        public Func<Stream> IndexStream { get; } = () => typeof(UIOptions).GetTypeInfo().Assembly
            .GetManifestResourceStream("Rules.Framework.Admin.UI.index.html");

        /// <summary>
        /// Gets or sets a route prefix for accessing the ui
        /// </summary>
        public string RoutePrefix { get; } = "rules";
    }
}