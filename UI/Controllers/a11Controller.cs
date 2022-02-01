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
        public IActionResult Simulation(int f06id)
        {
            var v = new a11SimulationViewModel() { f06ID = f06id };
            if (v.f06ID == 0)
            {
                return StopPage(false, "f06id missing");
            }
            v.a10ID = Factory.CBL.LoadUserParamInt("Simulation-a10ID");
            v.a08ID = Factory.CBL.LoadUserParamInt("Simulation-a08ID");
            v.a03ID = Factory.CBL.LoadUserParamInt("Simulation-a03ID");
            if (v.a03ID > 0)
            {
                v.a03Name = Factory.a03InstitutionBL.Load(v.a03ID).a03Name;
            }
            RefreshStateSimulation(v);




            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Simulation(Models.a11SimulationViewModel v, string oper)
        {
            RefreshStateSimulation(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "cleardata")
            {
                var lisA11 = Factory.a11EventFormBL.GetList(new BO.myQueryA11() { f06id = v.f06ID, a11issimulation = true });
                if (lisA11.Count() > 0)
                {
                    foreach (var c in lisA11)
                    {
                        Factory.a11EventFormBL.ClearF32(c.pid);
                    }
                }
                this.AddMessage("Provedeno.", "info");
                return View(v);

            }
            if (ModelState.IsValid)
            {
                if (v.a10ID == 0 || v.a08ID == 0 || v.a03ID == 0)
                {
                    this.AddMessage("Chybí vyplnit typ, téma akce nebo instituci.");
                    return View(v);
                }
                var mq = new BO.myQueryA11() { f06id = v.f06ID, a11issimulation = true };
                var lisA11 = Factory.a11EventFormBL.GetList(mq);
                if (lisA11.Count() == 0)
                {
                    
                    var recA01 = new BO.a01Event() { a01IsTemporary = true,a01TemporaryFlag=BO.a01TemporaryFlagENUM.FormSimulation, a10ID = v.a10ID, a08ID = v.a08ID, a03ID = v.a03ID, a01DateFrom = DateTime.Now, a01DateUntil = DateTime.Today.AddDays(2), j03ID_Creator = Factory.CurrentUser.pid, j02ID_Issuer = Factory.CurrentUser.j02ID };

                    var recA11 = new BO.a11EventForm() { f06ID = v.f06ID, a11IsSimulation = true, a37ID = v.a37ID, a11Description = Factory.tra("Simulace chování formuláře.") };
                    var lis = new List<BO.a11EventForm>();

                    var intA01ID = Factory.a01EventBL.Create(recA01, true, new List<BO.a11EventForm>() { recA11 }, null, null, null);

                    if (intA01ID > 0)
                    {
                        SaveSimulationData(v);
                        mq.a01id = intA01ID;
                        lisA11 = Factory.a11EventFormBL.GetList(mq);
                        return Redirect(Factory.App.UiftUrl + "/formular/" + lisA11.Last().pid.ToString());
                    }
                }
                else
                {
                    var recA01 = Factory.a01EventBL.Load(lisA11.Last().a01ID);
                    recA01.a01DateFrom = DateTime.Now;
                    recA01.a01DateUntil = DateTime.Today.AddDays(2);
                    recA01.a10ID = v.a10ID;
                    recA01.a08ID = v.a01ID;
                    recA01.a03ID = v.a03ID;
                    Factory.a01EventBL.SaveA01Record(recA01, Factory.a10EventTypeBL.Load(v.a10ID));
                    SaveSimulationData(v);
                    return Redirect(Factory.App.UiftUrl + "/formular/" + lisA11.Last().pid.ToString());
                }
            }



            return View(v);
        }

        private void SaveSimulationData(Models.a11SimulationViewModel v)
        {
            Factory.CBL.SetUserParam("Simulation-a10ID", v.a10ID.ToString());
            Factory.CBL.SetUserParam("Simulation-a08ID", v.a08ID.ToString());
            Factory.CBL.SetUserParam("Simulation-a03ID", v.a03ID.ToString());
        }



        private void RefreshStateSimulation(Models.a11SimulationViewModel v)
        {
            v.lisA10 = Factory.a10EventTypeBL.GetList(new BO.myQuery("a10"));
            v.lisA08 = Factory.a08ThemeBL.GetList(new BO.myQuery("a08"));
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);
            if (v.a03ID > 0)
            {
                var mq = new BO.myQuery("a37");
                mq.a03id = v.a03ID;
                v.lisA37 = Factory.a37InstitutionDepartmentBL.GetList(mq);
            }
        }

        public BO.Result ClearForm(int a11id)
        {
            var recA11 = Factory.a11EventFormBL.Load(a11id);
            bool b = Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal);
            if (!b)
            {
                var perm = Factory.a01EventBL.InhalePermission(Factory.a01EventBL.Load(recA11.a01ID));
                if (perm.HasPerm(BO.a01EventPermissionENUM.FullAccess) || perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Owner) || perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader))
                {
                    b = true;
                }
            }
            if (b)
            {
                Factory.a11EventFormBL.ClearF32(a11id);
            }
            return new BO.Result(false, Factory.tra("Operace dokončena."));
        }
        public IActionResult Protocol(int a11id)
        {
            var v = new a11ProtocolViewModel() { a11ID=a11id };
            v.Rec = Factory.a11EventFormBL.Load(v.a11ID);
            var mq = new BO.myQueryF32() { a11id = v.a11ID };
            v.lisF32 = Factory.f32FilledValueBL.GetListExtended(mq).OrderBy(p => p.f18TreeIndex).ThenBy(p => p.f19Ordinal);
            v.lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { a11id = a11id },null);
            return View(v);
        }
        public IActionResult ValidateForms(int pid, int a01id)
        {
            var v = new a11ValidateForms() { pid = pid, a01ID = a01id };
            var mq = new BO.myQueryA11();
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
            foreach (var recA11 in v.lisA11)
            {
                var cValidate = new FormValidation(Factory);
                var lis = cValidate.GetValidateResult(recA11.pid);
                foreach (var c in lis)
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
            
            var perm = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (!perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader) && !perm.HasPerm(BO.a01EventPermissionENUM.FullAccess))
            {
                return this.StopPage(true, "Pro editaci formuláře v akci nemáte oprávnění.", true);
            }
            return View(v);
            //return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
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
            var v = new a11AppendViewModel() { a01ID = a01id, SelectedKolikrat = 1 };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppend(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }
            if (v.RecA01.a01IsAllFormsClosed)
            {
                return this.StopPage(true, "Formuláře v akci jsou globálně uzavřeny.",true);
            }
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.",true);
            }
            var perm = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (!perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader) && !perm.HasPerm(BO.a01EventPermissionENUM.FullAccess))
            {
                return this.StopPage(true, "Přidávat formuláře do akce může pouze vedoucí týmu.", true);
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
                if (v.SelectedF06ID > 0 && v.SelectedKolikrat > 0)
                {
                    for (int i = 1; i <= v.SelectedKolikrat; i++)
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
                    this.AddMessage("Pro uložení chybí výběr formulářů."); return View(v);
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
                    int intA11ID = Factory.a11EventFormBL.Save(rec);
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
            var mq = new BO.myQueryA11() { a01id = v.a01ID };
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
            if (v.RecA01.a01IsAllFormsClosed)
            {
                return this.StopPage(true, "Formuláře v akci jsou globálně uzavřeny.",true);
            }
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.",true);
            }
            var perm = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (!perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader) && !perm.HasPerm(BO.a01EventPermissionENUM.FullAccess))
            {
                return this.StopPage(true, "Přidávat formuláře do akce může pouze vedoucí týmu.", true);
            }

            return View(v);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AppendPoll(Models.a11AppendPollViewModel v, string oper, string guid, string pids)
        {
            RefreshStateAppendPoll(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if ((oper == "lock" || oper == "unlock") && !string.IsNullOrEmpty(pids))
            {
                var a11ids = BO.BAS.ConvertString2ListInt(pids);
                foreach (int a11id in a11ids)
                {
                    var c = Factory.a11EventFormBL.Load(a11id);
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
                v.SetJavascript_CallOnLoad("/a01/RecPage?pid=" + v.RecA01.pid.ToString());
                return View(v);
            }
            if (oper == "delete" && !string.IsNullOrEmpty(pids))
            {
                var a11ids = BO.BAS.ConvertString2ListInt(pids);
                for (int i = 0; i <= a11ids.Count - 1; i++)
                {
                    Factory.CBL.DeleteRecord("a11", a11ids[i]);
                }
                //RefreshStateAppendPoll(v);
                v.SetJavascript_CallOnLoad("/a01/RecPage?pid=" + v.RecA01.pid.ToString());
                return View(v);
            }
            if (oper == "add")
            {
                if (v.SelectedF06ID > 0 && v.SelectedKolikrat > 0)
                {
                    
                    v.lisA11 = new List<BO.a11EventForm>();
                    var pins = GeneratePins(v);
                   
                    for (int i = 1; i <= v.SelectedKolikrat; i++)
                    {
                        var c = new BO.a11EventForm();
                        c.a01ID = v.RecA01.pid;
                        c.a11IsPoll = true;
                        c.a11AccessToken = pins[i - 1];                        
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
                    foreach (var c in v.lisA11)
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
                    v.SetJavascript_CallOnLoad("/a01/RecPage?pid=" + v.RecA01.pid.ToString());
                    return View(v);
                    //RefreshStateAppendPoll(v);

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

        private List<string> GeneratePins(Models.a11AppendPollViewModel v)  //vygeneruje sadu anketních PINů
        {
            var pins = new List<string>();
            var lisTestDuplPins = Factory.a11EventFormBL.GetList(new BO.myQueryA11() { a01id = v.a01ID }).Where(p => p.a11IsPoll == true);

            for (int i = 1; i <= v.SelectedKolikrat; i++)
            {
                string strPIN = "";
                var x = i;
                if (v.AccessTokenMinValue <= 0)
                {
                    strPIN = Factory.a11EventFormBL.GetRandomToken();
                }
                else
                {
                    strPIN = BO.BAS.RightString("0000" + (v.AccessTokenMinValue + i - 1).ToString(), 4);
                }
                while (pins.Contains(strPIN) || lisTestDuplPins.Where(p=>p.a11AccessToken==strPIN).Count()>0)
                {
                    if (v.AccessTokenMinValue <= 0)
                    {
                        strPIN = Factory.a11EventFormBL.GetRandomToken();
                    }
                    else
                    {
                        strPIN = BO.BAS.RightString("0000" + (v.AccessTokenMinValue + x - 1).ToString(), 4);
                    }                                        
                    x += 1;
                    if (x > 9000) break;
                }
                pins.Add(strPIN);

            }
            return pins;
        }
        private void RefreshStateAppendPoll(a11AppendPollViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01ID);

            var mq = new BO.myQueryA11();
            mq.a01id = v.a01ID;

            v.lisA11Saved = Factory.a11EventFormBL.GetList(mq).Where(p => p.a11IsPoll == true);

            var myqueryinline = "pids@list_int@" + string.Join(",", v.lisA11Saved.Select(p => p.pid));
            v.gridinput = new TheGridInput() { entity = "a11EventForm", master_entity = "a11poll", myqueryinline = myqueryinline, oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("a11", null, 0, myqueryinline);

            //v.gridinput = GetGridInput(v.a01ID);
        }



        //Průvodce pro anketní formulář
        //Plus notifikační zpráva
        public IActionResult AppendPollWizard(int a01id)
        {
            var v = new a11AppendPollWizardViewModel() { a01ID = a01id, AccessToken = Factory.a11EventFormBL.GetRandomToken() };
            if (v.a01ID == 0)
            {
                return this.StopPage(true, "a01id missing");
            }
            RefreshStateAppendPollWizard(v);
            if (v.RecA01 == null)
            {
                return RecNotFound(v);
            }
            if (v.RecA01.a01IsAllFormsClosed)
            {
                return this.StopPage(true, "Formuláře v akci jsou globálně uzavřeny.",true);
            }
            if (v.RecA01.isclosed)
            {
                return this.StopPage(true, "Tato akce je již uzavřena.",true);
            }
            var perm = Factory.a01EventBL.InhalePermission(v.RecA01);
            if (!perm.HasPerm(BO.a01EventPermissionENUM.ShareTeam_Leader) && !perm.HasPerm(BO.a01EventPermissionENUM.FullAccess))
            {
                return this.StopPage(true, "Přidávat formuláře do akce může pouze vedoucí týmu.", true);
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
                if (string.IsNullOrEmpty(v.AccessToken) || v.AccessToken.Length < 4)
                {
                    this.AddMessage("Přístupový PIN je příliš krátký."); return View(v);
                }
                if (string.IsNullOrEmpty(v.EmailAddress))
                {
                    this.AddMessage("Není vyplněn příjemce (e-mail adresa) poštovní zprávy."); return View(v);
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

                    var ret = Factory.MailBL.SendMessage(0, v.EmailAddress, "", v.MessageSubject, strBody, false, 111, c.pid);

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
            foreach (var c in lisA39.OrderBy(p => p.Person))
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