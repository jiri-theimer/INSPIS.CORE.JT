using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a11AppendPollViewModel: a11AppendViewModel
    {
        
        public string AccessToken { get; set; }
        public int AccessTokenMinValue { get; set; }
        public int SelectedK01ID { get; set; }
        public string SelectedK01fullname_desc { get; set; }

        public TheGridInput gridinput { get; set; }
    }
}
