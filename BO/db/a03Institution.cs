using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum SchoolPortalFlag
    {
        NotSpecified = 0,
        NotPublish = 1,
        Publish = 2        
    }
    public enum a03ParentFlagEnum
    {
        None=0,
        Master=1,
        Slave=2
    }
    public class a03Institution:BaseBO
    {
        [Key]
        public int a03ID { get; set; }
        public int a09ID { get; set; }
        public int a05ID { get; set; }
        public int a06ID { get; set; }
        public int a03ID_Founder { get; set; }
        public int a03ID_Supervisory { get; set; }
        public int a21ID { get; set; }
        public int a70ID { get; set; }
        public bool a03IsTestRecord { get; set; }

        public string a03REDIZO { get; set; }
        public string a03ICO { get; set; }
        public string a03Name { get; set; }  
        public string a03ShortName { get; set; }
        public string a03City { get; set; }
        public string a03Street { get; set; }
        public string a03PostCode { get; set; }
        public string a03Phone { get; set; }        
        public string a03Mobile { get; set; }
        public string a03Fax { get; set; }
        public string a03Email { get; set; }
        public string a03Web { get; set; }
        public string a03FounderCode { get; set; }
        public string a03DirectorFullName { get; set; }
        public string a03Slug { get; set; }
        public double a03Latitude { get; set; }
        public double a03Longitude { get; set; }

        public SchoolPortalFlag a03SchoolPortalFlag { get; set; }

        public DateTime a03DateUpdate_Address { get; set; }
        public DateTime a03DateUpdate_GMaps { get; set; }

        public a03ParentFlagEnum a03ParentFlag { get; set; }
        public int a03ID_Parent { get; set; }

        public string a05Name { get; set; }//combo
        public string a05UIVCode;
        public string a06Name;
        public string a21Name { get; set; }//combo
        public string a09Name { get; set; } //combo
        public string a09UIVCode;

     
        public string NamePlusRedizo
        {
            get
            {
                return this.a03REDIZO + " - " + this.a03Name;
            }
        }
    }
}
