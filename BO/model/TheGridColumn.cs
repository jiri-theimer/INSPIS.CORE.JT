using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheGridColumn
    {
        private string _CssClass;
        private string _Entity;
        private string _Field;
        private string _Prefix;
        public bool IsTimestamp;
        
        public string FieldType { get; set; }   //string, bool, int, num, date, datetime
        public string Header { get; set; }
        public string TranslateLang1 { get; set; }
        public string TranslateLang2 { get; set; }
        public string TranslateLang3 { get; set; }
        public string SqlSyntax { get; set; }
        public int DefaultColumnFlag { get; set; }  //1 - default grid sloupec i combo sloupec, 2 - pouze default grid sloupec
        
        public bool IsSortable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public bool IsFulltext { get; set; }
        public bool IsShowTotals { get; set; }
        public int FixedWidth { get; set; }
        public string EntityAlias { get; set; }
        public bool NotShowRelInHeader { get; set; }

        public string RelName { get; set; } //název relace ve from klauzuly - naplní se v getSelectedPallete
        public string RelSql { get; set; }  //sql relace from klauzule - naplní se v getSelectedPallete
        public string RelSqlDependOn { get; set; } //sql relace na které je závislá RelSql

        public string VisibleWithinEntityOnly { get; set; } //aplikovatelné pouze pro kategorie

        public string Entity
        {
            get
            {
                return _Entity;
            }
            set
            {
                _Entity = value;

                _Prefix = _Entity.Substring(0, 3);
            }
        }
        public string Field
        {
            get
            {
                return _Field;
            }
            set
            {
                _Field = value;
               
            }
        }
       

        public string CssClass
        {
            get
            {
                if (_CssClass == null)
                {
                    if (FieldType == "num" || FieldType == "num0" || FieldType=="num3" || FieldType == "num4" || FieldType=="num5") _CssClass = "tdn";
                    if (FieldType == "bool") _CssClass = "tdb";
                }                
                return _CssClass;
            }
            set
            {
                _CssClass = value;
            }
        }
        public string UniqueName
        {
            get
            {
                if (RelName == null)
                {
                    return "a__" + _Entity + "__" + _Field;
                }
                else
                {
                    return RelName + "__" + _Entity + "__" + _Field;
                }
            }
        }
        


        public string ColumnWidthPixels
        {
            get
            {
                if (this.FixedWidth == 0)
                {
                    return "auto";
                }
                else
                {
                    return this.FixedWidth.ToString() + "px";
                }
            }
        }

        public string NormalizedTypeName
        {
            get
            {
                if (FieldType == "num0" || FieldType == "num" || FieldType=="num3" || FieldType=="num4" || FieldType=="num5" || FieldType=="num1") return "num";
                if (FieldType == "date" || FieldType == "datetime" || FieldType== "datetimesec") return "date";
                return this.FieldType;
            }
            
        }
        
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
        }

        public string getFinalSqlSyntax_SELECT()
        {                        
            if (this.SqlSyntax == null)
            {
                if (this.RelName == null)
                {
                    return "a." + this.Field + " AS " + this.UniqueName;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return this.RelName + "." + this.Field + " AS " + this.UniqueName;   //v RelName je uložený název relace GRIDU
                }               
            }
            else
            {
                if (this.RelName == null)
                {
                    return this.SqlSyntax + " AS " + this.UniqueName;
                }else
                {
                    if (this.SqlSyntax.Contains("a.") || this.SqlSyntax.Contains("relname."))
                    {
                        return this.SqlSyntax.Replace("a.", this.RelName + ".").Replace("relname.", this.RelName) + " AS " + this.UniqueName;
                    }
                    else
                    {
                        return this.SqlSyntax + " AS " + this.UniqueName;
                    }
                    
                }
                                        
            }
            
        }
        public string getFinalSqlSyntax_WHERE()
        {
            if (this.SqlSyntax == null)
            {
                if (this.RelName == null)
                {
                    return "a." + this.Field;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return this.RelName + "." + this.Field;
                }                    
            }
            else
            {
                if (this.RelName == null)
                {
                    return this.SqlSyntax;
                }
                else
                {
                    return this.SqlSyntax.Replace("a.", this.RelName + ".").Replace("relname.", this.RelName);
                }                                    
            }

        }
        public string getFinalSqlSyntax_ORDERBY()
        {
            return getFinalSqlSyntax_WHERE();  //SQL pro ORDERBY je stejná jako WHERE
        }


        public string getFinalSqlSyntax_SUM()
        {
            if (this.IsShowTotals == false) return "NULL as " + this.UniqueName;
            if (this.SqlSyntax == null)
            {
                if (this.RelName == null)
                {
                    return "SUM(a." + this.Field + ") AS " + this.UniqueName;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return "SUM("+this.RelName + "." + this.Field + ") AS " +this.UniqueName;
                }
            }
            else
            {
                if (this.RelName == null)
                {
                    return "SUM(" + this.SqlSyntax + ") AS " + this.UniqueName;
                }
                else
                {
                    return "SUM(" + this.SqlSyntax.Replace("a.",this.RelName+".").Replace("relname.", this.RelName) + ") AS " +this.UniqueName;
                }
                    
            }

        }

        public string getSymbol()
        {
            switch (this.FieldType)
            {
                case "num":
                    return "0.0";
                case "num0":
                    return "000";
                case "date":
                    return "&#128197;";
                case "datetime":
                    return "&#128197;";
                case "bool":
                    return "&#10004;";
                default:
                    return "ab";
            }
        }

        

       
    }
}
