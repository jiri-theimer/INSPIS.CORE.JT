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

        public IActionResult Widgets()
        {
            var v = new WidgetsViewModel();
            var cW = new UI.WidgetSupport(Factory, "widgets");
            cW.PrepareWidgets(v);
            cW.InhaleWidgetsDataContent(v);
            

            return View(v);
        }
        public IActionResult Inspector(string masterflag)
        {
            if (string.IsNullOrEmpty(masterflag))
            {
                masterflag = Factory.CBL.LoadUserParam("DashboardInspector-masterflag", "");
            }
            var v = new DashboardInspector() { pid = Factory.CurrentUser.j02ID, NavTabs = new List<NavTab>() };   //leader/member/issuer
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            RefreshNavTabsInspector(v);

            //v.PeriodFilter = new PeriodViewModel();
            //v.PeriodFilter.IsShowButtonRefresh = true;
            //var per = InhalePeriodFilter();
            //v.PeriodFilter.PeriodValue = per.pid;
            //v.PeriodFilter.d1 = per.d1;
            //v.PeriodFilter.d2 = per.d2;

            
            return View(v);
        }


        private void RefreshNavTabsInspector(DashboardInspector v)
        {
            var c = Factory.j02PersonBL.LoadSummary(v.pid,"dashboard");
            v.NavTabs.Add(AddTab(Factory.tra("Dashboard"), "dashboard", "/Dashboard/Widgets?pid=" + Factory.CurrentUser.j02ID));
            v.NavTabs.Add(AddTab(Factory.tra("Můj kapacitní plán"), "gantt", "/j02/TabGantt?pid="+Factory.CurrentUser.j02ID.ToString()));
            string strBadge = null;
            if (c.a01_count_involved > 0) strBadge = c.a01_count_involved.ToString();
            v.NavTabs.Add(AddTab(Factory.tra("Jsem zapojen"), "involved", "/TheGrid/SlaveView?prefix=a01&master_flag=involved", false, strBadge));
            strBadge = null;
            if (c.a01_count_leader > 0) strBadge = c.a01_count_leader.ToString();
            v.NavTabs.Add(AddTab(Factory.tra("Vedoucí"), "leader", "/TheGrid/SlaveView?prefix=a01&master_flag=leader", false, strBadge));
            strBadge = null;
            if (c.a01_count_member > 0) strBadge = c.a01_count_member.ToString();
            v.NavTabs.Add(AddTab(Factory.tra("Člen"), "member", "/TheGrid/SlaveView?prefix=a01&master_flag=member", false, strBadge));
            strBadge = null;
            if (c.a01_count_invited > 0) strBadge = c.a01_count_invited.ToString();
            v.NavTabs.Add(AddTab(Factory.tra("Přizvaná osoba"), "invited", "/TheGrid/SlaveView?prefix=a01&master_flag=invited", false, strBadge));
            strBadge = null;
            if (c.a01_count_issuer > 0) strBadge = c.a01_count_issuer.ToString();
            v.NavTabs.Add(AddTab(Factory.tra("Zakladatel"), "issuer", "/TheGrid/SlaveView?prefix=a01&master_flag=issuer", false, strBadge));


            strBadge = null;            
            if (c.h04_count > 0) strBadge = c.h04_count.ToString();
            v.NavTabs.Add(AddTab("Úkoly/Lhůty", "h04", "/TheGrid/SlaveView?prefix=h04", true, strBadge));
            strBadge = null;
            if (c.h11_count > 0) strBadge = c.h11_count.ToString();
            v.NavTabs.Add(AddTab("Nástěnka", "h11", "/TheGrid/SlaveView?prefix=h11", true, strBadge));

            v.NavTabs.Add(AddTab("Inbox", "x40", "/TheGrid/SlaveView?prefix=x40"));
           


            string strDefTab = Factory.CBL.LoadUserParam("dashboard-tab-inspector");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=j02Person&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
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

        public IActionResult School(int a03id, int a10id)
        {
            if (a10id == 0)
            {
                a10id = Factory.CBL.LoadUserParamInt("DashboardSchool-a10id", 0);
            }
            if (a03id == 0)
            {
                a03id = Factory.CBL.LoadUserParamInt("DashboardSchool-a03id", 0);
            }

            var v = new DashboardSchool() { a03ID = a03id, a10ID = a10id };

            var mq = new BO.myQuery("h11") { IsRecordValid = true, MyRecordsDisponible = true };
            v.lisH11 = Factory.h11NoticeBoardBL.GetList(mq);

            mq = new BO.myQuery("a03") { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID };
            v.lisA03 = Factory.a03InstitutionBL.GetList(mq);
            if (v.a03ID == 0 && v.lisA03.Count() > 0)
            {
                v.a03ID = v.lisA03.First().pid;
            }
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            if (v.RecA03 == null)
            {
                return this.StopPage(false, "Nelze načíst profil instituce.");
            }
            mq = new BO.myQuery("a39") { IsRecordValid = true, a03id=v.a03ID };
            v.lisA39 = Factory.a39InstitutionPersonBL.GetList(mq);
            int intCurrentJ04ID = v.lisA39.Where(p => p.j02ID == Factory.CurrentUser.j02ID).First().j04ID_Explicit;
            if (intCurrentJ04ID == 0)
            {
                intCurrentJ04ID = Factory.CurrentUser.j04ID;
            }
            v.RecJ04 = Factory.j04UserRoleBL.Load(intCurrentJ04ID);
            v.lisA10 = new List<BO.a10EventType>();
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SchoolAdminUser, v.RecJ04.j04RoleValue))
            {
                //uživatel má právo spravovat uživatelské účty v instituci
                var c = new BO.a10EventType() { a10ID = -1, pid = -1, a10Name = Factory.tra("Správa uživatelských účtů") };
                v.lisA10.Add(c);
            }
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SchoolModuleTeachers, v.RecJ04.j04RoleValue))
            {
                //uživatel má přístup do modulu [Učitelé]                
                v.lisA10.Add(new BO.a10EventType() { a10ID = -2, pid = -2, a10Name = Factory.tra("Učitelé") });
            }
            if (v.RecJ04.j04IsAllowedAllEventTypes == false)
            {
                var lisJ08 = Factory.j04UserRoleBL.GetListJ08(v.RecJ04.pid);
                foreach (var c in lisJ08.Where(p => p.a10IsUse_K01 == false))
                {

                    v.lisA10.Add(new BO.a10EventType() { a10ID = c.a10ID, pid = c.a10ID, a10Name = c.a10Name });
                }
                foreach (var c in lisJ08.Where(p => p.a10IsUse_K01 == true))
                {

                    //v.lisA10.Add(new BO.a10EventType() { a10ID = c.a10ID, pid = c.a10ID, a10Name = c.a10Name });
                }
            }
            else
            {
                v.IsAllowCreateA01 = true;  //může zakládat všechny akce
                mq = new BO.myQuery("a10") { IsRecordValid = true };
                var lis = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10IsUse_K01 == false);
                foreach (var c in lis)
                {
                    v.lisA10.Add(new BO.a10EventType() { a10ID = c.a10ID, pid = c.a10ID, a10Name = c.a10Name });
                }
            }
            if (v.a10ID == 0 && v.lisA10.Where(p => p.a10ID > 0).Count() > 0)
            {
                v.a10ID = v.lisA10.Where(p => p.a10ID > 0).First().a10ID;
                Factory.CBL.SetUserParam("DashboardSchool-a10id", v.a10ID.ToString());
                return RedirectToAction("School");  //znovu načtení stránky, aby si grid dokázal načíst filtr podle a10ID
            }
            if (v.a10ID > 0)
            {
                v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
                v.GridColumns = "a__a01Event__a01Signature,a01_a08__a08Theme__a08Name,a01_b02__b02WorkflowStatus__b02Name,a__a01Event__a01DateFrom,a__a01Event__a01DateUntil";
                if (v.RecA10.a10CoreFlag == "injury")
                {
                    v.GridColumns += ",a01_xxa__v_uraz_jmenozraneneho__JmenoZraneneho,a01_xxb__v_uraz_datumzraneni__DatumZraneni,a01_xxc__v_uraz_poradovecislo__PoradoveCislo";
                }
            }

            if (v.IsAllowCreateA01 == false)
            {
                if (Factory.j04UserRoleBL.GetListJ08(v.RecJ04.pid).Where(p => p.a10ID == v.a10ID && p.j08IsAllowedCreate).Count() > 0)
                {
                    v.IsAllowCreateA01 = true;  //může zakládat tento typ akce
                }
            }
            
            return ViewTup(v, BO.j05PermValuEnum.SchoolAdminUser);
        }
    }
}