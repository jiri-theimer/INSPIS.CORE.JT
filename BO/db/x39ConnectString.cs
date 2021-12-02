using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class x39ConnectString:BaseBO
    {
        [Key]
        public int x39ID { get; set; }       
        public string x39Code { get; set; }
        public string x39Name { get; set; }
        public string x39Value { get; set; }
        public string x39Description { get; set; }
    }
}
