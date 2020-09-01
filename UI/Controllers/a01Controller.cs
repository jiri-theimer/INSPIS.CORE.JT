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
        public IActionResult AddAttachment(int pid)
        {
            var v = new a01AddAttachment() { a01id = pid,UploadGuid=BO.BAS.GetGuid() };
            if (v.a01id == 0)
            {
                return this.StopPageSubform("pid is missing");
            }

            RefreshStateAddAttachment(v);
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAttachment(a01AddAttachment v)
        {
            RefreshStateAddAttachment(v);
            if (ModelState.IsValid)
            {
                bool b=Factory.o27AttachmentBL.SaveChangesAndUpload(v.UploadGuid, 101, v.RecA01.pid);
                
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
            var mq = new BO.myQuery("a11");
            mq.a01id = v.pid;
            v.lisA11 = Factory.a11EventFormBL.GetList(mq);
            return View(v);
        }
        public IActionResult TabUcastnici(int pid)
        {
            var v = new a01TabUcastnici() { pid = pid };
            if (v.pid == 0)
            {
                return this.StopPageSubform("pid is missing");
            }
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            var mq = new BO.myQuery("a41");
            mq.a01id = v.pid;
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
            var mq = new BO.myQuery("o27");
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
            foreach (var c in v.lisB05)
            {
                c.b05SQL = "";
                if (c.pid > 0)
                {
                    var qry = v.lisB05.Where(p => p.pid > 0 && p.pid != c.pid && p.b06ID == c.b06ID && BO.BAS.ObjectDateTime2String(p.DateInsert) == BO.BAS.ObjectDateTime2String(c.DateInsert) && p.b02ID_To == c.b02ID_To);
                    if (qry.Count() > 0)
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
                return this.StopPage(true, "Editace karty akce je dostupná pouze vedoucímu týmu, vlastníkovi akce nebo administrátorovi.");
            }
            var tg = Factory.o51TagBL.GetTagging("a01", pid);
            v.TagPids = tg.TagPids;
            v.TagNames = tg.TagNames;
            v.TagHtml = tg.TagHtml;

            v.Toolbar = new MyToolbarViewModel(v.Rec) { AllowArchive = false, AllowClone = false };
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
                    this.Notify_RecNotSaved();
                    v.pid = 0;
                }
                else
                {
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("a01-RecPage-pid", pid.ToString());
                    }
                    v.RecA10 = Factory.a10EventTypeBL.Load(v.Rec.a10ID);
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
            //v.NavTabs.Add(new NavTab() { Name = "Historie událostí", Entity = "b05Workflow_History", Url = "/TheGrid/SlaveView?prefix=b05" });
            if (v.Rec.a01ParentID == 0)
            {
                v.NavTabs.Add(AddTab("Historie událostí", "viewHistorie", "/a01/TabHistory?pid=" + v.pid.ToString()));
            }            
            if (v.RecA10.a10IsUse_A41)
            {
                v.NavTabs.Add(AddTab("Účastníci akce", "viewUcastnici", "/a01/TabUcastnici?pid=" + v.pid.ToString()));
            }
            if (v.RecA10.a10IsUse_Period && v.Rec.a01ParentID==0)
            {
                v.NavTabs.Add(AddTab("Časový plán", "viewCapacity", "/a35/TabCapacity?pid=" + v.pid.ToString()));
            }            
            v.NavTabs.Add(AddTab("Formuláře", "viewFormulare", "/a01/TabForms?pid=" + v.pid.ToString()));
            v.NavTabs.Add(AddTab("Úkoly/Lhůty", "h04ToDo", "/TheGrid/SlaveView?prefix=h04"));
            if (v.Rec.a01ParentID == 0)
            {
                v.NavTabs.Add(AddTab("Přílohy", "viewAttachments", "/a01/TabAttachments?pid=" + v.pid.ToString()));
            }
            
            v.NavTabs.Add(AddTab("Související akce", "a01Event", "/TheGrid/SlaveView?prefix=a01"));

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


    }
}