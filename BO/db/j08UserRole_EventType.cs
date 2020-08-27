using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j08UserRole_EventType
    {
        [Key]
        public int j08ID { get; set; }
        public int j04ID { get; set; }
        public int a10ID { get; set; }
        public string a10Name { get; set; }//combo
        public bool j08IsLeader { get; set; }
        public bool j08IsMember { get; set; }
        public bool j08IsAllowedCreate { get; set; }
        public bool j08IsAllowedRead { get; set; }


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
