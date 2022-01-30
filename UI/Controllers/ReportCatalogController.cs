using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class ReportCatalogController : BaseController
    {
        public IActionResult Index()
        {
            var v = new ReportCatalogViewModel();

            v.TreeStateCookieName = "ReportCatalog_tree1_expanded";
            inhale_tree(v);

            return View(v);
        }



        private void inhale_tree(ReportCatalogViewModel v)
        {
            v.treeNodes = new List<myTreeNode>();
            var mq = new BO.myQuery("x32");
            v.lisX32 = Factory.x32ReportTypeBL.GetList(mq).OrderBy(p => p.x32TreeIndex);

            var mqX31 = new BO.myQueryX31();
            mqX31.CurrentUser = Factory.CurrentUser;
            mqX31.x31is4singlerecord = false;
            v.lisX31 = Factory.x31ReportBL.GetList(mqX31);
            var lisX34 = Factory.x31ReportBL.GetListX34();

            int x = 0;
            
            foreach (var recX32 in v.lisX32)
            {                
               
                var c = new myTreeNode()
                {
                    TreeIndex = recX32.x32TreeIndex,
                    TreeLevel = recX32.x32TreeLevel,
                    Text = recX32.x32Name,
                    TreeIndexFrom = recX32.x32TreeIndexFrom,
                    TreeIndexTo = recX32.x32TreeIndexTo,
                    ParentPid=recX32.x32ParentID,
                    Pid = recX32.pid,
                    Prefix = "x32",
                    Expanded=true,                            
                    TextOcas = "transparent"

                };

                x = lisX34.Where(p => p.x32ID == recX32.pid).Count();
                if (x > 0)
                {
                    c.Text += " (" + x.ToString() + ")";
                }

                v.treeNodes.Add(c);

            }

            var arrExpanded = BO.BAS.ConvertString2List(HttpContext.Request.Cookies[v.TreeStateCookieName], "|");
            foreach (var c in arrExpanded.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (v.treeNodes.Where(p => p.Pid.ToString() + "-" + p.TreeLevel.ToString() == c).Count() > 0)
                {
                    v.treeNodes.Where(p => p.Pid.ToString() + "-" + p.TreeLevel.ToString() == c).First().Expanded = true;
                }
            }
        }
    }
}
