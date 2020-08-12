using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class a42CreateViewModel:BaseViewModel
    {
        public int ActiveTabIndex { get; set; } = 1;
        public string UploadGuid { get; set; }
        public BO.a42Qes Rec { get; set; }

        public IEnumerable<BO.a10EventType> lisA10 { get; set; }
        public int a10ID { get; set; }

        public BO.a08Theme RecA08 { get; set; }
        public BO.a10EventType RecA10 { get; set; }
        public int a08ID { get; set; }
        public string a08Name { get; set; }
        public DateTime? a42DateFrom { get; set; }
        public DateTime? a42DateUntil { get; set; }


        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        public string a03IDs { get; set; }

        public int FilterA03ID { get; set; }
        public string FilterA03Name { get; set; }
        public int FilterA29ID { get; set; }
        public string FilterA29Name { get; set; }
        public int FilterA42ID { get; set; }
        public string FilterA42Name { get; set; }
        public int FilterB65ID { get; set; }
        public string FilterB65Name { get; set; }

        public string MessageSubject { get; set; }
        public string MessageBody { get; set; }
        public BO.RecipientFlagEnum MessageReceiverFlag { get; set; }

        public List<BO.a12ThemeForm> lisLeftA12 { get; set; }
        public List<BO.a12ThemeForm> lisRightA12 { get; set; }

        public List<BO.a12ThemeForm> lisPollA12 { get; set; }
        public int LeftA12ID { get; set; }
        public int RightA12ID { get; set; }
        public string SelectedA12IDs { get; set; }
    }
}
