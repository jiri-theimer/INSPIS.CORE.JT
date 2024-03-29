﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class FsmViewModel : BaseViewModel
    {
        public string entity { get; set; }
        //public BO.myQuery myQueryGrid { get; set; }
        //public string ExtendPagerHtml { get; set; }
        public TheGridInput gridinput { get; set; }

        public string entityTitle { get; set; }
        public string prefix { get; set; }
        //public int j72id { get; set; }
        //public int go2pid { get; set; }

        //public string master_entity { get; set; }
        public int master_pid { get; set; }
        public string myqueryinline { get; set; } //explicitní myquery ve tvaru název@typ@hodnota, lze předávat více parametrů najednou

        public List<NavTab> NavTabs;

        public string go2pid_url_in_iframe { get; set; }

        public PeriodViewModel period { get; set; } //fixní filtr v horním pruhu

        public string FilterA10ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterA10Name { get; set; }   //fixní filtr v horním pruhu
        public string FilterJ76ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterJ76Name { get; set; }   //fixní filtr v horním pruhu
        public string FilterMyInvolvement { get; set; } //fixní filtr v horním pruhu
        public string FilterH07ID { get; set; }     //fixní filtr v horním pruhu
        public string FilterH07Name { get; set; }   //fixní filtr v horním pruhu


        public int FilterX32ID { get; set; }        //kategorie nad pevnými tiskovými sestavami
        public IEnumerable<BO.x32ReportType> FilterlisX32 { get; set; } //zdroj kategorií pevných sestav


    }


    public class NavTab
    {

        public string Name { get; set; }
        public string Entity { get; set; }
        public string Url { get; set; }

        public string CssClass { get; set; } = "nav-link text-dark";


        public string Badge { get; set; }

    }
}