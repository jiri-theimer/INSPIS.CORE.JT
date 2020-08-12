using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab
{
    public class a01TabCapacityEdit:a01TabCapacity
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateUntil { get; set; }

        public string CheckedDays { get; set; }
        public string UnCheckedDays { get; set; }
    }
}
