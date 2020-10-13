using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;


namespace UI.Controllers
{
    public class o13Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new o13Record() { rec_pid = pid, rec_entity = "o13" };
            v.Rec = new BO.o13AttachmentType();
            if (v.rec_pid > 0)
            {                
                v.Rec = Factory.o13AttachmentTypeBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal_Ciselniky);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.o13Record v)
        {

            if (ModelState.IsValid)
            {
                BO.o13AttachmentType c = new BO.o13AttachmentType();
                if (v.rec_pid > 0) c = Factory.o13AttachmentTypeBL.Load(v.rec_pid);
                c.o13ParentID = v.Rec.o13ParentID;
                c.x29ID = v.Rec.x29ID;
                c.o13Name = v.Rec.o13Name;
                c.o13ParentID = v.Rec.o13ParentID;
                c.o13FilePrefix = v.Rec.o13FilePrefix;
                c.o13DefaultArchiveFolder = v.Rec.o13DefaultArchiveFolder;
                c.o13IsPortalDoc = v.Rec.o13IsPortalDoc;
                c.o13IsObjection = v.Rec.o13IsObjection;
                c.o13Description = v.Rec.o13Description;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                c.pid = Factory.o13AttachmentTypeBL.Save(c);
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }
    }
}