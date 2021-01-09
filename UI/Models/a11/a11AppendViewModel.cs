using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11AppendViewModel: BaseViewModel
    {
        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public List<BO.a11EventForm> lisA11 { get; set; }
        public IEnumerable<BO.a11EventForm> lisA11Saved { get; set; }

        public int SelectedF06ID { get; set; }
        public string SelectedForm { get; set; }
        public int SelectedKolikrat { get; set; }

        public int SelectedA37ID { get; set; }
        public string SelectedA37Name { get; set; }

        public int SelectedA25ID { get; set; }
        public string SelectedA25Name { get; set; }

        public string a11Description { get; set; }
       
    }
}
