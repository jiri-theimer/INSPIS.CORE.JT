using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a17DepartmentType:BaseBO
    {
        [Key]
        public int a17ID { get; set; }
        public string a17Name { get; set; }
        public string a17Description { get; set; }
        public string a17UIVCode { get; set; }

        public bool a17IsDefault { get; set; }

        public string CodePlusName;
    }
}
