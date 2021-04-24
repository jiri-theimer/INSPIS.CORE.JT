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

            RefreshState(v);
            
            
            
            return View(v);
        }


        private void RefreshState(a01CreateA57ViewModel v)
        {

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
                

                BO.a01Event c = new BO.a01Event();
                c.a10ID = v.RecA57.a10ID;
                c.a08ID = v.RecA57.a08ID;
                c.a03ID = v.a03ID;              
                c.j02ID_Issuer = Factory.CurrentUser.j02ID;
                
                

                c.pid = Factory.a01EventBL.Create(c, true, null, null, null, null);
                if (c.pid > 0)
                {

                    return RedirectToAction("RecPage", "a01", new { pid = c.pid });

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }

    }


}
