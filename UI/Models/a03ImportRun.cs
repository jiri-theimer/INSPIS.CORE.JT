using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a03ImportRun: BaseViewModel
    {
        public string Guid { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
    }
}
