using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a45EventRole:BaseBO
    {
        [Key]
        public int a45ID { get; set; }
        public string a45Name { get; set; }
        public bool a45IsManual { get; set; }
        public bool a45IsPlan { get; set; }
    }
}
