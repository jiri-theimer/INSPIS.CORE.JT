using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o53TagGroup:BaseBO
    {
        [Key]
        public int o53ID { get; set; }
        public int j02ID_Owner { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string o53Name { get; set; }
        public string o53Entities { get; set; }
        public int o53Ordinary { get; set; }
        public bool o53IsMultiSelect { get; set; }
        public string o53Field { get; set; }
    }
}
