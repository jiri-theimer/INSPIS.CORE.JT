using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x51Record:BaseRecordViewModel
    {
        public BO.x51HelpCore Rec { get; set; }

        public string HtmlContent { get; set; }

        public string Source { get; set; }

        public string EditorLanguageKey { get; set; }= "cs-CZ";
    }
}
