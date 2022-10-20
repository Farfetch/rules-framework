namespace Rules.Framework.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Rules.Framework.WebApi.Response;

    public class ContentTypeController : Controller
    {
        private readonly dynamic rulesEngine;
        private readonly IRulesService rulesService;

        public ContentTypeController(IRulesService rulesService, IServiceProvider serviceProvider)
        {
            this.rulesService = rulesService;

            //TODO
            var tead = System.Reflection.Assembly
                .GetAssembly(typeof(RulesEngine<,>))
                .GetTypes();

            this.rulesEngine = tead
                .FirstOrDefault(x => x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(RulesEngine<,>));

            var d = serviceProvider.GetService(rulesEngine);
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