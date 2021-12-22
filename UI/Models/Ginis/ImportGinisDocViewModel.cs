using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace UI.Models.Ginis
{
    public class ImportGinisDocViewModel:BaseViewModel
    {
        public BO.a01Event RecA01 { get; set; }
        public int a01id { get; set; }
        public string InputSpis { get; set; }
        public string InputDokument { get; set; }

        public string SelectedDokumentCombo { get; set; }
        
        public BO.Ginis.GinisDocument RecGinisDokument { get; set; }
        public List<BO.Ginis.GinisFile> lisSouboryDokumentu { get; set; }
        public string SelectedSouborCombo { get; set; }

        
        public List<BO.Ginis.GinisDocument> lisDokument { get; set; }

        public string GinisSpisUrl { get; set; }

        public IEnumerable<BO.o13AttachmentType> lisO13 { get; set; }
        public int SelectedO13ID { get; set; }
        
        public string o27Description { get; set; }

        public HttpClient httpclient { get; set; }
    }
}
