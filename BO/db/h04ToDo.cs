using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class h04ToDo: BaseBO
    {
        [Key]
        public int h04ID { get; set; }
        public int a01ID { get; set; }
        public int h07ID { get; set; }
        public int h05ID { get; set; }
        public int j03ID_Creator { get; set; }
        public int j02ID_Owner { get; set; }
        public string h04Signature { get; set; }
        public int h04SigID { get; set; }
        public string h04Name { get; set; }
        public DateTime h04Deadline { get; set; }
        public DateTime? h04ReminderDate { get; set; }
        public DateTime? h04CapacityPlanFrom { get; set; }
        public DateTime? h04CapacityPlanUntil { get; set; }
        public string h04Description { get; set; }
        public bool h04IsClosed { get; set; }

        public string h07Name { get; set; } //combo
        public bool h07IsToDo;
        public bool h07IsCapacityPlan;
        public string h05Name { get; set; } //combo
        public string h05NameLang1 { get; set; }
        public string h05NameLang2 { get; set; }
        public string a01Signature;

    }
}
