﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ReportModalViewModel:BaseViewModel
    {
        public int x29ID { get; set; }
        
        
        public BO.x31Report RecX31 { get; set; }
        public string ReportFileName { get; set; }

        public int SelectedX31ID { get; set; }
        public string SelectedReport { get; set; }
        public int rec_pid { get; set; }
        public string rec_prefix { get; set; }
        public string UserParamKey { get; set; }

    }
}
