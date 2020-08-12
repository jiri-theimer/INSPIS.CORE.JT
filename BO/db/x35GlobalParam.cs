using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x35GlobalParam: BaseBO
    {
        [Key]
        public int x35ID { get; set; }
        public string x35Key { get; set; }
        public string x35Value { get; set; }
        public string x35Description { get; set; }
    }
}
