using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a21InstitutionLegalType:BaseBO
    {
        [Key]
        public int a21ID { get; set; }
        public string a21Name { get; set; }
        public string a21Code { get; set; }
    }
}
