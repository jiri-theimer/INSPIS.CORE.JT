using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Ginis
{
    public class ImportGinisDocViewModel:BaseViewModel
    {
        public BO.a01Event RecA01 { get; set; }
        public int a01id { get; set; }
        public string InputSpis { get; set; }
        public string InputDokument { get; set; }

        public string SelectedDokumentCombo { get; set; }
        public string SelectedDokument { get; set; }
        public List<BO.Ginis.GinisDocument> lisDokument { get; set; }

        public string GinisSpisUrl { get; set; }
        
    }
}
