using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class PokusController : BaseController
    {
        public IActionResult PokusReport()
        {
            return View();
        }
        public IActionResult Strom()
        {
            var v = new UI.Models.Pokus();

            var mq = new BO.myQuery("f32");
            mq.a11id = 1640815;
            mq.f19id = 216138;
            var c = Factory.f32FilledValueBL.Load(3);

            c.pid = 0;
            c.a11ID = 0;


            var x = Factory.f32FilledValueBL.Save(c,true);
            

           

            return View(v);
        }

        [HttpPost]
        public IActionResult Strom(Models.Pokus v)
        {
            RefreshState(v);
            return View(v);
        }


        private void RefreshState(UI.Models.Pokus v)
        {
            v.treeNodes = new List<myTreeNode>();

            foreach (var rec in Factory.o13AttachmentTypeBL.GetList(new BO.myQuery("o13AttachmentType")))
            {
                var c = new myTreeNode()
                {
                    TreeIndex = rec.o13TreeIndex,
                    TreeLevel = rec.o13TreeLevel,
                    Text = rec.o13Name,
                    TreeIndexFrom = rec.o13TreeIndexFrom,
                    TreeIndexTo = rec.o13TreeIndexTo,
                    Pid = rec.pid,
                    ParentPid = rec.o13ParentID,
                    Prefix = "o13"

                };
                v.treeNodes.Add(c);

            }
        }
    }

}