using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a25EventFormGroup:BaseBO
    {
        [Key]
        public int a25ID { get; set; }
        public string a25Name { get; set; }
        public string a25Color { get; set; }
    }
}
