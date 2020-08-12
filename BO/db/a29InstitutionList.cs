using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a29InstitutionList:BaseBO
    {
        [Key]
        public int a29ID { get; set; }
        public string a29Name { get; set; }
        public string a29Description { get; set; }
    }
}
