using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class BatchUpdateController : BaseController
    {
        public IActionResult a01(string pids)
        {
            var v = new BatchUpdateA01() { pids = pids };
            if (string.IsNullOrEmpty(pids))
            {
                return this.StopPage(false, "Na vstupu chybí výběr záznamů.");
            }

            RefreshStateA01(v);
            if (v.CommonB01ID == 0)
            {
                this.AddMessage("U výběru akcí nelze měnit jejich workflow stav, protože mají vazbu na různé workflow šablony.","info");
            }
            

            return ViewTup(v,BO.j05PermValuEnum.A01BatchUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult a01(Models.BatchUpdateA01 v, string oper)
        {
            RefreshStateA01(v);
            
            if (oper == "postback")
            {
                return View(v);
            }
           
           
            
            if (ModelState.IsValid)
            {
               if (v.SelectedB02ID==0 && v.SelectedA10ID==0 && v.SelectedA08ID == 0 && string.IsNullOrEmpty(v.WorkflowComment) && v.a01DateFrom==null && v.a01DateUntil==null)
                {
                    this.AddMessage("Musíte specifikovat nějakou změnu.");
                    return View(v);
                }
                var errs = new List<string>();
               foreach(var c in v.lisA01)
                {
                    if (v.SelectedB02ID > 0)
                    {
                        c.b02ID = v.SelectedB02ID;
                    }
                    if (v.SelectedA08ID > 0)
                    {
                        c.a08ID = v.SelectedA08ID;
                    }
                    if (v.SelectedA10ID > 0)
                    {
                        c.a10ID = v.SelectedA10ID;
                    }
                    if (v.a01DateFrom != null && v.a01DateUntil != null && v.a01DateUntil>=v.a01DateFrom)
                    {
                        c.a01DateFrom = v.a01DateFrom;
                        c.a01DateUntil = v.a01DateUntil;
                    }
                    
                    if (Factory.a01EventBL.BatchUpdate(c) == 0)
                    {
                        errs.Add(c.a01Signature);
                    }
                    if (string.IsNullOrEmpty(v.WorkflowComment) == false)
                    {
                        Factory.WorkflowBL.SaveWorkflowComment(c.pid, v.WorkflowComment, null, null);
                    }
                }
               if (errs.Count() > 0)
                {
                    this.AddMessage(string.Format("Chyby v akcích: {0}.", string.Join(", ", errs)));
                }
                
                v.SetJavascript_CallOnLoad(0);
                return View(v);

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateA01(BatchUpdateA01 v)
        {
            var mq = new BO.myQuery("a01") { pids = BO.BAS.ConvertString2ListInt(v.pids) };
            v.lisA01 = Factory.a01EventBL.GetList(mq);
            if (v.lisA01.Select(p => p.b01ID).Distinct().Count() == 1)
            {
                v.CommonB01ID = v.lisA01.First().b01ID;
            }


        }




        public IActionResult a03(string pids)
        {
            var v = new BatchUpdateA03() { pids = pids };
            if (string.IsNullOrEmpty(pids))
            {
                return this.StopPage(false, "Na vstupu chybí výběr záznamů.");
            }

            RefreshStateA03(v);
            

            return ViewTup(v, BO.j05PermValuEnum.A03Admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult a03(Models.BatchUpdateA03 v, string oper)
        {
            RefreshStateA03(v);

            if (oper == "postback")
            {
                return View(v);
            }



            if (ModelState.IsValid)
            {
                if (v.SelectedA05ID == 0 && v.SelectedA21ID == 0 && v.SelectedA28ID == 0 && v.SelectedSupervisoryID==0 && v.SelectedParentFlag==-1)
                {
                    this.AddMessage("Musíte specifikovat nějakou změnu.");
                    return View(v);
                }
                var errs = new List<string>();
                
                foreach (var c in v.lisA03)
                {
                    if (v.SelectedA05ID > 0)
                    {
                        c.a05ID = v.SelectedA05ID;
                    }
                    if (v.SelectedA21ID > 0)
                    {
                        c.a21ID = v.SelectedA21ID;
                    }
                    if (v.SelectedA28ID > 0)
                    {
                        c.a28ID = v.SelectedA28ID;
                    }
                    if (v.SelectedParentFlag > -1)
                    {
                        c.a03ParentFlag = (BO.a03ParentFlagEnum) v.SelectedParentFlag;
                    }                                       
                }
                foreach (var c in v.lisA03)
                {
                    var d = c;
                    if (!Factory.a03InstitutionBL.ValidateBeforeSave(ref d))
                    {
                        errs.Add(d.NamePlusRedizo);
                    }
                }
                if (errs.Count() > 0)
                {
                    this.AddMessage(string.Format("Chyby v institucích: {0}.", string.Join(", ", errs)));
                }
                foreach (var c in v.lisA03)
                {
                    if (Factory.a03InstitutionBL.Save(c) == 0)
                    {
                        errs.Add(c.NamePlusRedizo);
                    }
                }
                

                v.SetJavascript_CallOnLoad(0);
                return View(v);

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshStateA03(BatchUpdateA03 v)
        {
            var mq = new BO.myQueryA03 { pids = BO.BAS.ConvertString2ListInt(v.pids) };
            v.lisA03 = Factory.a03InstitutionBL.GetList(mq);            


        }
    }


}