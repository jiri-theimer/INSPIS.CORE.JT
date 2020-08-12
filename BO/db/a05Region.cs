using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a05Region:BaseBO
    {
        [Key]        
        public int a05ID { get; set; }
        public string a05Name { get; set; }

        public int a05Ordinal { get; set; }

        public string a05UIVCode { get; set; }
        public string a05RZCode { get; set; }
        public string a05VUSCCode { get; set; }
    }
}
