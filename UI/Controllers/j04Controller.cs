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
    public class j04Controller : BaseController
    {
        public IActionResult Info(int pid)
        {
            var v = new j04RecPage() { pid = pid };
            if (v.pid > 0)
            {
                v.Rec = Factory.j04UserRoleBL.Load(v.pid);
                v.lisJ05 = Factory.j04UserRoleBL.GetListJ05(v.pid);
                v.lisJ08 = Factory.j04UserRoleBL.GetListJ08(v.pid);
            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new j04Record() { rec_pid = pid, rec_entity = "j04" };
            v.Rec = new BO.j04UserRole();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.j04UserRoleBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                var lisJ05 = Factory.j04UserRoleBL.GetListJ05(v.rec_pid);
                v.SelectedJ05IDs_EPIS1 = lisJ05.Where(p => p.j05PermFlag == BO.j05PermFlagEnum.EPIS1).Select(p => p.j05ID).ToList();
                v.SelectedJ05IDs_EPIS2 = lisJ05.Where(p => p.j05PermFlag == BO.j05PermFlagEnum.EPIS2).Select(p => p.j05ID).ToList();
                v.SelectedJ05IDs_PORTAL = lisJ05.Where(p => p.j05PermFlag == BO.j05PermFlagEnum.PORTAL).Select(p => p.j05ID).ToList();

                v.lisJ08 = Factory.j04UserRoleBL.GetListJ08(v.rec_pid).ToList();
                foreach (var c in v.lisJ08)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            RefreshState(v);
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.j04Record v,string oper,string guid)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "add_j08")
            {
                var c = new BO.j08UserRole_EventType() { TempGuid = BO.BAS.GetGuid() };
                v.lisJ08.Add(c);
                return View(v);
            }
            if (oper == "delete_j08")
            {
                v.lisJ08.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.j04UserRole c = new BO.j04UserRole();
                if (v.rec_pid > 0) c = Factory.j04UserRoleBL.Load(v.rec_pid);
                c.j04Name = v.Rec.j04Name;
                c.j04Aspx_PersonalPage = v.Rec.j04Aspx_PersonalPage;
                c.j04RelationFlag = v.Rec.j04RelationFlag;
                c.j04IsAllowedAllEventTypes = v.Rec.j04IsAllowedAllEventTypes;
                c.j04IsAllowInSchoolAdmin = v.Rec.j04IsAllowInSchoolAdmin;
                c.j04PortalFaceFlag = v.Rec.j04PortalFaceFlag;
                c.j04ViewUrl_Page = v.Rec.j04ViewUrl_Page;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                var j05ids = new List<int>();
                j05ids.InsertRange(0, v.SelectedJ05IDs_EPIS1);
                j05ids.InsertRange(0, v.SelectedJ05IDs_EPIS2);
                j05ids.InsertRange(0, v.SelectedJ05IDs_PORTAL);

                c.pid = Factory.j04UserRoleBL.Save(c,j05ids,v.lisJ08.Where(p=>p.IsTempDeleted==false).ToList());
                if (c.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }


            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(Models.Record.j04Record v)
        {
            var lisJ05 = Factory.FBL.GetListJ05();
            v.lisJ05_EPIS1 = lisJ05.Where(p => p.j05PermFlag==BO.j05PermFlagEnum.EPIS1);
            v.lisJ05_EPIS2 = lisJ05.Where(p => p.j05PermFlag == BO.j05PermFlagEnum.EPIS2);
            v.lisJ05_PORTAL = lisJ05.Where(p => p.j05PermFlag == BO.j05PermFlagEnum.PORTAL);
            if (v.lisJ08 == null)
            {
                v.lisJ08 = new List<BO.j08UserRole_EventType>();
            }
        }


    }
}