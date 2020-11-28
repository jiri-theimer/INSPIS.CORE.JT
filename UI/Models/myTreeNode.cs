using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class myTreeNode
    {
        public int Pid { get; set; }
        public int ParentPid { get; set; }
        public int TreeLevel { get; set; }
        public int TreeIndex { get; set; }
        public int TreeIndexFrom { get; set; }
        public int TreeIndexTo { get; set; }

        public string Text { get; set; }
        public string TextOcas { get; set; }

        public string Prefix { get; set; }

        public string Url { get; set; }

        public string ImgUrl { get; set; }
        public string CssClass { get; set; }

        public bool Expanded { get; set; }

       
    }

    
}
