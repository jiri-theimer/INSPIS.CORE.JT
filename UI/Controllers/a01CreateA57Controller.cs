using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class a01CreateA57Controller : BaseController
    {
        public IActionResult Index(int a57id,int a03id)
        {
            var v = new a01CreateA57ViewModel() { a57ID=a57id,a03ID=a03id };
            if (v.a57ID == 0)
            {
                return this.StopPage(true,"Na vstupu chybí vazba na autoevaluační šablonu.");
            }
            if (v.a03ID == 0)
            {
                return this.StopPage(true, "Na vstupu chybí vazba na školu.");
            }
            RefreshState(v);
            
            
            
            return View(v);
        }


        private void RefreshState(a01CreateA57ViewModel v)
        {
            if (v.lisSelectedF06IDs == null)
            {
                v.lisSelectedF06IDs = new List<int>();
            }
            if (v.RecA57 == null)
            {
                v.RecA57 = Factory.a57AutoEvaluationBL.Load(v.a57ID);
                v.a10ID = v.RecA57.a10ID;
                v.a08ID = v.RecA57.a08ID;
            }
            v.RecA03 = Factory.a03InstitutionBL.Load(v.a03ID);

            v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
            v.RecA08 = Factory.a08ThemeBL.Load(v.a08ID);
            v.lisA12 = Factory.a08ThemeBL.GetListA12(v.RecA08.pid);

            v.lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { a57id = v.a57ID },null);

            if (v.Rec == null)
            {
                v.Rec = new BO.a01Event();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Models.a01CreateA57ViewModel v, string oper)
        {
            RefreshState(v);
            
            if (oper != null)
            {               
                return View(v);
            }
            

            if (ModelState.IsValid)
            {
                if (v.lisSelectedF06IDs.Where(p => p > 0).Count() == 0)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jeden formulář.");return View(v);
                }
                BO.a01Event c = new BO.a01Event();
                c.a57ID = v.RecA57.pid;
                c.a10ID = v.RecA57.a10ID;
                c.a08ID = v.RecA57.a08ID;
                c.a03ID = v.a03ID;              
                c.j02ID_Issuer = Factory.CurrentUser.j02ID;
                
                if (!(DateTime.Today >= v.RecA57.a57CreateFrom && DateTime.Today <= v.RecA57.a57CreateUntil))
                {
                    this.AddMessage("Časová závora autoevaluační šablony nedovoluje založit akci.");return View(v);
                }

                var lisA11 = new List<BO.a11EventForm>();
                foreach(var f06id in v.lisSelectedF06IDs.Where(p => p > 0))
                {
                    lisA11.Add(new BO.a11EventForm() { f06ID = f06id });
                }

                c.pid = Factory.a01EventBL.Create(c, true, lisA11, null, null, null);
                if (c.pid > 0)
                {

                    return RedirectToAction("RecPageA57", "a01", new { pid = c.pid });

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

    }


}
