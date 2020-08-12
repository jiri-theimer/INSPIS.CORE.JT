using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class a37InstitutionDepartment:BaseBO
    {
        [Key]
        public int a37ID { get; set; }
        public int a17ID { get; set; }
        public int a03ID { get; set; }
        public string a37IZO { get; set; }
        public string a37Name { get; set; }
        public string a37City { get; set; }
        public string a37Street { get; set; }
        public string a37PostCode { get; set; }
        public string a37Phone { get; set; }
        public string a37Mobile { get; set; }
        public string a37Fax { get; set; }
        public string a37Email { get; set; }
        public string a37Web { get; set; }

        public string a03Name;
        public string a03REDIZO;
        public string a17Name { get; set; }//combo
        public string a17UIVCode;
    }
}
