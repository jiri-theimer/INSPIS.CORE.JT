using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o11BigtextContent:BaseBO
    {
        [Key]
        public int o11ID { get; set; }
        public int x29ID { get; set; }
        public int o11DataPID { get; set; }
        public string o11Name { get; set; }
        public string o11Html { get; set; }
        public string o11PlainText { get; set; }
    }
}
