using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FileUploadViewModel:BaseViewModel
    {
        public string Guid { get; set; }
        public int x29ID { get; set; }

        public int RecPid { get; set; }

        public IEnumerable<BO.o27Attachment> lisO27 { get; set; }   //již uložené přílohy
        
        public IEnumerable<BO.o13AttachmentType> lisO13 { get; set; }   //combo zdroj
        public int o13ID { get; set; }

        public List<BO.o27Attachment> lisTempFiles { get; set; } 

        public string o27Label { get; set; }    //popis
        public string o27PressMark { get;set;}  //číslo jednací
        
        public bool IsUsePressMark { get; set; }    //zda nabízet číslo jednací
    }
}
