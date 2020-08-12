using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class f06Record:BaseRecordViewModel
    {
        public BO.f06Form Rec { get; set; }

        
        public string SelectedF12Name { get; set; }

        public string j04IDs { get; set; }
        public string j04Names { get; set; }

        public string x31IDs { get; set; }
        public string x31Names { get; set; }

        public int SelectedO13ID { get; set; }
        public string TreeItem { get; set; }

        public List<BO.a13AttachmentToForm> lisA13 { get; set; }

        public string UploadGuid { get; set; }
    }
}
