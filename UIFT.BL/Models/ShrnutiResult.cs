using System;
using System.Collections.Generic;

namespace UIFT.Models
{
    public class ShrnutiResult
    {
        public List<Models.ShrnutiResultItem> Items { get; set; }

        public bool Success { get; set; }

        public string WorkflowMessage { get; set; }
    }
}
