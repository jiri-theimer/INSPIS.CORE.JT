using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x31Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x31Record() { rec_pid = pid, rec_entity = "x31",UploadGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.x31Report();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x31ReportBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("x32ReportType");
                mq.x31id = v.rec_pid;
                var lis = Factory.x32ReportTypeBL.GetList(mq);
                v.x32IDs = string.Join(",", lis.Select(p => p.pid));
                v.x32Names = string.Join(",", lis.Select(p => p.x32Name));
                mq = new BO.myQuery("j04UserRole");
                mq.x31id = v.rec_pid;                
                v.j04IDs = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.pid));
                v.j04Names = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.j04Name));

                

            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState(v);
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x31Record v)
        {
            RefreshState(v);
            if (ModelState.IsValid)
            {
                BO.x31Report c = new BO.x31Report();
                if (v.rec_pid > 0) c = Factory.x31ReportBL.Load(v.rec_pid);
                c.x29ID = v.Rec.x29ID;
                c.x31Name = v.Rec.x31Name;
                c.x31PID = v.Rec.x31PID;
                c.x31Description = v.Rec.x31Description;
                c.x31Is4SingleRecord = v.Rec.x31Is4SingleRecord;
                c.x31ReportFormat = v.Rec.x31ReportFormat;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> x32ids = BO.BAS.ConvertString2ListInt(v.x32IDs);
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                c.pid = Factory.x31ReportBL.Save(c, x32ids,j04ids);
                if (c.pid > 0)
                {
                    if (Factory.o27AttachmentBL.SaveSingleUpload(v.UploadGuid, 931, c.pid))
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }

                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(x31Record v)
        {
            if (v.rec_pid > 0)
            {
                var mq = new BO.myQuery("o27");
                mq.recpid = v.rec_pid;
                mq.x29id = 931;
                var lisO27 = Factory.o27AttachmentBL.GetList(mq, null);
                if (lisO27.Count() > 0)
                {
                    v.RecO27 = lisO27.First();
                }
            }
            
        }
    }
}