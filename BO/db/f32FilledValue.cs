using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class f32FilledValue:BaseBO
    {
        [Key]
        public int f32ID { get; set; }
        public int f21ID { get; set; }
        public int a11ID { get; set; }
        public int f19ID { get; set; }
        public int f33ID { get; set; }
        public string Value { get; set; }
        public string f32Comment { get; set; }
        public bool f32IsFilledByEval { get; set; }
        public double? f32Value_NumericSearch { get; set; }
        public DateTime? f32Value_DateSearch { get; set; }
        public string f32ValueAliasEvalList { get; set; }

        //readonly:
        public int f23ID;
        public int x24ID;
        public string f21Name;
        public bool f31IsPublished;



    }
}
