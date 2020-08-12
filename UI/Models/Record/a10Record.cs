using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class a10Record:BaseRecordViewModel
    {
        public BO.a10EventType Rec { get; set; }

        public string a08IDs { get; set; }
        public string a08Names { get; set; }

        public List<BO.a20EventType_UserRole_PersonalPage> lisA20 { get; set; }

        public int SelectedJ04ID { get; set; }
        public string SelectedRole { get; set; }
    }
}
