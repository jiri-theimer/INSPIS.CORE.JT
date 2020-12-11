using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j72TheGridTemplate : BaseBO
    {
        [Key]
        public int j72ID { get; set; }
        public int j03ID { get; set; }
        public string j72Name { get; set; }
        public bool j72IsSystem { get; set; }
        public bool j72IsPublic { get; set; }
        public string j72Entity { get; set; }
        public string j72MasterEntity { get; set; }
        public int MasterPID { get; set; }       //lze zrušit pouze pro průběžnou práci -> neukládá se do db, byť v sloupec existuje
        public string MasterFlag { get; set; }   //lze zrušit pouze pro průběžnou práci
        public int ContextMenuFlag { get; set; }   //lze zrušit pouze pro průběžnou práci

        public string OnDblClick { get; set; }  //lze zrušit pouze pro průběžnou práci: lze zrušit

        //[Required(ErrorMessage ="Grid musí obsahovat minimálně jeden sloupec.")]
        public string j72Columns { get; set; }
       
        public bool j72IsNoWrap { get; set; }
       
        public int j72SplitterFlag { get; set; }
        
        public int j72SelectableFlag { get; set; } = 1;

        public int MasterViewFlag { get; set; }    //lze zrušit 1: flatview s možností přepnutí na masterview, 2:masterview s možností přepnutí na flatview, 3: vždy pouze flatview bez možnosti přepnout
        

        public bool j72HashJ73Query;

        public string AddFilterID { get; set; }     //lze zrušit jedinečné ID implementace gridu na stránce pro účely hledání filtrovacích parametrů
        public string FixedColumns { get; set; }     //lze zrušit výčet pevně daných sloupců
    }
}
