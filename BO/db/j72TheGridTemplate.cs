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
        public int j72MasterPID { get; set; }       //pouze pro průběžnou práci -> neukládá se do db, byť v sloupec existuje
        public string j72MasterFlag { get; set; }   //pouze pro průběžnou práci
        public int j72ContextMenuFlag { get; set; }   //pouze pro průběžnou práci

        public string OnDblClick { get; set; }  //pouze pro průběžnou práci

        //[Required(ErrorMessage ="Grid musí obsahovat minimálně jeden sloupec.")]
        public string j72Columns { get; set; }
        public string j72SortDataField { get; set; }
        public string j72SortOrder { get; set; }
        public int j72PageSize { get; set; }
        public int j72CurrentPagerIndex { get; set; }
        public int j72CurrentRecordPid { get; set; }
        public bool j72IsNoWrap { get; set; }
        public string j72Filter { get; set; }
        public string j72ColumnsGridWidth { get; set; }
        public string j72ColumnsReportWidth { get; set; }
        public int j72SplitterFlag { get; set; }
        public int j72HeightPanel1 { get; set; }
        public int j72SelectableFlag { get; set; } = 1;

        public int MasterViewFlag { get; set; }    //1: flatview s testem na masterview, 2:masterview, 3: vždy pouze flatview bez možnosti přepnout
        

        public bool j72HashJ73Query;
    }
}
