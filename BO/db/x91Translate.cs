using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x91Translate:BaseBO
    {
        [Key]
        public int x91ID { get; set; }
        public string x91Code { get; set; }
        public string x91Orig { get; set; }
        public string x91Lang1 { get; set; }
        public string x91Lang2 { get; set; }
        public string x91Lang3 { get; set; }
        public string x91Lang4 { get; set; }
        public string x91Page { get; set; }
    }
}
