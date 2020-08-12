﻿using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class a39InstitutionPerson:BaseBO
    {
        [Key]
        public int a39ID { get; set; }

        public int j02ID { get; set; }
        public int a03ID { get; set; }

        public int j04ID_Explicit { get; set; }

        public bool a39IsAllowInspisWS { get; set; }
        public string a39Description { get; set; }

        public string RoleName;
        
        public string Person;
        public string j02Email;
        public string a03Name;
        public string a03REDIZO;
        public string a03ICO;
        public string a06Name;
    }
}
