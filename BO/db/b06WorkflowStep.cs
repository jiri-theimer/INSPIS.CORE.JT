using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b06WorkflowStep:BaseBO
    {
        [Key]
        public int b06ID { get; set; }
        public int b02ID { get; set; }
        public int b02ID_Target { get; set; }
        public int a45ID_NomineeTarget { get; set; }
        
        public int x26ID_Nominee_J02 { get; set; }
        public int x26ID_Nominee_J11 { get; set; }
        public string b06Name { get; set; }       
        public int b06Order { get; set; }
        public bool b06IsManualStep { get; set; }
        public bool b06IsEscalation_Timeout_Total { get; set; }
        public bool b06IsEscalation_Timeout_SLA { get; set; }
        public bool b06IsNominee { get; set; }
        public bool b06IsCommentRequired { get; set; }
        
        public bool b06IsNomineeRequired { get; set; }
        public bool b06IsHistoryAllowedToAll { get; set; }
        public bool b06IsFormAutoLock { get; set; }
        public bool b06IsFormValidationRequired { get; set; }
        public bool b06IsAttachmentTestRequired { get; set; }
        public bool b06IsAutoRun_Missing_Form { get; set; }         //1 - krok se spustí automaticky, pokud nejsou vyplněny povinné formuláře
        public bool b06IsAutoRun_Missing_Attachment { get; set; }   //1 - krok se spustí automaticky, pokud nejsou přiloženy povinné přílohy
        public bool b06IsRunOneInstanceOnly { get; set; }           //zda je povoleno spustit krok pouze jednou
        
        public string b06RunSQL { get; set; }
        public string b06ValidateBeforeRunSQL { get; set; }
        public string b06ValidateAutoMoveSQL { get; set; }
        public string b06ValidateBeforeErrorMessage { get; set; }

        public bool b06IsToDoGeneration { get; set; }
        public string b06ToDo_Name { get; set; }
        public int b06ToDo_h07ID { get; set; }
        public string b06ToDo_DeadlineField { get; set; }
        public int b06ToDo_DeadlineMove { get; set; }
        public string b06ToDo_Description { get; set; }
        public int b06ToDo_ReceiverFlag { get; set; }
        public string b06ToDo_CapacityPlanFromField { get; set; }
        public string b06ToDo_CapacityPlanUntilField { get; set; }
        public int b06ToDo_CapacityPlanFromMove { get; set; }
        public int b06ToDo_CapacityPlanUntilMove { get; set; }
        public bool b06ToDo_IsSendMail { get; set; }

        public string b06UC { get; set; }

        public string b01Name;
        public int b01ID;
        public string b02Name;
        public string TargetStatus { get; set; }    //combo
        public string h07Name { get; set; }     //combo
        
    }
}
