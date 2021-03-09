using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j72TheGridTemplate : BaseBO
    {
        [Key]
        public int j72ID { get; set; }
        public int j03ID { get; set; }
        public string j72Name { get; set; }
        public bool j72IsSystem { get; set; }           //výchozí grid uživatele - nelze odstranit, pouze jeden
        public bool j72IsTemplate4SystemGrid { get; set; }    //vzor pro úvodní (system) grid uživatelů
        public bool j72IsPublic { get; set; }
        public string j72Entity { get; set; }
        public string j72MasterEntity { get; set; }
       
        public string j72Columns { get; set; }
       
        public bool j72IsNoWrap { get; set; }
       
        public int j72SplitterFlag { get; set; }
        
        public int j72SelectableFlag { get; set; } = 1;

        
        public bool j72HashJ73Query;

        //public string AddFilterID { get; set; }     //lze zrušit jedinečné ID implementace gridu na stránce pro účely hledání filtrovacích parametrů
        //public string FixedColumns { get; set; }     //lze zrušit výčet pevně daných sloupců
    }
}
