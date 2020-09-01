using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o27Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new o27Record() { rec_pid = pid, rec_entity = "o27" };
            v.Rec = new BO.o27Attachment();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o27AttachmentBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec) {AllowClone=false };
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.o27Record v)
        {

            if (ModelState.IsValid)
            {
                BO.o27Attachment c = new BO.o27Attachment();
                if (v.rec_pid > 0) c = Factory.o27AttachmentBL.Load(v.rec_pid);
                c.o27Label = v.Rec.o27Label;
                c.o27Description = v.Rec.o27Description;
                c.o27DownloadGUID = BO.BAS.GetGuid();

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.o27AttachmentBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}