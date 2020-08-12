using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a02Inspector:BaseBO
    {
        [Key]
        public int a02ID { get; set; }
        public int a04ID { get; set; }
        public int j02ID { get; set; }


        public string PostAddress;
        public string a05Name;
        public string Person { get; set; }  //combo
        
        public string Inspectorate  //combo
        {
            get
            {
                return this.a05Name + ", " + this.PostAddress;
            }
            set
            {
                
            }
        }
    }
}
