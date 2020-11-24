using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class h11Record: BaseRecordViewModel
    {
        public BO.h11NoticeBoard Rec { get; set; }

        public string HtmlContent { get; set; }
        public string PlainText { get; set; }

        public string j04IDs { get; set; }
        public string j04Names { get; set; }
        public string EditorLanguageKey { get; set; } = "cs-CZ";
    }
}
