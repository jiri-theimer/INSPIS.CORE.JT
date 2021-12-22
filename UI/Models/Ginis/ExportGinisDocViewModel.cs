using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace UI.Models.Ginis
{
    public class ExportGinisDocViewModel: BaseViewModel
    {
        public int o27id { get; set; }
        public BO.o27Attachment RecO27 { get; set; }
        public BO.a01Event RecA01 { get; set; }
        public int a01id { get; set; }
        public string GinisSpisUrl { get; set; }
        public string DestSpis { get; set; }
        public string DestDokument { get; set; }

        public int ExportFlag { get; set; } //1: Vložit do již existujícího GINIS dokumentu, 2: Založit nový dokument v rámci existujícího spisu
        public string TypVazby { get; set; }  //elektronicky-obraz: Exportovat jako elektronický obraz, elektronicka-priloha: Exportovat jako přílohu

        public string Vec { get; set; }


        public List<BO.Ginis.GinisDocumentType> lisTypyDokumentu { get; set; }
        public string SelectedTypDokumentu { get; set; }

        public HttpClient httpclient { get; set; }
    }
}
