using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a57Record:BaseRecordViewModel
    {
        public BO.a57AutoEvaluation Rec { get; set; }
        public string SelectedA10Name { get; set; }
        public string SelectedA08Name { get; set; }
        public string UploadGuid { get; set; }
    }
}
