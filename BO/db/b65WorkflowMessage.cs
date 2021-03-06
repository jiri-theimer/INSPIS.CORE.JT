﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum b65SystemFlagEnum
    {
        None=0,        
        UserAccountLocked=1
    }
    public class b65WorkflowMessage:BaseBO
    {
        [Key]
        public int b65ID { get; set; }
        public int x29ID { get; set; }
        public string b65Name { get; set; }
        public string b65MessageSubject { get; set; }
        public string b65MessageBody { get; set; }
        
        public string x29Name { get; set; } //combo
        public string x29Prefix;
        public b65SystemFlagEnum b65SystemFlag { get; set; }
    }
}
