using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j02Person : BaseBO
    {
        [Key]
        public int j02ID { get; set; }
        public int j07ID { get; set; }        
                        
        public string j02Email { get; set; }        
               
        public string j02FirstName { get; set; }        
        public string j02LastName { get; set; }
        public string j02TitleBeforeName { get; set; }
        public string j02TitleAfterName { get; set; }

        public string j02Phone { get; set; }
        public string j02Mobile { get; set; }
        public string j02PID { get; set; }

        public bool j02IsInvitedPerson { get; set; }
        
        public string j02Address { get; set; }
        public string j02Position { get; set; }

        public int j03ID { get; }
        public string j03Login { get; }

        
        public string FullNameAsc { 
            get
            {
                
                return (this.j02TitleBeforeName + " " + this.j02FirstName + " " + this.j02LastName + " " + this.j02TitleAfterName).Trim();
            }
          
        }
        public string FullNameDesc
        {
            get
            {

                return (this.j02LastName + " " + this.j02FirstName + " " + this.j02TitleBeforeName).Trim();
            }

        }

        public string j07Name;
        public string a04City;
        public int a04ID;
        public int a05ID;
        public string TagHtml;

    }
}
