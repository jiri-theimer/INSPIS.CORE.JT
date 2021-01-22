using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Controllers
{
    public class publicController : Controller
    {
        private readonly BL.RunningApp _app;
        private readonly BL.TheGlobalParams _gp;
        private readonly BL.TheTranslator _tt;
        public publicController(BL.RunningApp app, BL.TheGlobalParams gp, BL.TheTranslator tt)
        {
            _app = app;
            _gp = gp;
            _tt = tt;
        }
        public IActionResult ical(string guid)
        {
            if (string.IsNullOrEmpty(_app.RobotUser))
            {
                return Content("RobotUser not defined.");
            }
            BO.RunningUser ru = new BO.RunningUser() { j03Login = _app.RobotUser };
            BL.Factory f = new BL.Factory(ru, _app, null, _gp, _tt);
            if (f.CurrentUser == null)
            {
                return Content("RobotUser not loaded");
            }

            var recJ02 = f.j02PersonBL.LoadByGuid(guid);

            var cgen = new IcalSupport();
            if (recJ02 == null)
            {
                return Content("personal guid not loaded");
            }

            string strFileName = recJ02.j02Guid + ".ics";


            Response.Headers["Content-Type"] = "text/calendar";
            Response.Headers["Content-Disposition"] = string.Format("attachment; filename={0}", strFileName);

            //var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "text/plain");

            return Content(cgen.getPersonalCalendar(f, recJ02, null, null));
        }
    }
}
