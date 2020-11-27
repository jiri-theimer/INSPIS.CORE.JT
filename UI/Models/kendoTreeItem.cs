using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class kendoTreeItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public string imageUrl { get; set; }

        
        public string prefix { get; set; }

        public List<kendoTreeItem> items { get; set; }
    }
}
