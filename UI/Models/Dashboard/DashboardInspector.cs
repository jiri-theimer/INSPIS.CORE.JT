﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Dashboard
{
    public class DashboardInspector:BaseViewModel
    {
        public BO.j02Person Rec { get; set; }
        public int pid { get; set; }
        public string SearchBoxA01 { get; set; }
        public string SearchBoxA03 { get; set; }


        public List<NavTab> NavTabs { get; set; }
        public string DefaultNavTabUrl { get; set; }
    }
}
