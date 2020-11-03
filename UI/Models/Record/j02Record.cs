﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class j02Record: BaseRecordViewModel
    {
        public BO.j02Person Rec { get; set; }
        public string EmployerName { get; set; }
        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
    }
}
