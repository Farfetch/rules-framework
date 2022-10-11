namespace Rules.Framework.Admin.Dashboard.Sample.Controllers
{
    using global::Rules.Framework.Admin.Dashboard.Sample.Enums;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRulesEngine<ContentTypes, ConditionTypes> rulesService;

        public HomeController(ILogger<HomeController> logger, IRulesEngine<ContentTypes, ConditionTypes> rulesService)
        {
            _logger = logger;
            this.rulesService = rulesService;
        }

        public async Task<IActionResult> Index()
        {
            var d = await this.rulesService.SearchAsync(new SearchArgs<ContentTypes, ConditionTypes>(ContentTypes.TestNumber,
                DateTime.Now, DateTime.Now));
            return View(d);
        }
    }
}