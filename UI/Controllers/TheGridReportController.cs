using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Controllers
{
    public class TheGridReportController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridReportController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }
        public IActionResult Index(int j72id,string pids)
        {            
            var v = new TheGridReport() { j72id = j72id };

            var gridState = Factory.j72TheGridTemplateBL.LoadState(j72id, Factory.CurrentUser.pid);

            v.prefix = gridState.j72Entity.Substring(0, 3);

            if (!string.IsNullOrEmpty(pids))
            {
                v.pids = BO.BAS.ConvertString2ListInt(pids);
            }

            var mq= new BO.InitMyQuery().Load(gridState.j72Entity);
            mq.SetPids(pids);
            mq.explicit_columns = _colsProvider.ParseTheGridColumns(v.prefix, gridState.j72Columns, Factory.CurrentUser.j03LangIndex);


            var dt = Factory.gridBL.GetList(mq, false);


            //TheGridInput input = new TheGridInput() { j72id = v.j72id, entity = gridState.j72Entity };
            
            //var cSup = new UI.TheGridSupport(input, Factory, _colsProvider);

            return View(v);
        }
    }
}
