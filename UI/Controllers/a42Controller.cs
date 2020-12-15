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
            v.lisA10 = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10Aspx_Insert != null && (p.a10Aspx_Insert.ToLower() == "a01_create_res.aspx" || p.a10Aspx_Insert.ToLower().StartsWith("/a42/create")));

            return View(v);
        }
        public IActionResult CompleteJob(int a42id)
        {
            var v = new a42CompleteJob() { a42ID = a42id };
            v.Rec = Factory.a42QesBL.Load(v.a42ID);
            if (v.Rec == null)
            {
                return StopPage(true, "Na vstupu chybí INEZ pid.");
            }
           
            return View(v);
        }
        public IActionResult PrepareMailJob(int a42id)
        {
            var v = new a42PrepareMailJob() { a42ID = a42id };
            v.Rec = Factory.a42QesBL.Load(v.a42ID);
            if (v.Rec == null)
            {
                return StopPage(true, "Na vstupu chybí INEZ pid.");
            }
            v.lisP85 = Factory.p85TempboxBL.GetList(v.Rec.a42JobGuid);
            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Rec.a42UploadGuid);
            v.lisA01 = Factory.a01EventBL.GetList(new BO.myQueryA01() { a42id = v.Rec.pid });
            v.MessageSubject = v.lisP85.Where(p => p.p85Prefix == "x40").First().p85FreeText01;
            v.MessageBody = v.lisP85.Where(p => p.p85Prefix == "x40").First().p85Message;

            return View(v);
        }
        [HttpPost]
        public IActionResult PrepareMailJob(a42PrepareMailJob v)
        {
            v.Rec = Factory.a42QesBL.Load(v.a42ID);
            v.lisP85 = Factory.p85TempboxBL.GetList(v.Rec.a42JobGuid);
            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Rec.a42UploadGuid);
            v.lisA01 = Factory.a01EventBL.GetList(new BO.myQueryA01() { a42id = v.Rec.pid });

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.MessageBody) == true || string.IsNullOrEmpty(v.MessageSubject) == true || v.Rec.j40ID==0)
                {
                    this.AddMessage("Chybí poštovní účet, předmět nebo obsah zprávy.");
                    v.Rec = Factory.a42QesBL.Load(v.a42ID);                    
                    return View(v);
                }
                BO.a42Qes c = Factory.a42QesBL.Load(v.a42ID);
                var lisP85 = Factory.p85TempboxBL.GetList(c.a42JobGuid, false, "x40");
                var recTemp = Factory.p85TempboxBL.Load(lisP85.First().pid);
                recTemp.p85Message = v.MessageBody;
                recTemp.p85FreeText01 = v.MessageSubject;
                Factory.p85TempboxBL.Save(recTemp);

                var cMerge = new BO.CLS.MergeContent();
                var lisX40 = Factory.MailBL.GetList(new BO.myQuery("x40") { a42id = v.Rec.pid }).Where(p=>p.x40BatchGuid==v.Rec.a42JobGuid);
                int x = 0;
                foreach (var cTemp in v.lisP85.Where(p=>p.p85Prefix=="x43"))     //p85OtherKey4: a03ID, p85OtherKey2: j02ID
                {
                    x += 1;
                    int intA01ID = v.lisA01.Where(p => p.a03ID == cTemp.p85OtherKey4).First().pid;
                    var dt = Factory.gridBL.GetList4MailMerge("a01", intA01ID) ;
                    string strBody = cMerge.GetMergedContent(v.MessageBody, dt);

                    var recX40 = new BO.x40MailQueue() {j40ID=c.j40ID,x40MailID=x,x40BatchGuid=v.Rec.a42JobGuid, x40Status=BO.x40StateFlag.InQueque,x40DataPID=intA01ID,x29ID=101, x40Subject = v.MessageSubject,x40Body=strBody,x40IsHtmlBody=false,x40Recipient=cTemp.p85FreeText01 };
                    if (v.lisTempFiles != null && v.lisTempFiles.Count()>0)
                    {
                        recX40.x40AttachmentsGuid = v.Rec.a42UploadGuid;
                    }
                    if (lisX40.Where(p=>p.x40DataPID==intA01ID && p.x29ID == 101).Count() > 0)
                    {
                        recX40.pid = lisX40.Where(p => p.x40DataPID == intA01ID && p.x29ID == 101).First().pid;
                    }
                    
                    Factory.MailBL.SaveX40(null, recX40);
                }
                Factory.a42QesBL.UpdateJobState(v.Rec.pid, BO.a42JobState.PreparedX40);
                return RedirectToAction("MailBatchFramework","Mail", new { batchguid = c.a42JobGuid });

            }
            this.Notify_RecNotSaved();
            return View(v);
        }

        public IActionResult CreateTempFinish(int a42id)
        {
            var v = new a42CreateTempFinishViewModel() { a42ID = a42id };
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
                if (v.lisP85.Where(p => p.p85Prefix == "x43" && p.p85OtherKey4 == c.p85OtherKey1).Count() == 0)
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
                var lisP85 = Factory.p85TempboxBL.GetList(c.a42JobGuid, false, "x40");
                var recTemp = Factory.p85TempboxBL.Load(lisP85.First().pid);
                recTemp.p85Message = v.MessageBody;
                recTemp.p85FreeText01 = v.MessageSubject;
                Factory.p85TempboxBL.Save(recTemp);
                return RedirectToAction("CompleteJob", new { a42id = c.pid });

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
            var v = new a42CreateViewModel() { a10ID = a10id, UploadGuid = BO.BAS.GetGuid(), MessageReceiverFlag = BO.RecipientFlagEnum.DirectorAddress };
            v.Rec = new BO.a42Qes();
            var mq = new BO.myQuery("a10");
            mq.IsRecordValid = true;
            v.lisA10 = Factory.a10EventTypeBL.GetList(mq).Where(p => p.a10Aspx_Insert != null && (p.a10Aspx_Insert.ToLower() == "a01_create_res.aspx" || p.a10Aspx_Insert.ToLower().StartsWith("/a42/create")));
            if (v.lisA10.Where(p => p.pid == v.a10ID).Count() == 0)
            {
                return RedirectToAction("CreatePre");
            }
            RefreshStateCreate(v);

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(a42CreateViewModel v, string oper, int b65id, string f06ids)
        {
            RefreshStateCreate(v);
            if (oper == "change_a08id")
            {
                v.lisPollA12 = Factory.a08ThemeBL.GetListA12(v.a08ID).Where(p => p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.DirectOnly || p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.None).ToList();
                return View(v);
            }

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "change_b65id")
            {
                var recB65 = Factory.b65WorkflowMessageBL.Load(b65id);
                v.MessageSubject = recB65.b65MessageSubject;
                v.MessageBody = recB65.b65MessageBody;
                return View(v);
            }


            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.MessageBody) == true || string.IsNullOrEmpty(v.MessageSubject) == true || v.Rec.j40ID == 0 || string.IsNullOrEmpty(v.Rec.a42Name)==true)
                {
                    this.AddMessage("Povinná pole: Název, Poštovní účet, Předmět zprávy a Obsah zprávy.");                    
                    return View(v);
                }
                BO.a42Qes c = new BO.a42Qes() { a08ID = v.a08ID, a42DateFrom = v.a42DateFrom, a42DateUntil = v.a42DateUntil, a42Name = v.Rec.a42Name, a42Description = v.Rec.a42Description, a42JobGuid = BO.BAS.GetGuid(), a42UploadGuid = v.UploadGuid };
                c.j40ID = v.Rec.j40ID;
                c.a42TestFlag = v.Rec.a42TestFlag;
                BO.a01Event recA01 = new BO.a01Event() { a10ID = v.a10ID, a08ID = v.a08ID };
                List<int> a03ids = BO.BAS.ConvertString2ListInt(v.a03IDs);
                var recX40 = new BO.x40MailQueue() { x40Subject = v.MessageSubject, x40Body = v.MessageBody, x40RecipientFlag = v.MessageReceiverFlag };

                c.pid = Factory.a42QesBL.PrepareTempData(c, recA01, v.lisRightA12, a03ids, recX40, v.lisPollA12.Where(p => p.IsTempDeleted == false).ToList());
                if (c.pid > 0)
                {
                    //Dávka je připravena k vygenerování akcí
                    Factory.a42QesBL.UpdateJobState(c.pid, BO.a42JobState.Draft);
                    return RedirectToAction("CreateTempFinish", new { a42id = c.pid });
                }
            }
            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateCreate(Models.a42CreateViewModel v)
        {
            if (v.a10ID > 0)
            {
                v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
            }
            if (v.a08ID > 0)
            {
                v.RecA08 = Factory.a08ThemeBL.Load(v.a08ID);
                var lisA12 = Factory.a08ThemeBL.GetListA12(v.RecA08.pid);

                v.lisLeftA12 = lisA12.Where(p => p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.DirectOnly || p.f06BindScopeQuery == BO.f06BindScopeQueryENUM.None).ToList();
                if (v.lisPollA12 == null)
                {
                    v.lisPollA12 = v.lisLeftA12;
                }
                v.lisRightA12 = new List<BO.a12ThemeForm>();
                if (string.IsNullOrEmpty(v.SelectedA12IDs) == false)
                {
                    foreach (var a12id in BO.BAS.ConvertString2ListInt(v.SelectedA12IDs))
                    {
                        var recA12 = v.lisLeftA12.First(p => p.a12ID == a12id);
                        v.lisRightA12.Add(recA12);
                        var x = v.lisLeftA12.FindIndex(p => p.pid == recA12.pid);
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
                    v.lisA01 = Factory.a01EventBL.GetList(new BO.myQueryA01() { a42id = pid });
                    v.lisX40= Factory.MailBL.GetList(new BO.myQuery("x40") { a42id = pid });
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
        public IActionResult Record(Models.Record.a42Record v, string oper)
        {
            
            if (oper == "changeperiod")
            {
                var c = Factory.a42QesBL.Load(v.rec_pid);
                c.a42DateFrom = v.ChangeDateFrom;
                c.a42DateUntil = v.ChangeDateUntil;
                if (Factory.a42QesBL.ChangePeriod(c)>0)
                {
                    v.SetJavascript_CallOnLoad(v.rec_pid);
                    return View(v);
                }
                else
                {
                    return View(v);
                }
            }
            if (oper == "deletecomplete")
            {
                if (Factory.a42QesBL.DeleteWithA01(v.rec_pid))
                {
                    v.SetJavascript_CallOnLoad(v.rec_pid);
                    return View(v);
                }
                else
                {
                    return View(v);
                }
            }
            if (oper== "stopmailing")
            {
                Factory.a42QesBL.UpdateJobState(v.rec_pid, BO.a42JobState.MailQueueStopped);
                Factory.MailBL.StopPendingMessagesInBatch(v.Rec.a42JobGuid);
                v.SetJavascript_CallOnLoad(v.rec_pid);
                return View(v);
            }            
            if (oper == "startmailing")
            {
                Factory.a42QesBL.UpdateJobState(v.rec_pid, BO.a42JobState.MailQueue);
                Factory.MailBL.RestartMessagesInBatch(v.Rec.a42JobGuid);
                v.SetJavascript_CallOnLoad(v.rec_pid);
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.a42Qes c = new BO.a42Qes();
                if (v.rec_pid > 0) c = Factory.a42QesBL.Load(v.rec_pid);
                c.a42Name = v.Rec.a42Name;
                c.a42DateFrom = v.Rec.a42DateFrom;
                c.a42DateUntil = v.Rec.a42DateUntil;
                c.a42Description = v.Rec.a42Description;
                c.j40ID = v.Rec.j40ID;
                c.a42TestFlag = v.Rec.a42TestFlag;

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
            var mq = new BO.myQueryA03();
            mq.IsRecordValid = true;
            mq.SetPids(a03ids);
            a03ids = string.Join(",", Factory.a03InstitutionBL.GetList(mq).Select(p => p.pid));
            return a03ids;
        }

        public BO.b65WorkflowMessage InhaleMessage(int b65id)
        {
            return Factory.b65WorkflowMessageBL.Load(b65id);


        }


        public BO.Result RunPartOfJob(int a42id, int take_records_per_job)
        {
            //Zpracuje najednou počet akcí=take_records_per_job 
            if (take_records_per_job == 0) take_records_per_job = 10;
            var recA42 = Factory.a42QesBL.Load(a42id);
            if (recA42 == null)
            {
                return new BO.Result(true, "Na vstupu chybí INEZ pid.");
            }
            var lisP85_A03_ALL = Factory.p85TempboxBL.GetList(recA42.a42JobGuid, false, "a03");
            var lisP85_A03 = Factory.p85TempboxBL.GetList(recA42.a42JobGuid, false, "a03").Where(p => p.p85IsFinished == false).Take(take_records_per_job);
            var lisP85 = Factory.p85TempboxBL.GetList(recA42.a42JobGuid, false);
            var lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(recA42.a42UploadGuid);

            int intOKs = 0;
            int intErrs = 0;
            int x = lisP85_A03_ALL.Where(p => p.p85IsFinished == true).Count();
            var ret = new BO.Result(false);
            if (lisP85_A03.Count() == 0)
            {
                Factory.a42QesBL.UpdateJobState(recA42.pid, BO.a42JobState.PreparedA01);    //akce vygenerovány
                ret.pid = 1;
                return ret;
            }
            foreach (var recTemp in lisP85_A03)
            {
                x += 1;
                var recA01 = new BO.a01Event() { a42ID = a42id, a01IsTemporary = false, a03ID = recTemp.p85OtherKey1, a10ID = recTemp.p85OtherKey2, a08ID = recA42.a08ID, a01DateFrom = recA42.a42DateFrom, a01DateUntil = recA42.a42DateUntil };
                recA01.j02ID_Issuer = Factory.CurrentUser.j02ID;
                recA01.j03ID_Creator = Factory.CurrentUser.pid;

                var lisA11 = new List<BO.a11EventForm>();
                foreach (var cTempF06 in lisP85.Where(p => p.p85Prefix == "a12"))
                {
                    var cA11 = new BO.a11EventForm() { f06ID = cTempF06.p85OtherKey2 };
                    if (cTempF06.p85FreeBoolean01 == true)
                    {
                        cA11.a11IsPoll = true;
                        cA11.a11AccessToken = Factory.a11EventFormBL.GetRandomToken();
                    }
                    lisA11.Add(cA11);
                }

                var intA01ID = Factory.a01EventBL.Create(recA01, true, lisA11, null, null, null);
                ret.Message += "<br>#" + x.ToString() + "/" + lisP85_A03_ALL.Count().ToString() + " # " + DateTime.Now.ToString() + ": " + Factory.a03InstitutionBL.Load(recA01.a03ID).NamePlusRedizo;
                if (intA01ID > 0)
                {
                    intOKs += 1;
                    recA01 = Factory.a01EventBL.Load(intA01ID);
                    ret.Message += ", Nová akce: " + recA01.a01Signature;
                }
                else
                {
                    intErrs += 1;
                    ret.Message += ", Chyba";
                }
                recTemp.p85IsFinished = true;
                Factory.p85TempboxBL.Save(recTemp);
            }
            Factory.a42QesBL.UpdateJobState(recA42.pid, BO.a42JobState.Generating);

            return ret;
        }
    }
}