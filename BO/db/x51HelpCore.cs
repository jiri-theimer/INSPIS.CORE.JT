using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x51HelpCore:BaseBO
    {
        [Key]
        public int x51ID { get; set; }
        public string x51Name { get; set; }
        public string x51ViewUrl { get; set; }
        public string x51ExternalUrl { get; set; }
        public string x51Html { get; set; }
        public string x51PlainText { get; set; }
        public string x51NearUrls { get; set; }
    }
}
