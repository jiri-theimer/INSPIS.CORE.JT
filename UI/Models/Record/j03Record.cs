using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j03Record:BaseRecordViewModel
    {
        public BO.j03User Rec { get; set; }
        public bool IsGridClipboard { get; set; }
        public BO.j02Person RecJ02 { get; set; }

        public string NewPassword { get; set; }
        public string VerifyPassword { get; set; }
        public bool IsDefinePassword { get; set; }
        public bool IsChangeLogin { get; set; }
        
        public string user_profile_oper { get; set; }

        public string ComboPerson { get; set; }

        public List<j03RecordAdminRoleValue> lisAdminRoleValues { get; set; }

        public IEnumerable<BO.b65WorkflowMessage> lisB65 { get; set; }
    }
}
