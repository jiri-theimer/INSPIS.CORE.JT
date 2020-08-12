using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j40Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j40Record() { rec_pid = pid, rec_entity = "j40" };
            v.Rec = new BO.j40MailAccount();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.MailBL.LoadJ40(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.Rec.j40SmtpPassword = null;
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
        public IActionResult Record(j40Record v)
        {
            if (ModelState.IsValid)
            {
                BO.j40MailAccount c = new BO.j40MailAccount();
                if (v.rec_pid > 0) c = Factory.MailBL.LoadJ40(v.rec_pid);
                c.j02ID = v.Rec.j02ID;
                c.j40UsageFlag = v.Rec.j40UsageFlag;
                c.j40SmtpHost = v.Rec.j40SmtpHost;
                c.j40SmtpPort = v.Rec.j40SmtpPort;
                c.j40SmtpName = v.Rec.j40SmtpName;
                c.j40SmtpEmail = v.Rec.j40SmtpEmail;
                c.j40SmtpUsePersonalReply = v.Rec.j40SmtpUsePersonalReply;
                c.j40SmtpLogin = v.Rec.j40SmtpLogin;
                if (String.IsNullOrEmpty(v.Rec.j40SmtpPassword) == false)
                {
                    c.j40SmtpPassword = v.Rec.j40SmtpPassword;
                }

                c.j40SmtpUseDefaultCredentials = v.Rec.j40SmtpUseDefaultCredentials;
                c.j40SmtpEnableSsl = v.Rec.j40SmtpEnableSsl;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid= Factory.MailBL.SaveJ40(c);
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