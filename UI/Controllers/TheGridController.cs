using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using UI.Models;


namespace UI.Controllers
{
    public class TheGridController : BaseController        
    {
       
        
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity);
            var c = new UI.TheGridSupport(Factory, _colsProvider) { extendpagerhtml = v.ExtendPagerHtml };
           
            return c.Event_HandleTheGridFilter(tgi, filter, v.myQueryGrid);
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity);
            var c = new UI.TheGridSupport(Factory, _colsProvider) { extendpagerhtml = v.ExtendPagerHtml };
            
            return c.Event_HandleTheGridOper(tgi, v.myQueryGrid);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi)  //TheGrid povinná metoda: zobrazení grid menu
        {

            var c = new UI.TheGridSupport(Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
        }
        public FileResult HandleTheGridExport(int j72id,string pathname,string format, string pids)  //TheGrid povinná metoda pro export dat
        {
            var gridState = Factory.j72TheGridTemplateBL.LoadState(j72id, Factory.CurrentUser.pid);
            var v = LoadFsmViewModel(gridState.j72Entity.Substring(0,3), 0, pathname.Split("/")[1].ToLower(), gridState.j72MasterEntity);
            var c = new UI.TheGridSupport(Factory, _colsProvider);
            
            var fullpath = c.Event_TheGridExport(format, j72id, pids, v.myQueryGrid);
            if (format == "csv")
            {
                return File(System.IO.File.ReadAllBytes(fullpath), "application/CSV", "export_" + v.prefix + ".csv");
            }
            if (format == "xlsx")
            {
                return File(System.IO.File.ReadAllBytes(fullpath), "application/vnd.ms-excel", "export_" + v.prefix + ".xlsx");
            }
            return null;


        }

        public IActionResult FlatView(string prefix, int go2pid)    //pouze grid bez subform
        {
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"flatview",null);
            
            v.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);

            return View(v);
        }

        

        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel, není zde filtrovací pruh s fixním filtrem
        {
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"masterview",null);
                      
            v.j72id = Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);
            
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "a01":
                    tabs.Add(AddTab("Info", "viewInfo", "/a01/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Historie událostí","viewHistory","/a01/TabHistory?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Účastníci akce", "viewUcastnici", "/a01/TabUcastnici?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Časový plán", "viewCapacity","/a35/TabCapacity?pid=" + AppendPid2Url(v.go2pid)));

                    tabs.Add(AddTab("Formuláře", "viewFormulare", "/a01/TabForms?pid=" + AppendPid2Url(v.go2pid)));

                    //tabs.Add(new NavTab() { Name = "Přílohy", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(AddTab("Úkoly/Lhůty","h04ToDo","SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Související akce", "a01Event", "SlaveView?prefix=a01"));
                    
                    break;
                case "a03":
                    tabs.Add(AddTab("Info", "viewInfo", "/a03/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Činnosti školy", "a37InstitutionDepartment", "SlaveView?prefix=a37"));
                    tabs.Add(AddTab("Vzdělávací obory","a19DomainToInstitutionDepartment", "SlaveView?prefix=a19"));
                    tabs.Add(AddTab("Kontaktní osoby","a39InstitutionPerson","SlaveView?prefix=a39"));
                    tabs.Add(AddTab("Učitelé", "k01Teacher", "SlaveView?prefix=k01"));
                    tabs.Add(AddTab("INEZ", "a42Qes", "SlaveView?prefix=a42"));
                    tabs.Add(AddTab( "Pojmenované seznamy",  "a29InstitutionList","SlaveView?prefix=a29"));

                    break;
                case "a42":
                    tabs.Add(AddTab("Info", "viewInfo","/a42/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue","SlaveView?prefix=x40"));
                    break;
                case "j02":
                    tabs.Add(AddTab("Info", "viewInfo", "/j02/Info?pid=" + AppendPid2Url(v.go2pid)));
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je zakladatelem"), Factory.App.Terminology_Akce), "a01Event", "SlaveView?prefix=a01",false));
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je účastníkem"), Factory.App.Terminology_Akce), "a01Event", "SlaveView?prefix=a01", false));
                    tabs.Add(AddTab("Instituce", "a03Institution", "SlaveView?prefix=a03"));
                    tabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue", "SlaveView?prefix=x40"));
                    tabs.Add(AddTab("PING Log", "j92PingLog", "SlaveView?prefix=j92"));
                    tabs.Add(AddTab("LOGIN Log", "j90LoginAccessLog", "SlaveView?prefix=j90"));
                    break;
                
                
                
            }
            string strDefTab = Factory.CBL.LoadUserParam("masterview-tab-" + prefix);
            var deftab = tabs[0];
            
            foreach (var tab in tabs)
            {
               if (tab.Url.Contains("?pid")==false)
                {
                    tab.Url += "&master_entity=" + v.entity + "&master_pid=" + AppendPid2Url(v.go2pid);
                   
                }

                if (strDefTab !="" && tab.Entity== strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka
                }
            }
            deftab.CssClass += " active";
            if (go2pid > 0)
            {
                v.go2pid_url_in_iframe = deftab.Url;
                
            }

            v.NavTabs = tabs;
            return View(v);
        }

        private string AppendPid2Url(int go2pid)
        {
            if (go2pid > 0)
            {
                return go2pid.ToString();
            }
            else
            {
                return  "@pid";
            }
        }
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid,string master_flag)    //podřízený subform v rámci MasterView
        {
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"slaveview", master_entity);
            v.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix + "-" + master_entity);
            v.master_entity = master_entity;
            v.master_pid = master_pid;
            v.master_flag = master_flag;
            
            if (String.IsNullOrEmpty(v.master_entity) || v.master_pid == 0)
            {
                AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
            }
            

            return View(v);
        }
        private string get_param_key(string strKey,string strMasterEntity)
        {
            if (strMasterEntity != null)
            {
                return (strKey += "-" + strMasterEntity);
            }
            else
            {
                return strKey;
            }
        }

        


        private FsmViewModel LoadFsmViewModel(string prefix,int go2pid,string pagename,string masterentity)
        {
            var v = new FsmViewModel() { prefix = prefix, go2pid = go2pid,contextmenuflag=1,myQueryGrid=new BO.myQuery(prefix) };
            BO.TheEntity c = Factory.EProvider.ByPrefix(prefix);
            v.entity = c.TableName;
            v.entityTitle = c.AliasPlural;
            
            
            if (v.entity == "")
            {
                AddMessage("Grid entita nebyla nalezena.");
            }
            
            if (v.prefix == "a01")
            {           
                v.myQueryGrid.a10id= Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a10id", masterentity));
                if (v.myQueryGrid.a10id > 0)
                {
                    v.FilterA10ID = v.myQueryGrid.a10id.ToString();
                    v.FilterA10Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a10name", masterentity));
                }
                
                
                if (pagename != "slaveview")    //v podformuláři akcí se nefiltruje podle tématu
                {
                    v.myQueryGrid.a08id= Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a08id", masterentity));
                    if (v.myQueryGrid.a08id > 0)
                    {
                        v.FilterA08ID = v.myQueryGrid.a08id.ToString();
                        v.FilterA08Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a08name", masterentity));
                    }
                    
                }
                
            }
            if (v.prefix == "h04")
            {                
                v.FilterH07ID = Factory.CBL.LoadUserParam(get_param_key("grid-filter-h07id",masterentity));
                v.FilterH07Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-h07name",masterentity));
               
            }


            if (v.prefix == "a01" || v.prefix == "h04")
            {
                v.FilterMyInvolvement = Factory.CBL.LoadUserParam(get_param_key("grid-filter-myinvolvement-" +v.prefix,masterentity));
                switch (v.FilterMyInvolvement)
                {
                    case "issuer":
                        v.myQueryGrid.j02id_issuer = Factory.CurrentUser.j02ID;                        
                        break;
                    case "leader":
                        v.myQueryGrid.j02id_leader = Factory.CurrentUser.j02ID;
                        break;
                    case "member":
                        v.myQueryGrid.j02id_member = Factory.CurrentUser.j02ID;
                        break;
                    case "involved":
                        v.myQueryGrid.j02id_involved = Factory.CurrentUser.j02ID;
                        break;
                    case "invited":
                        v.myQueryGrid.j02id_invited = Factory.CurrentUser.j02ID;
                        break;
                }
                

                v.period = new PeriodViewModel();
                v.period.IsShowButtonRefresh = true;
                var per = basUI.InhalePeriodDates(_pp,Factory,v.prefix,masterentity);
                v.period.PeriodValue = per.pid;
                v.period.d1 = per.d1;
                v.period.d2 = per.d2;
                v.myQueryGrid.global_d1 = v.period.d1;
                v.myQueryGrid.global_d2 = v.period.d2;
            }

            if (v.prefix=="a01" || v.prefix=="a03" || v.prefix == "j02")
            {
                if (pagename == "flatview")
                {
                    v.ExtendPagerHtml= "<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='switch_to_master(\"" + v.prefix+"\")'>Zapnout spodní panel</button>";
                }
                if (pagename == "masterview")
                {
                    v.ExtendPagerHtml = "<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='switch_to_flat(\"" + v.prefix + "\")'>" + Factory.tra("Vypnout spodní panel") + "</button>";
                }
            }

            return v;

        }
        
        

    }
}