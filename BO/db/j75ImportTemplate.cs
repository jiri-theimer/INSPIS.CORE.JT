using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j75ImportTemplate: BaseBO
    {
        [Key]
        public int j75ID { get; set; }
        public int x29ID { get; set; }
        public int j03ID { get; set; }
        public int a06ID { get; set; }
        public string j75Name { get; set; }
        public string j75Pairs { get; set; }
    }
}
