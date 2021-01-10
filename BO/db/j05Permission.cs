using System.ComponentModel.DataAnnotations;

namespace BO
{
    
   
    public class j05Permission
    {
        [Key]
        public int j05ID { get; set; }
        public string j05Name { get; set; }
        public string j05Name_Lang2 { get; set; }
        public int j05Value { get; set; }
        public int j05Order { get; set; }
        public j05PermFlagEnum j05PermFlag { get; set; }

        public string NamePlusLang2
        {
            get
            {
                return this.j05Name_Lang2 + " (" + this.j05Name + ")";
            }
        }
    }


    public enum j05PermFlagEnum
    {
        EPIS1 = 1,
        EPIS2 = 2,
        PORTAL = 3
    }

    public enum j05PermValuEnum
    {
        AdminGlobal = 1,
        FormDesigner = 2,
        WorkflowDesigner = 3,
        A01QueryBuilder = 4,
        Menu_A03 = 5,
        Menu_Events = 6,
        Menu_Reports = 7,
        Menu_Analyze = 8,
        H11Admin = 9,
        A01BatchUpdate = 10,
        A01Grid = 11,
        CallCentrum = 12,
        Menu_H04 = 13,
        None = 14,
        A03Admin = 15,
        HelpEditor = 16,
        StrategyPlanning = 17,
        Timesheet = 18,
        FEP_Editor = 19,         //EPIS II
        FEP_Approver = 20,      //EPIS II
        SEP_Editor = 21,        //EPIS II
        SEP_Evaluator = 22,      //EPIS II
        SEP_Approver = 23,      //EPIS II
        SchoolLeader = 24,
        SchoolAdminUser = 25,
        SEP_Analyst = 26,        //EPIS II
        CMSAdmin = 28,             //PORTAL - CMS administrátor
        PortalAdmin = 27,          //PORTAL - administrátor portálu

        SendMail = 29,          //můj profil + odeslat zprávu
        EditContactMedia_A03 = 30,
        Menu_Portal = 33,
        Menu_Helpdesk = 34,
        Menu_Epis = 35,
        Menu_RS = 36,           //menu INEZ
        Menu_Archive = 37,     //možnost se přepnout do archívu
        LinkToArchive_A03 = 38,           //odkaz na menu //Archív// v detailu instituce
        Menu_iSET = 39,
        ApproveDocumentsToPortal = 40,     //schvalovat dokumenty k publikování na portál
        FormReadonlyPreview = 41,          //Možnost readonly náhledu do všech formulářů
        SEP_SuperEvaluator = 43,           //stejne moznosti jako SEP_Evaluator, ale vidi vsechny SVP bez omezeni na publikovane
        SEP_Remover = 44,                    //oprávnění k rušení ŠVP a učebních plánů

        Menu_K01 = 45,                       //menu [Učitelé]
        Read_Personal_K01 = 46,                //přístup k personálním datům o učitelích
        Read_Encrypted_FormValues = 47,           //přístup k zašifrovaným odpovědím formulářových otázek
        SchoolModuleTeachers = 48,             //ve školním rozhraní přístup do modulu [Učitelé]
        Read_Personal_K01_Statistics = 49,   //přístup k personálním datům o učitelích ve Statistikách
        AdminGlobal_Ciselniky = 50             //Správce číselníků
    }
}
