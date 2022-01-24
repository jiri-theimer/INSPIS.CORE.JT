using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j77NamedQueryRow: QueryBuilderRow
    {
        [Key]
        public int j77ID { get; set; }
        public int j76ID { get; set; }

        public string j77Op { get; set; }
        public string j77BracketLeft { get; set; }
        public string j77BracketRight { get; set; }

        public string j77Column { get; set; }
        public string j77Operator { get; set; }
        public string j77Value { get; set; }
        public string j77ValueAlias { get; set; }

        public int j77ComboValue { get; set; }
        public int j77DatePeriodFlag { get; set; }
        public DateTime? j77Date1 { get; set; }
        public DateTime? j77Date2 { get; set; }

        public double j77Num1 { get; set; }
        public double j77Num2 { get; set; }
        public int j77Ordinal { get; set; }

        
    }
}
