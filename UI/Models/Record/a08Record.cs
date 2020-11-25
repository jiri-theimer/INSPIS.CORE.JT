using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a08Record:BaseRecordViewModel
    {
        public BO.a08Theme Rec { get; set; }
        public List<BO.a12ThemeForm> lisA12 { get; set; }
        public List<BO.a14AttachmentToTheme> lisA14 { get; set; }
        public IEnumerable<BO.o13AttachmentType> lisO13 { get; set; }

        public int SelectedF06ID { get; set; }
        public string SelectedForm { get; set; }
    }
}
