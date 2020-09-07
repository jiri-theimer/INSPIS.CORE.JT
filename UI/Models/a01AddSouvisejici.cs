using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01AddSouvisejici: BaseViewModel
    {
        public BO.a01Event RecA01 { get; set; }
        public BO.a01Event RecA01Selected { get; set; }
        public int a01id { get; set; }

        public int SelectedA01ID { get; set; }
        public string SearchBox { get; set; }
        public int SelectedA46ID { get; set; }
        public string a24Description { get; set; }
        
    }
}
