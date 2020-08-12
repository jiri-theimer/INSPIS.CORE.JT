using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a18DepartmentDomain: BaseBO
    {
        [Key]
        public int a18ID { get; set; }
        public string a18Name { get; set; }
        public string a18Code { get; set; }
        public string a18SubCode1 { get; set; }
        public string a18SubCode2 { get; set; }
        public string a18SubCode3 { get; set; }
        public string a18SubCode4 { get; set; }

        public string Skupina;
        
        public string Obor;
        
    }
}
