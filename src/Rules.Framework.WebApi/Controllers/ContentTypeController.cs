namespace Rules.Framework.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Rules.Framework.WebApi.Response;

    public class ContentTypeController : Controller
    {
        private readonly IRulesService rulesService;

        public ContentTypeController(IRulesService rulesService)
        {
            this.rulesService = rulesService;
        }

        [Route("rules/{controller}/list")]
        public IActionResult List()
        {
            var contents = this.rulesService.ListContents();

            var list = new List<ContentTypeDto>();

            foreach (var content in contents)
            {
                list.Add(new ContentTypeDto { Name = content });
            }

            return Ok(list);
        }
    }
}