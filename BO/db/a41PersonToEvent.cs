using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum EventRoleENUM
    {        
        Resitel = 1,
        Vedouci = 2,
        Zadavatel = 3,
        Ctenar = 4,
        Vlastnik = 5,
        PrizvanaOsoba=6,
        NominovanySchvalovatel = 98,
        NominovanyPrimoDosazeny = 99,
        _None=0
    }


    public class a41PersonToEvent : BaseBO
    {
        [Key]
        public int a41ID { get; set; }
        public int a01ID { get; set; }
        public int j02ID { get; set; }
        public int j11ID { get; set; }
        public EventRoleENUM a45ID { get; set; }
        public int b06ID_NomineeSource { get; set; }
        public bool a41IsAllocateAllPeriod { get; set; }


        //readonly        

        public string a45Name { get; set; } //combo
        public string j11Name { get; set; } //combo
        public bool a45IsPlan;
        public bool a45IsManual { get; set; }   //repeater
        public bool j02IsInvitedPerson;

        public string PersonCombo { get; set; }
        public string j02FirstName;
        public string j02LastName;
        public string j02TitleBeforeName;
        public string j02TitleAfterName;
        public string j02Email;
        public string PersonAsc
        {
            get
            {

                return (this.j02TitleBeforeName + " " + this.j02FirstName + " " + this.j02LastName + " " + this.j02TitleAfterName).Trim();
            }

        }
        public string PersonDesc
        {
            get
            {

                return (this.j02LastName + " " + this.j02FirstName + " " + this.j02TitleBeforeName).Trim();
            }

        }


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
