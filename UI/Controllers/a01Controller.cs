using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class a01Controller : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        public a01Controller(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;
        }

        //Odstranit akci
        public IActionResult KillRecord(int pid, bool confirm)
        {
            var v = new a01RecPage() { pid = pid };
            v.Rec = Factory.a01EventBL.Load(v.pid);
            v.RecLastEvent = Factory.b05Workflow_HistoryBL.LoadLast(v.pid);
            v.RecIssuer = Factory.j02PersonBL.Load(v.Rec.j02ID_Issuer);
            var tg = Factory.o51TagBL.GetTagging("a01", v.pid);
            v.Rec.TagHtml = tg.TagHtml;
            v.TagHtml = v.Rec.TagHtml;
            if (!v.Rec.a01IsTemporary)
            {
                if (!Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal))
                {
                    return this.StopPage(true, "Funkce je dostupná pouze administrátorovi.",true);
                }
            }

            if (confirm && Factory.CBL.DeleteRecord("a01", v.Rec.pid) == "1")
            {
                Factory.CBL.SetUserParam("a01-RecPage-pid", null);
                v.SetJavascript_CallOnLoad(v.Rec.pid);
                return View(v);
            }


            return View(v);
        }

        public IActionResult AddSouvisejici(int pid)
        {
            var v = new a01AddSouvisejici() { a01id = pid };
            if (v.a01id == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            RefreshStateAddSouvisejici(v);
            var perm = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal) || perm.PermValue == BO.a01EventPermissionENUM.FullAccess || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Owner || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Leader)
            {
            }
            else
            {
                return this.StopPage(true, "Funkce je dostupná pouze vedoucímu týmu nebo administrátorovi.",true);
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSouvisejici(a01AddSouvisejici v, string oper, int pid)
        {
            if (oper == "add_a01id")
            {
                v.SelectedA01ID = pid;
                RefreshStateAddSouvisejici(v);
                return View(v);
            }
            if (oper == "delete")
            {
                if (Factory.CBL.DeleteRecord("a24", pid) == "1")
                {
                    v.SetJavascript_CallOnLoad("/a01/RecPage");
                    RefreshStateAddSouvisejici(v);
                    return View(v);
                }
                
                
            }
            RefreshStateAddSouvisejici(v);

            if (ModelState.IsValid)
            {

                var c = new BO.a24EventRelation() { a01ID_Left = v.SelectedA01ID, a01ID_Right = v.RecA01.pid, a46ID = v.SelectedA46ID };
                c.pid = Factory.a01EventBL.SaveA24Record(c);
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad("/a01/RecPage?pid=" + v.SelectedA01ID.ToString());
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
        private void RefreshStateAddSouvisejici(a01AddSouvisejici v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01id);
            if (v.SelectedA01ID > 0)
            {
                v.RecA01Selected = Factory.a01EventBL.Load(v.SelectedA01ID);
            }
            v.lisA24 = Factory.a01EventBL.GetList_a24(v.a01id);

        }

        public IActionResult AddAttachment(int pid)
        {
            var v = new a01AddAttachment() { a01id = pid, UploadGuid = BO.BAS.GetGuid() };
            if (v.a01id == 0)
            {
                return this.StopPageSubform("pid is missing");
            }

            RefreshStateAddAttachment(v);
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.",true);
            }
            
            if (v.RecA01.a01ParentID > 0 && Factory.a01EventBL.GetList_a24(v.RecA01.pid).Where(p => p.a46ID == 5).Count() == 0)
            {
                return this.StopPage(true, "Tato akce je podřízená.", true);
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAttachment(a01AddAttachment v)
        {
            RefreshStateAddAttachment(v);
            if (ModelState.IsValid)
            {
                if (Factory.o27AttachmentBL.GetTempFiles(v.UploadGuid).Count() == 0)
                {
                    this.AddMessage("Chybí nahrát alespoň jedna příloha."); return View(v);
                }
                bool b = Factory.o27AttachmentBL.SaveChangesAndUpload(v.UploadGuid, 101, v.RecA01.pid);

                if (b)
                {

                    v.SetJavascript_CallOnLoad(v.RecA01.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateAddAttachment(a01AddAttachment v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01id);


        }

        public IActionResult TabSouvisejici(int pid)
        {
            var v = new a01TabSouvisejici() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            v.IsGridView = Factory.CBL.LoadUserParamBool("TabSouvisejici-IsGridView", false);
            v.gridinput = GetGridInput("TabSouvisejici",v.pid);
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            if (!v.IsGridView)
            {
                v.lisA24 = Factory.a01EventBL.GetList_a24(v.pid);
            }

            return View(v);
        }
        public IActionResult TabForms(int pid)
        {
            var v = new a01TabForms() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }

            v.IsGridView = Factory.CBL.LoadUserParamBool("TabForms-IsGridView", false);
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.RecA10 = Factory.a10EventTypeBL.Load(v.RecA01.a10ID);

            v.gridinput = GetGridInput("TabForms",v.pid);

            if (!v.IsGridView)
            {
                v.lisA11 = Factory.a11EventFormBL.GetList(new BO.myQueryA11() { a01id = v.pid }).OrderBy(p => p.a11IsPoll);
            }

            return View(v);
        }

        private TheGridInput GetGridInput(string pagename, int a01id)
        {
            var gi = new TheGridInput() { controllername = "a01", master_entity = "a01Event" };
            if (pagename.ToLower().Contains("tabforms"))
            {
                gi.entity = "a11EventForm";
                gi.query = new BO.myQueryA11() { a01id = a01id };
            }
            if (pagename.ToLower().Contains("tabsouvisejici"))
            {
                gi.entity = "a01Event";
                gi.query = new BO.myQueryA01() { a01id = a01id };
            }            
            gi.viewstate = a01id.ToString();
            return gi;
        }
        
        public IActionResult TabUcastnici(int pid)
        {
            var v = new a01TabUcastnici() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            var mq = new BO.myQueryA41() { a01id = v.pid };            
            v.lisA41 = Factory.a41PersonToEventBL.GetList(mq);
            return View(v);
        }
        public IActionResult TabAttachments(int pid)
        {
            var v = new a01TabAttachments() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);
            var mq = new BO.myQueryO27();
            mq.a01id = v.pid;
            mq.IsRecordValid = true;
            v.lisO27 = Factory.o27AttachmentBL.GetList(mq, null).OrderByDescending(p => p.pid);
            return View(v);
        }
        public IActionResult TabHistory(int pid)
        {
            var v = new a01TabHistory() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            var mq = new BO.myQuery("b05");
            mq.a01id = v.pid;
            v.lisB05 = Factory.b05Workflow_HistoryBL.GetList(mq);
            var lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { a01id = v.pid },null);
            IEnumerable<BO.a41PersonToEvent> lisA41 = null;
            if (v.lisB05.Any(p => p.b05IsCommentRestriction)){
                //v historii komentářů jsou zprávy cílené pro určité role
                lisA41= Factory.a41PersonToEventBL.GetList(new BO.myQueryA41() { a01id = pid, j02id = Factory.CurrentUser.j02ID });
            }
            foreach (var c in v.lisB05)
            {
                
                if (c.b05IsCommentRestriction && !Factory.IsUserAdmin())        //admin vidí veškerou historii akce
                {
                    var lisB04 = Factory.b05Workflow_HistoryBL.GetList_b04(c.pid);
                    if (lisB04.Count() > 0)
                    {
                        c.b05Comment = "["+string.Join(", ", lisB04.Select(p => p.a45Name)) + "]<br>" + c.b05Comment;
                        if (c.j03ID_Sys != Factory.CurrentUser.pid)
                        {
                            var qry = from a in lisB04 join b in lisA41 on a.a45ID equals (int)b.a45ID select a.b04ID;
                            if (qry.Count() == 0)
                            {
                                c.pid = 0;  //komentář není určený pro roli uživatele v akci
                            }
                        }
                        
                    }
                    
                }

                c.b05SQL = "";
                if (c.pid > 0)
                {
                    var qry = v.lisB05.Where(p =>p.pid > 0 && p.pid != c.pid && p.b05IsCommentOnly==false && p.b06ID == c.b06ID && BO.BAS.ObjectDateTime2String(p.DateInsert) == BO.BAS.ObjectDateTime2String(c.DateInsert) && p.b02ID_To == c.b02ID_To);
                    if (qry.Count() > 0)    //spojit více řádků dohromady, aby tabulka workflow historie byla čitelnější
                    {
                        if (c.a45ID_Nominee > 0)
                        {
                            if (qry.Where(p => p.j02ID_Nominee == c.j02ID_Nominee && p.a45ID_Nominee == c.a45ID_Nominee).Count() == 0)
                            {
                                c.b05SQL = c.a45Name + " ➝ <span style='color:red;'>" + c.b05Comment + "</span><br>";
                            }
                            c.b05SQL += string.Join("<br>", qry.Select(p => p.a45Name + " ➝ <span style='color:red;'>" + p.b05Comment + "</span>"));
                            c.b05Comment = null;
                        }
                        else
                        {
                            c.b05SQL = string.Join("<br>", qry.Select(p => p.b05Comment));
                        }
                        foreach (var cc in qry)
                        {
                            v.lisB05.First(p => p.pid == cc.pid).pid = -1;
                        }
                    }
                    else
                    {
                        if (c.a45ID_Nominee > 0)
                        {
                            c.b05SQL = c.a45Name + " ➝ <span style='color:red;'>" + c.b05Comment + "</span>";
                            c.b05Comment = "";
                        }
                    }
                    
                    if (c.b05IsCommentRestriction)
                    {
                        c.b05Comment = "<div style='background-color:#FFFFE0;'>" + c.b05Comment + "</div>";
                    }
                    

                    if (lisO27.Any(p => p.b05ID == c.pid))
                    {
                        foreach (var recO27 in lisO27.Where(p => p.b05ID == c.pid))
                        {
                            c.b05Comment += $"<br><a target='_blank' href='/FileUpload/FileDownloadInline?downloadguid={recO27.o27DownloadGUID}'>{recO27.o27OriginalFileName}</a>";
                        }
                    }
                }
            }
            v.lisB05 = v.lisB05.Where(p => p.pid > 0);
            return View(v);
        }

        //Úprava hlavičky akce
        public IActionResult Record(int pid)        //pouze EDIT existujícího záznamu
        {
            var v = new a01Record() { rec_entity = "a01", rec_pid = pid };
            if (v.rec_pid == 0)
            {
                return this.StopPage(true, "pid missing");
            }
            v.Rec = Factory.a01EventBL.Load(v.rec_pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            var perm = Factory.a01EventBL.InhalePermission(v.Rec);
            if (perm.PermValue == BO.a01EventPermissionENUM.FullAccess || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Owner || perm.PermValue == BO.a01EventPermissionENUM.ShareTeam_Leader || perm.IsRecordOwner == true)
            {
            }
            else
            {
                return this.StopPage(true, "Editace karty akce je dostupná pouze vedoucímu týmu, vlastníkovi akce nebo administrátorovi.",true);
            }
            var tg = Factory.o51TagBL.GetTagging("a01", pid);
            v.TagPids = tg.TagPids;
            v.TagNames = tg.TagNames;
            v.TagHtml = tg.TagHtml;

            v.Toolbar = new MyToolbarViewModel(v.Rec) { AllowArchive = false, AllowClone = false, AllowDelete = Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal) };
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a01Record v)
        {
            if (ModelState.IsValid)
            {
                var c = Factory.a01EventBL.Load(v.rec_pid);
                c.a01CaseCode = v.Rec.a01CaseCode;
                c.a08ID = v.Rec.a08ID;
                c.a03ID = v.Rec.a03ID;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a01EventBL.SaveA01Record(c, Factory.a10EventTypeBL.Load(c.a10ID));
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("a01", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult Info(int pid)
        {
            var v = new a01RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a01EventBL.Load(v.pid);
                if (v.Rec != null)
                {
                    v.RecLastEvent = Factory.b05Workflow_HistoryBL.LoadLast(v.pid);
                    v.RecIssuer = Factory.j02PersonBL.Load(v.Rec.j02ID_Issuer);
                    var tg = Factory.o51TagBL.GetTagging("a01", v.pid);
                    v.Rec.TagHtml = tg.TagHtml;
                    v.TagHtml = v.Rec.TagHtml;
                }

            }
            return View(v);
        }

        //Stránka akce úrazu
        public IActionResult RecPageInjury(int pid)
        {
            var v = new a01RecPageInjury() { pid = pid, Rec = new BO.a01Event() };
            v.Rec = Factory.a01EventBL.Load(v.pid);
            if (v.Rec == null)
            {
                this.Notify_RecNotSaved();
                return View(v);
            }
            v.RecA10 = Factory.a10EventTypeBL.Load(v.Rec.a10ID);
            v.RecLastEvent = Factory.b05Workflow_HistoryBL.LoadLast(v.pid);
            v.RecIssuer = Factory.j02PersonBL.Load(v.Rec.j02ID_Issuer);
            var tg = Factory.o51TagBL.GetTagging("a01", v.pid);
            v.Rec.TagHtml = tg.TagHtml;
            v.TagHtml = v.Rec.TagHtml;
            var mq = new BO.myQueryA11();
            mq.a01id = v.pid;
            v.lisA11 = Factory.a11EventFormBL.GetList(mq);

            Factory.CBL.SetUserParam("a01-RecPage-pid", v.pid.ToString());

            return View(v);

        }
        //Stránka akce       
        public IActionResult RecPage(int pid)
        {
            var v = new a01RecPage();
            v.pid = pid;
            v.NavTabs = new List<NavTab>();
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("a01-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.a01EventBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {

                    if (v.Rec.a01IsTemporary)
                    {
                        this.AddMessage("Toto je akce pro simulaci formuláře. Běheme hodiny systém akci automaticky smaže.", "info");
                    }
                    v.RecA10 = Factory.a10EventTypeBL.Load(v.Rec.a10ID);
                    if (v.RecA10.a10ViewUrl_Page != null)
                    {
                        return Redirect(Factory.a01EventBL.GetPageUrl(v.Rec));    //typ akce má jinou stránku než tuto default
                    }
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("a01-RecPage-pid", pid.ToString());
                    }

                    v.RecLastEvent = Factory.b05Workflow_HistoryBL.LoadLast(v.pid);
                    v.RecIssuer = Factory.j02PersonBL.Load(v.Rec.j02ID_Issuer);

                    RefreshNavTabs(v);
                    var tg = Factory.o51TagBL.GetTagging("a01", v.pid);
                    v.Rec.TagHtml = tg.TagHtml;
                    v.TagHtml = v.Rec.TagHtml;
                }
            }


            if (v.pid == 0)
            {
                v.Rec = new BO.a01Event();
            }

            return View(v);


        }


        //Stránka akce       
        public IActionResult RecPageHelpdesk(int pid)
        {
            var v = new a01RecPage();
            v.pid = pid;
            v.NavTabs = new List<NavTab>();
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("a01-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.a01EventBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {

                    
                    v.RecA10 = Factory.a10EventTypeBL.Load(v.Rec.a10ID);
                   
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("a01-RecPage-pid", pid.ToString());
                    }

                    v.RecLastEvent = Factory.b05Workflow_HistoryBL.LoadLast(v.pid);
                    v.RecIssuer = Factory.j02PersonBL.Load(v.Rec.j02ID_Issuer);

                    RefreshNavTabs(v);
                    var tg = Factory.o51TagBL.GetTagging("a01", v.pid);
                    v.Rec.TagHtml = tg.TagHtml;
                    v.TagHtml = v.Rec.TagHtml;
                }
            }


            if (v.pid == 0)
            {
                v.Rec = new BO.a01Event();
            }

            return View(v);


        }



        private void RefreshNavTabs(a01RecPage v)
        {
            var c = Factory.a01EventBL.LoadSummary(v.pid);
            string strBadge = null;
            //v.NavTabs.Add(new NavTab() { Name = "Historie událostí", Entity = "b05Workflow_History", Url = "/TheGrid/SlaveView?prefix=b05" });
            bool bolIAPodrizene = false;
            if (v.Rec.a01ParentID>0 && Factory.a01EventBL.GetList_a24(v.Rec.pid).Where(p => p.a46ID == 5).Count() > 0)
            {
                bolIAPodrizene = true;
            }
            if (v.Rec.a01ParentID == 0 || bolIAPodrizene)
            {
                v.NavTabs.Add(AddTab("Historie událostí", "viewHistorie", "/a01/TabHistory?pid=" + v.pid.ToString()));
            }
            if (v.RecA10.a10IsUse_A41)
            {
                strBadge = null;
                if (c.a41_count > 0)
                {
                    strBadge = c.a41_count.ToString();
                }
                if (Factory.App.Implementation == "HD")
                {
                    v.NavTabs.Add(AddTab("Řešitelé požadavku", "viewUcastnici", "/a01/TabUcastnici?pid=" + v.pid.ToString(), true, strBadge));
                }
                else
                {
                    v.NavTabs.Add(AddTab("Účastníci akce", "viewUcastnici", "/a01/TabUcastnici?pid=" + v.pid.ToString(), true, strBadge));
                }
                
            }
            if ((v.RecA10.a10IsUse_Period && v.Rec.a01ParentID == 0))
            {
                v.NavTabs.Add(AddTab("Časový plán", "viewCapacity", "/a35/TabCapacity?pid=" + v.pid.ToString()));
            }

            strBadge = null;
            if (c.a11count_nonpoll > 0 || c.a11count_poll > 0)
            {
                strBadge = c.a11count_nonpoll.ToString() + "+" + c.a11count_poll.ToString();
            }
            if (Factory.App.Implementation != "HD")
            {
                v.NavTabs.Add(AddTab("Formuláře", "viewFormulare", "/a01/TabForms?pid=" + v.pid.ToString(), true, strBadge));
            }
            
            strBadge = null;
            if (c.h04_actual > 0 || c.h04_closed > 0)
            {
                strBadge = c.h04_actual.ToString() + " + " + c.h04_closed.ToString();
            }
            v.NavTabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "/TheGrid/SlaveView?prefix=h04", true, strBadge));

            strBadge = null;
            if (c.o27_count > 0)
            {
                strBadge = c.o27_count.ToString();
            }
            if (v.Rec.a01ParentID == 0 || bolIAPodrizene)
            {
                
                v.NavTabs.Add(AddTab("Přílohy", "viewAttachments", "/a01/TabAttachments?pid=" + v.pid.ToString(), true, strBadge));
            }
            

            strBadge = null;
            if (c.a01_souvisejici > 0 || c.a01_podrizene > 0)
            {
                strBadge = c.a01_podrizene.ToString() + " + " + c.a01_souvisejici.ToString();
            }
            if (Factory.App.Implementation == "HD")
            {
                v.NavTabs.Add(AddTab("Související požadavky", "a01Event", "/a01/TabSouvisejici?pid=" + v.pid.ToString(), true, strBadge));
            }
            else
            {
                v.NavTabs.Add(AddTab("Související akce", "a01Event", "/a01/TabSouvisejici?pid=" + v.pid.ToString(), true, strBadge));
            }

            strBadge = null;
            if (c.x40_count > 0)
            {
                strBadge = c.x40_count.ToString();
            }
            v.NavTabs.Add(AddTab("OUTBOX", "x40MailQueue", "/TheGrid/SlaveView?prefix=x40", false, strBadge));

            string strDefTab = Factory.CBL.LoadUserParam("recpage-tab-a01");
            var deftab = v.NavTabs[0];

            foreach (var tab in v.NavTabs)
            {
                tab.Url += "&master_entity=a01Event&master_pid=" + v.pid.ToString();
                if (strDefTab != null && tab.Entity == strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka                    
                }
            }
            deftab.CssClass += " active";
            v.DefaultNavTabUrl = deftab.Url;
        }


        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var a01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(tgi.pathname,a01id), Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);

        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var a01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(tgi.pathname,a01id), Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var a01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(tgi.pathname,a01id), Factory, _colsProvider);

            return c.Event_HandleTheGridMenu(tgi);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var a01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(tgi.pathname,a01id), Factory, _colsProvider);

            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }
        //-----------Konec GRID událostí-------------


    }
}