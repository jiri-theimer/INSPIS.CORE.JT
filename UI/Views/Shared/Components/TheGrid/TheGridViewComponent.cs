using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent : ViewComponent
    {
        BL.Factory _f;
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;
        public TheGridViewComponent(BL.Factory f, BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _f = f;
            _colsProvider = cp;
            _pp = pp;
        }

        public IViewComponentResult
            Invoke(string entity, int j72id, int go2pid, string master_entity, string oncmclick, string ondblclick, string fixedcolumns,string extendpagerhtml,string controllername,BO.myQuery myquery)
        {

            var ret = new TheGridViewModel() { ControllerName = controllername };
            ret.Entity = entity;
            //var mq = new BO.myQuery(entity);

           
            BO.TheGridState gridState = null;
            if (j72id > 0)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(j72id, _f.CurrentUser.pid);
            }
            if (gridState == null)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(entity, _f.CurrentUser.pid, master_entity);  //výchozí, systémový grid: j72IsSystem=1
            }
         
            if (gridState == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols = _colsProvider.getDefaultPallete(false, myquery);    //výchozí paleta sloupců

                var recJ72 = new BO.j72TheGridTemplate() { j72IsSystem = true, j72Entity = entity, j03ID = _f.CurrentUser.pid, j72Columns = String.Join(",", cols.Select(p => p.UniqueName)), j72MasterEntity = master_entity };

                var intJ72ID = _f.j72TheGridTemplateBL.Save(recJ72, null, null, null);
                gridState = _f.j72TheGridTemplateBL.LoadState(intJ72ID, _f.CurrentUser.pid);
            }
            if (!string.IsNullOrEmpty(fixedcolumns))
            {
                gridState.j72Columns = fixedcolumns;
            }
            gridState.j75CurrentRecordPid = go2pid;
            gridState.j72MasterEntity = master_entity;

            

            var cSup = new UI.TheGridSupport(_f, _colsProvider) { extendpagerhtml = extendpagerhtml,oncmclick=oncmclick,ondblclick=ondblclick,fixedcolumns=fixedcolumns };

            ret.firstdata = cSup.GetFirstData(gridState, myquery);
            
            ret.ondblclick = ondblclick;
            ret.oncmclick = oncmclick;
            ret.GridState = gridState;
            ret.Columns = _colsProvider.ParseTheGridColumns(myquery.Prefix, gridState.j72Columns, _f.CurrentUser.j03LangIndex);
            ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, ret.Columns);
            ret.MasterEntity = master_entity;
            ret.FixedColumns = fixedcolumns;
            
            

            return View("Default", ret);




        }
    }
}
