using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class h11RecPage: BaseViewModel
    {
        public BO.h11NoticeBoard Rec { get; set; }
        public string HtmlContent { get; set; }
        public int pid { get; set; }

        public string TagHtml { get; set; }
    }
}
