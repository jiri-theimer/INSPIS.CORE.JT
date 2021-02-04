using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01CreateHelpdeskViewModel: BaseViewModel
    {
        public BO.a01Event Rec { get; set; }
        public int a10ID { get; set; }
        public string a10Name { get; set; }
        public BO.a10EventType RecA10 { get; set; }

        public BO.j02Person RecJ02 { get; set; }

        public int j02ID { get; set; }
        public string Person { get; set; }

        public int a03ID { get; set; }
        public string Institution { get; set; }
        public bool IsComboA03 { get; set; }

        public string UploadGuid { get; set; }


    }
}
