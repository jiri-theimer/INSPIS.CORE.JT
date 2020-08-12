using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class b06Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone, int b02id)
        {
            var v = new b06Record() { rec_pid = pid, rec_entity = "b06", b02ID = b02id };
            v.Rec = new BO.b06WorkflowStep();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.b06WorkflowStepBL.Load(v.rec_pid);
                v.b02ID = v.Rec.b02ID;
                v.b01ID = v.Rec.b01ID;
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }                
                
                var mq = new BO.myQuery("o13");
                mq.b06id = v.rec_pid;                
                v.o13IDs = string.Join(",", Factory.o13AttachmentTypeBL.GetList(mq).Select(p => p.pid));
                v.o13Names = string.Join(",", Factory.o13AttachmentTypeBL.GetList(mq).Select(p => p.o13Name));

                mq = new BO.myQuery("j04");
                mq.b06id = v.rec_pid;
                mq.param1 = "b12";
                v.b12_j04IDs = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.pid));
                v.b12_j04Names = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.j04Name));

                v.lisB13 = Factory.b06WorkflowStepBL.GetListB13(v.rec_pid).ToList();
                foreach (var c in v.lisB13)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                v.lisB11 = Factory.b06WorkflowStepBL.GetListB11(v.rec_pid).ToList();
                foreach (var c in v.lisB11)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                
                v.lisB10 = Factory.b06WorkflowStepBL.GetListB10(v.rec_pid).ToList();
                foreach (var c in v.lisB10)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                var lisB08 = Factory.b06WorkflowStepBL.GetListB08(v.rec_pid);
                v.b08_a45IDs = string.Join(",", lisB08.Where(p=>p.a45ID>0).Select(p => p.a45ID));
                v.b08_a45Names = string.Join(",", lisB08.Where(p => p.a45ID > 0).Select(p => p.a45Name));
                v.b08_j04IDs = string.Join(",", lisB08.Where(p=>p.j04ID>0).Select(p => p.j04ID));
                v.b08_j04Names = string.Join(",", lisB08.Where(p => p.j04ID > 0).Select(p => p.j04Name));
                if (lisB08.Where(p => p.j11ID > 0).Count()>0)
                {
                    v.j11ID_b08_99 = lisB08.Where(p => p.j11ID > 0).First().j11ID;
                    v.j11Name_b08_99 = lisB08.Where(p => p.j11ID > 0).First().j11Name;
                }
                if (lisB08.Where(p => p.b06ID_NomineeSource > 0).Count() > 0)
                {
                    v.b06ID_NomineeSource = lisB08.Where(p => p.b06ID_NomineeSource > 0).First().b06ID_NomineeSource;
                    v.b06Name_NomineeSource = Factory.b06WorkflowStepBL.Load(v.b06ID_NomineeSource).b06Name;
                }

            }
            RefreshState(v);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.b06Record v, string oper, string guid,int f06id)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add_b11")
            {
                var c = new BO.b11WorkflowMessageToStep() { TempGuid = BO.BAS.GetGuid() };
                v.lisB11.Add(c);
                return View(v);
            }
            if (oper == "delete_b11")
            {
                v.lisB11.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }            
           
            if (oper == "add_b10")
            {
                var c = new BO.b10WorkflowCommandCatalog_Binding() { TempGuid = BO.BAS.GetGuid() };
                v.lisB10.Add(c);
                return View(v);
            }
            if (oper == "add_b13")
            {
                var c = new BO.b13WorkflowRequiredFormsToStep() { TempGuid = BO.BAS.GetGuid(),f06ID=f06id };
                if (f06id > 0)
                {
                    c.f06Name = Factory.f06FormBL.Load(f06id).f06Name;
                    v.SelectedF06ID = 0;v.SelectedForm = null;
                }                
                v.lisB13.Add(c);
                return View(v);
            }
            if (oper == "delete_b13")
            {
                v.lisB13.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "delete_b10")
            {
                v.lisB10.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.b06WorkflowStep c = new BO.b06WorkflowStep();
                if (v.rec_pid > 0) c = Factory.b06WorkflowStepBL.Load(v.rec_pid);
                c.b06Name = v.Rec.b06Name;
                c.b06IsManualStep = v.Rec.b06IsManualStep;
                c.b02ID = v.b02ID;
                c.b02ID_Target = v.Rec.b02ID_Target;
                c.a45ID_NomineeTarget = v.Rec.a45ID_NomineeTarget;
                c.x26ID_Nominee_J02 = v.Rec.x26ID_Nominee_J02;
                c.x26ID_Nominee_J11 = v.Rec.x26ID_Nominee_J11;

                c.b06RunSQL = v.Rec.b06RunSQL;
                c.b06ValidateBeforeRunSQL = v.Rec.b06ValidateBeforeRunSQL;
                c.b06ValidateBeforeErrorMessage = v.Rec.b06ValidateBeforeErrorMessage;
                c.b06ValidateAutoMoveSQL = v.Rec.b06ValidateAutoMoveSQL;

                c.b06Order = v.Rec.b06Order;

                c.b06IsCommentRequired = v.Rec.b06IsCommentRequired;
                c.b06IsEscalation_Timeout_Total = v.Rec.b06IsEscalation_Timeout_Total;
                c.b06IsEscalation_Timeout_SLA = v.Rec.b06IsEscalation_Timeout_SLA;
                c.b06IsManualStep = v.Rec.b06IsManualStep;
                c.b06IsNominee = v.Rec.b06IsNominee;
                c.b06IsNomineeRequired = v.Rec.b06IsNomineeRequired;
                c.b06IsHistoryAllowedToAll = v.Rec.b06IsHistoryAllowedToAll;
                c.b06IsFormAutoLock = v.Rec.b06IsFormAutoLock;
                c.b06IsFormValidationRequired = v.Rec.b06IsFormValidationRequired;
                c.b06IsAttachmentTestRequired = v.Rec.b06IsAttachmentTestRequired;
                c.b06IsAutoRun_Missing_Form = v.Rec.b06IsAutoRun_Missing_Form;
                c.b06IsAutoRun_Missing_Attachment = v.Rec.b06IsAutoRun_Missing_Attachment;
                c.b06IsRunOneInstanceOnly = v.Rec.b06IsRunOneInstanceOnly;

                c.b06IsToDoGeneration = v.Rec.b06IsToDoGeneration;
                c.b06ToDo_Name = v.Rec.b06ToDo_Name;
                c.b06ToDo_DeadlineField = v.Rec.b06ToDo_DeadlineField;
                c.b06ToDo_DeadlineMove = v.Rec.b06ToDo_DeadlineMove;
                c.b06ToDo_h07ID = v.Rec.b06ToDo_h07ID;
                c.b06ToDo_ReceiverFlag = v.Rec.b06ToDo_ReceiverFlag;
                c.b06ToDo_Description = v.Rec.b06ToDo_Description;
                c.b06ToDo_CapacityPlanFromField = v.Rec.b06ToDo_CapacityPlanFromField;
                c.b06ToDo_CapacityPlanFromMove = v.Rec.b06ToDo_CapacityPlanFromMove;
                c.b06ToDo_CapacityPlanUntilField = v.Rec.b06ToDo_CapacityPlanUntilField;
                c.b06ToDo_CapacityPlanUntilMove = v.Rec.b06ToDo_CapacityPlanUntilMove;
                c.b06ToDo_IsSendMail = v.Rec.b06ToDo_IsSendMail;
               

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                List<int> o13ids = BO.BAS.ConvertString2ListInt(v.o13IDs);
                List<int> b12_j04ids = BO.BAS.ConvertString2ListInt(v.b12_j04IDs);
                List<int> b12_a45ids = BO.BAS.ConvertString2ListInt(v.b12_a45IDs);
                var lisB08 = new List<BO.b08WorkflowReceiverToStep>();
                if (v.b08_j04IDs != null)
                {
                    foreach(var j04id in BO.BAS.ConvertString2ListInt(v.b08_j04IDs))
                    {
                        lisB08.Add(new BO.b08WorkflowReceiverToStep() { j04ID = j04id });
                    }
                }
                if (v.b08_a45IDs != null)
                {
                    foreach (var a45id in BO.BAS.ConvertString2ListInt(v.b08_a45IDs))
                    {
                        var cB08 = new BO.b08WorkflowReceiverToStep() { a45ID = a45id };
                        if (a45id == 99)
                        {
                            cB08.j11ID = v.j11ID_b08_99;
                        }
                        if (a45id == 98)
                        {
                            cB08.b06ID_NomineeSource = v.b06ID_NomineeSource;
                        }
                        lisB08.Add(cB08);
                    }
                }

                c.pid = Factory.b06WorkflowStepBL.Save(c,lisB08, v.lisB11.Where(p => p.IsTempDeleted == false).ToList(),v.lisB13.Where(p=>p.IsTempDeleted==false).ToList(),o13ids,b12_j04ids,b12_a45ids, v.lisB10.Where(p => p.IsTempDeleted == false).ToList());
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(b06Record v)
        {
            v.RecB02 = Factory.b02WorkflowStatusBL.Load(v.b02ID);
            
            if (v.lisB11 == null)
            {
                v.lisB11 = new List<BO.b11WorkflowMessageToStep>();
            }
            if (v.lisB13 == null)
            {
                v.lisB13 = new List<BO.b13WorkflowRequiredFormsToStep>();
            }
            if (v.lisB10 == null)
            {
                v.lisB10 = new List<BO.b10WorkflowCommandCatalog_Binding>();
            }
            foreach (var c in v.lisB10.Where(p => p.b09ID > 0))
            {
                c.b09ParametersCount = Factory.FBL.LoadB09(c.b09ID).b09ParametersCount;
            }
        }
    }
}