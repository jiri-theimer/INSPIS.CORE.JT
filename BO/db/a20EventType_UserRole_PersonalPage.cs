using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class a20EventType_UserRole_PersonalPage
    {
        public int a20ID { get; set; }
        public int a10ID { get; set; }
        public int j04ID { get; set; }
        public string a20Aspx_Framework { get; set; }

        public string j04Name { get; set; } //combo

        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:flex";
                }
            }
        }
    }
}
