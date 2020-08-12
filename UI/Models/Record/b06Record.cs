using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class b06Record: BaseRecordViewModel
    {
        public BO.b06WorkflowStep Rec { get; set; }

        public int b01ID { get; set; }
        public int b02ID { get; set; }
        public BO.b02WorkflowStatus RecB02 { get; set; }

        public List<BO.b13WorkflowRequiredFormsToStep> lisB13 { get; set; }
        public int SelectedF06ID { get; set; }
        public string SelectedForm { get; set; }

        public string o13IDs { get; set; }
        public string o13Names { get; set; }
        public string b12_j04IDs { get; set; }
        public string b12_j04Names { get; set; }
        public string b12_a45IDs { get; set; }
        public string b12_a45Names { get; set; }
        public string b08_a45IDs { get; set; }
        public string b08_a45Names { get; set; }
        public string b08_j04IDs { get; set; }
        public string b08_j04Names { get; set; }
        public List<BO.b11WorkflowMessageToStep> lisB11 { get; set; }
        public List<BO.b10WorkflowCommandCatalog_Binding> lisB10 { get; set; }

        public string a45Name_NomineeTarget { get; set; }   //combo
        
        public int j11ID_b08_99 { get; set; }
        public string j11Name_b08_99 { get; set; }
       public int b06ID_NomineeSource { get; set; }
        public string b06Name_NomineeSource { get; set; }
    }
}
