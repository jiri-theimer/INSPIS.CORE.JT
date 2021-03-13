using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using UIFT;

namespace EPIS.UIFT.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly AppConfiguration Configuration;

        public TestController(LinkGenerator generator, AppConfiguration configuration)
        {
            this.Configuration = configuration;
            _linkGenerator = generator;
        }

        public IActionResult t1()
        {
            return Content(_linkGenerator.GetPathByAction("Index", "Error", new { code = 18 }, "/uift/"));
        }
    }
}
