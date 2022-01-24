using System;
using System.ComponentModel.DataAnnotations;


namespace BO
{
    public class j76NamedQuery: BaseBO
    {
        [Key]        
        public int j76ID { get; set; }
        public int j03ID { get; set; }
        public string j76Name { get; set; }
        public string j76Entity { get; set; }

        public bool j76IsPublic { get; set; }
    }
}
