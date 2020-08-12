using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a29Record:BaseRecordViewModel
    {
        public BO.a29InstitutionList Rec { get; set; }
        public string a03IDs { get; set; }

        public int SelectedA03ID { get; set; }
        public string SelectedInstitution { get; set; }
    }
}
