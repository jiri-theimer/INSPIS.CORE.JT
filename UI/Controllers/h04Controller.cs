using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto.Parameters;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{

    public class h04Controller : BaseController
    {
        public IActionResult RecPage(int pid)
        {
            var v = new h04RecPage() { pid = pid };
                        
            if (v.pid == 0)
            {
                v.pid = Factory.CBL.LoadUserParamInt("h04-RecPage-pid");
            }
            if (v.pid > 0)
            {
                v.Rec = Factory.h04ToDoBL.Load(v.pid);
                if (v.Rec == null)
                {
                    this.Notify_RecNotFound();
                    v.pid = 0;
                }
                else
                {
                    v.RecH07 = Factory.h07ToDoTypeBL.Load(v.Rec.h07ID);
                    v.MenuCode = v.Rec.h04Signature;
                    v.TagHtml = Factory.o51TagBL.GetTagging("h04", v.pid).TagHtml;
                    if (pid > 0)
                    {
                        Factory.CBL.SetUserParam("h04-RecPage-pid", pid.ToString());
                    }
                    v.RecA01 = Factory.a01EventBL.Load(v.Rec.a01ID);

                    v.lisJ02 = Factory.j02PersonBL.GetList(new BO.myQueryJ02() { h04id = v.Rec.pid });
                    
                }

            }

            

            return View(v);

        }

        public IActionResult Record(int pid, bool isclone, int a01id)
        {
            var v = new h04Record() { rec_pid = pid, rec_entity = "h04", a01ID = a01id };
            v.Rec = new BO.h04ToDo();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.h04ToDoBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.a01ID = v.Rec.a01ID;
                v.SelectedH07ID = v.Rec.h07ID;
                if (v.Rec.h07IsCapacityPlan && v.Rec.h04CapacityPlanFrom != null)
                {
                    v.IsDefineCapacityPlan = true;
                }                
                if (v.a01ID == 0)
                {
                    return this.StopPage(true, "Na vstupu chybí ID akce.",true);
                }
                RefreshState(v);

                var mq = new BO.myQueryJ02() { h04id = v.rec_pid };
                
                v.j02IDs = string.Join(",", Factory.j02PersonBL.GetList(mq).Select(p => p.j02ID));
                
                
                v.j11IDs = string.Join(",", Factory.j11TeamBL.GetList(new BO.myQuery("j11") { h04id = v.rec_pid }).Select(p => p.j11ID));

            }
            if (v.rec_pid == 0)
            {
                v.Rec.h04Deadline = DateTime.Today.AddDays(1).AddHours(12);
                v.IsNotifyAfterSave = true;
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.h04Record v, string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.h04ToDo c = new BO.h04ToDo() { a01ID = v.a01ID };
                if (v.rec_pid > 0) c = Factory.h04ToDoBL.Load(v.rec_pid);
                c.h07ID = v.SelectedH07ID;
                c.h05ID = v.Rec.h05ID;
                c.h04Name = v.Rec.h04Name;
                c.h04Description = v.Rec.h04Description;
                c.h04Deadline = v.Rec.h04Deadline;
                c.h04ReminderDate = v.Rec.h04ReminderDate;
                c.h04Signature = v.Rec.h04Signature;
                if (v.IsDefineCapacityPlan)
                {
                    c.h04CapacityPlanFrom = v.Rec.h04CapacityPlanFrom;
                    c.h04CapacityPlanUntil = v.Rec.h04CapacityPlanUntil;
                }

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var j02ids = BO.BAS.ConvertString2ListInt(v.j02IDs);
                var j11ids = BO.BAS.ConvertString2ListInt(v.j11IDs);
                c.pid = Factory.h04ToDoBL.Save(c, j02ids, j11ids);
                if (c.pid > 0)
                {
                    if (v.IsNotifyAfterSave) Factory.h04ToDoBL.NotifyByMail(c.pid);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(Models.Record.h04Record v)
        {
            if (v.SelectedH07ID > 0)
            {
                v.RecH07 = Factory.h07ToDoTypeBL.Load(v.SelectedH07ID);
                v.SelectedH07Name = v.RecH07.h07Name;
            }
        }

        public IActionResult MoveStatus(int pid)
        {
            var v = new h04MoveStatusViewModel() { pid = pid,IsNotifyAfterSave=true };
            v.Rec = Factory.h04ToDoBL.Load(pid);
            v.SelectedH05ID = v.Rec.h05ID;
            v.SelectedH05Name = v.Rec.h05Name;


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MoveStatus(Models.h04MoveStatusViewModel v)
        {
            v.Rec = Factory.h04ToDoBL.Load(v.pid);
            if (ModelState.IsValid)
            {
                var c = Factory.h04ToDoBL.Load(v.pid);
                c.h05ID = v.SelectedH05ID;
                c.pid = Factory.h04ToDoBL.Save(c, null, null);
                if (c.pid > 0)
                {
                    if (v.IsNotifyAfterSave) Factory.h04ToDoBL.NotifyByMail(c.pid);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        
    }
}