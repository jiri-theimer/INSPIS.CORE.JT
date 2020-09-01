using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabAttachments: BaseViewModel
    {
        public IEnumerable<BO.o27Attachment> lisO27 { get; set; }
        public BO.a01Event RecA01 { get; set; }        
        public int pid { get; set; }
    }
}
