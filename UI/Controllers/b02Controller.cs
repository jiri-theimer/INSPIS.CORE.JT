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
    public class b02Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new b02RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.b02WorkflowStatusBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("b02", pid);
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone, int b01id)
        {
            var v = new b02Record() { rec_pid = pid, rec_entity = "b02", b01ID=b01id };
            v.Rec = new BO.b02WorkflowStatus();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b02WorkflowStatusBL.Load(v.rec_pid);
                v.b01ID = v.Rec.b01ID;
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.lisB07 = Factory.b02WorkflowStatusBL.GetListB07(v.rec_pid).ToList();
                foreach (var c in v.lisB07)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                v.lisB03 = Factory.b02WorkflowStatusBL.GetListB03(v.rec_pid).ToList();
                foreach (var c in v.lisB03)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                v.lisB10 = Factory.b02WorkflowStatusBL.GetListB10(v.rec_pid).ToList();
                foreach (var c in v.lisB10)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.WorkflowDesigner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b02Record v, string oper, string guid)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add_b07")
            {                
                var c = new BO.b07WorkflowMessageToStatus() { TempGuid = BO.BAS.GetGuid() };
                v.lisB07.Add(c);
                return View(v);
            }
            if (oper == "delete_b07")
            {
                v.lisB07.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "add_b03")
            {
                var c = new BO.b03WorkflowReceiverToStatus() { TempGuid = BO.BAS.GetGuid() };
                v.lisB03.Add(c);
                return View(v);
            }
            if (oper == "delete_b03")
            {
                v.lisB03.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "add_b10")
            {
                var c = new BO.b10WorkflowCommandCatalog_Binding() { TempGuid = BO.BAS.GetGuid() };
                v.lisB10.Add(c);
                return View(v);
            }
            if (oper == "delete_b10")
            {
                v.lisB10.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.b02WorkflowStatus c = new BO.b02WorkflowStatus();
                if (v.rec_pid > 0) c = Factory.b02WorkflowStatusBL.Load(v.rec_pid);
                c.b02Name = v.Rec.b02Name;
                c.b02Ident = v.Rec.b02Ident;
                c.b01ID = v.b01ID;
                if (v.Rec.b02Color == "#000000")
                {
                    v.Rec.b02Color = "";
                }                
                c.b02Color = v.Rec.b02Color;
                c.b02Order = v.Rec.b02Order;
                c.b02IsDefaultStatus = v.Rec.b02IsDefaultStatus;
                c.b02IsHoldStatus = v.Rec.b02IsHoldStatus;
                c.b02IsSeparateTab = v.Rec.b02IsSeparateTab;
                c.b02IsDurationSLA = v.Rec.b02IsDurationSLA;
                c.b02IsCommentForbidden = v.Rec.b02IsCommentForbidden;
                c.b02TimeOut_Total = v.Rec.b02TimeOut_Total;
                c.b02TimeOut_SLA = v.Rec.b02TimeOut_SLA;
                c.b02Message4UIFT = v.Rec.b02Message4UIFT;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.b02WorkflowStatusBL.Save(c,v.lisB07.Where(p=>p.IsTempDeleted==false).ToList(), v.lisB03.Where(p => p.IsTempDeleted == false).ToList(), v.lisB10.Where(p => p.IsTempDeleted == false).ToList());
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(b02Record v)
        {
            v.RecB01 = Factory.b01WorkflowTemplateBL.Load(v.b01ID);
            if (v.lisB07 == null)
            {
                v.lisB07 = new List<BO.b07WorkflowMessageToStatus>();
            }
            if (v.lisB03 == null)
            {
                v.lisB03 = new List<BO.b03WorkflowReceiverToStatus>();
            }
            if (v.lisB10 == null)
            {
                v.lisB10 = new List<BO.b10WorkflowCommandCatalog_Binding>();
            }
            foreach(var c in v.lisB10.Where(p=>p.b09ID>0))
            {
                c.b09ParametersCount = Factory.FBL.LoadB09(c.b09ID).b09ParametersCount;
            }
        }
    }
}