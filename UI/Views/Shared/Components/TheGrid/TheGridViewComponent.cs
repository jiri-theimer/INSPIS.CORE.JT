using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using UI.Controllers;
using UI.Models;

namespace UI.Views.Shared.Components.TheGrid
{
    public class TheGridViewComponent : ViewComponent
    {
        BL.Factory _f;
        private readonly BL.TheColumnsProvider _colsProvider;
        //private readonly BL.ThePeriodProvider _pp;
        public TheGridViewComponent(BL.Factory f, BL.TheColumnsProvider cp)
        {
            _f = f;
            _colsProvider = cp;
            //_pp = pp;
        }

        public IViewComponentResult
            Invoke(TheGridInput input)
        {

            
            //ret.Entity = entity;
            //var mq = new BO.myQuery(entity);
            if (input.query == null)
            {
                input.query = new BO.myQuery(input.entity);
            }


            BO.TheGridState gridState = null;
            if (input.j72id > 0)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(input.j72id, _f.CurrentUser.pid);
            }
            if (gridState == null)
            {
                gridState = _f.j72TheGridTemplateBL.LoadState(input.entity, _f.CurrentUser.pid, input.master_entity);  //výchozí, systémový grid: j72IsSystem=1
            }

            if (gridState == null)   //pro uživatele zatím nebyl vygenerován záznam v j72 -> vygenerovat
            {
                var cols = _colsProvider.getDefaultPallete(false, input.query);    //výchozí paleta sloupců

                var recJ72 = new BO.j72TheGridTemplate() { j72IsSystem = true, j72Entity = input.entity, j03ID = _f.CurrentUser.pid, j72Columns = String.Join(",", cols.Select(p => p.UniqueName)), j72MasterEntity = input.master_entity };

                var intJ72ID = _f.j72TheGridTemplateBL.Save(recJ72, null, null, null);
                gridState = _f.j72TheGridTemplateBL.LoadState(intJ72ID, _f.CurrentUser.pid);
            }
            if (!string.IsNullOrEmpty(input.fixedcolumns))
            {
                gridState.j72Columns = input.fixedcolumns;
            }
            gridState.j75CurrentRecordPid = input.go2pid;
            gridState.j72MasterEntity = input.master_entity;



            //var cSup = new UI.TheGridSupport(_f, _colsProvider) { extendpagerhtml = input.extendpagerhtml, oncmclick = input.oncmclick, ondblclick = input.ondblclick, fixedcolumns = input.fixedcolumns };
            var cSup = new UI.TheGridSupport(input,_f, _colsProvider);

            var ret = new TheGridViewModel();
            ret.GridInput = input;
            ret.firstdata = cSup.GetFirstData(gridState);

            //ret.ondblclick = ondblclick;
            //ret.oncmclick = oncmclick;
            ret.GridState = gridState;
            ret.Columns = _colsProvider.ParseTheGridColumns(input.entity.Substring(0, 3), gridState.j72Columns, _f.CurrentUser.j03LangIndex);
            ret.AdhocFilter = _colsProvider.ParseAdhocFilterFromString(gridState.j75Filter, ret.Columns);
            //ret.MasterEntity = master_entity;
            //ret.FixedColumns = fixedcolumns;
            //ret.viewstate = viewstate;


            return View("Default", ret);




        }
    }
}
