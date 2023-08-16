namespace Rules.Framework.WebUI
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal interface IHttpRequestHandler
    {
        HttpMethod HttpMethod { get; }

        string[] ResourcePaths { get; }

        Task HandleAsync(HttpContext httpContext);
    }
}