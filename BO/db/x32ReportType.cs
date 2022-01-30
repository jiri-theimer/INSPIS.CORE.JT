using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x32ReportType:BaseBO
    {
        [Key]
        public int x32ID { get; set; }

        public string x32Name { get; set; }
        public int x32Ordinal { get; set; }
        public int x32ParentID { get; set; }
        public string x32Description { get; set; }

        public int x32TreeLevel { get; set; }
        public int x32TreeIndex { get; set; }
        public int x32TreeIndexFrom { get; set; }
        public int x32TreeIndexTo { get; set; }

        public string ParentName { get; set; }    //combo
        public string ParentPath;
        public string TreeItem { get; set; }

        
    }
}
