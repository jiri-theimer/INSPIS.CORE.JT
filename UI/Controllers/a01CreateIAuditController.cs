using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;


namespace UI.Controllers
{
    public class a01CreateIAuditController : BaseController
    {
        public IActionResult Index(int a03id)      //založení 3 akce v rámci IA
        {
            var v = new a01CreateIAudit() { a03ID = a03id,a01DateFrom=DateTime.Today.AddDays(1),a01DateUntil=DateTime.Today.AddDays(5) };
            if (a03id > 0)
            {
                v.a03ID = a03id;
                v.Institution = Factory.a03InstitutionBL.Load(a03id).NamePlusRedizo;
            }
            RefreshState(v);
            return View(v);
        }

        [HttpPost]
        public IActionResult Index(a01CreateIAudit v, string oper, string guid,int f06id,int krat)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "a08_change_master")
            {
                v.lisA11_Master.Clear();
                return View(v);
            }
            if (oper == "a08_change_slave1")
            {
                v.lisA11_Slave1.Clear();
                return View(v);
            }
            if (oper == "a08_change_slave2")
            {
                v.lisA11_Slave2.Clear();
                return View(v);
            }
            if (oper == "f06_add_master" && f06id > 0)
            {
                for (int i = 1; i <= krat; i++)
                {
                    var c = new BO.a11EventForm() { f06ID = f06id, TempGuid = BO.BAS.GetGuid() };
                    c.f06Name = Factory.f06FormBL.Load(f06id).f06Name;
                    v.lisA11_Master.Add(c);
                }

                return View(v);
            }
            if (oper == "f06_delete_master")
            {
                v.lisA11_Master.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "f06_clear_master")
            {
                v.lisA11_Master.Clear();
                return View(v);
            }

            if (oper == "j02_add" && v.SelectedJ02ID > 0)
            {
                var c = new BO.a41PersonToEvent() { TempGuid = BO.BAS.GetGuid() };

                if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                {
                    if (v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).Count() > 0)
                    {
                        c = v.lisA41.Where(p => p.j02ID == v.SelectedJ02ID).First();
                        if (c.IsTempDeleted == true)
                        {
                            c.IsTempDeleted = false;
                            return View(v);
                        }
                        else
                        {
                            this.AddMessage("Tato osoba již je v seznamu.");
                            return View(v);
                        }
                    }
                }
                c.j02ID = v.SelectedJ02ID;
                c.PersonCombo = Factory.j02PersonBL.Load(c.j02ID).FullNameDesc;
                c.a45ID = BO.EventRoleENUM.Resitel;
                c.a45Name = Factory.FBL.LoadA45((int)c.a45ID).a45Name;
                c.a45IsManual = true;
                v.lisA41.Add(c);
                return View(v);
            }
            if (oper == "j02_delete")
            {
                v.lisA41.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }


            if (ModelState.IsValid)
            {


            }

            this.Notify_RecNotSaved();
            return View(v);

        }

        public void RefreshState(a01CreateIAudit v)
        {
            if (v.RecA01_Master == null) v.RecA01_Master = new BO.a01Event();
            if (v.RecA01_Slave1 == null) v.RecA01_Slave1 = new BO.a01Event();
            if (v.RecA01_Slave2 == null) v.RecA01_Slave2 = new BO.a01Event();
            if (v.a10ID_Master == 0)
            {
                v.RecA10_Master = new BO.a10EventType();
            }
            else
            {
                v.RecA10_Master = Factory.a10EventTypeBL.Load(v.a10ID_Master);
            }
            this.AddMessage(v.RecA10_Master.a10Name);
            if (v.a10ID_Slave1 == 0)
            {
                v.RecA10_Slave1 = new BO.a10EventType();
            }
            else
            {
                v.RecA10_Slave1 = Factory.a10EventTypeBL.Load(v.a10ID_Slave1);
            }
            if (v.a10ID_Slave2 == 0)
            {
                v.RecA10_Slave2 = new BO.a10EventType();
            }
            else
            {
                v.RecA10_Slave2 = Factory.a10EventTypeBL.Load(v.a10ID_Slave2);
            }

            if (v.lisA41 == null)
            {
                v.lisA41 = new List<BO.a41PersonToEvent>();
            }
            if (v.lisA11_Master == null)
            {
                v.lisA11_Master = new List<BO.a11EventForm>();
            }
            if (v.lisA11_Slave1 == null)
            {
                v.lisA11_Slave1 = new List<BO.a11EventForm>();
            }
            if (v.lisA11_Slave2 == null)
            {
                v.lisA11_Slave2 = new List<BO.a11EventForm>();
            }


            if (v.RecA01_Master.a08ID > 0)
            {
                v.lisA12_Master = Factory.a08ThemeBL.GetListA12(v.RecA01_Master.a08ID);
            }
            if (v.RecA01_Slave1.a08ID > 0)
            {
                v.lisA12_Slave1 = Factory.a08ThemeBL.GetListA12(v.RecA01_Slave1.a08ID);
            }
            if (v.RecA01_Slave2.a08ID > 0)
            {
                v.lisA12_Slave2 = Factory.a08ThemeBL.GetListA12(v.RecA01_Slave2.a08ID);
            }
        }
    }
}
