using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class Pokus:BaseViewModel
    {
        public string TreeState { get; set; }

        public List<UI.Models.myTreeNode> treeNodes { get; set; }

        public List<UI.Models.kendoTreeItem> kendoItems { get; set; }

        public List<UI.Models.myTreeNode> treeItems { get; set; }

        public string JsonTreeDatasource { get; set; }

        public int SelectedID { get; set; }
        public int a10ID { get; set; }
        public int a08ID { get; set; }

        public TheGridInput gridinput { get; set; }
        public string ExtendPagerHtml { get; set; }
    }
}
