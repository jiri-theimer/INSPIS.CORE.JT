using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a10EventType: BaseBO
    {
        [Key]
        public int a10ID { get; set; }
        public int b01ID { get; set; }
        public int a45ID_Creator { get; set; }
        public string a10Name { get; set; }
        public string a10Description { get; set; }
        public string a10Aspx_Insert { get; set; }
        public string a10Aspx_Framework { get; set; }
        public bool a10IsUse_A08 { get; set; }
        public bool a10IsUse_A03 { get; set; }
        public bool a10IsUse_A41 { get; set; }
        public bool a10IsUse_DeadLine { get; set; }
        public bool a10IsUse_Name { get; set; }
        public bool a10IsUse_CaseCode { get; set; }
        public bool a10IsUse_Period { get; set; }
        public bool a10IsUse_A41_J11 { get; set; }
        public bool a10IsUse_A41_Leader { get; set; }
        public bool a10IsUse_Poll { get; set; }
        public bool a10IsUse_K01 { get; set; }
        public bool a10IsUse_ReChangeForms { get; set; }
        public bool a10IsEpis2 { get; set; }
        public bool a10IsSupportCloning { get; set; }
        public string a10Ident { get; set; }
        public int a10OneSchoolInstanceLimit { get; set; }
        public int a10Linker_a10ID { get; set; }
        public int a10Linker_a08ID { get; set; }
        public string a10LinkerDB { get; set; }

        public string b01Name { get; set; }//combo

        public string a10ViewUrl_Insert { get; set; } //nové pole v CORE
        public string a10ViewUrl_Page { get; set; } //nové pole v CORE

        

    }
}
