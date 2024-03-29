﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class AdminOneFormController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;

        public AdminOneFormController(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;            
        }

        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var f06id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(f06id), Factory, _colsProvider);

            return c.Event_HandleTheGridFilter(tgi, filter);

        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var f06id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(f06id), Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);      

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var f06id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(f06id), Factory, _colsProvider);
            
            return c.Event_HandleTheGridMenu(tgi);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var f06id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(f06id), Factory, _colsProvider);
            
            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }        
        //-----------Konec GRID událostí-------------

        public IActionResult Index(int f06id,string view)
        {            
            var v = new AdminOneForm() { f06ID = f06id,view=view };
            v.IsShowF19ID = Factory.CBL.LoadUserParamBool("AdminOneForm-IsShowF19ID", false);
            if (string.IsNullOrEmpty(v.view) == true)
            {
                v.view = Factory.CBL.LoadUserParam("AdminOneForm-View-", "tree");
            }
            else
            {
                Factory.CBL.SetUserParam("AdminOneForm-View-", v.view);
            }
            if (v.f06ID == 0)
            {
                v.f06ID = Factory.CBL.LoadUserParamInt("AdminOneForm-f06ID");
            }
            else
            {
                if (v.f06ID > 0)
                {
                    Factory.CBL.SetUserParam("AdminOneForm-f06ID", v.f06ID.ToString());
                }
            }
            if (v.f06ID == 0)
            {
                return RedirectToAction("Forms", "Admin",new { prefix = "f06" });
            }
            v.RecF06 = Factory.f06FormBL.Load(v.f06ID);
            if (v.RecF06 == null)
            {
                return this.StopPage(false, "Formulář nelze načíst.",true);
            }

            v.gridinput = GetGridInput(v.f06ID);
            
            if (v.view=="tree")
            {
                v.TreeStateCookieName = "AdminOneForm_tree1_expanded_" + v.f06ID.ToString();
                inhale_tree(v);
            }
            return ViewTup(v, BO.j05PermValuEnum.FormDesigner);
        }

        private TheGridInput GetGridInput(int f06id)
        {
            var gi = new TheGridInput() { entity = "f19Question",controllername="AdminOneForm" };
            gi.query = new BO.myQueryF19() { f06id = f06id };
            gi.viewstate = f06id.ToString();
            return gi;
        }


        private void inhale_tree(UI.Models.AdminOneForm v)
        {            
            v.treeNodes = new List<myTreeNode>();
            
            var mq = new BO.myQuery("f18FormSegment");
            mq.f06id = v.f06ID;
            var lis = Factory.f18FormSegmentBL.GetList(mq);
           
            var lisF19 = Factory.f19QuestionBL.GetList(new BO.myQueryF19() { f06id = v.f06ID },false);
            mq = new BO.myQuery("f26BatteryBoard");
            mq.f06id = v.f06ID;
            var lisF26 = Factory.f26BatteryBoardBL.GetList(mq);
            int x = 0;
            

            foreach (var recF18 in lis)
            {
                var c = new myTreeNode()
                {
                    TreeIndex = recF18.f18TreeIndex*100000,
                    TreeLevel = recF18.f18TreeLevel,
                    Text = recF18.f18Name,
                    TreeIndexFrom = recF18.f18TreeIndexFrom * 100000,
                    TreeIndexTo = recF18.f18TreeIndexTo * 100000,
                    Pid = recF18.pid,
                    ParentPid = recF18.f18ParentID,
                    Prefix = "f18",
                    CssClass="segment",
                    ImgUrl="/images/bullet4.gif"

                };
                if (recF18.f18Ordinal != 0)
                {
                    c.Text += " #" + recF18.f18Ordinal.ToString();
                }
                if (recF18.isclosed)
                {
                    c.CssClass = "closed_item";
                }
                v.treeNodes.Add(c);
                
                foreach(var recF26 in lisF26.Where(p => p.f18ID == recF18.pid))
                {
                                        
                    var cF26 = new myTreeNode()
                    {
                        Pid = recF26.pid,
                        ParentPid = recF18.f18ID,
                        Text = recF26.f26Name,
                        Prefix = "f26",
                        TreeIndex = c.TreeIndex + 1,
                        TreeLevel = c.TreeLevel + 1,
                        TreeIndexFrom = c.TreeIndex + 1,
                        TreeIndexTo = c.TreeIndex + 1,
                        ImgUrl = "/images/battery.png"
                    };
                    if (recF26.f26Ordinal != 0)
                    {
                        cF26.Text += " #" + recF26.f26Ordinal.ToString();
                    }
                    if (recF26.isclosed)
                    {
                        cF26.CssClass = "closed_item";
                    }
                    v.treeNodes.Add(cF26);

                    x = 0;
                    foreach (var recF19In in lisF19.Where(p => p.f26ID == cF26.Pid && p.f18ID==recF18.pid)){
                        x += 1;
                        var cIN = new myTreeNode()
                        {
                            Pid = recF19In.pid,
                            ParentPid = cF26.Pid,
                            Text = recF19In.f19Name,                           
                            Prefix = "f19",
                            TreeIndex = cF26.TreeIndex + x,
                            TreeLevel = cF26.TreeLevel + 1,
                            TreeIndexFrom = cF26.TreeIndex + x,
                            TreeIndexTo = cF26.TreeIndex + x,
                            ImgUrl = "/images/"+recF19In.Icon
                        };
                        if (v.IsShowF19ID)
                        {
                            cIN.Text = cIN.Pid.ToString() + " - " + cIN.Text;
                        }
                        if (recF19In.f19Ordinal != 0)
                        {
                            cIN.Text += " #" + recF19In.f19Ordinal.ToString();
                        }
                        
                        if (recF19In.f19IsRequired)
                        {
                            cIN.CssClass += " required";
                        }
                        if (recF19In.isclosed)
                        {
                            cIN.CssClass += " closed_item";
                        }
                        if (cIN.CssClass != null) cIN.CssClass = cIN.CssClass.Trim();
                        v.treeNodes.Add(cIN);
                        cF26.TreeIndexTo = cF26.TreeIndexFrom + x;
                    }
                }

                x = 0;
                int intCount = lisF19.Where(p => p.f18ID == recF18.pid && p.f26ID == 0).Count();
                if (intCount > 0)
                {
                    c.TreeIndexTo = c.TreeIndexTo + intCount;
                    foreach (var recP19 in lisF19.Where(p => p.f18ID == recF18.pid && p.f26ID==0))
                    {
                        x += 1;
                        c = new myTreeNode()
                        {
                            Pid = recP19.pid,
                            ParentPid = recF18.f18ID,
                            Text = recP19.f19Name,
                            Prefix = "f19",
                            TreeIndex = recF18.f18TreeIndex * 100000 + x,
                            TreeLevel = recF18.f18TreeLevel + 1,
                            TreeIndexFrom = recF18.f18TreeIndex * 100000 + x,
                            TreeIndexTo = recF18.f18TreeIndex * 100000 + x,
                            ImgUrl="/images/"+recP19.Icon
                        };
                        if (v.IsShowF19ID)
                        {
                            c.Text = c.Pid.ToString() + " - " + c.Text;
                        }
                        if (recP19.f19Ordinal != 0)
                        {
                            c.Text += " #" + recP19.f19Ordinal.ToString();
                        }
                        if (recP19.f19IsRequired)
                        {
                            c.CssClass += " required";
                        }
                        if (recP19.isclosed)
                        {
                            c.CssClass += "closed_item";
                        }
                        if (c.CssClass != null) c.CssClass = c.CssClass.Trim();
                        
                        v.treeNodes.Add(c);
                    }
                }
                

            }

            

            var arrExpanded = BO.BAS.ConvertString2List(HttpContext.Request.Cookies[v.TreeStateCookieName], "|");
            foreach (var c in arrExpanded.Where(p=>!string.IsNullOrEmpty(p)))
            {
                if (v.treeNodes.Where(p => p.Pid.ToString() + "-" + p.TreeLevel.ToString() == c).Count()>0)
                {
                    v.treeNodes.Where(p => p.Pid.ToString() + "-" + p.TreeLevel.ToString() == c).First().Expanded = true;
                }
            }

            

        }
    }
}