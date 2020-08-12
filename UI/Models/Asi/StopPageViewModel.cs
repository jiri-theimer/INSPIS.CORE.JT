using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class StopPageViewModel : BaseViewModel
    {
        public string Message {get;set; }
        public bool IsModal { get; set; } = true;
        public bool IsSubform { get; set; }
    }
}
