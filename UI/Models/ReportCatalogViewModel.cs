using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ReportCatalogViewModel: BaseViewModel
    {
        public IEnumerable<BO.x32ReportType> lisX32 { get; set; }
        public IEnumerable<BO.x31Report> lisX31 { get; set; }

        public string TreeState { get; set; }
        public List<UI.Models.myTreeNode> treeNodes { get; set; }

       
        public string TreeStateCookieName { get; set; }
    }


    
}
