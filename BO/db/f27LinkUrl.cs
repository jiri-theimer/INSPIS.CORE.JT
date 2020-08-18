using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f27LinkUrl:BaseBO
    {
        [Key]
        public int f27ID { get; set; }
        public string f27Name { get; set; }
        public string f27URL { get; set; }
    }
}
