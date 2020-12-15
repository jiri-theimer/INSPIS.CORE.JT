using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BL
{
    public interface IStatBL
    {
        
        public List<BO.StatColumn> GetList_StatColumns(List<int>f19ids);
        public bool GenerateStatMatrix(string strGUID, BO.myQueryA01 mq, List<BO.StatColumn> lisCols, BO.StatValueMode valueMODE, bool bolConvertNullCheckboxToZero, bool bolIncludeBlankA11IDs, bool bolConvertZeroChkListValsToNull, bool bolTestEncryptedValues);
        public DataTable GetList_StatMatrix(string strGUID, string strAddSqlWHERE, List<BO.StatColumn> lisCols, BO.StatGroupByMode GroupByMode);

    }
    class StatBL : BaseBL, IStatBL
    {
        public StatBL(BL.Factory mother) : base(mother)
        {

        }
        public bool GenerateStatMatrix(string strGUID,BO.myQueryA01 mq,List<BO.StatColumn> lisCols,BO.StatValueMode valueMODE,bool bolConvertNullCheckboxToZero,bool bolIncludeBlankA11IDs,bool bolConvertZeroChkListValsToNull,bool bolTestEncryptedValues)
        {

           DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql("", mq, _mother.CurrentUser);
           
            string strW_A01 = fq.SqlWhere;  //sql where klauzule seznamu akcí
            if (string.IsNullOrEmpty(strW_A01))
            {
                strW_A01 = "a.a01IsTemporary=0";
            }
            else
            {
                strW_A01 += " AND a.a01IsTemporary=0";
            }
            
            string strF_A01 = "a01Event a INNER JOIN a10EventType a10 ON a.a10ID=a10.a10ID LEFT OUTER JOIN a03Institution a03 ON a.a03ID=a03.a03ID LEFT OUTER JOIN a08Theme a08 ON a.a08ID=a08.a08ID LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID";    //sql pro vnořený seznam akcí

            string strColsInsertOcas = "";
            string strFinalS = "SELECT a11ID,'" + strGUID + "',max(a01ID) as a01ID,min(a01Signature) as a01Signature";
            sb("SELECT a11s.a11ID,a01s.a01ID,a01s.a01Signature");
            int x = 1;
            foreach (var col in lisCols)
            {
                string strWW = "f32s.f19ID=" + col.f19ID.ToString();
                if (col.f21ID > 0)
                {
                    strWW += " AND f32s.f21ID=" + col.f21ID.ToString();
                }
                if (!col.ThisColumnIsComment)
                {
                    //klasický sloupec
                    switch (valueMODE)
                    {
                        case BO.StatValueMode.Nazev:
                            sb(",CASE WHEN " + strWW + " THEN CASE WHEN f19s.f23ID IN (2,4,5) AND f32s.f32Value='1' THEN f21s.f21Name ELSE CASE WHEN f19s.f23ID=10 THEN f32s.f32ValueAliasEvalList ELSE f32s.f32Value END END END as col" + x.ToString());
                            break;
                        case BO.StatValueMode.PID:
                            sb(",CASE WHEN " + strWW + " THEN CASE WHEN f19s.f23ID IN (2,4,5) AND f32s.f32Value='1' AND f19s.f19IsMultiselect=0 THEN convert(varchar(10),f32s.f21ID) ELSE f32s.f32Value END END as col" + x.ToString());
                            break;
                        case BO.StatValueMode.StatID:
                            sb(",CASE WHEN " + strWW + " THEN CASE WHEN f19s.f23ID IN (2,4,5) AND f32s.f32Value='1' THEN isnull(f21s.f21ExportValue,'?') ELSE f32s.f32Value END END as col" + x.ToString());
                            break;
                    }
                }
                else
                {
                    //sloupec s komentářem
                    sb(",CASE WHEN " + strWW + " THEN f32s.f32Comment END as col" + x.ToString());
                }
                strFinalS += ",max(left(col" + x.ToString() + ",1000)) as col" + x.ToString();
                strColsInsertOcas += ",col" + x.ToString();
                x += 1;
            }

            sb(" FROM f32FilledValue f32s INNER JOIN a11EventForm a11s ON f32s.a11ID=a11s.a11ID");
            sb(" INNER JOIN f19Question f19s ON f32s.f19ID=f19s.f19ID");
            if (valueMODE==BO.StatValueMode.Nazev || valueMODE == BO.StatValueMode.StatID)
            {
                sb(" INNER JOIN f21ReplyUnit f21s ON f32s.f21ID=f21s.f21ID");
            }
            sb(" INNER JOIN a01Event a01s ON a11s.a01ID=a01s.a01ID");
           
            sb(" WHERE f32s.f19ID IN (" + string.Join(",", lisCols.Select(p => p.f19ID).Distinct()) + ")");

            sb(" AND a11s.a01ID IN (SELECT a.a01ID FROM " + strF_A01 + " WHERE " + strW_A01 + ")");
            sb(" AND NOT EXISTS (SELECT 1 FROM f35FilledQuestionHidden f35 WHERE f32s.a11ID=f35.a11ID AND f32s.f19ID=f35.f19ID AND f35.f35IsHidden=1)");

            
            strFinalS += " FROM (" + sbret() + ") sm GROUP BY sm.a11ID";

            sb("INSERT INTO p86TempStat(a11ID,p86GUID,a01ID,a01Signature" + strColsInsertOcas + ") " + strFinalS);

            
            bool b=_db.RunSql(sbret(), null, 120); //120 sekund timeout
            
           
            if (b)
            {
                if (bolIncludeBlankA11IDs)
                {
                    InsertBlankA11sToP86(strGUID, String.Join(",", lisCols.Select(p => p.f19ID).Distinct()), strW_A01, strF_A01);
                }
                UpdateAdditionalP86Fields(strGUID, false);
            }
            NormalizeBooleanQuestions(strGUID, lisCols);
            if (b && bolTestEncryptedValues)
            {
                HandleEncryptedValues(strGUID, lisCols);
            }
            return b;
        }

        private void HandleEncryptedValues(string strGUID, List<BO.StatColumn> lisCols)
        {
            if (lisCols.Where(p => p.f19IsEncrypted == true).Count() == 0)
            {
                return; //ve výstupu nejsou žádné šifrované otázky
            }
            var f06IDs = lisCols.Select(p => p.f06ID).Distinct().ToList();
            var lisF07 = _db.GetList<BO.f07Form_UserRole_EncryptedPermission>("select * FROM f07Form_UserRole_EncryptedPermission WHERE f06ID IN (" + String.Join(",", f06IDs) + ")");
            string sU = "";
            foreach(var col in lisCols.Where(p => p.f19IsEncrypted == true))
            {
                if (lisF07.Where(p=>p.f06ID==col.f06ID && p.j04ID == _mother.CurrentUser.j04ID).Count() == 0)
                {
                    //uživatel nemá přístup k obsahu této zašifrované otázky
                    if (sU == "")
                    {
                        sU = "UPDATE p86TempStat SET " + col.colField + " = '" + BO.BAS.EncryptedString() + "'";
                    }
                    else
                    {
                        sU += ", " + col.colField + " = '" + BO.BAS.EncryptedString() + "'";
                    }
                }
            }
            if (sU != "")
            {
                _db.RunSql(sU + " WHERE p86GUID=@guid", new { guid = strGUID });
            }
        }

        private void NormalizeBooleanQuestions(string strGUID,List<BO.StatColumn> lisCols)
        {
            foreach (var colF19ID in lisCols.Where(p => p.f23ID == 3).Select(p => p.f19ID).Distinct())
            {
                int intF19ID = colF19ID;
                string sW = "";string sU = "UPDATE p86TempStat";
                foreach(var col in lisCols.Where(p => p.f19ID == intF19ID))
                {
                    if (sW == "")
                    {
                        sW = sW = " WHERE p86GUID=@guid AND (" + col.colField + " ='1'";
                        sU += " SET " + col.colField + "=CASE WHEN " + col.colField + " IS NULL THEN '0' ELSE " + col.colField + " END";
                    }
                    else
                    {
                        sW += " OR " + col.colField + " ='1'";
                        sU += ", " + col.colField + "=CASE WHEN " + col.colField + " IS NULL THEN '0' ELSE " + col.colField + " END";
                    }
                }
                sW += ")";
                _db.RunSql(sU + " " + sW, new { guid = strGUID });
            }
            foreach (var colF19ID in lisCols.Where(p => p.f23ID == 3).Select(p => p.f19ID).Distinct())
            {
                int intF19ID = colF19ID;
                string sW = ""; string sU = "UPDATE p86TempStat";
                foreach (var col in lisCols.Where(p => p.f19ID == intF19ID))
                {
                    if (sW == "")
                    {
                        sW = sW = " WHERE p86GUID=@guid AND ISNULL(" + col.colField + ",'0') ='0'";
                        sU += " SET " + col.colField + "=NULL";
                    }
                    else
                    {
                        sW += " AND ISNULL(" + col.colField + ",'0') ='0'";
                        sU += ", " + col.colField + "=NULL";
                    }
                }
                _db.RunSql(sU + " " + sW, new { guid = strGUID });
            }
        }
        private void InsertBlankA11sToP86(string strGUID,string strF19IDs,string strSQL_WHERE_A01,string strSQL_FROM_A01)
        {
            sb("INSERT INTO p86TempStat(a11ID,p86GUID,a01ID,p86IsBlank)");
            sb("SELECT a11ID,'" + strGUID + "',a01ID,1 FROM a11EventForm");
            sb(" WHERE a01ID IN (SELECT a.a01ID FROM " + strSQL_FROM_A01 + " WHERE " + strSQL_WHERE_A01 + ")");
            sb(" AND f06ID IN (SELECT f18.f06ID FROM f18FormSegment f18 INNER JOIN f19Question f19 ON f18.f18ID=f19.f18ID WHERE f19.f19ID IN (" + strF19IDs + "))");
            sb(" AND a11ID NOT IN (SELECT a11ID FROM p86TempStat WHERE p86GUID=@guid)");
            _db.RunSql(sbret(), new { guid = strGUID });
        }
        private void UpdateAdditionalP86Fields(string strGUID,bool bolGroupByA01ID)
        {
            sb("UPDATE p86TempStat SET a10Name=a10.a10Name,a08Name=a08.a08Name,b02Name=b02.b02Name");
            sb(",a01Signature=a01.a01Signature,a01DateFrom=a01.a01DateFrom,a01DateUntil=a01.a01DateUntil,a01DateClosed=a01.a01DateClosed");
            sb(",a03REDIZO=a03.a03REDIZO,a03Name=a03.a03Name,a05Name=a05.a05Name,a09Name=a09.a09Name,a03ID=a03.a03ID,a03ICO=a03.a03ICO");
            if (!bolGroupByA01ID)
            {
                sb(",a11LockDate=a11.a11LockDate,a37IZO=a37.a37IZO,a17Name=a17.a17UIVCode+' '+a17.a17Name,a25ID=a11.a25ID,a25Name=a25.a25Name,a11AccessToken=a11.a11AccessToken,a11IsLastF06Instance=a11.a11IsLastF06Instance,k01PID=k01.k01PID,Teacher=k01.k01LastName+' '+k01.k01FirstName+isnull(' '+k01.k01TitleBeforeName,'')");
            }
            sb(" FROM p86TempStat a");
            if (!bolGroupByA01ID)
            {
                sb(" INNER JOIN a11EventForm a11 ON a.a11ID=a11.a11ID INNER JOIN a01Event a01 ON a11.a01ID=a01.a01ID");
            }
            else
            {
                sb(" INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID");
            }
            sb(" INNER JOIN a10EventType a10 ON a01.a10ID=a10.a10ID INNER JOIN a08Theme a08 ON a01.a08ID=a08.a08ID");
            sb(" LEFT OUTER JOIN a03Institution a03 ON a01.a03ID=a03.a03ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02 ON a01.b02ID=b02.b02ID");
            sb(" LEFT OUTER JOIN a09FounderType a09 ON a03.a09ID=a09.a09ID");
            sb(" LEFT OUTER JOIN a05Region a05 on a03.a05ID=a05.a05ID");
            if (!bolGroupByA01ID)
            {
                sb(" LEFT OUTER JOIN a37InstitutionDepartment a37 ON a11.a37ID=a37.a37ID LEFT OUTER JOIN a17DepartmentType a17 ON a37.a17ID=a17.a17ID");
                sb(" LEFT OUTER JOIN a25EventFormGroup a25 ON a11.a25ID=a25.a25ID");
                sb(" LEFT OUTER JOIN k01Teacher k01 ON a11.k01ID=k01.k01ID");
            }
            sb(" WHERE a.p86GUID=@guid");
            _db.RunSql(sbret(), new { guid = strGUID });
        }

        

        public List<BO.StatColumn> GetList_StatColumns(List<int> f19ids)
        {

            sb("select a.f19ID,a.f21ID,f19.f19Name,f21.f21Name,f21.f21ExportValue,f19.x24ID");
            sb(",convert(varchar(10),a.f19ID)+'-'+convert(varchar(10),a.f21ID) as colName,f19.f23ID");
            sb(",f19.f19Ordinal,f21.f21Ordinal,f19.f26ID,f18.f18TreeIndex,f19.f19IsMultiselect,isnull(f26.f26Ordinal,99999),f18.f18TreeIndex,f18.f06ID");
            sb(",CASE WHEN f26.f26Ordinal IS NULL THEN f19.f19Ordinal ELSE f26.f26Ordinal END,f21.f21IsCommentAllowed,0 as ThisColumnIsComment,f19.f19IsEncrypted,f18.f06ID");
            sb(" FROM f20ReplyUnitToQuestion a INNER JOIN f19Question f19 ON a.f19ID=f19.f19ID INNER JOIN f21ReplyUnit f21 ON a.f21ID=f21.f21ID INNER JOIN f18FormSegment f18 ON f19.f18ID=f18.f18ID");
            sb(" LEFT OUTER JOIN f26BatteryBoard f26 ON f19.f26ID=f26.f26ID");
            sb(" WHERE a.f19ID IN (" + string.Join(",", f19ids) + ") AND f21.f21validfrom<=getdate() AND f21.f21validuntil>=getdate() AND f19.f23ID IN (3,5) AND f19.f19IsMultiselect=1");
            sb(" UNION select f19.f19ID,0 as f21ID,f19.f19Name,NULL as f21Name,NULL as f21ExportValue,f19.x24ID");
            sb(",convert(varchar(10),f19.f19ID) as colName,f19.f23ID");
            sb(",f19.f19Ordinal,0 as f21Ordinal,f19.f26ID,f18.f18TreeIndex,f19.f19IsMultiselect,isnull(f26.f26Ordinal,99999),f18.f18TreeIndex,f18.f06ID");
            sb(",CASE WHEN f26.f26Ordinal IS NULL THEN f19.f19Ordinal ELSE f26.f26Ordinal END,convert(bit,0) as f21IsCommentAllowed,0 as ThisColumnIsComment,f19.f19IsEncrypted,f18.f06ID");
            sb(" FROM f19Question f19 INNER JOIN f18FormSegment f18 ON f19.f18ID=f18.f18ID");
            sb(" LEFT OUTER JOIN f26BatteryBoard f26 ON f19.f26ID=f26.f26ID");
            sb(" WHERE f19.f19ID IN (" + String.Join(",", f19ids) + ") AND f19.f19validfrom<=getdate() AND f19.f19validuntil>=getdate() AND f19.f23ID NOT IN (3,5)");
            sb(" UNION select a.f19ID,a.f21ID,f19.f19Name,f21.f21Name,f21.f21ExportValue,f19.x24ID");
            sb(",convert(varchar(10),a.f19ID)+'-'+convert(varchar(10),a.f21ID) as colName,f19.f23ID");
            sb(",f19.f19Ordinal,f21.f21Ordinal,f19.f26ID,f18.f18TreeIndex,f19.f19IsMultiselect,isnull(f26.f26Ordinal,99999),f18.f18TreeIndex,f18.f06ID");
            sb(",CASE WHEN f26.f26Ordinal IS NULL THEN f19.f19Ordinal ELSE f26.f26Ordinal END,f21.f21IsCommentAllowed,convert(bit,1) as ThisColumnIsComment,f19.f19IsEncrypted,f18.f06ID");
            sb(" FROM f20ReplyUnitToQuestion a INNER JOIN f19Question f19 ON a.f19ID=f19.f19ID INNER JOIN f21ReplyUnit f21 ON a.f21ID=f21.f21ID INNER JOIN f18FormSegment f18 ON f19.f18ID=f18.f18ID");
            sb(" LEFT OUTER JOIN f26BatteryBoard f26 ON f19.f26ID=f26.f26ID");
            sb(" WHERE a.f19ID IN (" + String.Join(",", f19ids) + ") AND f21.f21validfrom<=getdate() AND f21.f21validuntil>=getdate() AND f21.f21IsCommentAllowed=1");

            sb(" ORDER BY f18.f06ID,f18.f18TreeIndex,CASE WHEN f26.f26Ordinal IS NULL THEN f19.f19Ordinal ELSE f26.f26Ordinal END,f26ID,f19ID,f21Ordinal,f21ExportValue");
                        
            var lis = _db.GetList<BO.StatColumn>(sbret()).ToList();
            for(int x = 0; x < lis.Count(); x++)
            {
                lis[x].colField = "col" + (x+1).ToString();
            }
            

            return lis;
        }



        private DataTable GetList_StatMatrix_GroupBy(string strGUID, string strAddSqlWHERE, List<BO.StatColumn> lisCols, BO.StatGroupByMode GroupByMode)
        {
            //sloučit vyplněná data podle formulářových skupin - a25ID
            sb("SELECT max(a01ID) as a01ID,max(a10Name) as a10Name,max(a08Name) as a08Name,max(b02Name) as b02Name,max(a01Signature) as a01Signature,max(a01DateFrom) as a01DateFrom,max(a01DateUntil) as a01DateUntil,max(a01DateClosed) as a01DateClosed");
            sb(",max(a03REDIZO) as a03REDIZO,max(a03Name) as a03Name,max(a05Name) as a05Name,max(a09Name) as a09Name,max(a03ICO) as a03ICO");
            sb(",NULL as a11LockDate,max(a37IZO) as a37IZO,max(a17UIVCode+' '+a17Name) as a17Name,max(a25ID) as a25ID,max(a25Name) as a25Name,max(a11AccessToken) as a11AccessToken");
            sb(",convert(bit,min(convert(int,p86IsBlank))) as p86IsBlank,max(k01PID) as k01PID,max(Teacher) as Teacher");
            foreach(var col in lisCols)
            {
                sb(",MAX(" + col.colField + ") as " + col.colField);
            }
            sb(" FROM p86TempStat WHERE p86GUID=@guid");
            if (!string.IsNullOrEmpty(strAddSqlWHERE))
            {
                sb(" AND (" + strAddSqlWHERE + ")");
            }
            switch (GroupByMode)
            {
                case BO.StatGroupByMode.GroupByA01:
                    sb(" GROUP BY a01ID,a25ID");
                    sb(" ORDER BY a01ID,a25ID");
                    break;
                case BO.StatGroupByMode.GroupByA03:
                    sb(" AND a11IsLastF06Instance=1");
                    sb(" GROUP BY a03ID");
                    sb(" ORDER BY min(a03Name)");
                    break;
                case BO.StatGroupByMode.GroupByA37:
                    sb(" GROUP BY a03ID,a37IZO");
                    sb(" ORDER BY min(a03Name),a37IZO");
                    break;
                default:
                    sb(" GROUP BY a11ID");
                    sb(" ORDER BY a01ID,max(a25ID)");
                    break;
            }
            var pars = new List<DL.Param4DT>();
            pars.Add(new DL.Param4DT() { ParamType = "string", ParName = "guid", ParValue = strGUID });
            return _db.GetDataTable(sbret(), pars);
        }

        public DataTable GetList_StatMatrix(string strGUID, string strAddSqlWHERE, List<BO.StatColumn> lisCols, BO.StatGroupByMode GroupByMode)
        {
            if (GroupByMode !=BO.StatGroupByMode.NoGroup && lisCols != null)
            {
                return GetList_StatMatrix_GroupBy(strGUID, strAddSqlWHERE, lisCols, GroupByMode);
            }

            sb("select * FROM p86TempStat WHERE p86GUID=@guid");
            if (!string.IsNullOrEmpty(strAddSqlWHERE))
            {
                sb(" AND (" + strAddSqlWHERE + ")");
            }
            sb(" ORDER BY a01ID,a11ID");

            var pars = new List<DL.Param4DT>();
            pars.Add(new DL.Param4DT() { ParamType = "string", ParName = "guid", ParValue = strGUID });

            return _db.GetDataTable(sbret(), pars);
        }

    }
}
