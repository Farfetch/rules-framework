using Microsoft.AspNetCore.Mvc;
using Rules.Framework.Admin.WebApi.Response;

namespace Rules.Framework.Admin.WebApi.Controllers
{
    public class ContentTypeController : Controller
    {
        [Route("rules/{controller}/list")]
        public IActionResult List()
        {
            var list = new List<ContentTypeDto>();

            for (int i = 0; i < 5; i++)
            {
                list.Add(new ContentTypeDto { Name = $"test{i}" });
            }

            return Ok(list);
        }
    }
}