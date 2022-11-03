namespace Rules.Framework.UI
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal interface IHttpRequestHandler
    {
        Task<bool> HandleAsync(HttpRequest httpRequest, HttpResponse httpResponse);
    }
}