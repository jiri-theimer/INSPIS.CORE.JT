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
            return ViewTup(v, BO.j05PermValuEnum.WorkflowDesigner);
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
                var c = new myTreeNode()
                {                    
                    TreeIndex = x,
                    TreeLevel = 1,
                    Text = recB02.b02Name,
                    TreeIndexFrom = x,
                    TreeIndexTo = x,
                    Pid = recB02.pid,                    
                    Prefix = "b02"                    

                };
                if (recB02.isclosed)
                {
                    c.CssClass = "closed_item";
                }
                if (recB02.b02Color != null)
                {
                    c.Text += "<span style='background-color:" + recB02.b02Color + ";'>&nbsp;&nbsp;&nbsp;&nbsp;</span>";
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
                        TreeIndexTo = x
                    };
                    if (recB06.isclosed)
                    {
                        cB06.CssClass = "closed_item";
                    }
                    if (recB06.b02ID_Target > 0)
                    {
                        cB06.Text += " -> " + recB06.TargetStatus;
                    }
                    v.treeNodes.Add(cB06);
                    c.TreeIndexTo = x;
                }
            }
        }
    }
}