namespace Rules.Framework.WebUI
{
    using System;

    internal class WebUIOptionsRegistry
    {
        public WebUIOptions RegisteredOptions { get; private set; }

        public void Register(WebUIOptions webUIOptions)
        {
            if (webUIOptions == null)
            {
                throw new ArgumentNullException(nameof(webUIOptions));
            }

            this.RegisteredOptions = webUIOptions;
        }
    }
}