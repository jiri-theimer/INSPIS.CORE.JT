using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o13AttachmentType:BaseBO
    {
        [Key]
        public int o13ID { get; set; }

        public string o13Name { get; set; }
        public int x29ID { get; set; }
        public int o13ParentID { get; set; }
        public string o13Description { get; set; }
        public string o13FilePrefix { get; set; }
        public string o13DefaultArchiveFolder { get; set; }
        public int o13TreeLevel { get; set; }
        public int o13TreeIndex { get; set; }
        public int o13TreeIndexFrom { get; set; }
        public int o13TreeIndexTo { get; set; }
        public bool o13IsPortalDoc { get; set; }
        public bool o13IsObjection { get; set; }

        public string ParentName { get; set; }    //combo
        public string ParentPath;
        public string TreeItem { get; set; }    //combo

        public string x29Name { get; set; } //combo

        public string SharpFolder;  //folder s dvojtými backslashi
    }
}
