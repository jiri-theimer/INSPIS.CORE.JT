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

        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi,List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            int master_pid = 0;
            string master_flag = null;
            if (tgi.viewstate != null)
            {
                master_pid = Convert.ToInt32(tgi.viewstate[0]);
                master_flag = tgi.viewstate[1];
            }
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity,master_pid,master_flag);
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            int master_pid = 0;
            string master_flag = null;
            if (tgi.viewstate != null)
            {
                master_pid = Convert.ToInt32(tgi.viewstate[0]);
                master_flag = tgi.viewstate[1];
            }
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, master_pid, master_flag);
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            int master_pid = 0;
            string master_flag = null;
            if (tgi.viewstate != null)
            {
                master_pid = Convert.ToInt32(tgi.viewstate[0]);
                master_flag = tgi.viewstate[1];
            }
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, master_pid, master_flag);
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
        }

        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            int master_pid = 0;
            string master_flag = null;
            if (tgi.viewstate != null)
            {
                master_pid = Convert.ToInt32(tgi.viewstate[0]);
                master_flag = tgi.viewstate[1];
            }
            var v = LoadFsmViewModel(tgi.prefix, 0, tgi.pathname.Split("/").Last().ToLower(), tgi.master_entity, master_pid, master_flag);
            var c = new UI.TheGridSupport(v.gridinput, Factory, _colsProvider);
            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }        
        //-----------Konec GRID událostí-------------

        public IActionResult FlatView(string prefix, int go2pid)    //pouze grid bez subform
        {
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"flatview",null,0,null);
            
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);

            return View(v);
        }

        

        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel, není zde filtrovací pruh s fixním filtrem
        {
            if (!TestGridPermissions(prefix))
            {
                return this.StopPage(false, "Nemáte oprávnění pro tento GRID přehled.");
            }
            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"masterview",null,0,null);
                      
            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);
            
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "a01":
                    tabs.Add(AddTab("Info", "viewInfo", "/a01/Info?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Historie událostí","viewHistory","/a01/TabHistory?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Účastníci akce", "viewUcastnici", "/a01/TabUcastnici?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Časový plán", "viewCapacity","/a35/TabCapacity?pid=" + AppendPid2Url(v.gridinput.go2pid)));

                    tabs.Add(AddTab("Formuláře", "viewFormulare", "/a01/TabForms?pid=" + AppendPid2Url(v.gridinput.go2pid)));

                    //tabs.Add(new NavTab() { Name = "Přílohy", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(AddTab("Úkoly/Lhůty","h04ToDo","SlaveView?prefix=h04"));
                    tabs.Add(AddTab("Související akce", "a01Event", "SlaveView?prefix=a01"));
                    
                    break;
                case "a03":
                    tabs.Add(AddTab("Info", "viewInfo", "/a03/Info?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Činnosti školy", "a37InstitutionDepartment", "SlaveView?prefix=a37"));
                    tabs.Add(AddTab("Vzdělávací obory","a19DomainToInstitutionDepartment", "SlaveView?prefix=a19"));
                    tabs.Add(AddTab("Kontaktní osoby","a39InstitutionPerson","SlaveView?prefix=a39"));
                    tabs.Add(AddTab("Učitelé", "k01Teacher", "SlaveView?prefix=k01"));
                    tabs.Add(AddTab("INEZ", "a42Qes", "SlaveView?prefix=a42"));
                    tabs.Add(AddTab( "Pojmenované seznamy",  "a29InstitutionList","SlaveView?prefix=a29"));

                    break;
                case "a42":
                    tabs.Add(AddTab("Info", "viewInfo","/a42/Info?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Svázané akce", "a01Event", "SlaveView?prefix=a01"));
                    tabs.Add(AddTab("Outbox", "x40MailQueue","SlaveView?prefix=x40"));
                    break;
                case "j02":
                    tabs.Add(AddTab("Info", "viewInfo", "/j02/Info?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    tabs.Add(AddTab("Časový plán", "gantt", "/a35/TabPersonalTimeline?pid=" + AppendPid2Url(v.gridinput.go2pid)));
                    var s = Factory.tra("Akce");
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je zakladatelem"), s), "a01Event", "SlaveView?prefix=a01",false));
                    tabs.Add(AddTab(string.Format(Factory.tra("{0}: je účastníkem"), s), "a01Event", "SlaveView?prefix=a01", false));
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
                    tab.Url += "&master_entity=" + v.entity + "&master_pid=" + AppendPid2Url(v.gridinput.go2pid);
                   
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
            //var pathpars = new List<BO.StringPair>();
            //pathpars.Add(new BO.StringPair() { Key = "master_pid", Value = master_pid.ToString() });
            //if (master_flag != null)
            //{
            //    pathpars.Add(new BO.StringPair() { Key = "master_flag", Value = master_flag });
            //}

            FsmViewModel v = LoadFsmViewModel(prefix, go2pid,"slaveview", master_entity,master_pid,master_flag);
            

            v.gridinput.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix + "-" + master_entity);
            
            
            

            if (String.IsNullOrEmpty(master_entity) || master_pid == 0)
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


        private bool TestGridPermissions(string prefix)
        {

            switch (prefix)
            {
                case "a01":
                    return Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.A01Grid);
                case "a03":
                    return Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.Menu_A03);
                case "h04":
                    return Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.Menu_H04);
                case "a42":
                    return Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.Menu_RS);
                default:
                    return true;
            }
        }
        private FsmViewModel LoadFsmViewModel(string prefix,int go2pid,string pagename,string masterentity,int master_pid,string master_flag)
        {
            var v = new FsmViewModel() { prefix = prefix,master_pid=master_pid,master_flag=master_flag };

            BO.TheEntity c = Factory.EProvider.ByPrefix(prefix);
            v.entity = c.TableName;
            v.entityTitle = c.AliasPlural;

            v.gridinput = new TheGridInput() {entity=v.entity, go2pid = go2pid, master_entity = masterentity };
            

            if (v.entity == "")
            {
                AddMessage("Grid entita nebyla nalezena.");
            }
                        
            

            if (master_pid>0)
            {
                v.gridinput.viewstate = master_pid.ToString() + "|" + master_flag;
                
            }

            switch (v.prefix)
            {
                case "a01":
                    var mqA01 = new BO.InitMyQuery().LoadA01(masterentity, master_pid, master_flag);
                    mqA01.a10id = Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a10id", masterentity));
                    if (mqA01.a10id > 0)
                    {
                        v.FilterA10ID = mqA01.a10id.ToString();
                        v.FilterA10Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a10name", masterentity));
                    }
                    if (pagename != "slaveview")    //v podformuláři akcí se nefiltruje podle tématu
                    {
                        mqA01.a08id = Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a08id", masterentity));
                        if (mqA01.a08id > 0)
                        {
                            v.FilterA08ID = mqA01.a08id.ToString();
                            v.FilterA08Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a08name", masterentity));
                        }

                    }

                    v.FilterMyInvolvement = Factory.CBL.LoadUserParam(get_param_key("grid-filter-myinvolvement-a01", masterentity));
                    switch (v.FilterMyInvolvement)
                    {
                        case "issuer":
                            mqA01.j02id_issuer = Factory.CurrentUser.j02ID;break;
                        case "leader":
                            mqA01.j02id_leader = Factory.CurrentUser.j02ID;break;
                        case "member":
                            mqA01.j02id_member = Factory.CurrentUser.j02ID;break;
                        case "involved":
                            mqA01.j02id_involved = Factory.CurrentUser.j02ID;break;
                        case "invited":
                            mqA01.j02id_invited = Factory.CurrentUser.j02ID;break;
                    }

                    v.gridinput.query = mqA01;
                    break;
                case "h04":
                    var mqH04 = new BO.InitMyQuery().LoadH04(masterentity, master_pid, master_flag);
                    v.FilterMyInvolvement = Factory.CBL.LoadUserParam(get_param_key("grid-filter-myinvolvement-h04", masterentity));
                    switch (v.FilterMyInvolvement)
                    {
                        case "issuer":
                            mqH04.j02id_issuer = Factory.CurrentUser.j02ID; break;                        
                        case "member":
                            mqH04.j02id_member = Factory.CurrentUser.j02ID; break;                       
                    }

                    mqH04.h07id = Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-h07id", masterentity));
                    if (mqH04.h07id > 0)
                    {
                        v.FilterH07ID = mqH04.h07id.ToString();
                        v.FilterH07Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-h07name", masterentity));
                    }
                    

                    v.gridinput.query = mqH04;
                    break;
                default:
                    v.gridinput.query = new BO.InitMyQuery().Load(prefix, masterentity, master_pid, master_flag);
                    break;
            }

            if (!Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal))
            {
                v.gridinput.query.MyRecordsDisponible = true;
            }
            
           


            if (v.prefix == "a01" || v.prefix == "h04")
            {
                
                v.period = new PeriodViewModel();
                v.period.IsShowButtonRefresh = true;
                var per = basUI.InhalePeriodDates(_pp,Factory,v.prefix,masterentity);
                v.period.PeriodValue = per.pid;
                v.period.d1 = per.d1;
                v.period.d2 = per.d2;
                v.gridinput.query.global_d1 = v.period.d1;
                v.gridinput.query.global_d2 = v.period.d2;
            }

            if (v.prefix=="a01" || v.prefix=="a03" || v.prefix == "j02")
            {
                if (pagename == "flatview")
                {
                    v.gridinput.extendpagerhtml= "<button type='button' class='btn btn-secondary btn-sm mx-4 nonmobile' onclick='switch_to_master(\"" + v.prefix+"\")'>Zapnout spodní panel</button>";
                }
                if (pagename == "masterview")
                {
                    v.gridinput.extendpagerhtml = "<button type='button' class='btn btn-secondary btn-sm mx-4' onclick='switch_to_flat(\"" + v.prefix + "\")'>" + Factory.tra("Vypnout spodní panel") + "</button>";
                }
            }

            

            return v;

        }

        //private BO.baseQuery InhaleQueryA01(FsmViewModel v,string pagename, string masterentity)
        //{            
        //    var mq = new BO.myQueryA01();
        //    mq.MyRecordsDisponible = true;

        //    mq.a10id = Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a10id", masterentity));
        //    if (mq.a10id > 0)
        //    {
        //        v.FilterA10ID = mq.a10id.ToString();
        //        v.FilterA10Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a10name", masterentity));
        //    }
        //    if (pagename != "slaveview")    //v podformuláři akcí se nefiltruje podle tématu
        //    {
        //        mq.a08id = Factory.CBL.LoadUserParamInt(get_param_key("grid-filter-a08id", masterentity));
        //        if (mq.a08id > 0)
        //        {
        //            v.FilterA08ID = mq.a08id.ToString();
        //            v.FilterA08Name = Factory.CBL.LoadUserParam(get_param_key("grid-filter-a08name", masterentity));
        //        }

        //    }
        //    v.FilterMyInvolvement = Factory.CBL.LoadUserParam(get_param_key("grid-filter-myinvolvement-a01", masterentity));
        //    switch (v.FilterMyInvolvement)
        //    {
        //        case "issuer":
        //            mq.j02id_issuer = Factory.CurrentUser.j02ID; break;
        //        case "leader":
        //            mq.j02id_leader = Factory.CurrentUser.j02ID; break;
        //        case "member":
        //            mq.j02id_member = Factory.CurrentUser.j02ID; break;
        //        case "involved":
        //            mq.j02id_involved = Factory.CurrentUser.j02ID; break;
        //        case "invited":
        //            mq.j02id_invited = Factory.CurrentUser.j02ID; break;
        //    }

        //    return mq;
        //}
        
        

    }
}