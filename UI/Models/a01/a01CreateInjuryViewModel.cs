using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01CreateInjuryViewModel: BaseViewModel
    {
        
        public int a10ID { get; set; }
        public BO.a10EventType RecA10 { get; set; }
        public int a03ID { get; set; }
        public BO.a03Institution RecA03 { get; set; }

        public int a37ID { get; set; }
        public string a37Name { get; set; }
    }
}
