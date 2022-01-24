using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j73TheGridQuery:QueryBuilderRow
    {
        [Key]
        public int j73ID { get; set; }
        public int j72ID { get; set; }

        public string j73Op { get; set; }
        public string j73BracketLeft { get; set; }
        public string j73BracketRight { get; set; }

        public string j73Column { get; set; }        
        public string j73Operator { get; set; }
        public string j73Value { get; set; }
        public string j73ValueAlias { get; set; }

        public int j73ComboValue { get; set; }
        public int j73DatePeriodFlag { get; set; }
        public DateTime? j73Date1 { get; set; }
        public DateTime? j73Date2 { get; set; }

        public double j73Num1 { get; set; }
        public double j73Num2 { get; set; }
        public int j73Ordinal { get; set; }


        public string MasterPrefix { get; set; }
        public int MasterPid { get; set; }

    }
}
