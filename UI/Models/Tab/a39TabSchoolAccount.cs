using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a39TabSchoolAccount: BaseViewModel
    {
        public IEnumerable<BO.a39InstitutionPerson> lisA39 { get; set; }
        public BO.a03Institution RecA03 { get; set; }
        public int a03ID { get; set; }
    }
}
