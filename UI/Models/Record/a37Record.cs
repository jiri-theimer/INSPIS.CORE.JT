using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a37Record:BaseRecordViewModel
    {
        public BO.a37InstitutionDepartment Rec { get; set; }

        public BO.a03Institution RecA03 { get; set; }
        public int a03ID { get; set; }
    }
}
