using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a39RecordByPerson: BaseRecordViewModel
    {
        public BO.a39InstitutionPerson Rec { get; set; }

        public BO.j02Person RecJ02 { get; set; }


        public int j02ID { get; set; }
        public string a03Name { get; set; }  //kvůli combo
        public string RoleName { get; set; }    //kvůli combo


        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
