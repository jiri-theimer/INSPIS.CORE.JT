using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a09FounderType:BaseBO
    {
        [Key]
        public int a09ID { get; set; }
        public string a09Name { get; set; }
        public string a09Description { get; set; }
        public string a09UIVCode { get; set; }
        public int a09Ordinal { get; set; }
    }
}
