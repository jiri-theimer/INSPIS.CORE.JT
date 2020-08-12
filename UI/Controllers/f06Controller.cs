using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class f06Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new f06RecPage() { pid=pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.f06FormBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("f06", pid);               
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Clone(int pid, string destname, string destexportcode)
        {
            var v = new f06Clone() { f06ID = pid,DestName=destname, DestExportCode = destexportcode };
            v.RecSource = Factory.f06FormBL.Load(v.f06ID);
            return View(v);
        }
        public BO.Result CloneRun(int pid, string destname, string destexportcode)
        {
            var rec = Factory.f06FormBL.Load(pid);           
            return Factory.f06FormBL.CloneF06(rec, destname, destexportcode);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (isclone && pid>0)
            {
                return RedirectToAction("Clone", new { pid = pid });
            }
            var v = new f06Record() { rec_pid = pid, rec_entity = "f06",UploadGuid=BO.BAS.GetGuid() };
            v.Rec = new BO.f06Form();
            RefreshState(v);
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.f06FormBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (v.Rec.f12ID > 0)
                {                    
                    v.SelectedF12Name = Factory.f12FormTypeBL.Load(v.Rec.f12ID).TreeItem;
                }
                v.lisA13 = Factory.f06FormBL.GetListA13(v.rec_pid).ToList();
                foreach (var c in v.lisA13)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }

                var mq = new BO.myQuery("j04UserRole");
                mq.f06id = v.rec_pid;
                var lis = Factory.j04UserRoleBL.GetList(mq);
                v.j04IDs = string.Join(",", lis.Select(p => p.pid));
                v.j04Names = string.Join(",", lis.Select(p => p.j04Name));
                mq = new BO.myQuery("x31Report");
                mq.f06id = v.rec_pid;
                v.x31IDs = string.Join(",", Factory.x31ReportBL.GetList(mq).Select(p => p.pid));
                v.x31Names = string.Join(",", Factory.x31ReportBL.GetList(mq).Select(p => p.x31Name));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.f06Record v, string oper, string guid, int o13id)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (v.lisA13.Where(p => p.f06ID == o13id && p.IsTempDeleted == false).Count() > 0)
                {
                    this.AddMessage("Tento typ přílohy již je v seznamu.");
                }
                else
                {
                    var recO13 = Factory.o13AttachmentTypeBL.Load(o13id);
                    var c = new BO.a13AttachmentToForm() { o13ID = recO13.pid, o13Name = recO13.o13Name, TempGuid = BO.BAS.GetGuid() };
                    v.lisA13.Add(c);
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA13.First(p => p.TempGuid == guid).IsTempDeleted = true;               
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.f06Form c = new BO.f06Form();
                if (v.rec_pid > 0) c = Factory.f06FormBL.Load(v.rec_pid);                
                c.f06Name = v.Rec.f06Name;
                c.f06RazorTemplate = v.Rec.f06RazorTemplate;                
                c.f06Hint = v.Rec.f06Hint;
                c.f06ExportCode = v.Rec.f06ExportCode;
                c.f12ID = v.Rec.f12ID;
                c.f06IsTemplate = v.Rec.f06IsTemplate;
                c.f06UserLockFlag = v.Rec.f06UserLockFlag;
                c.f06IsA37Required = v.Rec.f06IsA37Required;

                c.f06RelationWithTeacher = v.Rec.f06RelationWithTeacher;
                c.f06BindScopeQuery = v.Rec.f06BindScopeQuery;
                c.f06IsA01PeriodStrict = v.Rec.f06IsA01PeriodStrict;
                c.f06IsA01ClosedStrict = v.Rec.f06IsA01ClosedStrict;
                c.f06IsReportDialog = v.Rec.f06IsReportDialog;
                c.f06IsWorkflowDialog = v.Rec.f06IsWorkflowDialog;
                c.f06IsExportToDoc = v.Rec.f06IsExportToDoc;
                c.f06Ordinal = v.Rec.f06Ordinal;

                c.f06Linker_DestF06ID = v.Rec.f06Linker_DestF06ID;
                c.f06Linker_DestDB = v.Rec.f06Linker_DestDB;
                c.f06LinkerObject = v.Rec.f06LinkerObject;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> x31ids = BO.BAS.ConvertString2ListInt(v.x31IDs);
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                c.pid = Factory.f06FormBL.Save(c,j04ids,x31ids,v.lisA13.Where(p => p.IsTempDeleted == false).ToList());
                if (c.pid > 0)
                {
                    if (Factory.o27AttachmentBL.SaveChangesAndUpload(v.UploadGuid, 406, c.pid))
                    {
                        v.SetJavascript_CallOnLoad(c.pid);
                        return View(v);
                    }
                    
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(f06Record v)
        {
            if (v.lisA13 == null)
            {
                v.lisA13 = new List<BO.a13AttachmentToForm>();
            }
        }
    }
}