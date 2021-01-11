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
        public IActionResult Index(int a03id,int a10id_master,int a10id_slave1,int a10id_slave2)      //založení 3 akce v rámci IA
        {
            var v = new a01CreateIAudit() { a03ID = a03id,a01DateFrom=DateTime.Today.AddDays(1),a01DateUntil=DateTime.Today.AddDays(5),a10ID_Master=a10id_master,a10ID_Slave1=a10id_slave1,a10ID_Slave2=a10id_slave2 };
            if (v.a10ID_Master == 0)
            {
                v.a10ID_Master = Factory.CBL.LoadUserParamInt("CreateIAudit-a10ID_Master");                
            }
            if (v.a10ID_Slave1 == 0)
            {
                v.a10ID_Slave1 = Factory.CBL.LoadUserParamInt("CreateIAudit-a10ID_Slave1");
            }
            if (v.a10ID_Slave2== 0)
            {
                v.a10ID_Slave2 = Factory.CBL.LoadUserParamInt("CreateIAudit-a10ID_Slave2");
            }

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
            if (oper== "a10_change_master")
            {
                Factory.CBL.SetUserParam("CreateIAudit-a10ID_Master", v.a10ID_Master.ToString());
                v.RecA01_Master.a08ID = 0;v.RecA01_Master.a08Name = "";
                v.lisA11_Master.Clear();                
                return View(v);
            }
            if (oper == "a10_change_slave1")
            {
                Factory.CBL.SetUserParam("CreateIAudit-a10ID_Slave1", v.a10ID_Slave1.ToString());
                v.RecA01_Slave1.a08ID = 0; v.RecA01_Slave1.a08Name = "";
                v.lisA11_Slave1.Clear();
                return View(v);
            }
            if (oper == "a10_change_slave2")
            {
                Factory.CBL.SetUserParam("CreateIAudit-a10ID_Slave2", v.a10ID_Slave2.ToString());
                v.RecA01_Slave2.a08ID = 0; v.RecA01_Slave2.a08Name = "";
                v.lisA11_Slave2.Clear();
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
            //formulářové operace pro slave1:
            if (oper == "f06_add_slave1" && f06id > 0)
            {
                for (int i = 1; i <= krat; i++)
                {
                    var c = new BO.a11EventForm() { f06ID = f06id, TempGuid = BO.BAS.GetGuid() };
                    c.f06Name = Factory.f06FormBL.Load(f06id).f06Name;
                    v.lisA11_Slave1.Add(c);
                }

                return View(v);
            }
            if (oper == "f06_delete_slave1")
            {
                v.lisA11_Slave1.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "f06_clear_slave1")
            {
                v.lisA11_Slave1.Clear();
                return View(v);
            }

            //formulářové operace pro slave2:
            if (oper == "f06_add_slave2" && f06id > 0)
            {
                for (int i = 1; i <= krat; i++)
                {
                    var c = new BO.a11EventForm() { f06ID = f06id, TempGuid = BO.BAS.GetGuid() };
                    c.f06Name = Factory.f06FormBL.Load(f06id).f06Name;
                    v.lisA11_Slave2.Add(c);
                }

                return View(v);
            }
            if (oper == "f06_delete_slave2")
            {
                v.lisA11_Slave2.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "f06_clear_slave2")
            {
                v.lisA11_Slave2.Clear();
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
                v.RecA01_Master.j02ID_Issuer = Factory.CurrentUser.j02ID;
                v.RecA01_Slave1.a10ID = v.a10ID_Slave1; v.RecA01_Slave1.a03ID = v.a03ID; v.RecA01_Slave1.a01DateFrom = v.a01DateFrom; v.RecA01_Slave1.a01DateUntil = v.a01DateUntil;v.RecA01_Slave1.j02ID_Issuer = Factory.CurrentUser.j02ID;
                v.RecA01_Slave2.a10ID = v.a10ID_Slave2; v.RecA01_Slave2.a03ID = v.a03ID; v.RecA01_Slave2.a01DateFrom = v.a01DateFrom; v.RecA01_Slave2.a01DateUntil = v.a01DateUntil; v.RecA01_Slave2.j02ID_Issuer = Factory.CurrentUser.j02ID;

                if (!ValidateBeforeSave(v))
                {
                    return View(v);
                }
                

                v.RecA01_Master.a10ID = v.a10ID_Master;v.RecA01_Master.a03ID = v.a03ID;v.RecA01_Master.a01DateFrom = v.a01DateFrom;v.RecA01_Master.a01DateUntil = v.a01DateUntil;                
                v.RecA01_Master.pid = Factory.a01EventBL.Create(v.RecA01_Master, true, v.lisA11_Master.Where(p => p.IsTempDeleted == false).ToList(), v.lisA41.Where(p => p.IsTempDeleted == false).ToList(), null, null);
                if (v.RecA01_Master.pid == 0)
                {
                    return View(v);
                }
                
                v.RecA01_Slave1.pid = Factory.a01EventBL.Create(v.RecA01_Slave1, true, v.lisA11_Slave1.Where(p => p.IsTempDeleted == false).ToList(), v.lisA41.Where(p => p.IsTempDeleted == false).ToList(), null, null);
                var recA24 = new BO.a24EventRelation() { a01ID_Right = v.RecA01_Master.pid, a01ID_Left = v.RecA01_Slave1.pid, a46ID = v.a46ID };
                Factory.a01EventBL.SaveA24Record(recA24);

                if (v.RecA10_Slave2.pid > 0)
                {                    
                    v.RecA01_Slave2.pid = Factory.a01EventBL.Create(v.RecA01_Slave2, true, v.lisA11_Slave2.Where(p => p.IsTempDeleted == false).ToList(), v.lisA41.Where(p => p.IsTempDeleted == false).ToList(), null, null);
                    recA24 = new BO.a24EventRelation() { a01ID_Right = v.RecA01_Master.pid, a01ID_Left = v.RecA01_Slave2.pid, a46ID = v.a46ID };
                    Factory.a01EventBL.SaveA24Record(recA24);

                }

                return RedirectToAction("RecPage", "a01", new { pid =v.RecA01_Master.pid });
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
                v.a10Name_Master = v.RecA10_Master.a10Name;
            }
            
            if (v.a10ID_Slave1 == 0)
            {
                v.RecA10_Slave1 = new BO.a10EventType();
            }
            else
            {
                v.RecA10_Slave1 = Factory.a10EventTypeBL.Load(v.a10ID_Slave1);
                v.a10Name_Slave1 = v.RecA10_Slave1.a10Name;
            }
            if (v.a10ID_Slave2 == 0)
            {
                v.RecA10_Slave2 = new BO.a10EventType();
            }
            else
            {
                v.RecA10_Slave2 = Factory.a10EventTypeBL.Load(v.a10ID_Slave2);
                v.a10Name_Slave2 = v.RecA10_Slave2.a10Name;
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

        private bool ValidateBeforeSave(a01CreateIAudit v)
        {    
            if (v.a03ID == 0)
            {
                this.AddMessage("Chybí vyplnit instituci."); return false;
            }
            if (v.a01DateFrom == null || v.a01DateUntil==null || v.a01DateUntil<v.a01DateFrom)
            {
                this.AddMessage("Chybí vyplnit časový plán akce."); return false;
            }
            if (v.lisA41.Where(p => p.a45ID == BO.EventRoleENUM.Vedouci).Count() == 0)
            {
                this.AddMessage("Chybí definice vedoucího akce."); return false;
            }
            if (v.RecA10_Master.pid == 0 || v.RecA01_Master.a08ID==0)
            {
                this.AddMessage("Chybí vyplnit typ nebo téma nadřízené akce."); return false;
            }
            if (v.RecA10_Slave1.pid == 0 || v.RecA01_Slave1.a08ID == 0)
            {
                this.AddMessage("Chybí vyplnit typ nebo téma první podřízené akce."); return false;
            }

            
            if (!Factory.a01EventBL.ValidateA11(v.RecA01_Slave1, v.lisA11_Slave1.Where(p => p.IsTempDeleted == false).ToList()))
            {
                return false;
            }

            if (v.RecA01_Slave2.a10ID > 0)
            {
                if (!Factory.a01EventBL.ValidateA11(v.RecA01_Slave2, v.lisA11_Slave2.Where(p => p.IsTempDeleted == false).ToList()))
                {
                    return false;
                }
            }

            return true;
        }

        
    }
}
