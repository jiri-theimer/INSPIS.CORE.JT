using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class a01CreateHelpdeskController : BaseController
    {
        public IActionResult Index(int a10id, int j02id)
        {
            
            var v = new a01CreateHelpdeskViewModel() { j02ID = j02id,a10ID=a10id,UploadGuid=BO.BAS.GetGuid() };

            RefreshState(v);
            RefreshInstitution(v);
            v.a10ID = a10id;

            v.Rec = new BO.a01Event();
            return View(v);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Models.a01CreateHelpdeskViewModel v, string oper)
        {
            RefreshState(v);
            
            RefreshInstitution(v);

            if (oper == "j02_change")
            {
                v.a03ID = 0;
                v.Institution = "";
                RefreshInstitution(v);
                return View(v);
            }
            if (oper == "a10_change")
            {                
                return View(v);
            }

            if (ModelState.IsValid)
            {
                if (v.RecA10 == null)
                {
                    this.AddMessage("Musíte vybrat typ požadavku.");
                    return View(v);
                }
                if (string.IsNullOrEmpty(v.Rec.a01Description) == true || v.Rec.a01Description.Length < 20)
                {
                    this.AddMessage("Popis požadavku je příliš stručný.");
                    return View(v);
                }

                BO.a01Event c = new BO.a01Event();
                c.a10ID = v.a10ID;
                c.a03ID = v.a03ID;
                if (c.a03ID == 0)
                {
                    c.a01InstitutionPlainText = v.Rec.a01InstitutionPlainText;
                }
                c.j02ID_Issuer = v.j02ID;
                c.a08ID = v.Rec.a08ID;
                c.a01Description = v.Rec.a01Description;
                

                c.pid = Factory.a01EventBL.Create(c, true, null, null, null, null);
                if (c.pid > 0)
                {
                    Factory.o27AttachmentBL.SaveChangesAndUpload(v.UploadGuid, 101, c.pid);

                    return RedirectToAction("RecPageHelpdesk", "a01", new { pid = c.pid });

                }
            }

            this.Notify_RecNotSaved();
            return View(v);
        }


        private void RefreshInstitution(Models.a01CreateHelpdeskViewModel v)
        {
            var mq = new BO.myQueryA03() { IsRecordValid = true, j02id = v.j02ID };
            var lis = Factory.a03InstitutionBL.GetList(mq);
            if (lis.Count() > 0)
            {
                v.a03ID = lis.First().pid;
                v.Institution = lis.First().NamePlusRedizo;
                v.IsComboA03 = true;
            }
            else
            {
                v.IsComboA03 = false;
            }
        }

        private void RefreshState(Models.a01CreateHelpdeskViewModel v)
        {
            
            if (v.Rec == null)
            {
                v.Rec = new BO.a01Event();
            }
            if (v.a10ID > 0)
            {
                v.RecA10 = Factory.a10EventTypeBL.Load(v.a10ID);
                v.a10Name = v.RecA10.a10Name;
            }

            if (v.j02ID == 0)
            {
                v.j02ID = Factory.CurrentUser.j02ID;
            }
            v.RecJ02 = Factory.j02PersonBL.Load(v.j02ID);
            v.Person = v.RecJ02.FullNameDesc;
            
            

        }

    }


}
