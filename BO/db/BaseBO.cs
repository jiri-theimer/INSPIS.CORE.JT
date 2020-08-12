using System;

namespace BO
{
    public abstract class BaseBO
    {
        public int pid { get; set; }        
        public bool isclosed;
        public string entity;
        public string UserInsert;
        public DateTime? DateInsert;

        public string UserUpdate;
        public DateTime? DateUpdate;

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }



    }
}
