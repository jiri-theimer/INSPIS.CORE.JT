using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a39Record : BaseRecordViewModel
    {
        public BO.a39InstitutionPerson Rec { get; set; }

        public BO.a03Institution RecA03 { get; set; }
        public int a03ID { get; set; }
        public string Person { get; set; }  //kvůli combo
        public string RoleName { get; set; }    //kvůli combo


        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
