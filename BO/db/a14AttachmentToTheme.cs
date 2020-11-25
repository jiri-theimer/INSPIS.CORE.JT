using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a14AttachmentToTheme:BaseBO
    {
        [Key]
        public int a14ID { get; set; }
        public int a08ID { get; set; }
        public int o13ID { get; set; }
        public int a14Ordinal { get; set; }
        public bool a14IsRequired { get; set; }

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
