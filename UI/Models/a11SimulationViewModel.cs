using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11SimulationViewModel: BaseViewModel
    {
        public int f06ID { get; set; }
        public int a01ID { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public BO.f06Form RecF06 { get; set; }
        public List<BO.a11EventForm> lisA11 { get; set; }

        public int a10ID { get; set; }
        public IEnumerable<BO.a10EventType> lisA10 { get; set; }
        public int a08ID { get; set; }
        public IEnumerable<BO.a08Theme> lisA08 { get; set; }

        public int a03ID { get; set; }
        public string a03Name { get; set; }

        public int a37ID { get; set; }
        public IEnumerable<BO.a37InstitutionDepartment> lisA37 { get; set; }
    }
}
