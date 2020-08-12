using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a42Qes:BaseBO
    {
        [Key]
        public int a42ID { get; set; }
        public int a08ID { get; set; }
        public string a42Name { get; set; }
        public DateTime? a42DateFrom { get; set; }
        public DateTime? a42DateUntil { get; set; }
        
        public string f06IDs_Poll { get; set; }
        public string a42Description { get; set; }

        public int a42TempRowsA01 { get; set; }
        public int a42TempRowsX40 { get; set; }
        public string a42JobGuid { get; set; }
        public string a42UploadGuid { get; set; }
        public int a42JobState { get; set; }    //0:nic, 1: Návrh, 2: Generují se akce,3: Zastaveno generování,4: Akce vygenerovány, 5: Rozesílají se zprávy, 6: Zastaveno odesílání zpráv, 7: Hotovo

        public string a08Name;
    }
}
