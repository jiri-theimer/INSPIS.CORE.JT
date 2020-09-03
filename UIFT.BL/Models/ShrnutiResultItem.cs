using System;
using System.Collections.Generic;

namespace UIFT.Models
{
    public class ShrnutiResultItem
    {
        public string Sekce { get; set; }

        public string Otazka { get; set; }

        public int SekceId { get; set; }

        public int OtazkaId { get; set; }

        public string Message { get; set; }

        public string SkipExpression { get; set; }
    }
}
