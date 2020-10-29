using System.ComponentModel.DataAnnotations;
namespace BO
{
    public class a28SchoolType:BaseBO
    {
        [Key]
        public int a28ID { get; set; }
        public string a28Name { get; set; }
        public string a28Code { get; set; }
        public int a28Ordinary { get; set; }
        
    }
}
