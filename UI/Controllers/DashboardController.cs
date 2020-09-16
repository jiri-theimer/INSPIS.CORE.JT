using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Dashboard;
using UI.Models.Record;

namespace UI.Controllers
{
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Inspector()
        {
            var v = new DashboardInspector() { pid = Factory.CurrentUser.j02ID };
            v.Rec = Factory.j02PersonBL.Load(v.pid);
            return View(v);
        }
    }
}