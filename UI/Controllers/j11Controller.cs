using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;
using UI.Models.Recpage;

namespace UI.Controllers
{
    public class j11Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new j11RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j11TeamBL.Load(v.pid);
                if (!v.Rec.j11IsAllUsers)
                {
                    v.lisJ02 = Factory.j02PersonBL.GetList(new BO.myQueryJ02() { j11id = pid });
                }
                
               
            }
            return View(v);
        }
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
                
                
                v.j02IDs = string.Join(",", Factory.j02PersonBL.GetList(new BO.myQueryJ02() { j11id = v.rec_pid }).Select(p => p.pid));
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState_Record(v);

            return ViewTupCiselnik(v, BO.j03AdminRoleValueFlagEnum.uzivatel_er);
        }

        private void RefreshState_Record(j11Record v)
        {
            if (string.IsNullOrEmpty(v.j02IDs)) v.j02IDs = "-1";
            v.gridinput = new TheGridInput() { entity = "j02Person", master_entity = "j11record", myqueryinline = "pids@list_int@" + v.j02IDs, oncmclick = "", ondblclick = "" };
            v.gridinput.query = new BO.InitMyQuery().Load("j02", null, 0, "pids@list_int@" + v.j02IDs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j11Record v,string oper)
        {
            RefreshState_Record(v);
            if (oper == "postback")
            {
                return View(v);
            }
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

            var mq = new BO.myQueryJ02() { a05id = a05id,IsRecordValid=true };           
            return Factory.j02PersonBL.GetList(mq).Select(p => p.pid).ToList();
        }

        public string RemoveClosed(string j02ids)
        {
            var mq = new BO.myQueryJ02();
            mq.IsRecordValid = true;
            mq.SetPids(j02ids);
            j02ids = string.Join(",", Factory.j02PersonBL.GetList(mq).Select(p => p.pid));
            return j02ids;
        }
    }
}