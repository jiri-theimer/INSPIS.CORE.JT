using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Bcpg.OpenPgp;
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

        private string GetTempFilePath()
        {
            return Factory.App.TempFolder + "\\" + Factory.CurrentUser.j03Login + "_Stat_CheckedIDs.txt";
        }
        private string GetTempFilePathFilter()
        {
            return Factory.App.TempFolder + "\\" + Factory.CurrentUser.j03Login + "_AddFilter.txt";
        }
        public IActionResult Index()
        {
            var v = new StatViewModel() { guid = BO.BAS.GetGuid() };
            v.lisTemp = LoadAddFilterFromTemp(v);

            v.f06IDs = Factory.CBL.LoadUserParam("Stat-f06IDs");
            v.ValuesMode = (BO.StatValueMode)Factory.CBL.LoadUserParamInt("Stat-ValuesMode", 1);
            v.GroupByMode = (BO.StatGroupByMode)Factory.CBL.LoadUserParamInt("Stat-GroupByMode", 0);
            v.IsZeroRow = Factory.CBL.LoadUserParamBool("Stat-IsZeroRow", true);
            v.IsBlankA11IDs = Factory.CBL.LoadUserParamBool("Stat-IsBlankA11IDs", false);
            v.GuidAddFilter = BO.BAS.GetGuid();
            if (string.IsNullOrEmpty(v.f06IDs) == false && System.IO.File.Exists(GetTempFilePath()))
            {
                v.CheckedIDs = System.IO.File.ReadAllText(GetTempFilePath());
            }




            RefreshStateIndex(v);
            inhale_tree1(v);

            return ViewTup(v, BO.j05PermValuEnum.Menu_Analyze);
        }

        [HttpPost]
        public IActionResult Index(StatViewModel v, string oper, int f06id, int index)
        {

            RefreshStateIndex(v);
            inhale_tree1(v);

            if (oper == "add_f06ids")
            {
                Factory.CBL.SetUserParam("Stat-f06IDs", v.f06IDs);
                return View(v);
            }
            if (oper == "remove_f06id")
            {
                var lis = BO.BAS.ConvertString2ListInt(v.f06IDs);
                if (lis.Contains(f06id))
                {
                    lis.Remove(f06id);
                    v.f06IDs = string.Join(",", lis);
                    Factory.CBL.SetUserParam("Stat-f06IDs", v.f06IDs);
                    RefreshStateIndex(v);
                }

                return View(v);
            }
            if (oper == "addfilter")
            {
                v.ActiveTabIndex = 2;
                var c = new BO.p85Tempbox() { p85GUID = v.GuidAddFilter, p85FreeText01 = v.SelectedAddQueryField, p85FreeText02 = v.lisCols.Where(p => p.ComboValue == v.SelectedAddQueryField).First().ComboText };
                if (v.lisTemp.Where(p => p.p85FreeText01 == v.SelectedAddQueryField).Count() > 0)
                {
                    this.AddMessage("Tato veličina již byla vložena do filtru.");
                    return View(v);
                }
                v.lisTemp.Add(c);
                return View(v);
            }
            if (oper == "delete_temp")
            {
                v.lisTemp[index].p85IsDeleted = true;
                v.ActiveTabIndex = 2;
                return View(v);
            }
            if (oper == "postback")
            {
                this.AddMessageTranslated(v.lisCols.Count().ToString());
                return View(v);
            }

            Factory.CBL.SetUserParam("Stat-ValuesMode", ((int)v.ValuesMode).ToString());
            Factory.CBL.SetUserParam("Stat-GroupByMode", ((int)v.GroupByMode).ToString());
            Factory.CBL.SetUserParam("Stat-IsZeroRow", BO.BAS.GB(v.IsZeroRow));
            Factory.CBL.SetUserParam("Stat-IsBlankA11IDs", BO.BAS.GB(v.IsBlankA11IDs));


            if (ModelState.IsValid)
            {
                var f06ids = BO.BAS.ConvertString2ListInt(v.f06IDs);
                if (f06ids.Count() == 0)
                {
                    this.AddMessage("Musíte vybrat formulář."); return View(v);
                }
                if (string.IsNullOrEmpty(v.CheckedIDs) == true || v.lisF19 == null || v.lisF19.Count() == 0)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jednu otázku."); return View(v);
                }
                var mq = new BO.myQuery("f19");
                mq.f06ids = f06ids;
                string strF19IDs = ParseCheckedF19IDs(v.CheckedIDs);
                System.IO.File.WriteAllText(GetTempFilePath(), v.CheckedIDs);
                //mq.SetPids(strF19IDs);
                //var lisF19 = Factory.f19QuestionBL.GetList(mq);
                //var lisCols = Factory.StatBL.GetList_StatColumns(lisF19.Select(p=>p.f19ID).ToList());

                bool bolTestEncryptedValues = !Factory.CurrentUser.TestPermission(BO.j05PermValuEnum.Read_Encrypted_FormValues);

                mq = new BO.myQuery("a01");

                var lis = new List<string>();
                if (v.PeriodFilter.d1 != null)
                {
                    lis.Add("a.a01DateFrom>=" + BO.BAS.GD(v.PeriodFilter.d1));
                }
                if (v.PeriodFilter.d2 != null)
                {
                    lis.Add("a.a01DateUntil<=" + BO.BAS.GD(v.PeriodFilter.d2));
                }

                int intJ72ID = BO.BAS.InInt(v.SelectedJ72ID);
                if (intJ72ID > 0)
                {
                    var recJ72 = Factory.j72TheGridTemplateBL.Load(intJ72ID);
                    mq.lisJ73 = Factory.j72TheGridTemplateBL.GetList_j73(intJ72ID, "a01");
                    DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql("", mq, Factory.CurrentUser);
                    if (!string.IsNullOrEmpty(fq.SqlWhere))
                    {
                        lis.Add("(" + fq.SqlWhere + ")");
                    }
                }

                if (lis.Count() > 0)
                {
                    mq.explicit_sqlwhere = string.Join(" AND ", lis);
                }

                v.guid = BO.BAS.GetGuid();


                bool b = Factory.StatBL.GenerateStatMatrix(v.guid, mq, v.lisCols, v.ValuesMode, false, v.IsBlankA11IDs, v.IsZeroRow, bolTestEncryptedValues);

                if (oper == "excel")
                {
                    v.GridGuid = null;
                    Export2Excel(v);
                    v.ActiveTabIndex = 3;
                }
                if (oper == "grid" || oper == "change_period")
                {
                    v.GridGuid = v.guid;
                    Export2Grid(v);
                    v.ActiveTabIndex = 3;
                }

            }

            return View(v);
        }

        private void Export2Grid(StatViewModel v)
        {
            var sb1 = new System.Text.StringBuilder();
            var sb2 = new System.Text.StringBuilder();
            sb1.Append("a__p86TempStat__a01Signature,a__p86TempStat__a03REDIZO,a__p86TempStat__a37IZO,a__p86TempStat__a17Name,a__p86TempStat__a03Name,a__p86TempStat__a09Name,a__p86TempStat__a05Name");
            sb1.Append(",a__p86TempStat__a01DateFrom,a__p86TempStat__a01DateUntil,a__p86TempStat__b02Name,a__p86TempStat__a10Name,a__p86TempStat__a08Name,a__p86TempStat__a25Name");
            sb2.Append(",,,,,,,,,,,,");
            v.GridContainerCssStyle = null;

            foreach (var c in v.lisCols)
            {
                sb1.Append(",a__p86TempStat__" + c.colField);
                sb2.Append("," + c.colName);
            }
            if (v.lisCols.Count() > 10)
            {
                v.GridContainerCssStyle = "width: " + (1000 + v.lisCols.Count() * 100).ToString() + "px;overflow-x:auto;";
            }

            v.GridColumns = sb1.ToString();
            v.GridHeaders = sb2.ToString();
            Factory.CBL.SetUserParam("Stat-GridGuid", v.guid);
            Factory.CBL.SetUserParam("Stat-AddFilterSql", GetAddFilterSqlWhere(v));
        }

        private void Export2Excel(StatViewModel v)
        {
            var dt = Factory.StatBL.GetList_StatMatrix(v.guid, GetAddFilterSqlWhere(v), v.lisCols, v.GroupByMode);
            var cExcel = new UI.dataExport();
            var cols = new List<BO.StringPair>();
            cols.Add(new BO.StringPair() { Key = "a01Signature", Value = Factory.tra("ID akce") });
            cols.Add(new BO.StringPair() { Key = "a03REDIZO", Value = "REDIZO" });
            cols.Add(new BO.StringPair() { Key = "a37IZO", Value = "IZO" });
            cols.Add(new BO.StringPair() { Key = "a17Name", Value = "Typ IZO" });
            cols.Add(new BO.StringPair() { Key = "a03Name", Value = Factory.tra("Škola") });
            cols.Add(new BO.StringPair() { Key = "a09Name", Value = Factory.tra("Typ zřizovatele") });
            cols.Add(new BO.StringPair() { Key = "a05Name", Value = Factory.tra("Kraj") });
            cols.Add(new BO.StringPair() { Key = "a01DateFrom", Value = Factory.tra("Od") });
            cols.Add(new BO.StringPair() { Key = "a01DateUntil", Value = Factory.tra("Do") });

            cols.Add(new BO.StringPair() { Key = "b02Name", Value = Factory.tra("Stav") });
            cols.Add(new BO.StringPair() { Key = "a10Name", Value = Factory.tra("Typ akce") });
            cols.Add(new BO.StringPair() { Key = "a08Name", Value = Factory.tra("Téma akce") });


            foreach (var col in v.lisCols)
            {
                var c = new BO.StringPair() { Value = col.colName, Key = col.colField };
                cols.Add(c);
            }

            if (cExcel.ToXLSX(dt, Factory.App.TempFolder + "\\" + v.guid + ".xlsx", cols))
            {
                var mq = new BO.myQuery("xx1");

                string strCheckedF19IDs = ParseCheckedF19IDs(v.CheckedIDs);
                mq.f19ids = BO.BAS.ConvertString2ListInt(strCheckedF19IDs);
                mq.explicit_orderby = "f18Ordinal,f18Name,f19Ordinal,f19Name,f21Ordinal,f21Name";
                var lisColsHelp = Factory.f21ReplyUnitBL.GetListJoinedF19(mq);
                cExcel.StatVysvetlivky(Factory.App.TempFolder + "\\" + v.guid + ".xlsx", lisColsHelp, Factory);
                v.XlsExportTempFileName = v.guid + ".xlsx";
                this.AddMessage("MS-EXCEL dokument vygenerován.", "info");
            }

        }

        private void SaveAddFilter2Temp(StatViewModel v)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < v.lisTemp.Count(); i++)
            {
                if (i > 0)
                {
                    sb.Append("$$");
                }
                sb.Append(v.lisTemp[i].p85IsDeleted.ToString() + "|" + v.lisTemp[i].p85FreeText01 + "|" + v.lisTemp[i].p85FreeText02 + "|" + v.lisTemp[i].p85FreeText03 + "|" + v.lisTemp[i].p85FreeText04);
            }
            if (sb.Length == 0)
            {
                if (System.IO.File.Exists(GetTempFilePathFilter()))
                {
                    System.IO.File.Delete(GetTempFilePathFilter());
                }
            }
            else
            {
                System.IO.File.WriteAllText(GetTempFilePathFilter(), sb.ToString());
            }

        }
        private List<BO.p85Tempbox> LoadAddFilterFromTemp(StatViewModel v)
        {
            var ret = new List<BO.p85Tempbox>();
            if (System.IO.File.Exists(GetTempFilePathFilter()))
            {
                var lis = BO.BAS.ConvertString2List(System.IO.File.ReadAllText(GetTempFilePathFilter()), "$$");
                foreach (var s in lis)
                {
                    var arr = BO.BAS.ConvertString2List(s, "|");
                    var c = new BO.p85Tempbox() { p85IsDeleted = Convert.ToBoolean(arr[0]), p85FreeText01 = arr[1], p85FreeText02 = arr[2], p85FreeText03 = arr[3], p85FreeText04 = arr[4] };
                    if (c.p85IsDeleted == false)
                    {
                        ret.Add(c);
                    }

                }

            }

            return ret;
        }


        private void RefreshStateIndex(StatViewModel v)
        {
            if (v.lisTemp == null)
            {
                v.lisTemp = new List<BO.p85Tempbox>();
            }
            v.lisVztah = new List<BO.StringPair>();
            v.lisVztah.Add(new BO.StringPair() { Key = "OR", Value = Factory.tra("NEBO") });
            v.lisVztah.Add(new BO.StringPair() { Key = "AND", Value = Factory.tra("A ZÁROVEŇ") });

            v.lisOperator = new List<BO.StringPair>();
            v.lisOperator.Add(new BO.StringPair() { Key = "", Value = Factory.tra("Nefiltrovat") });
            v.lisOperator.Add(new BO.StringPair() { Key = "IS NOT NULL", Value = Factory.tra("Je vyplněno") });
            v.lisOperator.Add(new BO.StringPair() { Key = "=", Value = Factory.tra("Je rovno") });
            v.lisOperator.Add(new BO.StringPair() { Key = ">=", Value = Factory.tra("Je větší nebo rovno než") });
            v.lisOperator.Add(new BO.StringPair() { Key = ">", Value = Factory.tra("Je větší než") });
            v.lisOperator.Add(new BO.StringPair() { Key = "<", Value = Factory.tra("Je menší než") });
            v.lisOperator.Add(new BO.StringPair() { Key = "<=", Value = Factory.tra("Je menší nebo rovno než") });

            v.PeriodFilter = new PeriodViewModel() { IsShowButtonRefresh = false };
            v.PeriodFilter.IsShowButtonRefresh = true;
            var per = InhalePeriodFilter();
            v.PeriodFilter.PeriodValue = per.pid;
            v.PeriodFilter.d1 = per.d1;
            v.PeriodFilter.d2 = per.d2;

            v.lisF06 = null; v.lisF18 = null; v.lisF19 = null; v.lisCols = null;
            if (!string.IsNullOrEmpty(v.f06IDs))
            {
                var mq = new BO.myQuery("f06");
                mq.SetPids(v.f06IDs);
                v.lisF06 = Factory.f06FormBL.GetList(mq);
                mq = new BO.myQuery("f18");
                mq.f06ids = v.lisF06.Select(p => p.pid).ToList();
                mq.explicit_orderby = "a.f06ID,a.f18TreeIndex";
                v.lisF18 = Factory.f18FormSegmentBL.GetList(mq);
                mq = new BO.myQuery("f19");
                mq.f06ids = v.lisF06.Select(p => p.pid).ToList();
                v.lisF19 = Factory.f19QuestionBL.GetList(mq);

                if (!string.IsNullOrEmpty(v.CheckedIDs))
                {
                    mq = new BO.myQuery("f19");
                    mq.f06ids = v.lisF06.Select(p => p.pid).ToList();
                    string strCheckedF19IDs = ParseCheckedF19IDs(v.CheckedIDs);
                    mq.SetPids(strCheckedF19IDs);
                    var lisCheckedF19 = Factory.f19QuestionBL.GetList(mq);
                    if (lisCheckedF19.Count() > 0)
                    {
                        v.lisCols = Factory.StatBL.GetList_StatColumns(lisCheckedF19.Select(p => p.f19ID).ToList());
                    }

                }

            }

            v.lisJ72 = Factory.j72TheGridTemplateBL.GetList("a01Event", Factory.CurrentUser.pid, null).Where(p => p.j72HashJ73Query == true);
            foreach (var c in v.lisJ72.Where(p => p.j72IsSystem == true))
            {
                c.j72Name = Factory.tra("Výchozí GRID");
            }

            SaveAddFilter2Temp(v);


        }

        private string GetAddFilterSqlWhere(StatViewModel v)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in v.lisTemp.Where(p => p.p85IsDeleted == false && string.IsNullOrEmpty(p.p85FreeText04) == false))
            {
                var arr = c.p85FreeText01.Split("-");
                var intF19ID = BO.BAS.InInt(arr[0]);
                var recF19 = Factory.f19QuestionBL.Load(intF19ID);
                var intF21ID = BO.BAS.InInt(arr[1]);
                var strVal = c.p85FreeText05;
                var strVztah = c.p85FreeText03;
                var strOperator = c.p85FreeText04;
                var strCF = "";
                for (int i = 0; i < v.lisCols.Count(); i++)
                {
                    if (v.lisCols[i].ComboValue == c.p85FreeText01)
                    {
                        strCF = "col" + (i + 1).ToString(); break;
                    }
                }
                if (strOperator == "IS NOT NULL")
                {
                    sb.Append(" " + strVztah + " " + strCF + " IS NOT NULL");
                }
                else
                {
                    if (recF19.f23ID == 1)
                    {
                        //textbox
                        if (recF19.x24ID == 1 || recF19.x24ID == 3)
                        {
                            //'integer nebo decimal - filtrování podle čísel
                            sb.Append(" " + strVztah + " " + " (CASE WHEN ISNUMERIC(" + strCF + ")=1 THEN CONVERT(FLOAT,REPLACE(" + strCF + ",',','.')) END) " + strOperator + " " + strVal.Replace(",", "."));
                        }
                        else
                        {
                            sb.Append(" " + strVztah + " " + strCF + " " + strOperator + " " + BO.BAS.GS(strVal));
                        }
                    }
                    else
                    {
                        sb.Append(" " + strVztah + " " + strCF + " " + strOperator + " " + BO.BAS.GS(strVal));
                    }

                }


            }

            if (sb.Length == 0)
            {
                return null;
            }
            else
            {
                string s = sb.ToString();
                //System.IO.File.WriteAllText("c:\\temp\\hovado.txt", s);
                return BO.BAS.RightString(s, s.Length - 4);
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

        private string ParseCheckedF19IDs(string strCheckedIDs)
        {
            var lis = BO.BAS.ConvertString2List(strCheckedIDs);
            string s = string.Join(",", lis.Where(p => p.Substring(0, 3) == "f19")).Replace("f19-", "");
            return s;
        }
        private void inhale_tree1(UI.Models.StatViewModel v)
        {
            if (v.lisF06 == null) return;

            v.treeNodes = new List<myTreeNode>();
            var lisChecked = BO.BAS.ConvertString2List(v.CheckedIDs);
            int x = 0;
            foreach (var recF06 in v.lisF06)
            {
                x += 1;
                var c = new myTreeNode() { TreeIndex = x, TreeLevel = 1, Text = recF06.f06Name, ImgUrl = "/images/form.png", Pid = recF06.pid, Prefix = "f06", Expanded = true };
                if (lisChecked.Exists(p => p == "f06-" + c.Pid.ToString())) c.Checked = true;
                v.treeNodes.Add(c);


                foreach (var recF18 in v.lisF18.Where(p => p.f06ID == recF06.pid))
                {
                    x += 1;
                    c = new myTreeNode() { TreeIndex = x, TreeLevel = recF18.f18TreeLevel + 1, Text = recF18.f18Name, ImgUrl = "/images/bullet4.gif", Pid = recF18.pid, Prefix = "f18" };
                    if (recF18.f18ParentID == 0)
                    {
                        c.ParentPid = recF06.pid;
                    }
                    else
                    {
                        c.ParentPid = recF18.f18ParentID;
                    }
                    if (lisChecked.Exists(p => p == "f18-" + c.Pid.ToString())) c.Checked = true;
                    v.treeNodes.Add(c);

                    foreach (var recF19 in v.lisF19.Where(p => p.f18ID == recF18.pid))
                    {
                        x += 1;
                        c = new myTreeNode() { ParentPid = recF18.pid, TreeIndex = x, TreeLevel = recF18.f18TreeLevel + 2, Text = recF19.pid.ToString() + " - " + recF19.f19Name, ImgUrl = "/images/" + recF19.Icon, Pid = recF19.pid, Prefix = "f19" };
                        if (lisChecked.Exists(p => p == "f19-" + c.Pid.ToString())) c.Checked = true;
                        v.treeNodes.Add(c);
                    }
                }
            }



        }
    }
}