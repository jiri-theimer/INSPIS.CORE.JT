using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class PasswordPolicity
    {
        public bool RequireDigit { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
    }
}
