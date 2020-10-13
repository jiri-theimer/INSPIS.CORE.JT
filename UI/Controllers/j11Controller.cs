using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class j11Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j11Record() { rec_pid = pid, rec_entity = "j11" };
            v.Rec = new BO.j11Team();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j11TeamBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("j02Person");                
                mq.j11id =v.rec_pid;
                v.j02IDs = string.Join(",", Factory.j02PersonBL.GetList(mq).Select(p => p.pid));
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
        public IActionResult Record(Models.Record.j11Record v)
        {

            if (ModelState.IsValid)
            {
                BO.j11Team c = new BO.j11Team();
                if (v.rec_pid > 0) c = Factory.j11TeamBL.Load(v.rec_pid);
                c.j11Name = v.Rec.j11Name;
                c.j11Description = v.Rec.j11Description;                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                List<int> j02ids = BO.BAS.ConvertString2ListInt(v.j02IDs);
                c.pid = Factory.j11TeamBL.Save(c,j02ids);
                if (c.pid > 0)
                {
                   
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        public List<int> GetJ02IDsByRegion(int a05id)
        {
            if (a05id == 0) { return null; };

            var mq = new BO.myQuery("j02Person");
            mq.IsRecordValid = true;
            mq.a05id = a05id;
            return Factory.j02PersonBL.GetList(mq).Select(p => p.pid).ToList();
        }
    }
}