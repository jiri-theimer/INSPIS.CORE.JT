using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a04Inspectorate:BaseBO
    {
        [Key]
        public int a04ID { get; set; }        
        public int a05ID { get; set; }
        public string a04Name { get; set; }
        public string a04City { get; set; }
        public string a04Street { get; set; }
        public string a04PostCode { get; set; }
        public string a04Phone { get; set; }
        public string a04Mobile { get; set; }
        public string a04Fax { get; set; }
        public string a04Email { get; set; }
        public bool a04IsRegional { get; set; }

        public string a05Name { get; set; } //combo
        public string PostAddress
        {
            get
            {
                return this.a04Street + ", " + this.a04PostCode + " " + this.a04City;
            }
        }
        public string Name
        {
            get
            {
                return this.a05Name + ", " + this.a04Street + ", " + this.a04PostCode + " " + this.a04City;
            }
        }
    }
}
