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

namespace UI.Controllers
{
    public class h04Controller : BaseController
    {
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
                    return this.StopPage(true, "Na vstupu chybí ID akce.");
                }
                RefreshState(v);

                var mq = new BO.myQuery("j02");
                mq.h04id = v.rec_pid;
                v.j02IDs = string.Join(",", Factory.j02PersonBL.GetList(mq).Select(p => p.j02ID));
                mq = new BO.myQuery("j11");
                mq.h04id = v.rec_pid;
                v.j11IDs = string.Join(",", Factory.j11TeamBL.GetList(mq).Select(p => p.j11ID));

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
                c.pid = Factory.h04ToDoBL.Save(c, j02ids, j11ids, v.IsNotifyAfterSave);
                if (c.pid > 0)
                {
                    if (v.IsNotifyAfterSave) handle_notification(c.pid);

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
                c.pid = Factory.h04ToDoBL.Save(c, null, null, v.IsNotifyAfterSave);
                if (c.pid > 0)
                {
                    if (v.IsNotifyAfterSave) handle_notification(c.pid);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

            this.Notify_RecNotSaved();
            return View(v);
        }

        private void handle_notification(int pid)
        {
            var rec = Factory.h04ToDoBL.Load(pid);
            var mq = new BO.myQuery("b65");
            mq.IsRecordValid = true;
            var lis = Factory.b65WorkflowMessageBL.GetList(mq);
            string strBody = rec.h07Name + ": " + rec.h04Name + Constants.vbCrLf + Constants.vbCrLf + rec.h04Description;
            string strSubject = rec.h07Name + ": " + rec.h04Signature;

            var recH07 = Factory.h07ToDoTypeBL.Load(rec.h07ID);
            if (recH07.b65ID > 0)
            {
                var recB65 = Factory.b65WorkflowMessageBL.Load(recH07.b65ID);
                strBody = recB65.b65MessageBody;
                strSubject = recB65.b65MessageSubject;
            }
            
            mq = new BO.myQuery("j02");
            mq.h04id = pid;
            mq.IsRecordValid = true;
            var strTo = string.Join(",", Factory.j02PersonBL.GetList(mq).Select(p => p.j02Email));

            var ret=Factory.MailBL.SendMessage(0, strTo, null, strSubject, strBody, false,604,pid);

        }
    }
}