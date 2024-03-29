﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult LogAsUser(string login, string code)
        {
            var v = new AdminLogAsUser() { Login = login, Code = code };

            return ViewTup(v, BO.j05PermValuEnum.AdminGlobal);
        }
        [HttpPost]
        public IActionResult LogAsUser(AdminLogAsUser v)
        {
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(v.Login) || string.IsNullOrEmpty(v.Code))
                {
                    this.AddMessage("Login i Ověřovací kód je povinné zadat."); return View(v);
                }
                var recJ03 = Factory.j03UserBL.LoadByLogin(v.Login, 0);
                if (recJ03 == null)
                {
                    this.AddMessage("Zadaný login neexistuje."); return View(v);
                }
                if (v.Code != Factory.CurrentUser.j03Login.Substring(0, 2)+BO.BAS.ObjectDateTime2String(DateTime.Now,"ddHH"))
                {
                    this.AddMessage("Ověřovací kód není správný."); return View(v);
                }

                if (recJ03.j02Email == null) { recJ03.j02Email = "info@marktime.cz"; };
                var userClaims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, recJ03.j03Login),
                new Claim("access_token","inspis_core_token"),
                new Claim(ClaimTypes.Email, recJ03.j02Email)
                 };

                var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                var xx = new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.Now.AddHours(1) };
                HttpContext.SignInAsync(userPrincipal, xx);

                if (recJ03 != null)
                {

                    return Redirect("/Home/Index");
                    
                }

            }

            return View(v);
        }

        public IActionResult CiselnikyX31(int go2pid)   //katalog report šablon
        {
            var v = new AdminPageX31() { prefix = "x31", go2pid = go2pid };
            v.QueryX32ID = Factory.CBL.LoadUserParamInt("Admin/CiselnikyX31-x32id");
            v.lisX32 = Factory.x32ReportTypeBL.GetList(new BO.myQuery("x32"));

            v.gridinput = new TheGridInput() { entity = "x31Report", go2pid = v.go2pid };            
            v.gridinput.query = new BO.myQueryX31() { x32id = v.QueryX32ID };
            v.gridinput.myqueryinline = "x32id@int@" + v.QueryX32ID.ToString();            
            

            if (Factory.CBL.LoadUserParam("Admin /Ciselniky-prefix") != "x31")
            {
                Factory.CBL.SetUserParam("Admin/Ciselniky-prefix", "x31");
            }

            return View(v);
            
        }

        public IActionResult Ciselniky(string prefix, int go2pid, string view)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid, view = view };
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam("Admin/Ciselniky-prefix","a02");
            }
            else
            {
                if (Factory.CBL.LoadUserParam("Admin/Ciselniky-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam("Admin/Ciselniky-prefix", v.prefix);
                }
            }
            if (v.prefix == "x31")
            {
                return RedirectToAction("CiselnikyX31", new { go2pid = go2pid });    //report šablony mají vlastní stránku
            }
            inhale_entity(ref v, v.prefix);
            
            v.gridinput = GetGridInput(v.entity,v.prefix,go2pid=v.go2pid);
            if (v.prefix == "o13" || v.prefix == "x32")
            {
                if (string.IsNullOrEmpty(v.view) == true)
                {
                    v.view = Factory.CBL.LoadUserParam("Admin-Ciselniky-View-" + v.prefix, "tree");
                }
                else
                {
                    Factory.CBL.SetUserParam("Admin-Ciselniky-View-" + v.prefix, v.view);
                }
                if (v.prefix == "o13" && v.view == "tree")
                {
                    inhale_tree_o13(v);
                }
                if (v.prefix == "x32" && v.view == "tree")
                {
                    inhale_tree_x32(v);
                }
            }

            return View(v);
        }
        public IActionResult Users(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam("Admin/Users-prefix","j03");
            }
            else
            {
                if (Factory.CBL.LoadUserParam("Admin/Users-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam("Admin/Users-prefix", v.prefix);
                }
            }
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,v.go2pid);
            return View(v);
        }
        public IActionResult Forms(string prefix, int go2pid, string view)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid, view = view };
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam("Admin/Forms-prefix","f06");
            }
            else
            {
                if (Factory.CBL.LoadUserParam("Admin/Forms-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam("Admin/Forms-prefix", v.prefix);
                }
            }
            inhale_entity(ref v, v.prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,v.go2pid);
            if (v.prefix == "f12")
            {
                if (string.IsNullOrEmpty(v.view) == true)
                {
                    v.view = Factory.CBL.LoadUserParam("Admin-Forms-View-" + v.prefix, "tree");
                }
                else
                {
                    Factory.CBL.SetUserParam("Admin-Forms-View-" + v.prefix, v.view);
                }
                if (v.view == "tree")
                {
                    inhale_tree_f12(v);
                }
            }

            return View(v);
        }
        public IActionResult Workflow(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            inhale_entity(ref v, prefix);
            v.gridinput = GetGridInput(v.entity,v.prefix,v.go2pid);
            return View(v);
        }

        public IActionResult Kategorie(string prefix, int go2pid)
        {
            var v = new AdminPage() { prefix = prefix, go2pid = go2pid };
            if (v.prefix == null)
            {
                v.prefix = Factory.CBL.LoadUserParam("Admin/Kategorie-prefix", "o53");
            }
            else
            {
                if (Factory.CBL.LoadUserParam("Admin/Kategorie-prefix") != v.prefix)
                {
                    Factory.CBL.SetUserParam("Admin/Kategorie-prefix", v.prefix);
                }
            }
            inhale_entity(ref v, v.prefix);
            
            v.gridinput = GetGridInput(v.entity, v.prefix, v.go2pid);
            return View(v);
        }


        private void inhale_entity(ref AdminPage v, string prefix)
        {
            if (prefix != null)
            {
                var c = Factory.EProvider.ByPrefix(prefix);
                v.entity = c.TableName;
                switch (Factory.CurrentUser.j03LangIndex)
                {
                    case 1:
                        v.entityTitle = c.TranslateLang1;
                        break;
                    case 2:
                        v.entityTitle = c.TranslateLang2;
                        break;
                    default:
                        v.entityTitle = c.AliasPlural;
                        break;
                }
                
            }
        }

        private void inhale_tree_o13(UI.Models.AdminPage v)
        {
            v.treeNodes = new List<myTreeNode>();
            var lis = Factory.o13AttachmentTypeBL.GetList(new BO.myQueryO13());
            foreach (var rec in lis)
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
                    Prefix = "o13",
                    Expanded=true

                };

                v.treeNodes.Add(c);

            }
        }
        private void inhale_tree_x32(UI.Models.AdminPage v)
        {
            v.treeNodes = new List<myTreeNode>();
            var lis = Factory.x32ReportTypeBL.GetList(new BO.myQuery("x32Report"));
            foreach (var rec in lis)
            {
                var c = new myTreeNode()
                {
                    TreeIndex = rec.x32TreeIndex,
                    TreeLevel = rec.x32TreeLevel,
                    Text = rec.x32Name,
                    TreeIndexFrom = rec.x32TreeIndexFrom,
                    TreeIndexTo = rec.x32TreeIndexTo,
                    Pid = rec.pid,
                    ParentPid = rec.x32ParentID,
                    Prefix = "x32",
                    Expanded=true

                };
                v.treeNodes.Add(c);

            }
        }
        private void inhale_tree_f12(UI.Models.AdminPage v)
        {
            v.treeNodes = new List<myTreeNode>();
            var lis = Factory.f12FormTypeBL.GetList(new BO.myQuery("f12FormType"));
            foreach (var rec in lis)
            {
                var c = new myTreeNode()
                {
                    TreeIndex = rec.f12TreeIndex,
                    TreeLevel = rec.f12TreeLevel,
                    Text = rec.f12Name,
                    TreeIndexFrom = rec.f12TreeIndexFrom,
                    TreeIndexTo = rec.f12TreeIndexTo,
                    Pid = rec.pid,
                    ParentPid = rec.f12ParentID,
                    Prefix = "f12",
                    Expanded=true

                };
                v.treeNodes.Add(c);

            }
        }


        public IActionResult System()
        {

            return View();
        }
        public BO.Result GenerateSpGenerateCreateUpdateScript(string scope)
        {
            var lis = Factory.FBL.GetList_SysObjects();
            if (scope == "_core")
            {
                lis = lis.Where(p => p.Name.StartsWith("_core"));
            }
            Factory.FBL.GenerateCreateUpdateScript(lis);

            return new BO.Result(false, "Soubor byl vygenerován (do TEMPu)");
        }

        private TheGridInput GetGridInput(string entity,string prefix,int go2pid)
        {
            var gi = new TheGridInput();
            gi.entity = entity;
            gi.query = new BO.InitMyQuery().Load(prefix);
            gi.go2pid = go2pid;
            gi.j72id = Factory.CBL.LoadUserParamInt("admin-j72id-" + prefix);
            return gi;
        }

    }
}