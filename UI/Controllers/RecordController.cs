using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class RecordController : BaseController
    {
        public IActionResult GridMultiSelect(string prefix)
        {
            var c = Factory.EProvider.ByPrefix(prefix);
            var v = new GridMultiSelect() { entity = c.TableName,prefix=prefix,entityTitle=c.AliasPlural };

            v.gridinput = GetGridInput(v.entity);
            return View(v);
        }

        private TheGridInput GetGridInput(string entity)
        {
            var gi = new TheGridInput() { entity = entity, controllername = "TheGrid",ondblclick=null,oncmclick=null };            
            gi.query = new BO.InitMyQuery().Load(entity.Substring(0, 3));
            gi.j72id = Factory.CBL.LoadUserParamInt("GridMultiSelect-j72id-"+entity.Substring(0,3));
            return gi;
        }

        public IActionResult RecordValidity(string strD1,string strD2)
        {
            
            var v = new RecordValidity();
            if (strD1 == null)
            {
                v.d1 = DateTime.Now;
            }
            else
            {
                v.d1 = BO.BAS.String2Date(strD1);
            }
            if (strD2 == null)
            {
                v.d2 = new DateTime(3000,1,1);
            }
            else
            {
                v.d2 = BO.BAS.String2Date(strD2);
                if (v.d2.Year < 3000)
                {
                    v.d2= v.d2.AddDays(1).AddMinutes(-1);
                }
            }
            return View(v);
        }
        [HttpPost]
        public IActionResult RecordValidity(RecordValidity v,string oper)
        {
           if (oper == "now")
            {
                v.d2 = DateTime.Now;
                v.IsAutoClose = true;
               
            }
            return View(v);

        }
    }
}