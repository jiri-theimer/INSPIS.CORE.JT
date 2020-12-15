using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BO
{
    public static class BAS
    {
        public static string GB(bool b)
        {
            if (b)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
        public static bool BG(string b)
        {
            if (b == null) return false;

            if (b=="1" || b.ToLower()=="true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string GS(string s)
        {
            if(string.IsNullOrEmpty(s) == true)
            {
                return "NULL";
            }
            s = s.Replace("'", "");
            return "'" + s + "'";
        }
        public static string GSS(string s)
        {
            if (string.IsNullOrEmpty(s) == true)
            {
                return "";
            }
            s = s.Replace("'", "").Replace("--", "##");
            return s;
        }
        public static string GD(DateTime? d, DateTime? valIfNull=null)
        {
            if (d == null)
            {
                if (valIfNull == null)
                {
                    return "NULL";
                }
                else
                {
                    d = valIfNull;
                }
                
            }
            return "CONVERT(datetime,'" + ObjectDate2String(d, "dd.MM.yyyy") + "',104)";
        }
        public static string GN(double n)
        {
            return n.ToString().Replace(",", ".");
        }
        public static string IIFS(bool b,string value_true,string value_false)
        {
            if (b)
            {
                return value_true;
            }
            else
            {
                return value_false;
            }
        }
        public static string OM2(string s,int maxlen)
        {
            if (s == null) return null;
            if (s.Length > maxlen)
            {
                return s.Substring(0, maxlen - 1) + "...";
            }
            else
            {
                return s;
            }
        }
        public static string LeftString(string s, int maxlen)
        {
            if (s == null) return null;
            if (s.Length > maxlen)
            {
                return s.Substring(0, maxlen);
            }
            else
            {
                return s;
            }
        }
        public static int InInt(string s)
        {
            if (int.TryParse(s, out int x))
            {
                return x;
            }
            else
            {
                return 0;
            }
        }
        public static Double InDouble(string s)
        {
            if (double.TryParse(s, out Double x))
            {
                return x;
            }
            else
            {
                return 0;
            }
        }

        public static List<string> ConvertString2List(string s, string strDelimiter = ",")
        {
            var lis = new List<string>();

            if (s == null)
                return lis;
            
            lis.AddRange(s.Split(strDelimiter));

            return lis;
        }
        public static List<int> ConvertString2ListInt(string s, string strDelimiter = ",")
        {
            var lis = new List<int>();

            if (String.IsNullOrEmpty(s) == true)
                return lis;
            foreach(var ss in s.Split(strDelimiter))
            {
                lis.Add(BO.BAS.InInt(ss));
            }
            
            return lis;
        }
        public static string RemoveValueFromDelimitedString(string s,string sremove, string strDelimiter = ",")
        {
            List<string> lis = ConvertString2List(s, strDelimiter);
            if (lis.Contains(sremove))
            {
                lis.Remove(sremove);
            }
            
            return String.Join(strDelimiter, lis);
        }

        public static int? TestIntAsDbKey(int intPID)
        {
            if (intPID == 0)
            {
                return null;
            }
            else
            {
                return intPID;
            }
        }
        public static double? TestDouleAsDbKey(double dbl)
        {
            if (dbl == 0)
            {
                return null;
            }
            else
            {
                return dbl;
            }
        }
        public static decimal? TestDecimalAsDbKey(decimal dcl)
        {
            if (dcl == 0)
            {
                return null;
            }
            else
            {
                return dcl;
            }
        }
        
        public static DateTime String2Date(string d)
        {
            string[] arr = d.Split(".");
            if (arr.Length < 3) return (DateTime.Today);
            if (arr[2].Contains(":"))
            {                //obsahuje čas: dd.MM.yyyy HH:mm
                string[] hhmm= arr[2].Substring(5,arr[2].Length-5).Split(":");
                arr[2] = arr[2].Substring(0, 4);
                int h = int.Parse(hhmm[0]);
                int m = int.Parse(hhmm[1]);
                return (new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]),h,m,0));
            }
            else
            {
                return (new DateTime(int.Parse(arr[2].Substring(0,4)), int.Parse(arr[1]), int.Parse(arr[0])));
            }
            
        }
        public static string DayOfWeekString(DateTime d)
        {
            switch ((int)d.DayOfWeek)
            {
                case 0:
                    return "Neděle";
                case 1:
                    return "Pondělí";
                case 2:
                    return "Úterý";
                case 3:
                    return "Středa";                
                case 4:
                    return "Čtvrtek";
                case 5:
                    return "Pátek";
                case 6:
                    return "Sobota";                
                default:
                    return "??";
            }
        }

        public static DateTime ConvertDateTo235959(DateTime d)
        {
            
            return new DateTime(d.Year, d.Month, d.Day).AddDays(1).AddSeconds(-1);
        }
        public static string ObjectDate2String(object d,string format="dd.MM.yyyy ddd")
        {
            if (d == System.DBNull.Value || d==null) return "";
            return Convert.ToDateTime(d).ToString(format);
        }
        public static string ObjectDateTime2String(object d, string format = "dd.MM.yyyy HH:mm")
        {            
            return ObjectDate2String(d, format);
        }
        public static string Number2String(double n)
        {
            return string.Format("{0:#,0.00}",n);
        }

        

        public static string GetGuid()
        {

            return System.Guid.NewGuid().ToString("N");
        }
       


        public static string FormatFileSize(int byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824)
                size = String.Format("{0:##.##}", byteCount / 1073741824) + " GB";
            else if (byteCount >= 1048576)
                size = String.Format("{0:##.##}", byteCount / 1048576) + " MB";
            else if (byteCount >= 1024)
                size = String.Format("{0:##.##}", byteCount / 1024) + " KB";
            else if (byteCount > 0 && byteCount < 1024)
                size = byteCount.ToString() + " Bytes";

            return size;
        }


        public static string ParseCellValueFromDb(System.Data.DataRow dbRow, BO.TheGridColumn c)
        {
            if (dbRow[c.UniqueName] == System.DBNull.Value)
            {
                return "";
            }
            switch (c.FieldType)
            {
                case "bool":
                    if (Convert.ToBoolean(dbRow[c.UniqueName]) == true)
                    {
                        return "&#10004;";
                    }
                    else
                    {
                        return "";
                    }
                case "num0":
                    return string.Format("{0:#,0}", dbRow[c.UniqueName]);

                case "num":
                    return string.Format("{0:#,0.00}", dbRow[c.UniqueName]);
                case "num3":
                    return string.Format("{0:#,0.000}", dbRow[c.UniqueName]);
                case "num4":
                    return string.Format("{0:#,0.0000}", dbRow[c.UniqueName]);
                case "num5":
                    return string.Format("{0:#,0.00000}", dbRow[c.UniqueName]);
                case "num1":
                    return string.Format("{0:#,0.0}", dbRow[c.UniqueName]);

                case "date":
                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy");


                case "datetime":

                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy HH:mm");
                case "datetimesec":

                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("dd.MM.yyyy HH:mm:ss");
                case "time":
                    return Convert.ToDateTime(dbRow[c.UniqueName]).ToString("HH:mm");
                default:
                    return dbRow[c.UniqueName].ToString();
            }


        }

        public static string RightString(string input, int num)
        {
            if (num > input.Length)
            {
                num = input.Length;
            }
            return input.Substring(input.Length - num);
        }

        public static string Uvozovky2Apostrofy(string s)
        {
            if (string.IsNullOrEmpty(s) == true)
            {
                return s;
            }
            if (s.Contains(Char.ConvertFromUtf32(34)))
            {
                return s.Replace(Char.ConvertFromUtf32(34), "'");
            }
            else
            {
                return s;
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                email = email.Trim();
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string EncryptedString()
        {
            return "********";
        }

        public static string Text2Html(string s)
        {
            if (s == null) return s;
            if (s.Contains("\r\n"))
            {
                return s.Replace("\r\n", "<br>");
            }
            else
            {
                if (s.Contains("\r"))
                {
                    return s.Replace("\r", "<br>");
                }
            }

            return s;
        }


        public static string OcistitSQL(string strSQL)
        {
            if (strSQL == null) return null;

            strSQL = strSQL.Replace("/*", "");
            strSQL = strSQL.Replace("*/", "");
            strSQL = strSQL.Replace("--", "");
            strSQL = strSQL.Replace("drop ", "", StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("truncate ", "", StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("delete ", "", StringComparison.OrdinalIgnoreCase);

            return strSQL;

        }

    }

}




    

