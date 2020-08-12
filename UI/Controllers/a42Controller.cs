using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class a42Controller : BaseController
    {
        public IActionResult CreatePre()
        {
            var v = new a42CreateViewModel();
            var mq = new BO.myQuery("a10");
            mq.IsRecordValid = true;
            v.lisA10 = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10Aspx_Insert !=null && ( p.a10Aspx_Insert.ToLower() == "a01_create_res.aspx" || p.a10Aspx_Insert.ToLower().StartsWith("/a42/create")));
           
            return View(v);
        }
        public IActionResult CompleteJob(int a42id)
        {
            var v = new a42CompleteJob() { a42ID = a42id};            
            v.Rec = Factory.a42QesBL.Load(v.a42ID);
            if (v.Rec == null)
            {
                return this.StopPage(false, "Na vstupu chybí INEZ pid.");
            }
            var lisP85 = Factory.p85TempboxBL.GetList(v.Rec.a42JobGuid,false,"a03").Where(p=>p.p85IsFinished==false).Take(10);
            var lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Rec.a42UploadGuid);
            
            foreach(var recTemp in lisP85)
            {
                var recA01 = new BO.a01Event() {a01IsTemporary=true, a03ID = recTemp.p85OtherKey1,a10ID=recTemp.p85OtherKey2,a08ID = v.Rec.a08ID,a01DateFrom=v.Rec.a42DateFrom,a01DateUntil=v.Rec.a42DateUntil };

                Factory.a01EventBL.Create(recA01,true,null,null,null,null);
            }
            if (v.CurrentA03ID > 0)
            {
                v.CurrentRecA03 = Factory.a03InstitutionBL.Load(v.CurrentA03ID);
            }
            
            return View(v);
        }
        public IActionResult CreateTempFinish(int a42id)
        {           
            var v = new a42CreateTempFinishViewModel() { a42ID = a42id};
            v.Rec = Factory.a42QesBL.Load(a42id);
            v.JobGuid = v.Rec.a42JobGuid;
            v.UploadGuid = v.Rec.a42UploadGuid;
            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Rec.a42UploadGuid);
            v.lisP85 = Factory.p85TempboxBL.GetList(v.Rec.a42JobGuid);
            v.a03Count = v.lisP85.Where(p => p.p85Prefix == "a03").Count();
            v.MessageSubject = v.lisP85.Where(p => p.p85Prefix == "x40").First().p85FreeText01;
            v.MessageBody = v.lisP85.Where(p => p.p85Prefix == "x40").First().p85Message;

            foreach (var c in v.lisP85.Where(p => p.p85Prefix == "a03"))
            {
                if (v.lisP85.Where(p=>p.p85Prefix=="x43" && p.p85OtherKey4 == c.p85OtherKey1).Count() == 0)
                {
                    v.a03CountNoEmail += 1;
                }
            }
            
            return View(v);
        }
        [HttpPost]        
        public IActionResult CreateTempFinish(a42CreateTempFinishViewModel v)
        {            
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.MessageBody) == true || string.IsNullOrEmpty(v.MessageSubject) == true)
                {
                    this.AddMessage("Chybí předmět nebo obsah zprávy.");
                    v.Rec = Factory.a42QesBL.Load(v.a42ID);
                    v.JobGuid = v.Rec.a42JobGuid;
                    v.UploadGuid = v.Rec.a42UploadGuid;
                    v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Rec.a42UploadGuid);
                    v.lisP85 = Factory.p85TempboxBL.GetList(v.Rec.a42JobGuid);
                    v.a03Count = v.lisP85.Where(p => p.p85Prefix == "a03").Count();
                    return View(v);
                }
                BO.a42Qes c = Factory.a42QesBL.Load(v.a42ID);
                var lisP85=Factory.p85TempboxBL.GetList(c.a42JobGuid, false, "x40");
                var recTemp = Factory.p85TempboxBL.Load(lisP85.First().pid);
                recTemp.p85Message = v.MessageBody;
                recTemp.p85FreeText01 = v.MessageSubject;
                Factory.p85TempboxBL.Save(recTemp);
                return RedirectToAction("CompleteJob", new { a42id = c.pid});

            }
            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult Create(int a10id)
        {
            if (a10id == 0)
            {
                return RedirectToAction("CreatePre");
            }
            var v = new a42CreateViewModel() { a10ID = a10id, UploadGuid=BO.BAS.GetGuid(),MessageReceiverFlag=BO.RecipientFlagEnum.DirectorAddress };
            v.Rec = new BO.a42Qes();
            var mq = new BO.myQuery("a10");
            mq.IsRecordValid = true;
            v.lisA10 = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10Aspx_Insert !=null && ( p.a10Aspx_Insert.ToLower() == "a01_create_res.aspx" || p.a10Aspx_Insert.ToLower().StartsWith("/a42/create")));
            if (v.lisA10.Where(p => p.pid == v.a10ID).Count()== 0)
            {
                return RedirectToAction("CreatePre");
            }
            RefreshStateCreate(v);

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(a42CreateViewModel v, string oper,int b65id,string f06ids)
        {
            RefreshStateCreate(v);
            if (oper== "change_a08id")
            {
                v.lisPollA12 = Factory.a08ThemeBL.GetListA12(v.a08ID).Where(p => p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.DirectOnly || p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.None).ToList();
                return View(v);
            }
            
            if (oper== "postback")
            {
                return View(v);
            }
            if (oper== "change_b65id")
            {
                var recB65 = Factory.b65WorkflowMessageBL.Load(b65id);
                v.MessageSubject = recB65.b65MessageSubject;
                v.MessageBody = recB65.b65MessageBody;
                return View(v);
            }
            
            
            if (ModelState.IsValid)
            {
                BO.a42Qes c = new BO.a42Qes() { a08ID = v.a08ID, a42DateFrom = v.a42DateFrom, a42DateUntil = v.a42DateUntil, a42Name = v.Rec.a42Name,a42Description=v.Rec.a42Description,a42JobGuid= BO.BAS.GetGuid(),a42UploadGuid=v.UploadGuid };
                BO.a01Event recA01 = new BO.a01Event() { a10ID = v.a10ID, a08ID = v.a08ID };
                List<int> a03ids = BO.BAS.ConvertString2ListInt(v.a03IDs);
                var recX40 = new BO.x40MailQueue() { x40Subject = v.MessageSubject, x40Body = v.MessageBody, x40RecipientFlag = v.MessageReceiverFlag };
                
                c.pid = Factory.a42QesBL.PrepareTempData(c,recA01,v.lisRightA12,a03ids,recX40);
                if (c.pid > 0)
                {
                    //Dávka je připravena k vygenerování akcí

                    return RedirectToAction("CreateTempFinish", new { a42id = c.pid });
                }
            }
            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateCreate(Models.a42CreateViewModel v)
        {
            if (v.a10ID>0)
            {
                v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);                
            }
            if (v.a08ID > 0)
            {
                v.RecA08 = Factory.a08ThemeBL.Load(v.a08ID);
                var lisA12 = Factory.a08ThemeBL.GetListA12(v.RecA08.pid);
                
                v.lisLeftA12= lisA12.Where(p=>p.f06BindScopeQuery==BO.f06BindScopeQueryENUM.DirectOnly || p.f06BindScopeQuery==BO.f06BindScopeQueryENUM.None).ToList();
                if (v.lisPollA12 == null)
                {
                    v.lisPollA12 = v.lisLeftA12;
                }
                v.lisRightA12 = new List<BO.a12ThemeForm>();
                if (string.IsNullOrEmpty(v.SelectedA12IDs) == false)
                {
                    foreach(var a12id in BO.BAS.ConvertString2ListInt(v.SelectedA12IDs))
                    {
                        var recA12 = v.lisLeftA12.First(p => p.a12ID == a12id);                        
                        v.lisRightA12.Add(recA12);
                        var x=v.lisLeftA12.FindIndex(p => p.pid == recA12.pid);
                        if (x > -1)
                        {
                            v.lisLeftA12.RemoveAt(x);
                        }
                    }
                }                
            }

        }

        public IActionResult Info(int pid)
        {
            var v = new a42RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a42QesBL.Load(v.pid);
                if (v.Rec != null)
                {
                    v.TagHtml = Factory.o51TagBL.GetTagging("a42", v.pid).TagHtml;
                    
                }
            }

            return View(v);
        }

        public IActionResult Record(int pid, bool isclone)
        {
            var v = new a42Record() { rec_pid = pid, rec_entity = "a42" };
            v.Rec = new BO.a42Qes();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a42QesBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("a42", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

                
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
        public IActionResult Record(Models.Record.a42Record v)
        {

            if (ModelState.IsValid)
            {
                BO.a42Qes c = new BO.a42Qes();
                if (v.rec_pid > 0) c = Factory.a42QesBL.Load(v.rec_pid);
                c.a42Name = v.Rec.a42Name;
                c.a42DateFrom = v.Rec.a42DateFrom;
                c.a42DateUntil = v.Rec.a42DateUntil;
                c.a42Description = v.Rec.a42Description;
                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.a42QesBL.Save(c);
                if (c.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("a42", c.pid, v.TagPids);

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        public string RemoveClosed(string a03ids)
        {
            var mq = new BO.myQuery("a03");
            mq.IsRecordValid = true;
            mq.SetPids(a03ids);
            a03ids = string.Join(",", Factory.a03InstitutionBL.GetList(mq).Select(p => p.pid));
            return a03ids;
        }

        public BO.b65WorkflowMessage InhaleMessage(int b65id)
        {
            return Factory.b65WorkflowMessageBL.Load(b65id);
            
            
        }
    }
}