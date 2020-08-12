using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class f06Clone:BaseViewModel
    {
        public BO.f06Form RecSource { get; set; }
        public int f06ID { get; set; }
        public string DestName { get; set; }
        public string DestExportCode { get; set; }
    }
}
