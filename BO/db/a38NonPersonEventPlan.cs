using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a38NonPersonEventPlan: BaseBO
    {
        [Key]
        public int a38ID { get; set; }
        public int a01ID { get; set; }
        public int j02ID { get; set; }
        public int j23ID { get; set; }
        public int j25ID { get; set; }
        public DateTime a38PlanDate { get; set; }
        public bool a38IsDriver { get; set; }
        public string a38Location { get; set; }

        //readonly:
        public string j02FirstName;
        public string j02LastName;
        public string j02TitleBeforeName;
        public string j02TitleAfterName;
        public string j23Name;
        public string j23Code;
        public int a01ParentID;

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
    }
}
