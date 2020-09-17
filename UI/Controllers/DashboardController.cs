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
        private readonly BL.ThePeriodProvider _pp;
        public DashboardController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Inspector(string masterflag)
        {
            if (string.IsNullOrEmpty(masterflag))
            {
                masterflag = Factory.CBL.LoadUserParam("DashboardInspector-masterflag", "");
            }
            var v = new DashboardInspector() { pid = Factory.CurrentUser.j02ID,GridMasterFilter=masterflag };   //leader/member/issuer
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            v.PeriodFilter = new PeriodViewModel();
            v.PeriodFilter.IsShowButtonRefresh = true;
            var per = InhalePeriodFilter();
            v.PeriodFilter.PeriodValue = per.pid;
            v.PeriodFilter.d1 = per.d1;
            v.PeriodFilter.d2 = per.d2;

            var mq = new BO.myQuery("h11") { IsRecordValid = true, MyRecordsDisponible = true };
            v.lisH11 = Factory.h11NoticeBoardBL.GetList(mq);
            mq=new BO.myQuery("h04") { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID, h06TodoRole=1 };
            v.lisH04 = Factory.h04ToDoBL.GetList(mq);

            return View(v);
        }

        private BO.ThePeriod InhalePeriodFilter()
        {
            var ret = _pp.ByPid(0);
            int x = Factory.CBL.LoadUserParamInt("grid-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("grid-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("grid-period-d2");

            }

            return ret;
        }
    }
}