using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f22ReplySet:BaseBO
    {
        [Key]
        public int f22ID { get; set; }
        public string f22Name { get; set; }
        public string f22Description { get; set; }
        public int f22Ordinal { get; set; }
        public string f22UC { get; set; }
    }
}
