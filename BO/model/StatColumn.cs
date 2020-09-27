using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class StatColumn
    {
        public string colField { get; set; }
        public string colName;
        public int f19ID;
        public string f19Name;
        public int f19Ordinal;
        public int f21Ordinal;
        public int f21ID;
        public string f21Name;
        public string f21ExportValue;
        public int f23ID;
        public int x24ID;
        public bool f19IsMultiselect;
        public bool f21IsCommentAllowed;
        public bool ThisColumnIsComment;
        public bool f19IsEncrypted;
        public int f06ID;

        public string ComboText { get
            {                
                switch (this.f23ID)
                {
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return this.f19Name + " -> " + this.f21Name + " [" + this.f21ID.ToString() + "]";
                    default:
                        return this.f19Name + " [" + this.f19ID.ToString() + "]";
                }
            }
        }
        public string ComboValue
        {
            get
            {
                return this.f19ID.ToString() + "-" + this.f21ID.ToString();
            }
        }
    }

    public enum StatValueMode
    {
        Nazev = 1,
        StatID = 2,
        PID = 3
    }
    public enum StatGroupByMode
    {
        NoGroup = 0,
        GroupByA01 = 1,
        GroupByA03 = 2,
        GroupByA37 = 3
    }

}
