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
    }
}
