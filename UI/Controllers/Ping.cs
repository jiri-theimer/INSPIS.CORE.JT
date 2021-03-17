using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Controllers
{
    public class Ping : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Time = DateTime.Now.ToString();
            return View();
        }
    }
}
