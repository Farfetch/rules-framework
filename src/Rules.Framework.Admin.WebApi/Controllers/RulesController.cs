namespace Rules.Framework.Admin.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Rules.Framework.Admin.WebApi.Response;

    public class RuleController : Controller
    {
        private readonly IServiceProvider serviceProvider;

        public RuleController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            serviceProvider.GetService(typeof(IRulesEngine<,>));
        }

        [Route("rules/{controller}/list")]
        public IActionResult List()
        {
            var list = new List<RuleDto>();

            for (int i = 0; i < 5; i++)
            {
                list.Add(new RuleDto
                {
                    Id = i,
                    Name = $"Rule {i}",
                    Value = $"Formula {i}",
                    DateEnd = null,
                    DateStart = DateTime.UtcNow,
                    Active = true,
                    Conditions = new List<ConditionDto>
                    {
                        new ConditionDto
                        {
                            Condition = "BusinessModel",
                            Operator = "=",
                            Value = "2"
                        },
                        new ConditionDto
                        {
                            Condition = "ShippingTo",
                            Operator = "=",
                            Value = "BR"
                        },
                        new ConditionDto
                        {
                            Condition = "BillingTo",
                            Operator = "<>",
                            Value = "US"
                        }
                    }
                });
            }

            return Ok(list);
        }
    }
}