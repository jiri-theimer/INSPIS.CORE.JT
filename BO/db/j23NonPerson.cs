using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j23NonPerson: BaseBO
    {
        [Key]
        public int j23ID { get; set; }
        public int j24ID { get; set; }
        public int a05ID { get; set; }
        public string j23Code { get; set; }
        public string j23Name { get; set; }
        public string j23Description { get; set; }

        public string j24Name { get; set; } //combo
        public string a05Name { get; set; } //combo

        public bool j24IsDriver;

        public string NamePlusCode
        {
            get
            {
                if (this.j23Code == null)
                {
                    return this.j23Name;
                }
                else
                {
                    return this.j23Name + " (" + this.j23Code + ")";
                }
                
            }
        }

    }
}
