using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class AdminOneForm: BaseViewModel
    {
        public int f06ID { get; set; }
        public string view { get; set; }
        public BO.f06Form RecF06 { get; set; }

        public int go2pid { get; set; }


        public string TreeState { get; set; }
        public List<UI.Models.myTreeNode> treeNodes { get; set; }
        
        public bool IsShowF19ID { get; set; }
        public BO.myQuery myQueryGrid { get; set; }
    }
}
