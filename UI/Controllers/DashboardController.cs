using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Dashboard;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public DashboardController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }

        public IActionResult TabSchoolA01Grid(int a03id,int a10id)   //podformulář pro grid školních akcí
        {
            var v = new a01TabSchoolGrid() { a03ID = a03id,a10ID=a10id };
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            
            var lisA39 = Factory.a39InstitutionPersonBL.GetList(new BO.myQuery("a39") { IsRecordValid = true, a03id = v.a03ID });
            int intCurrentJ04ID = lisA39.Where(p => p.j02ID == Factory.CurrentUser.j02ID).First().j04ID_Explicit;
            if (intCurrentJ04ID == 0)
            {
                intCurrentJ04ID = Factory.CurrentUser.j04ID;
            }
            v.RecJ04 = Factory.j04UserRoleBL.Load(intCurrentJ04ID);

            return View(v);
        }
        public IActionResult TabSchoolAccounts(int a03id)   //podformulář pro správu školních účtů v rámci školy
        {
            var v = new a39TabSchoolAccount() { a03ID = a03id };
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            if (v.RecA03 == null)
            {
                return this.StopPage(false, "Nelze načíst profil instituce.");
            }

            var lisA03 = Factory.a03InstitutionBL.GetList(new BO.myQuery("a03") { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID });
            if (!lisA03.Any(p => p.a03ID == v.a03ID))
            {
                return this.StopPage(false, "Nemáte oprávnění spravovat účty této školy.");
            }

            v.lisA39 = Factory.a39InstitutionPersonBL.GetList(new BO.myQuery("a39") { IsRecordValid = true, a03id = v.a03ID });

            return ViewTup(v, BO.j05PermValuEnum.SchoolAdminUser);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Widgets");
        }

        public IActionResult Widgets(string skin)
        {
            if (string.IsNullOrEmpty(skin))
            {
                skin = "index";
            }
            var v = new WidgetsViewModel() { Skin = skin,IsSubform=false };
            if (v.Skin=="inspector" || v.Skin == "school")
            {
                v.IsSubform = true;
            }
            
            var cW = new UI.WidgetSupport(Factory, skin);
            cW.PrepareWidgets(v);
            cW.InhaleWidgetsDataContent(v);
            

            return View(v);
        }
        public IActionResult Inspector()
        {           
            var v = new DashboardInspector() { pid = Factory.CurrentUser.j02ID, NavTabs = new List<NavTab>() }; 
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            RefreshNavTabsInspector(v);

            
            return View(v);
        }


        private void RefreshNavTabsInspector(DashboardInspector v)
        {
            var c = Factory.j02PersonBL.LoadSummary(v.pid,"dashboard");
            v.NavTabs.Add(AddTab(Factory.tra("Dashboard"), "dashboard", "/Dashboard/Widgets?skin=inspector&pid=" + Factory.CurrentUser.j02ID));
            v.NavTabs.Add(AddTab(Factory.tra("Můj kapacitní plán"), "gantt", "/j02/TabGantt?pid="+Factory.CurrentUser.j02ID.ToString()));
           
            //if (c.a01_count_involved > 0) strBadge = c.a01_count_involved.ToString();
            //v.NavTabs.Add(AddTab(Factory.tra("Akce"), "a01", "/TheGrid/SlaveView?prefix=a01", false, strBadge));
            v.NavTabs.Add(AddTab(Factory.tra("Akce"), "a01", "/TheGrid/SlaveView?prefix=a01", false, null));
            //strBadge = null;
            //if (c.a01_count_leader > 0) strBadge = c.a01_count_leader.ToString();
            //v.NavTabs.Add(AddTab(Factory.tra("Vedoucí"), "leader", "/TheGrid/SlaveView?prefix=a01&master_flag=leader", false, strBadge));
            //strBadge = null;
            //if (c.a01_count_member > 0) strBadge = c.a01_count_member.ToString();
            //v.NavTabs.Add(AddTab(Factory.tra("Člen"), "member", "/TheGrid/SlaveView?prefix=a01&master_flag=member", false, strBadge));
            //strBadge = null;
            //if (c.a01_count_invited > 0) strBadge = c.a01_count_invited.ToString();
            //v.NavTabs.Add(AddTab(Factory.tra("Přizvaná osoba"), "invited", "/TheGrid/SlaveView?prefix=a01&master_flag=invited", false, strBadge));
            //strBadge = null;
            //if (c.a01_count_issuer > 0) strBadge = c.a01_count_issuer.ToString();
            //v.NavTabs.Add(AddTab(Factory.tra("Zakladatel"), "issuer", "/TheGrid/SlaveView?prefix=a01&master_flag=issuer", false, strBadge));

            
            v.NavTabs.Add(AddTab("Úkoly/Lhůty", "h04", "/TheGrid/SlaveView?prefix=h04", true, parse_badge(c.h04_count)));          
            v.NavTabs.Add(AddTab("Nástěnka", "h11", "/TheGrid/SlaveView?prefix=h11", true, parse_badge(c.h11_count)));

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

        

        public IActionResult School(int a03id)
        {
          
            if (a03id == 0)
            {
                a03id = Factory.CBL.LoadUserParamInt("DashboardSchool-a03id", 0);
            }

            var v = new DashboardSchool() { a03ID = a03id, pid = Factory.CurrentUser.j02ID, NavTabs = new List<NavTab>() };
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            var mq = new BO.myQuery("h11") { IsRecordValid = true, MyRecordsDisponible = true };
            v.lisH11 = Factory.h11NoticeBoardBL.GetList(mq);

            mq = new BO.myQuery("a03") { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID };
            v.lisA03 = Factory.a03InstitutionBL.GetList(mq);
            if (v.lisA03.Count() == 0)
            {
                return this.StopPage(false, "Váš osobní profil nemá vazbu na instituci (školu).");
            }
            if (v.a03ID == 0)
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

            RefreshNavTabsSchool(v);
            
            //if (v.a10ID > 0)
            //{
            //    v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
            //    v.GridColumns = "a__a01Event__a01Signature,a01_a08__a08Theme__a08Name,a01_b02__b02WorkflowStatus__b02Name,a__a01Event__a01DateFrom,a__a01Event__a01DateUntil";
            //    if (v.RecA10.a10CoreFlag == "injury")
            //    {
            //        v.GridColumns += ",a01_xxa__v_uraz_jmenozraneneho__JmenoZraneneho,a01_xxb__v_uraz_datumzraneni__DatumZraneni,a01_xxc__v_uraz_poradovecislo__PoradoveCislo";
            //    }
            //}

            //if (v.IsAllowCreateA01 == false)
            //{
            //    if (Factory.j04UserRoleBL.GetListJ08(v.RecJ04.pid).Where(p => p.a10ID == v.a10ID && p.j08IsAllowedCreate).Count() > 0)
            //    {
            //        v.IsAllowCreateA01 = true;  //může zakládat tento typ akce
            //    }
            //}

            return View(v);
            //return ViewTup(v, BO.j05PermValuEnum.SchoolAdminUser);
        }

        private void RefreshNavTabsSchool(DashboardSchool v)
        {
            
            v.NavTabs.Add(AddTab(Factory.tra("Dashboard"), "dashboard", "/Dashboard/Widgets?skin=school&pid=" + Factory.CurrentUser.j02ID));
            
            
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SchoolAdminUser, v.RecJ04.j04RoleValue))
            {
                //uživatel má právo spravovat uživatelské účty v instituci                
                v.NavTabs.Add(AddTab("Správa uživatelských účtů", "a39", "/Dashboard/TabSchoolAccounts?a03id="+v.RecA03.pid.ToString(), true, parse_badge(v.lisA39.Count())));
            }
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.SchoolModuleTeachers, v.RecJ04.j04RoleValue))
            {
                //uživatel má přístup do modulu [Učitelé]    
                v.NavTabs.Add(AddTab("Učitelé", "k01", "/TheGrid/SlaveViewSchoolK01", true, null));                
            }

            if (v.RecJ04.j04IsAllowedAllEventTypes == false)
            {
                var lisJ08 = Factory.j04UserRoleBL.GetListJ08(v.RecJ04.pid);
                foreach (var c in lisJ08.Where(p => p.a10IsUse_K01 == false))
                {
                    v.NavTabs.Add(AddTab(c.a10Name, "a01", "/Dashboard/TabSchoolA01Grid?a03id="+v.a03ID.ToString()+"&a10id=" + c.a10ID.ToString(), false, null));

                }

            }
            else
            {
                v.IsAllowCreateA01 = true;  //může zakládat všechny akce
                var mq = new BO.myQuery("a10") { IsRecordValid = true };
                var lis = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10IsUse_K01 == false);
                foreach (var c in lis)
                {
                    v.NavTabs.Add(AddTab(c.a10Name, "a01", "/Dashboard/TabSchoolA01Grid?a03id=" + v.a03ID.ToString() + "&a10id=" + c.a10ID.ToString(), false, null));
                }
            }

            
            v.NavTabs.Add(AddTab("Nástěnka", "h11", "/TheGrid/SlaveView?prefix=h11", true, parse_badge(v.lisH11.Count())));

         


            string strDefTab = Factory.CBL.LoadUserParam("dashboard-tab-school");
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

        private string parse_badge(int intCount)
        {
            if (intCount > 0)
            {
                return intCount.ToString();
            }
            return null;
        }
    }
}