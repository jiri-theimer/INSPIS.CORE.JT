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
    public class a10Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new a10RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a10EventTypeBL.Load(v.pid);
                var tg = Factory.o51TagBL.GetTagging("a10", pid);
                v.TagHtml = tg.TagHtml;

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a10Record() { rec_pid = pid, rec_entity = "a10" };
            v.Rec = new BO.a10EventType();
            RefreshState(v);
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a10EventTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.lisA20 = Factory.a10EventTypeBL.GetListA20(v.rec_pid).ToList();
               
                
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTupCiselnik(v,BO.j03AdminRoleValueFlagEnum.akce_er);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a10Record v, string oper, int j04id, string guid)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (j04id == 0)
                {
                    return View(v);
                }
                if (v.lisA20.Where(p => p.j04ID == j04id && p.IsTempDeleted == false).Count() > 0)
                {
                    this.AddMessage("Tato role již je v seznamu.");
                }
                else
                {
                    var recJ04 = Factory.j04UserRoleBL.Load(j04id);
                    var c = new BO.a20EventType_UserRole_PersonalPage() { j04ID = recJ04.pid, j04Name = recJ04.j04Name, TempGuid = BO.BAS.GetGuid() };
                    v.lisA20.Add(c);
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA20.First(p => p.TempGuid == guid).IsTempDeleted = true;                
                return View(v);
            }

            if (ModelState.IsValid)
            {
                BO.a10EventType c = new BO.a10EventType();
                if (v.rec_pid > 0) c = Factory.a10EventTypeBL.Load(v.rec_pid);
                c.b01ID = v.Rec.b01ID;
                c.a10Name = v.Rec.a10Name;
                c.a10Aspx_Insert = v.Rec.a10Aspx_Insert;
                c.a10Aspx_Framework = v.Rec.a10Aspx_Framework;
                c.a10ViewUrl_Page = v.Rec.a10ViewUrl_Page;
                c.a10ViewUrl_Insert = v.Rec.a10ViewUrl_Insert;
                c.a10Ident = v.Rec.a10Ident;
                c.a10Description = v.Rec.a10Description;

                c.a10IsUse_A08 = v.Rec.a10IsUse_A08;
                c.a10IsUse_A03 = v.Rec.a10IsUse_A03;
                c.a10IsUse_A41 = v.Rec.a10IsUse_A41;
                c.a10IsUse_CaseCode = v.Rec.a10IsUse_CaseCode;
                c.a10IsUse_DeadLine = v.Rec.a10IsUse_DeadLine;
                c.a10IsUse_A41_J11 = v.Rec.a10IsUse_A41_J11;
                c.a10IsUse_A41_Leader = v.Rec.a10IsUse_A41_Leader;
                c.a10IsUse_Name = v.Rec.a10IsUse_Name;

                c.a10IsUse_Period = v.Rec.a10IsUse_Period;
                c.a10IsUse_Poll = v.Rec.a10IsUse_Poll;
                c.a10IsEpis2 = v.Rec.a10IsEpis2;
                c.a10IsSupportCloning = v.Rec.a10IsSupportCloning;

                c.a10OneSchoolInstanceLimit = v.Rec.a10OneSchoolInstanceLimit;
                c.a10IsUse_K01 = v.Rec.a10IsUse_K01;
                c.a10IsUse_ReChangeForms = v.Rec.a10IsUse_ReChangeForms;

                c.a10Linker_a10ID = v.Rec.a10Linker_a10ID;
                c.a10Linker_a08ID = v.Rec.a10Linker_a08ID;
                c.a10LinkerDB = v.Rec.a10LinkerDB;
                c.a10CoreFlag = v.Rec.a10CoreFlag;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> a08ids = BO.BAS.ConvertString2ListInt(v.a08IDs);
                
                c.pid = Factory.a10EventTypeBL.Save(c, a08ids,v.lisA20.Where(p=>p.IsTempDeleted==false).ToList());
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }


        private void RefreshState(a10Record v)
        {
            if (v.lisA20 == null)
            {
                v.lisA20 = new List<BO.a20EventType_UserRole_PersonalPage>();
            }
        }
    }
}