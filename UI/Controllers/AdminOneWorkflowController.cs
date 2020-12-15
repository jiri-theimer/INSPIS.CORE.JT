using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class AdminOneWorkflowController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        public AdminOneWorkflowController(BL.TheColumnsProvider cp)
        {
            _colsProvider = cp;
        }

        //-----------Začátek GRID událostí-------------
        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.StringPair> pathpars, List<BO.TheGridColumnFilter> filter) //TheGrid povinná metoda: sloupcový filtr
        {
            var b01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(b01id),Factory, _colsProvider);
            
            return c.Event_HandleTheGridFilter(tgi, filter);
        }
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi, List<BO.StringPair> pathpars)    //TheGrid povinná metoda: změna třídění, pageindex, změna stránky
        {
            var b01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(b01id), Factory, _colsProvider);

            return c.Event_HandleTheGridOper(tgi);

        }
        public string HandleTheGridMenu(TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda: zobrazení grid menu
        {
            var b01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(b01id), Factory, _colsProvider);
            return c.Event_HandleTheGridMenu(tgi.j72id);
        }
        public TheGridExportedFile HandleTheGridExport(string format, string pids, TheGridUIContext tgi, List<BO.StringPair> pathpars)  //TheGrid povinná metoda pro export dat
        {
            var b01id = Convert.ToInt32(tgi.viewstate[0]);
            var c = new UI.TheGridSupport(GetGridInput(b01id), Factory, _colsProvider);

            return c.Event_HandleTheGridExport(format, tgi.j72id, pids);
        }
        //-----------Konec GRID událostí-------------

        public IActionResult Index(int b01id, string view)
        {
            var v = new AdminOneWorkflow() { b01ID = b01id, view = view };
            if (string.IsNullOrEmpty(v.view) == true)
            {
                v.view = Factory.CBL.LoadUserParam("AdminOneWorkflow-View-", "tree");
            }
            else
            {
                Factory.CBL.SetUserParam("AdminOneWorkflow-View-", v.view);
            }
            if (v.b01ID == 0)
            {
                v.b01ID = Factory.CBL.LoadUserParamInt("AdminOneWorkflow-b01ID");
            }
            else
            {
                if (v.b01ID > 0)
                {
                    Factory.CBL.SetUserParam("AdminOneWorkflow-b01ID", v.b01ID.ToString());
                }
            }
            if (v.b01ID == 0)
            {
                return RedirectToAction("Workflow", "Admin", new { prefix = "b01" });
            }
            v.RecB01 = Factory.b01WorkflowTemplateBL.Load(v.b01ID);
            if (v.RecB01 == null)
            {
                return this.StopPage(false, "Workflow šablonu nelze načíst.");
            }

            if (v.view == "tree")
            {
                inhale_tree(v);
            }

            v.gridinput = GetGridInput(v.b01ID);
            

            return ViewTup(v, BO.j05PermValuEnum.WorkflowDesigner);
        }

        private TheGridInput GetGridInput(int b01id)
        {
            var gi = new TheGridInput() { entity = "b02WorkflowStatus",controllername="AdminOneWorkflow" };
            gi.query= new BO.myQuery("b02") { b01id = b01id };
            gi.viewstate = b01id.ToString();
            return gi;
        }

        private void inhale_tree(UI.Models.AdminOneWorkflow v)
        {
            v.treeNodes = new List<myTreeNode>();
            var mq = new BO.myQuery("b02");
            mq.b01id = v.b01ID;
            var lisB02 = Factory.b02WorkflowStatusBL.GetList(mq);
            mq = new BO.myQuery("b06");
            mq.b01id = v.b01ID;
            var lisB06 = Factory.b06WorkflowStepBL.GetList(mq);

            int x = 0;

            foreach (var recB02 in lisB02)
            {
                x += 1;
                if (recB02.b02IsDefaultStatus)
                {
                    recB02.b02Name += "**";
                }
                var c = new myTreeNode()
                {                    
                    TreeIndex = x,
                    TreeLevel = 1,
                    Text = recB02.b02Name,                   
                    TreeIndexFrom = x,
                    TreeIndexTo = x,
                    Pid = recB02.pid,                    
                    Prefix = "b02",
                    ImgUrl="/images/ministamp.gif",
                    CssClass="stav",
                    Expanded=true,
                    TextOcas="transparent"

                };
                if (recB02.isclosed)
                {
                    c.CssClass = "closed_item";
                }
                if (recB02.b02Color != null)
                {
                    c.TextOcas = recB02.b02Color;
                }
                v.treeNodes.Add(c);

                foreach (var recB06 in lisB06.Where(p => p.b02ID == recB02.pid))
                {
                    x += 1;
                    var cB06 = new myTreeNode()
                    {
                        Pid = recB06.pid,
                        ParentPid = c.Pid,                        
                        Prefix = "b06",
                        Text=recB06.b06Name,
                        TreeIndex = x,
                        TreeLevel = 2,
                        TreeIndexFrom = x,
                        TreeIndexTo = x,
                        ImgUrl= "/images/bullet2.gif",
                        TextOcas = "transparent"
                    };
                    if (recB06.isclosed)
                    {
                        cB06.CssClass = "closed_item";
                    }
                    if (recB06.b02ID_Target > 0)
                    {
                        cB06.Text += " -> " + recB06.TargetStatus;
                        cB06.ImgUrl = "/images/bullet1.gif";
                    }
                    v.treeNodes.Add(cB06);
                    c.TreeIndexTo = x;
                }
            }
        }
    }
}