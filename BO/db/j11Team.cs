using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j11Team:BaseBO
    {
        [Key]
        public int j11ID { get; set; }
        public string j11Name { get; set; }
        public string j11Description { get; set; }
        public bool j11IsAllUsers { get; set; }
    }
}
