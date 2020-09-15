using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum a42JobState
    {
        NotSpecified = 0,
        Draft = 1,              //Úvodní návrh
        Generating = 2,         //Generují se akce
        GeneratingStopped = 3,  //Pozastaveno generování akcí
        PreparedA01 = 4,        //Akce vygenerovány
        PreparedX40 = 5,         //Poštovní zprávy vygenerovány
        MailQueue= 6,             //Rozesílání zpráv
        MailQueueStopped=7,
        Finished= 99              //Hotovo
    }

    public class a42Qes : BaseBO
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
        public a42JobState a42JobState { get; set; }

        public string a08Name;
    }
}
