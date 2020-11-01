using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FileUploadSingleViewModel:BaseViewModel
    {
        public string Guid { get; set; }

        public List<BO.o27Attachment> lisTempFiles { get; set; }

        public string TargetFlag { get; set; }      //možné hodnoty: a03import
    }
}
