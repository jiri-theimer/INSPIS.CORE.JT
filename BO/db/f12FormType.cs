using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f12FormType:BaseBO
    {
        [Key]
        public int f12ID { get; set; }

        public string f12Name { get; set; }
       
        public int f12ParentID { get; set; }
        public string f12Description { get; set; }

        public int f12TreeLevel { get; set; }
        public int f12TreeIndex { get; set; }
        public int f12TreeIndexFrom { get; set; }
        public int f12TreeIndexTo { get; set; }        

        public string ParentName { get; set; }    //combo
        public string ParentPath;
        public string TreeItem;
    }
}
