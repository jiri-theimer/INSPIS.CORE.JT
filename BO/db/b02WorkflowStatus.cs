using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum b02SynchroTargetEnum
    {
        PodrizeneAkce=0,
        NadrizenouAkci=1,
        SouvisejiciAkce=2,
        Nic=3,

    }
    public enum b02SynchroFieldsEnum
    {
        StavTypInsPlanTym=0,
        Stav=1,
        TypInsPlanTym=2,
        Nic = 3
    }
    public class b02WorkflowStatus:BaseBO
    {
        [Key]
        public int b02ID { get; set; }
        public int b01ID { get; set; }
        public string b02Name { get; set; }
        public string b02Ident { get; set; }
        public string b02Color { get; set; }
        public int b02Order { get; set; }
        public bool b02IsDefaultStatus { get; set; }
        public bool b02IsHoldStatus { get; set; }
        public bool b02IsSeparateTab { get; set; }
        public bool b02IsDurationSLA { get; set; }
        public bool b02IsCommentForbidden { get; set; }
        public int b02TimeOut_Total { get; set; }
        public int b02TimeOut_SLA { get; set; }
        public string b02Message4UIFT { get; set; }

        public string b02UC { get; set; }
        public b02SynchroTargetEnum b02SynchroTargetFlag { get; set; }
        public b02SynchroFieldsEnum b02SynchroFieldsFlag { get; set; }

        public string b01Name;
    }
}
