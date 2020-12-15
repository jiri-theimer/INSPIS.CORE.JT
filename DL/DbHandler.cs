using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace DL
{
    public class DbHandler
    {
        public BO.RunningUser CurrentUser { get; set; }
        private string _conString;
        private string _logDir;
        public DbHandler(string connectstring, BO.RunningUser ru, string strLogDir)
        {
            _conString = connectstring;
            this.CurrentUser = ru;
            _logDir = strLogDir;
        }



        public string RunSp(string strProcName, ref Dapper.DynamicParameters pars,int? timeout_seconds=null)
        {            
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    con.Query(strProcName, pars, null, true, timeout_seconds, System.Data.CommandType.StoredProcedure);
                    if (string.IsNullOrEmpty(pars.Get<string>("err_ret"))==false)
                    {
                        this.CurrentUser.AddMessage(pars.Get<string>("err_ret"));
                        return pars.Get<string>("err_ret");

                    }
                    else
                    {
                        return "1";
                    }
                }
                catch (Exception e)
                {
                    log_error(e, strProcName, pars);
                    return e.Message;

                }


            }
        }


        public T Load<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL).FirstOrDefault();
                }
                catch (Exception e)
                {
                    log_error(e, strSQL);
                    return default(T);
                }

            }
        }
        public T Load<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {

                try
                {
                    return con.Query<T>(strSQL, param).FirstOrDefault();
                }
                catch (Exception e)
                {
                    log_error(e, strSQL, param);
                    return default(T);
                }

            }
        }
        public T Load<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL, pars).FirstOrDefault();
                }
                catch (Exception e)
                {
                    log_error(e, strSQL, pars);
                    return default(T);
                }
                

            }
        }
        public IEnumerable<T> GetList<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    if (CurrentUser.j03IsDebugLog)
                    {
                        log_debug(strSQL, "GetList strSQL", null);
                    }
                    return con.Query<T>(strSQL);
                }
                catch (Exception e)
                {
                    log_error(e, strSQL);
                    return null;
                }


            }
        }
        public IEnumerable<T> GetList<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    if (CurrentUser.j03IsDebugLog)
                    {
                        log_debug(strSQL, "GetList param", param);
                    }
                    return con.Query<T>(strSQL, param);
                }
                catch (Exception e)
                {
                    log_error(e, strSQL, param);
                    return null;
                }

            }
        }
        public IEnumerable<T> GetList<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            //var t0 = DateTime.Now;
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    if (CurrentUser.j03IsDebugLog)
                    {
                        log_debug(strSQL, "GetList Dapper.DynamicParameters pars", pars);
                    }
                    return con.Query<T>(strSQL, pars);
                   
                }
                catch (Exception e)
                {
                    log_error(e, strSQL, pars);
                    return null;
                }

            }
        }

        public System.Data.DataTable GetDataTable(string strSQL, List<DL.Param4DT> pars = null)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection con = new SqlConnection(_conString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strSQL;


                cmd.Connection = con;
                if (pars != null)
                {
                    foreach (var p in pars)
                    {
                        cmd.Parameters.AddWithValue(p.ParName, p.ParValue);
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                try
                {
                    if (CurrentUser.j03IsDebugLog)
                    {
                        log_debug(strSQL, "GetDataTable", null);
                    }
                    adapter.Fill(dt);
                }
                catch (Exception e)
                {
                    log_error(e, strSQL);
                }


                return dt;

            }
        }

        public int SaveRecord(string strTable, DynamicParameters pars, BO.BaseBO rec,bool isvalidity=true,bool istimestamp=true)
        {
            string strPrefix = strTable.Substring(0, 3);
            var strPidField = strPrefix + "ID";
            var s = new System.Text.StringBuilder();
            bool bolInsert = true;
            if (rec.pid > 0) bolInsert = false;

            if (bolInsert)
            {
                s.Append(string.Format("INSERT INTO {0} (", strTable));
                if (istimestamp)
                {
                    pars.Add(strPrefix + "DateInsert", DateTime.Now, System.Data.DbType.DateTime);
                    pars.Add(strPrefix + "UserInsert", this.CurrentUser.j03Login, System.Data.DbType.String);
                }                
            }
            else
            {
                s.Append(string.Format("UPDATE {0} SET ", strTable));
            }
            if (istimestamp)
            {
                pars.Add(strPrefix + "DateUpdate", DateTime.Now, System.Data.DbType.DateTime);
                pars.Add(strPrefix + "UserUpdate", this.CurrentUser.j03Login, System.Data.DbType.String);
            }
            
            if (isvalidity)
            {
                if (rec.ValidFrom == null) rec.ValidFrom = System.DateTime.Now;
                pars.Add(strPrefix + "ValidFrom", rec.ValidFrom, System.Data.DbType.DateTime);
                if (rec.ValidUntil == null) rec.ValidUntil = new DateTime(3000, 1, 1);
                pars.Add(strPrefix + "ValidUntil", rec.ValidUntil, System.Data.DbType.DateTime);
            }
            


            string strF = "", strV = "";

            foreach (var strP in pars.ParameterNames.Where(p => p != "pid"))
            {


                if (bolInsert)
                {
                    strF += "," + strP;
                    strV += ",@" + strP;
                }
                else
                {
                    strV += "," + strP + " = @" + strP;
                }


            }
            strV = strV.Substring(1, strV.Length - 1);
            if (bolInsert)
            {
                strF = strF.Substring(1, strF.Length - 1);
                s.Append(strF + ") VALUES (" + strV + ")");
            }
            else
            {
                s.Append(strV);
                s.Append(" WHERE " + strPidField + " = @pid");
            }


            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    if (bolInsert)
                    {
                        s.Append("; SELECT CAST(SCOPE_IDENTITY() as int) as Value");

                        return con.Query<BO.GetInteger>(s.ToString(), pars).FirstOrDefault().Value;
                    }
                    else
                    {
                        if (con.Execute(s.ToString(), pars) > 0)
                        {
                            return pars.Get<int>("pid");
                        }
                    }
                }
                catch (Exception e)
                {

                    log_error(e, s.ToString(), pars);
                }


            }


            return 0;
        }


        //public bool RunSql(string strSQL, Dapper.DynamicParameters pars, int? timeout_seconds = null)
        //{
        //    using (SqlConnection con = new SqlConnection(_conString))
        //    {
        //        try
        //        {
        //            con.Query(strSQL, pars,null,false,timeout_seconds);
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            log_error(e, strSQL, pars);
        //            return false;
        //        }

        //    }
        //    return false;
        //}
        public bool RunSql(string strSQL, object param = null, int? timeout_seconds = null)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    
                    if (con.Execute(strSQL, param,null,timeout_seconds) > 0)
                    {
                        
                        if (CurrentUser.j03IsDebugLog)
                        {
                            log_debug(strSQL, "RunSql", param);
                        }                        
                        return true;
                    }
                }
                catch (Exception e)
                {
                    log_error(e, strSQL, param);
                    return false;
                }

            }
            return false;
        }

        public string GetSQL1_Ocas(string strPrefix, bool isthegrid = false, bool isvalidity = true,bool istimestamp=true)
        {
            switch (strPrefix)
            {
                case "a01":
                    if (isthegrid == true)
                    {
                        return "a.a01ID as pid,dbo._core_a01_isclosed(a.a01IsClosed,a.a01ValidFrom,a.a01ValidUntil) as isclosed,bc.b02Color as bgcolor,a.a01ParentID as parentpid,a.a01ChildsCount as childscount,a.a01IsTemporary as issimulation";
                    }
                    else
                    {
                        return "a.a01ID as pid,dbo._core_a01_isclosed(a.a01IsClosed,a.a01ValidFrom,a.a01ValidUntil) as isclosed,'a01' as entity,a.a01DateInsert as DateInsert,a.a01UserInsert as UserInsert,a.a01DateUpdate as DateUpdate,a.a01UserUpdate as UserUpdate,a.a01ValidFrom as ValidFrom,a.a01ValidUntil as ValidUntil,a.a01ParentID as parentpid,a.a01ChildsCount as childscount,a.a01IsTemporary as issimulation";
                    }

                case "a11":
                    string s = "a.a11ID as pid,dbo._core_a11_isclosed(a.a11IsLocked,a.a11IsLockedByWorkflow,a11_f06.f06ValidFrom,a11_f06.f06ValidUntil,a11_f06.f06IsA01ClosedStrict,a11_f06.f06IsA01PeriodStrict,a11_a01.a01IsAllFormsClosed,a11_a01.a01IsClosed,a11_a01.a01ValidFrom,a11_a01.a01ValidUntil) as isclosed,case when a.a11IsInProcessing=1 then 'yellow' end as bgcolor";
                    if (isthegrid == true)
                    {
                        return s;
                    }
                    else
                    {
                        return s + ",a.a11DateUpdate as DateUpdate,a.a11DateInsert as DateInsert,a.a11UserUpdate as UserUpdate,a.a11UserInsert as UserInsert";

                    }
                case "a03":
                    if (isthegrid == true)
                    {
                        return string.Format("a.{0}ID as pid,convert(bit,CASE WHEN GETDATE() BETWEEN a.{0}ValidFrom AND a.{0}ValidUntil THEN 0 ELSE 1 end) as isclosed,a.a03ParentFlag as parentflag,a.a03IsTestRecord as istestrecord", strPrefix);
                    }
                    else
                    {
                        return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.{0}ValidFrom AND a.{0}ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity,a.{0}DateInsert as DateInsert,a.{0}UserInsert as UserInsert,a.{0}DateUpdate as DateUpdate,a.{0}UserUpdate as UserUpdate,a.{0}ValidFrom as ValidFrom,a.{0}ValidUntil as ValidUntil,a.a03ParentFlag as parentflag,a.a03IsTestRecord as istestrecord", strPrefix);
                    }
                default:
                    if (isvalidity == true)
                    {
                        if (isthegrid == true)
                        {
                            return string.Format("a.{0}ID as pid,convert(bit,CASE WHEN GETDATE() BETWEEN a.{0}ValidFrom AND a.{0}ValidUntil THEN 0 ELSE 1 end) as isclosed", strPrefix);
                        }
                        else
                        {
                            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.{0}ValidFrom AND a.{0}ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity,a.{0}DateInsert as DateInsert,a.{0}UserInsert as UserInsert,a.{0}DateUpdate as DateUpdate,a.{0}UserUpdate as UserUpdate,a.{0}ValidFrom as ValidFrom,a.{0}ValidUntil as ValidUntil", strPrefix);
                        }

                    }
                    else
                    {
                        if (isthegrid == true)
                        {
                            return string.Format("a.{0}ID as pid,convert(bit,0) as isclosed", strPrefix);
                        }
                        else
                        {
                            if (istimestamp == true)
                            {
                                return string.Format("a.{0}ID as pid,0 as isclosed,'{0}' as entity,a.{0}DateInsert as DateInsert,a.{0}UserInsert as UserInsert,a.{0}DateUpdate as DateUpdate,a.{0}UserUpdate as UserUpdate", strPrefix);
                            }
                            else
                            {
                                return string.Format("a.{0}ID as pid,0 as isclosed,'{0}' as entity", strPrefix);
                            }
                            

                        }

                    }

            }

        }


        private void log_debug(string strSQL,string strProc, object param=null)
        {
            var strPath = string.Format("{0}\\sql-debug-{1}-{2}.log", _logDir,CurrentUser.j03Login, DateTime.Now.ToString("yyyy.MM.dd"));
            System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString() + ", proc: " + strProc });

            //TimeSpan dur = t1 - t0;
            //System.IO.File.AppendAllLines(strPath, new List<string>() { "dur: "+dur.TotalSeconds.ToString()+ ", t0: " + t0.ToString() + ", t1: " + t1.ToString() });
            System.IO.File.AppendAllLines(strPath, new List<string>() { "sql: " + strSQL });
            if (param != null)
            {
                System.IO.File.AppendAllLines(strPath, new List<string>() { "PARAMs: " + param.ToString() });
            }
            
        }
        private void log_debug(string strSQL, string strProc, DynamicParameters pars)
        {
            var strPath = string.Format("{0}\\sql-debug-{1}-{2}.log", _logDir, CurrentUser.j03Login, DateTime.Now.ToString("yyyy.MM.dd"));
            System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString() + ", proc: " + strProc });

            System.IO.File.AppendAllLines(strPath, new List<string>() { "sql: " + strSQL });
            if (pars != null)
            {
                string strVal = "";
                foreach (var strP in pars.ParameterNames)
                {
                    if (pars.Get<dynamic>(strP) != null)
                    {
                        strVal = pars.Get<dynamic>(strP).ToString();
                    }
                    else
                    {
                        strVal = "NULL";
                    }
                    System.IO.File.AppendAllLines(strPath, new List<string>() { "PARAM: " + strP + ", VALUE: " + strVal });
                }
            }
        }
        private void log_error(Exception e, string strSQL, DynamicParameters pars)
        {
            CurrentUser.AddMessage(e.Message);
            var strPath = string.Format("{0}\\sql-error-{1}.log", _logDir, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString(), "CURRENT USER-login: " + CurrentUser.j03Login, "CURRENT USER-name:" + CurrentUser.FullName, "SQL:", strSQL });

            if (pars != null)
            {
                string strVal = "";
                foreach (var strP in pars.ParameterNames)
                {

                    if (pars.Get<dynamic>(strP) != null)
                    {
                        strVal = pars.Get<dynamic>(strP).ToString();
                    }
                    else
                    {
                        strVal = "NULL";
                    }
                    System.IO.File.AppendAllLines(strPath, new List<string>() { "PARAM: " + strP + ", VALUE: " + strVal });

                }
            }

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", "ERROR: ", e.Message });
        }

        private void log_error(Exception e, string strSQL, object param = null)
        {
            if (CurrentUser != null) CurrentUser.AddMessage(e.Message);
            var strPath = string.Format("{0}\\sql-error-{1}.log", _logDir, DateTime.Now.ToString("yyyy.MM.dd"));

            var strParams = "";
            if (param != null)
            {
                strParams = param.ToString();
            }
            if (CurrentUser != null)
            {
                System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString(), "CURRENT USER-login: " + CurrentUser.j03Login, "CURRENT USER-name:" + CurrentUser.FullName, "SQL:", strSQL, "", "PARAMs:", strParams, "", "ERROR:", e.Message });
            }
            else
            {
                System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString(), "SQL:", strSQL, "", "PARAMs:", strParams, "", "ERROR:", e.Message });
            }


        }


        public string ParseMergeSQL(string strSQL, string strPIDValue,string par1=null,string par2=null)
        {
            strSQL = strSQL.Replace("#pid#", strPIDValue, StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("[%pid%]", strPIDValue, StringComparison.OrdinalIgnoreCase);
            if (par1 != null)
            {
                par1 = OcistitSQL(par1);
                strSQL = strSQL.Replace("#par1#", par1, StringComparison.OrdinalIgnoreCase);
            }
            if (par2 != null)
            {
                par2 = OcistitSQL(par2);
                strSQL = strSQL.Replace("#par2#", par2, StringComparison.OrdinalIgnoreCase);
            }            
            return OcistitSQL(strSQL);
        }
        private string OcistitSQL(string strSQL)
        {
            strSQL = strSQL.Replace("/*", "");
            strSQL = strSQL.Replace("*/", "");
            strSQL = strSQL.Replace("--", "");
            strSQL = strSQL.Replace("drop ", "", StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("truncate ", "", StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("delete ", "", StringComparison.OrdinalIgnoreCase);

            return strSQL;
        }

        public int GetIntegerFromSql(string strSQL)
        {
            var dt = GetDataTable(strSQL);
            if (dt.Rows.Count == 0)
            {
                return 0;
            }
            try
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch
            {
                return 0;
            }
            
        }

    }
}
