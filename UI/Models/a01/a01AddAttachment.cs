using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a01AddAttachment: BaseViewModel
    {
        public BO.a01Event RecA01 { get; set; }
        public int a01id { get; set; }
        public string UploadGuid { get; set; }
    }
}
