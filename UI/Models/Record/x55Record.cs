using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class x55Record: BaseRecordViewModel
    {
        public BO.x55Widget Rec { get; set; }

        public string j04IDs { get; set; }
        public string j04Names { get; set; }
        public string HtmlHelp { get; set; }
        public string EditorLanguageKey { get; set; } = "cs-CZ";

    }
}
