using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class workflowController : BaseController
    {
        public IActionResult Dialog(int pid)
        {
            var v = new WorkflowDialogViewMode() { pid = pid,UploadGuid=BO.BAS.GetGuid() };

            RefreshStateDialog(v);
            return View(v);
        }


        private void RefreshStateDialog(WorkflowDialogViewMode v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.pid);
            v.PermA01 = Factory.a01EventBL.InhalePermission(v.RecA01);

            var mq = new BO.myQuery("a41");
            mq.a01id = v.pid;
            mq.MyRecordsDisponible = true;
            mq.CurrentUser = Factory.CurrentUser;
            v.lisA41 = Factory.a41PersonToEventBL.GetList(mq);

            mq = new BO.myQuery("b06");
            mq.b02id = v.RecA01.b02ID;
            if (mq.b02id == 0) mq.b02id = -1;   //akce nemá nahozený workflow stav!!
            var lisB06 = Factory.b06WorkflowStepBL.GetList(mq).OrderBy(p => p.b06Order);

            v.lisB06 = new List<BO.b06WorkflowStep>();
            foreach (var c in lisB06)
            {
                if (IsStepAvailable4Me(c, v))
                {
                    if (c.b02ID_Target > 0) c.b06Name += " -> " + c.TargetStatus;
                    v.lisB06.Add(c);
                }
            }
            bool bolIsCommentForbidden = false;
            if (v.RecA01.b02ID > 0)
            {
                bolIsCommentForbidden = Factory.b02WorkflowStatusBL.Load(v.RecA01.b02ID).b02IsCommentForbidden;
            }
            if (bolIsCommentForbidden == false)
            {
                var c = new BO.b06WorkflowStep() { b06ID = 0, pid = 0, b06Name = "Doplnit pouze komentář nebo nahrát přílohu" };
                v.lisB06.Add(c);
            }

        }

        private bool IsStepAvailable4Me(BO.b06WorkflowStep rec, WorkflowDialogViewMode v)
        {
            var lisB08 = Factory.b06WorkflowStepBL.GetListB08(rec.pid);
            var mq = new BO.myQuery("a39");
            mq.a03id = v.RecA01.a03ID;
            var lisA39 = Factory.a39InstitutionPersonBL.GetList(mq).Where(p => p.j02ID == Factory.CurrentUser.j02ID);

            foreach (var c in lisB08)
            {
                switch (c.a45ID)
                {
                    case 1: //člen týmu
                        if (v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_Member)
                        {
                            return true;
                        }
                        break;
                    case 2: //vedoucí týmu
                        if (v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_Leader)
                        {
                            return true;
                        }
                        break;
                    case 6: //přizvaná osoba
                        if (v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_InvitedPerson)
                        {
                            return true;
                        }
                        break;
                    case 3: //zadavatel
                        if (v.PermA01.PermValue == BO.a01EventPermissionENUM.HD_Requestor)
                        {
                            return true;
                        }
                        break;
                    case 5: //vlastník
                        if (v.PermA01.PermValue == BO.a01EventPermissionENUM.ShareTeam_Owner)
                        {
                            return true;
                        }
                        break;
                }
                if (c.a45ID > 0 && c.a45ID < 10)
                {
                    if (v.lisA41.Where(p => Convert.ToInt32(p.a45ID) == c.a45ID).Count() > 0)
                    {
                        return true;
                    }

                }
                if (c.a45ID == 98 && c.b06ID_NomineeSource > 0)   //nominovaný v jiném kroku
                {
                    if (v.lisA41.Where(p => p.b06ID_NomineeSource == c.b06ID_NomineeSource).Count() > 0)
                    {
                        return true;
                    }
                }



                if (c.a45ID == 0 && c.j04ID == Factory.CurrentUser.j04ID) //okruh příjemců kroků podle aplikační role
                {
                    var recJ04 = Factory.j04UserRoleBL.Load(c.j04ID);
                    if (recJ04.j04RelationFlag == BO.j04RelationFlagEnum.NoRelation) return true;
                    if (recJ04.j04RelationFlag == BO.j04RelationFlagEnum.A03 && lisA39.Count() > 0) return true;  //osoba je ze školy akce
                    if (recJ04.j04RelationFlag == BO.j04RelationFlagEnum.A05 && v.RecA01.a05ID == Factory.CurrentUser.a05ID) return true; //osoba je z regionu školy
                }

                if (lisA39.Count() > 0 && c.j04ID > 0 && c.j04ID == lisA39.First().j04ID_Explicit)
                {
                    return true;    //osoba má jinou globální a školní roli
                }

            }
            return false;
        }
    }
}