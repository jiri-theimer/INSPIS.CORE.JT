using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class x55Controller : BaseController
    {
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new x55Record() { rec_pid = pid, rec_entity = "x55" };
            v.Rec = new BO.x55Widget();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.x55WidgetBL.Load(v.rec_pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

                var mq = new BO.myQuery("j04") { x55id = v.rec_pid, explicit_orderby = "j04Name" };
                v.j04IDs = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.pid));
                v.j04Names = string.Join(",", Factory.j04UserRoleBL.GetList(mq).Select(p => p.j04Name));
            }
            

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return ViewTup(v,BO.j05PermValuEnum.AdminGlobal);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.Record.x55Record v)
        {

            if (ModelState.IsValid)
            {
                BO.x55Widget c = new BO.x55Widget();
                if (v.rec_pid > 0) c = Factory.x55WidgetBL.Load(v.rec_pid);
                c.x55Name = v.Rec.x55Name;
                c.x55Code = v.Rec.x55Code;
                c.x55IsSystem = v.Rec.x55IsSystem;
                c.x55TypeFlag = v.Rec.x55TypeFlag;
                c.x55TableSql = v.Rec.x55TableSql;
                c.x55TableColHeaders = v.Rec.x55TableColHeaders;
                c.x55TableColTypes = v.Rec.x55TableColTypes;
                c.x55Content = v.Rec.x55Content;
                c.x55Ordinal = v.Rec.x55Ordinal;
                c.x55Image = v.Rec.x55Image;
                c.x55Description = v.Rec.x55Description;
                c.x55BoxBackColor = v.Rec.x55BoxBackColor;
                c.x55HeaderForeColor = v.Rec.x55HeaderForeColor;
                c.x55HeaderBackColor = v.Rec.x55HeaderBackColor;
                c.x55BoxMaxHeight = v.Rec.x55BoxMaxHeight;
                c.x55DataTablesLimit = v.Rec.x55DataTablesLimit;
                c.x55DataTablesButtons = v.Rec.x55DataTablesButtons;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);

                c.pid = Factory.x55WidgetBL.Save(c,j04ids);
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