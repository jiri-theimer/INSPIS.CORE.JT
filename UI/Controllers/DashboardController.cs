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
        private readonly BL.TheColumnsProvider _colsProvider;
        public DashboardController(BL.ThePeriodProvider pp, BL.TheColumnsProvider cp)
        {
            _pp = pp;
            _colsProvider = cp;
        }

        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var a03id = Convert.ToInt32(tgi.viewstate[0]);
            var a10id = Convert.ToInt32(tgi.viewstate[1]);
            var c = new UI.TheGridSupport(GetSchoolA01GridInput(a03id,a10id,tgi.fixedcolumns), Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);

        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var a03id = Convert.ToInt32(tgi.viewstate[0]);
            var a10id = Convert.ToInt32(tgi.viewstate[1]);
            var c = new UI.TheGridSupport(GetSchoolA01GridInput(a03id, a10id, tgi.fixedcolumns), Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var a03id = Convert.ToInt32(tgi.viewstate[0]);
            var a10id = Convert.ToInt32(tgi.viewstate[1]);
            var c = new UI.TheGridSupport(GetSchoolA01GridInput(a03id, a10id, tgi.fixedcolumns), Factory, _colsProvider);

            return c.Event_HandleTheGridMenu(tgi.j72id);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var a03id = Convert.ToInt32(tgi.viewstate[0]);
            var a10id = Convert.ToInt32(tgi.viewstate[1]);
            var c = new UI.TheGridSupport(GetSchoolA01GridInput(a03id, a10id, tgi.fixedcolumns), Factory, _colsProvider);

            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }
        //-----------Konec GRID událostí-------------

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
            if (v.a10ID>0 && Factory.j04UserRoleBL.GetListJ08(v.RecJ04.pid).Where(p => p.a10ID == v.a10ID && p.j08IsAllowedCreate).Count() > 0)
            {
                v.IsAllowCreateA01 = true;  //může zakládat akce tohoto typu
            }

            v.gridinput = GetSchoolA01GridInput(v.a03ID, v.a10ID,v.GridColumns);

            v.period = new PeriodViewModel() { IsShowButtonRefresh = true };            
            var per =basUI.InhalePeriodDates(_pp,Factory,"a01", "a03Institution");
            v.period.PeriodValue = per.pid;
            v.period.d1 = per.d1;
            v.period.d2 = per.d2;

            return View(v);
        }

        private TheGridInput GetSchoolA01GridInput(int a03id,int a10id,string fixedcolumns)
        {
            var gi = new TheGridInput() {entity="a01Event", controllername = "Dashboard" };
            gi.query = new BO.myQueryA01() { a03id = a03id,a10id=a10id };
            gi.fixedcolumns = fixedcolumns;
            gi.ondblclick = "a01_doubleclick";
            gi.viewstate = a03id.ToString() + "|" + a10id.ToString();
            return gi;
        }

        public IActionResult TabSchoolAccounts(int a03id)   //podformulář pro správu školních účtů v rámci školy
        {
            var v = new a39TabSchoolAccount() { a03ID = a03id };
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);
            if (v.RecA03 == null)
            {
                return this.StopPage(false, "Nelze načíst profil instituce.");
            }

            var lisA03 = Factory.a03InstitutionBL.GetList(new BO.myQueryA03() { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID });
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

            //var cW = new UI.WidgetSupport(Factory, skin);
            //cW.PrepareWidgets(v);
            //cW.InhaleWidgetsDataContent(v);

            PrepareWidgets(v);
            InhaleWidgetsDataContent(v);

            return View(v);
        }
        public IActionResult Helpdesk(int b01id=0)
        {
            var v = new DashboardHelpdesk() { pid = Factory.CurrentUser.j02ID,b01ID=b01id, NavTabs = new List<NavTab>() };
            v.lisB01 = Factory.b01WorkflowTemplateBL.GetList(new BO.myQuery("b01"));
            if (v.b01ID == 0)
            {
                v.b01ID = Factory.CBL.LoadUserParamInt("dashboard-b01id-helpdesk");
            }
            if (v.b01ID == 0)
            {
                v.b01ID = v.lisB01.First().pid;
            }
            v.Rec = Factory.j02PersonBL.Load(v.pid);

            RefreshNavTabsHelpdesk(v);


            return View(v);
        }

        private void RefreshNavTabsHelpdesk(DashboardHelpdesk v)
        {
            var c = Factory.j02PersonBL.LoadSummary(v.pid, "dashboard");
            v.NavTabs.Add(AddTab("Dashboard", "dashboard", "/Dashboard/Widgets?skin=inspector&pid=" + Factory.CurrentUser.j02ID));
            
            //if (c.a01_count_involved > 0) strBadge = c.a01_count_involved.ToString();
            
            v.NavTabs.Add(AddTab(Factory.tra("Požadavky"), "a01", "/TheGrid/SlaveView?prefix=a01", false, null));

            v.NavTabs.Add(AddTab(Factory.tra("Přímo mně přidělené"), "member", "/TheGrid/SlaveView?prefix=a01&master_flag=member", false, null));
            var lisB02 = Factory.b02WorkflowStatusBL.GetList(new BO.myQuery("b02")).Where(p => p.b01ID == v.b01ID && p.b02IsSeparateTab==true);
            foreach(var recB02 in lisB02)
            {
                v.NavTabs.Add(AddTab(recB02.b02Name, "b02-"+ recB02.pid.ToString(), "/TheGrid/SlaveView?prefix=a01&master_entity=b02&master_pid="+recB02.pid.ToString(),false, null));
            }

            v.NavTabs.Add(AddTab("Nástěnka", "h11", "/TheGrid/SlaveView?prefix=h11", true, parse_badge(c.h11_count)));

            v.NavTabs.Add(AddTab("Inbox", "x40", "/TheGrid/SlaveView?prefix=x40"));



            string strDefTab = Factory.CBL.LoadUserParam("dashboard-tab-helpdesk");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                if (!tab.Url.Contains("master_entity"))
                {
                    tab.Url += "&master_entity=j02Person&master_pid=" + v.pid.ToString();
                }
                
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
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
            v.NavTabs.Add(AddTab("Dashboard", "dashboard", "/Dashboard/Widgets?skin=inspector&pid=" + Factory.CurrentUser.j02ID));
            v.NavTabs.Add(AddTab("Časový plán", "gantt", "/a35/TabPersonalTimeline?pid="+Factory.CurrentUser.j02ID.ToString()));
           
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

            
            v.lisA03 = Factory.a03InstitutionBL.GetList(new BO.myQueryA03() { IsRecordValid = true, j02id = Factory.CurrentUser.j02ID });
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
                if (lis.Count() <= 8)
                {
                    foreach (var c in lis)
                    {
                        v.NavTabs.Add(AddTab(c.a10Name, "a01", "/Dashboard/TabSchoolA01Grid?a03id=" + v.a03ID.ToString() + "&a10id=" + c.a10ID.ToString(), false, null));
                    }
                }
                else
                {
                    v.NavTabs.Add(AddTab("Akce", "a01", "/Dashboard/TabSchoolA01Grid?a03id=" + v.a03ID.ToString(), false, null));
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

        public BO.Result SaveWidgetState(string s, string skin)
        {
            var rec = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            rec.x56DockState = s;
            rec.x56Skin = skin;
            Factory.x55WidgetBL.SaveState(rec);
            return new BO.Result(false);
        }
        public BO.Result RemoveWidget(int x55id, string skin)
        {
            var recX55 = Factory.x55WidgetBL.Load(x55id);
            var recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() > 0)
            {
                boxes.Remove(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                Factory.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }

            return new BO.Result(true, "widget not found");
        }
        public BO.Result InsertWidget(int x55id, string skin)
        {
            var recX55 = Factory.x55WidgetBL.Load(x55id);
            var recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin);
            var boxes = BO.BAS.ConvertString2List(recX56.x56Boxes);
            if (boxes.Where(p => p == recX55.x55Code).Count() == 0)
            {
                boxes.Add(recX55.x55Code);
                recX56.x56Boxes = string.Join(",", boxes);
                Factory.x55WidgetBL.SaveState(recX56);
                return new BO.Result(false);
            }
            return new BO.Result(true, "widget not found");
        }
        public BO.Result SavePocetSloupcu(int x, string skin)
        {
            Factory.CBL.SetUserParam("Widgets-ColumnsPerPage-" + skin, x.ToString());
            return new BO.Result(false);            
        }

        public BO.Result Clear2FactoryState(string skin)    //vyčistí plochu do továrního nastavení
        {
            Factory.x55WidgetBL.Clear2FactoryState(Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, skin));
            return new BO.Result(false);            
        }


        //naplění boxy reálnými daty
        private void InhaleWidgetsDataContent(WidgetsViewModel v)
        {
            foreach (var rec in v.lisUserWidgets)
            {
                if (rec.x55TableSql != null && rec.x55TableColHeaders != null)
                {
                    string s = rec.x55TableSql;
                    s = DL.BAS.ParseMergeSQL(s, Factory.CurrentUser.j02ID.ToString()).Replace("@j04id", Factory.CurrentUser.j04ID.ToString().Replace("@j03id", Factory.CurrentUser.pid.ToString()));
                    var dt = Factory.gridBL.GetListFromPureSql(s);
                    if (dt.Rows.Count >= rec.x55DataTablesLimit && rec.x55DataTablesLimit > 0)
                    {
                        rec.IsUseDatatables = true;
                    }
                    var cGen = new BO.CLS.Datatable2Html(new BO.CLS.Datatable2HtmlDef() { ColHeaders = rec.x55TableColHeaders, ColTypes = rec.x55TableColTypes, ClientID = rec.x55Code, IsUseDatatables = rec.IsUseDatatables });
                    rec.x55Content = cGen.CreateHtmlTable(dt, 1000);

                }
                else
                {
                    switch (rec.x55Code.ToLower())
                    {
                        case "pandulak":
                            var pandulak = new ThePandulak(Factory.App.AppRootFolder + "\\wwwroot\\images\\pandulak");
                            rec.x55Content = string.Format("<img src='/images/pandulak/{0}'/>", pandulak.getPandulakImage(1));
                            if (v.ColumnsPerPage <= 2)
                            {
                                rec.x55Content += string.Format("<img src='/images/pandulak/{0}'/>", pandulak.getPandulakImage(2));
                            }
                            break;
                    }
                }
                if (rec.IsUseDatatables && rec.x55DataTablesButtons > BO.x55DataTablesBtns.None)
                {
                    v.IsExportButtons = true;   //zobrazovat tlačítka XLS/CSV/COPY
                }
                if (rec.IsUseDatatables && rec.x55DataTablesButtons == BO.x55DataTablesBtns.ExportPrintPdf)
                {
                    v.IsPdfButtons = true;      //zobrazovat i tlačítko PDF
                }
                if (v.IsPdfButtons || rec.x55DataTablesButtons == BO.x55DataTablesBtns.ExportPrint)
                {
                    v.IsPrintButton = true;      //zobrazovat i tlačítko PDF
                }

            }
        }
        //rozvržení prostoru na ploše
        private void PrepareWidgets(WidgetsViewModel v)
        {
            var mq = new BO.myQuery("x55") { IsRecordValid = true, MyRecordsDisponible = true, CurrentUser = Factory.CurrentUser };
            var hodnoty = new List<string>() { null,v.Skin };
            if (v.Skin == "inspector" || v.Skin == "index")
            {
                hodnoty.Add("inspector_index");
            }
            v.lisAllWidgets = Factory.x55WidgetBL.GetList(mq).Where(p => hodnoty.Contains(p.x55Skin));
           


            v.lisUserWidgets = new List<BO.x55Widget>();
            v.ColumnsPerPage = Factory.CBL.LoadUserParamInt("Widgets-ColumnsPerPage-" + v.Skin, 2);
            v.recX56 = Factory.x55WidgetBL.LoadState(Factory.CurrentUser.pid, v.Skin);
            v.DockStructure = new WidgetsEnvironment(v.recX56.x56DockState);

            if (Factory.CurrentUser.j03LangIndex == 2)
            {
                v.DataTables_Localisation = "/lib/datatables/localisation/uk_UA.json";
            }
            else
            {
                v.DataTables_Localisation = "/lib/datatables/localisation/cs_CZ.json";
            }

            if (v.recX56 == null || v.recX56.x56Boxes == null)
            {
                return; //uživatel nemá na ploše žádný widget, dál není třeba pokračovat
            }

            var boxes = BO.BAS.ConvertString2List(v.recX56.x56Boxes);
            foreach (string s in boxes)
            {
                if (v.lisAllWidgets.Where(p => p.x55Code == s).Count() > 0)
                {
                    v.lisUserWidgets.Add(v.lisAllWidgets.Where(p => p.x55Code == s).First());
                }
            }


            foreach (var onestate in v.DockStructure.States)
            {
                if (v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).Count() > 0)
                {
                    var c = v.lisUserWidgets.Where(p => p.pid.ToString() == onestate.Value).First();
                    switch (onestate.Key)
                    {
                        case "2":
                            if (v.ColumnsPerPage >= 2) v.DockStructure.Col2.Add(c);
                            break;
                        case "3":
                            if (v.ColumnsPerPage >= 3) v.DockStructure.Col3.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }
                }
            }
            foreach (var c in v.lisUserWidgets)
            {
                if ((v.DockStructure.Col1.Contains(c) || v.DockStructure.Col2.Contains(c) || v.DockStructure.Col3.Contains(c)) == false)
                {
                    switch (v.ColumnsPerPage)
                    {
                        case 2 when (v.DockStructure.Col1.Count() >= 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() >= 2):
                            v.DockStructure.Col3.Add(c);
                            break;
                        case 3 when (v.DockStructure.Col1.Count() >= 2 && v.DockStructure.Col2.Count() < 2):
                            v.DockStructure.Col2.Add(c);
                            break;
                        default:
                            v.DockStructure.Col1.Add(c);
                            break;
                    }

                }
            }
            switch (v.ColumnsPerPage)
            {
                case 1:
                    v.BoxColCss = "col-12";
                    break;
                case 2:
                    v.BoxColCss = "col-lg-6";
                    break;
                case 3:
                    v.BoxColCss = "col-sm-6 col-lg-4";
                    break;
            }
        }
    }
}