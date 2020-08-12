using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class b02Record: BaseRecordViewModel
    {
        public BO.b02WorkflowStatus Rec { get; set; }

        public int b01ID { get; set; }
        public BO.b01WorkflowTemplate RecB01 { get; set; }

        public List<BO.b07WorkflowMessageToStatus> lisB07 { get; set; }
        public List<BO.b03WorkflowReceiverToStatus> lisB03 { get; set; }
        public List<BO.b10WorkflowCommandCatalog_Binding> lisB10 { get; set; }
    }
}
