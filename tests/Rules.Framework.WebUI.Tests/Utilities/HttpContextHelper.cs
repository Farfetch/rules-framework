namespace Rules.Framework.WebUI.Tests.Utilities
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    internal static class HttpContextHelper
    {
        public static async Task<HttpContext> CreateContext(string path, string method, object requestBody = null)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = path;
            context.Request.Method = method;
            context.Request.ContentType = "application/json";

            if (requestBody is not null)
            {
                var body = JsonConvert.SerializeObject(requestBody,
                    new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                using (var writer = new StreamWriter(context.Request.Body, Encoding.UTF8))
                {
                    await writer.WriteAsync(body).ConfigureAwait(false);
                }
            }

            context.Response.Body = new MemoryStream();

            return context;
        }

        public static HttpContext CreateDefaultContext()
        {
            return new DefaultHttpContext();
        }

        public static HttpContext CreateHttpContext(string httpMethod, string resourcePath)
        {
            var context = new DefaultHttpContext();

            context.Request.Path = resourcePath;
            context.Request.Method = httpMethod;

            context.Response.Body = new MemoryStream();

            return context;
        }

        public static async Task<T> ReadResponse<T>(HttpResponse response)
        {
            //Rewind the stream
            response.Body.Seek(0, SeekOrigin.Begin);

            T responseDto;

            using (var reader = new StreamReader(response.Body, Encoding.UTF8))
            {
                var requestMessage = await reader.ReadToEndAsync().ConfigureAwait(false);

                responseDto = JsonConvert.DeserializeObject<T>(requestMessage,
                    new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
            }

            return responseDto;
        }
    }
}