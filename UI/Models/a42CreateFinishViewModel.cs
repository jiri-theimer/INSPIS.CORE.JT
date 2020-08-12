using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a42CreateTempFinishViewModel: BaseViewModel
    {
        public int a42ID { get; set; }
        public string JobGuid { get; set; }
        public string UploadGuid { get; set; }
        public BO.a42Qes Rec { get; set; }

       
        public int a03Count { get; set; }
        public int a03CountNoEmail { get; set; }

        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }

        public List<BO.o27Attachment> lisTempFiles { get; set; }

        public IEnumerable<BO.p85Tempbox> lisP85 { get; set; }
    }
}
