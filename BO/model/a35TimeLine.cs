using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class a35TimeLine
    {
        public int j02ID { get; set; }
        public int a01ID { get; set; }
        public DateTime a35PlanDate { get; set; }
        public int Krat { get; set; }
       
        public string j02FirstName;
        public string j02LastName;
        public string j02TitleBeforeName;
        

        
        public string PersonDesc
        {
            get
            {

                return (this.j02LastName + " " + this.j02FirstName + " " + this.j02TitleBeforeName).Trim();
            }

        }
    }
}
