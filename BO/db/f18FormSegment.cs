using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f18FormSegment:BaseBO
    {
        [Key]
        public int f18ID { get; set; }
        public int f06ID { get; set; }
        public string f18Name { get; set; }
        public string f18Text { get; set; }
        public string f18RazorTemplate { get; set; }
        public string f18SupportingText { get; set; }
        public string f18ReadonlyExpression { get; set; }
        public int f18Ordinal { get; set; }
        public bool f18IsDisplayOrdinal { get; set; }
        public bool f18IsDisplayUserOrdinal { get; set; }

        public int f18ParentID { get; set; }
        public string f18Description { get; set; }
        public string f18UC { get; set; }

        public int f18TreeLevel { get; set; }
        public int f18TreeIndex { get; set; }
        public int f18TreeIndexFrom { get; set; }
        public int f18TreeIndexTo { get; set; }

        public string ParentName { get; set; }    //combo
        public string ParentPath;
        public string TreeItem;
    }
}
