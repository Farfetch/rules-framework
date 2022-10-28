namespace Rules.Framework.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Rules.Framework.WebApi.Response;

    public class ContentTypeController : Controller
    {
        private readonly IRulesEngine rulesEngine;

        public ContentTypeController(IRulesEngine rulesEngine)
        {
            this.rulesEngine = rulesEngine;
        }

        [Route("rules/{controller}/list")]
        public IActionResult List()
        {
            var contents = this.rulesEngine.GetContentTypes();

            var list = new List<ContentTypeDto>();

            foreach (var content in contents)
            {
                list.Add(new ContentTypeDto { Name = content.Name });
            }

            return Ok(list);
        }
    }
}