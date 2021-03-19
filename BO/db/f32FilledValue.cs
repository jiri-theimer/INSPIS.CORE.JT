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

    public class f32FilledValueExtened : f32FilledValue
    {
        public string f19Name { get; }
        public int f18ID { get; }
        public string f18Name { get; }
        public bool f19IsRequired { get; }
        public int f19Ordinal { get; }
        public int f18TreeIndex { get; }
        public string x24Name { get; }
    }
}
