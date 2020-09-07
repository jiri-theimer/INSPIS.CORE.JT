using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace BO
{
    public class ItemValidationResult
    {
        public string Sekce { get; set; }
        public string Otazka { get; set; }
        public int SekceId { get; set; }
        public int OtazkaId { get; set; }
        public string Message { get; set; }

        public int f06ID { get; set; }
        public string f06Name { get; set; }
        public int a11ID { get; set; }
    }
}
