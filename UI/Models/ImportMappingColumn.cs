using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ImportMappingColumn
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public string TargetField { get; set; }

        public string CssTrStyle { get; set; }
    }
}
