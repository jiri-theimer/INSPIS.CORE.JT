using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;
using UI.Models.Tab;

namespace UI.Controllers
{
    public class a11Controller : BaseController
    {
        public IActionResult ValidateForms(int pid,int a01id)
        {
            var v = new a11ValidateForms() { pid = pid,a01ID=a01id };
            var mq = new BO.myQuery("a11");
            if (v.pid > 0)
            {
                var recA11 = Factory.a11EventFormBL.Load(pid);
                v.RecA01 = Factory.a01EventBL.Load(recA11.a01ID);
                mq.pids = new List<int> { pid };                
                
            }
            else
            {
                v.RecA01 = Factory.a01EventBL.Load(a01id);                
                mq.a01id = a01id;
            }


            v.lisA11 = Factory.a11EventFormBL.GetList(mq);
            v.lisResult = new List<BO.ItemValidationResult>();
            foreach(var recA11 in v.lisA11)
            {
                var cValidate = new FormValidation(Factory);
                var lis=cValidate.GetValidateResult(recA11.pid);
                foreach(var c in lis)
                {
                    v.lisResult.Add(c);
                }
            }
            

            return View(v);
        }
        public IActionResult Info(int pid)
        {
            var v = new a11RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.a11EventFormBL.Load(v.pid);
                v.RecA01 = Factory.a01EventBL.Load(v.Rec.a01ID);
                v.RecPermission = Factory.a01EventBL.InhalePermission(v.RecA01);

            }

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone, int a01id)
        {
            var v = new a11Record() { rec_pid = pid, rec_entity = "a11", a01ID = a01id };
            v.Rec = new BO.a11EventForm();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.a11EventFormBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                v.a01ID = v.Rec.a01ID;
              
            }
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing.");
            }
            RefreshState(ref v);
            v.Toolbar = new MyToolbarViewModel(v.Rec) { AllowArchive = false };
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.a11Record v)
        {
            RefreshState(ref v);
            if (ModelState.IsValid)
            {                
                BO.a11EventForm c = Factory.a11EventFormBL.Load(v.rec_pid);               
                c.a11Description = v.Rec.a11Description;
                c.a37ID = v.Rec.a37ID;
                c.a11IsLocked = v.Rec.a11IsLocked;
                c.a25ID = v.Rec.a25ID;
                c.k01ID = v.Rec.k01ID;
                c.a11TeacherPID = v.Rec.a11TeacherPID;
                if (c.a11IsPoll)
                {
                    c.a11AccessToken = v.Rec.a11AccessToken;
                }

                c.pid = Factory.a11EventFormBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(ref Models.Record.a11Record v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            var recA11 = Factory.a11EventFormBL.Load(v.rec_pid);
            v.RecF06 = Factory.f06FormBL.Load(recA11.f06ID);
        }



        //Přidat formuláře do akce:
        public IActionResult Append(int a01id)
        {
            var v = new a11AppendViewModel() { a01ID = a01id,SelectedKolikrat=1 };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppend(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Append(Models.a11AppendViewModel v, string oper, string guid)
        {
            RefreshStateAppend(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add")
            {
                if (v.SelectedF06ID > 0 && v.SelectedKolikrat>0)
                {
                    for(int i = 1; i <= v.SelectedKolikrat; i++)
                    {
                        var c = new BO.a11EventForm() { TempGuid = BO.BAS.GetGuid() };

                        c.f06ID = v.SelectedF06ID;
                        c.f06Name = v.SelectedForm;
                        c.a25ID = v.SelectedA25ID;
                        c.a25Name = v.SelectedA25Name;
                        c.a37ID = v.SelectedA37ID;
                        c.a37Name = v.SelectedA37Name;
                        c.a11Description = v.a11Description;

                        v.lisA11.Add(c);
                    }
                    
                    return View(v);
                }
                else
                {
                    this.AddMessage("Musíte vybrat formulář.");
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA11.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                v.lisA11 = v.lisA11.Where(p => p.IsTempDeleted == false).ToList();
                if (v.lisA11.Count() == 0)
                {
                    this.AddMessage("Pro uložení chybí výběr formulářů.");return View(v);
                }
                foreach (var c in v.lisA11)
                {
                    c.a01ID = v.a01ID;
                    if (Factory.a11EventFormBL.ValidateBeforeSave(c) == false)
                    {
                        return View(v);
                    }
                }
                foreach (var c in v.lisA11)
                {
                    var rec = new BO.a11EventForm() { a01ID = v.a01ID };
                    rec.f06ID = c.f06ID;
                    rec.a25ID = c.a25ID;
                    rec.a37ID = c.a37ID;
                    rec.a11Description = c.a11Description;
                    int intA11ID= Factory.a11EventFormBL.Save(rec);
                    if (intA11ID == 0)
                    {
                        return View(v);
                    }

                }
                v.SetJavascript_CallOnLoad(v.a01ID);
                return View(v);

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateAppend(a11AppendViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            if (v.lisA11 == null)
            {
                v.lisA11 = new List<BO.a11EventForm>();
            }
            var mq = new BO.myQuery("a11");
            mq.a01id = v.a01ID;
            v.lisA11Saved = Factory.a11EventFormBL.GetList(mq);

        }

        //ANKETNí formuláře
        //Přidat anketní formuláře do akce:
        public IActionResult AppendPoll(int a01id)
        {
            var v = new a11AppendPollViewModel() { a01ID = a01id, SelectedKolikrat = 1 };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppendPoll(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppendPoll(Models.a11AppendPollViewModel v, string oper, string guid,string pids)
        {
            RefreshStateAppendPoll(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "lock" || oper=="unlock")
            {
                foreach(var c in v.lisA11Saved)
                {
                    if (oper == "lock")
                    {
                        c.a11IsLocked = true;
                    }
                    else
                    {
                        c.a11IsLocked = false;
                    }
                    Factory.a11EventFormBL.Save(c);
                }
                return View(v);
            }
            if (oper=="delete" && pids != ""){
                var a11ids = BO.BAS.ConvertString2ListInt(pids);
                for(int i = 0; i <= a11ids.Count - 1; i++)
                {
                    Factory.CBL.DeleteRecord("a11", a11ids[i]);
                }
                RefreshStateAppendPoll(v);
                return View(v);
            }
            if (oper == "add")
            {
                if (v.SelectedF06ID > 0 && v.SelectedKolikrat > 0)
                {
                    var intMinToken = v.AccessTokenMinValue;
                    v.lisA11 = new List<BO.a11EventForm>();
                    for (int i = 1; i <= v.SelectedKolikrat; i++)
                    {
                        var c = new BO.a11EventForm();
                        c.a01ID = v.RecA01.pid;
                        c.a11IsPoll = true;
                        if (intMinToken <= 0)
                        {
                            c.a11AccessToken = Factory.a11EventFormBL.GetRandomToken();
                        }
                        else
                        {
                            c.a11AccessToken = (BO.BAS.RightString("0000" + (intMinToken + i - 1).ToString(),4));
                        }                        
                        c.f06ID = v.SelectedF06ID;
                        c.f06Name = v.SelectedForm;
                        c.a25ID = v.SelectedA25ID;
                        c.a25Name = v.SelectedA25Name;
                        c.a37ID = v.SelectedA37ID;
                        c.a37Name = v.SelectedA37Name;
                        c.a11Description = v.a11Description;
                        c.k01ID = v.SelectedK01ID;
                        v.lisA11.Add(c);                                                
                    }
                    foreach(var c in v.lisA11)
                    {
                        if (!Factory.a11EventFormBL.ValidateBeforeSave(c))
                        {
                            return View(v);
                        }
                    }
                    foreach (var c in v.lisA11)
                    {
                        Factory.a11EventFormBL.Save(c);
                    }
                    RefreshStateAppendPoll(v);
                    return View(v);
                }
                else
                {
                    this.AddMessage("Musíte vybrat formulář.");
                }

                return View(v);
            }
            if (oper == "delete")
            {
                v.lisA11.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            

            
            return View(v);
        }

        private void RefreshStateAppendPoll(a11AppendPollViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            
            var mq = new BO.myQuery("a11");
            mq.a01id = v.a01ID;
            
            v.lisA11Saved = Factory.a11EventFormBL.GetList(mq).Where(p=>p.a11IsPoll==true);

        }


        //Průvodce pro anketní formulář
        //Plus notifikační zpráva
        public IActionResult AppendPollWizard(int a01id)
        {
            var v = new a11AppendPollWizardViewModel() { a01ID = a01id,AccessToken= Factory.a11EventFormBL.GetRandomToken()};
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppendPollWizard(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppendPollWizard(Models.a11AppendPollWizardViewModel v, string oper)
        {
            RefreshStateAppendPollWizard(v);
            if (oper == "postback")
            {
                return View(v);
            }

            if (oper == "email")
            {
                if (v.SelectedEmail != null)
                {
                    v.EmailAddress = v.SelectedEmail;
                }                
                return View(v);
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.AccessToken) || v.AccessToken.Length <4)
                {
                    this.AddMessage("Přístupový PIN je příliš krátký.");return View(v);
                }
                if (string.IsNullOrEmpty(v.EmailAddress))
                {
                    this.AddMessage("Není vyplněn příjemce (e-mail adresa) poštovní zprávy.");return View(v);
                }
                var c = new BO.a11EventForm();
                c.a01ID = v.RecA01.pid;
                c.a11IsPoll = true;
                c.a11AccessToken = v.AccessToken;
                c.f06ID = v.SelectedF06ID;
                c.f06Name = v.SelectedForm;                
                c.a37ID = v.SelectedA37ID;
                c.a37Name = v.SelectedA37Name;
                c.a11Description = v.a11Description;
               

                c.pid = Factory.a11EventFormBL.Save(c);
                if (c.pid > 0)
                {
                    //odeslat zprávu mailem
                    var dt = Factory.gridBL.GetList4MailMerge("a11", c.pid);
                    var cMerge = new BO.CLS.MergeContent();
                    var strBody = cMerge.GetMergedContent(v.MessageBody, dt);
                    
                    var ret=Factory.MailBL.SendMessage(0, v.EmailAddress, "", v.MessageSubject, strBody, false, 111, c.pid);
                    
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshStateAppendPollWizard(a11AppendPollWizardViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            v.lisEmails = new List<BO.StringPair>();
            
            var mq = new BO.myQuery("a39");
            mq.a03id = v.RecA01.a03ID;
            var lisA39 = Factory.a39InstitutionPersonBL.GetList(mq).Where(p => p.j02Email != null);
            foreach (var c in lisA39.OrderBy(p=>p.Person))
            {
                v.lisEmails.Add(new BO.StringPair() { Key = c.j02Email, Value = c.Person + " [" + c.j02Email + "]" });
            }

        }


        public BO.b65WorkflowMessage GetWorkflowMessage(int b65id)
        {
            return Factory.b65WorkflowMessageBL.Load(b65id);
        }

        
    }
}