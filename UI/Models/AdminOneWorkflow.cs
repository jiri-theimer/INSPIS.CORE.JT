using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class AdminOneWorkflow:BaseViewModel
    {
        public int b01ID { get; set; }
        public string view { get; set; }
        public BO.b01WorkflowTemplate RecB01 { get; set; }

        public int go2pid { get; set; }


        public string TreeState { get; set; }
        public List<UI.Models.myTreeNode> treeNodes { get; set; }

        public TheGridInput gridinput { get; set; }
        
    }
}
