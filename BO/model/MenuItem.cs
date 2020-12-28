using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class MenuItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsDivider { get; set; }
        public bool IsHeader { get; set; }
        public bool IsActive { get; set; }
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Target { get; set; }
        private string _Url;        
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;

                
            }
        }

        

    }
}
