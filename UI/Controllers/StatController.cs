using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Tls;
using UI.Models;

namespace UI.Controllers
{
    public class StatController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public StatController(BL.ThePeriodProvider pp)
        {
            _pp = pp;
        }

        public IActionResult Index()
        {
            var v = new StatViewModel() { guid = BO.BAS.GetGuid() };
            v.f06IDs = Factory.CBL.LoadUserParam("Stat-f06IDs");
            v.ValuesMode = (BO.StatValueMode) Factory.CBL.LoadUserParamInt("Stat-ValuesMode", 1);
            v.GroupByMode = (BO.StatGroupByMode)Factory.CBL.LoadUserParamInt("Stat-GroupByMode", 0);
            v.IsZeroRow = Factory.CBL.LoadUserParamBool("Stat-IsZeroRow",true);
            v.IsBlankA11IDs = Factory.CBL.LoadUserParamBool("Stat-IsBlankA11IDs", false);
            RefreshStateIndex(v);
           
            return View(v);
        }

        [HttpPost]
        public IActionResult Index(StatViewModel v, string oper)
        {
            RefreshStateIndex(v);

            if (oper == "change_f06ids")
            {
                Factory.CBL.SetUserParam("Stat-f06IDs", v.f06IDs);
                return View(v);
            }
            
            Factory.CBL.SetUserParam("Stat-ValuesMode", ((int)v.ValuesMode).ToString());
            Factory.CBL.SetUserParam("Stat-GroupByMode", ((int)v.GroupByMode).ToString());
            Factory.CBL.SetUserParam("Stat-IsZeroRow", BO.BAS.GB(v.IsZeroRow));
            Factory.CBL.SetUserParam("Stat-IsBlankA11IDs", BO.BAS.GB(v.IsBlankA11IDs));

           
            if (ModelState.IsValid)
            {
                var f06ids = BO.BAS.ConvertString2ListInt(v.f06IDs);
                if (f06ids.Count()==0)
                {
                    this.AddMessage("Musíte vybrat formulář.");return View(v);
                }
                var mq = new BO.myQuery("f19");
                mq.f06ids = f06ids;
                var lisF19 = Factory.f19QuestionBL.GetList(mq);
                var lisCols = Factory.StatBL.GetList_StatColumns(lisF19.Select(p=>p.f19ID).ToList());

                bool bolTestEncryptedValues = !Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.Read_Encrypted_FormValues);

                mq = new BO.myQuery("a01");

                v.guid = BO.BAS.GetGuid();
                bool b = Factory.StatBL.GenerateStatMatrix(v.guid, mq, lisCols, v.ValuesMode, false, v.IsBlankA11IDs, v.IsZeroRow, bolTestEncryptedValues);

                if (oper == "excel")
                {
                    
                    Export2Excel(v,lisCols);
                }
                
            }

                return View(v);
        }

        private void Export2Excel(StatViewModel v, List<BO.StatColumn> lisCols)
        {
            var dt = Factory.StatBL.GetList_StatMatrix(v.guid, null, lisCols, v.GroupByMode);
            var cExcel = new UI.dataExport();
            var cols = new List<BO.StringPair>();
            cols.Add(new BO.StringPair() { Key = "a01Signature", Value = "ID akce" });
            cols.Add(new BO.StringPair() { Key = "a03REDIZO", Value = "REDIZO" });
            cols.Add(new BO.StringPair() { Key = "a37IZO", Value = "IZO" });
            cols.Add(new BO.StringPair() { Key = "a17Name", Value = "Typ IZO" });
            cols.Add(new BO.StringPair() { Key = "a03Name", Value = "Škola" });
            cols.Add(new BO.StringPair() { Key = "a09Name", Value = "Typ zřizovatele" });
            cols.Add(new BO.StringPair() { Key = "a05Name", Value = "Kraj" });
            cols.Add(new BO.StringPair() { Key = "a01DateFrom", Value = "Od" });
            cols.Add(new BO.StringPair() { Key = "a01DateUntil", Value = "Do" });
            
            cols.Add(new BO.StringPair() { Key = "b02Name", Value = "Stav" });
            cols.Add(new BO.StringPair() { Key = "a10Name", Value = "Typ akce" });
            cols.Add(new BO.StringPair() { Key = "a08Name", Value = "Téma akce" });
            
            
            foreach (var col in lisCols)
            {
                var c = new BO.StringPair() { Value = col.colName, Key = col.colField };
                cols.Add(c);
            }
           
            if (cExcel.ToXLSX(dt, Factory.App.TempFolder + "\\" + v.guid + ".xlsx", cols))
            {
                v.XlsExportTempFileName = v.guid + ".xlsx";
                this.AddMessage("MS-EXCEL dokument vygenerován.", "info");
            }
            
        }


        private void RefreshStateIndex(StatViewModel v)
        {

            v.PeriodFilter = new PeriodViewModel() { IsShowButtonRefresh = false };
            v.PeriodFilter.IsShowButtonRefresh = true;
            var per = InhalePeriodFilter();
            v.PeriodFilter.PeriodValue = per.pid;
            v.PeriodFilter.d1 = per.d1;
            v.PeriodFilter.d2 = per.d2;

            if (!string.IsNullOrEmpty(v.f06IDs))
            {
                var mq = new BO.myQuery("f06");
                mq.SetPids(v.f06IDs);
                v.lisF06 = Factory.f06FormBL.GetList(mq);
            }
            
        }



        private BO.ThePeriod InhalePeriodFilter()
        {
            var ret = _pp.ByPid(0);
            int x = Factory.CBL.LoadUserParamInt("stat-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("stat-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("stat-period-d2");

            }

            return ret;
        }
    }
}