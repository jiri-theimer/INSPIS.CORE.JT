using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public interface IFBL
    {
        public IEnumerable<BO.GetString> GetListAutoComplete(int o15flag);
        public BO.b09WorkflowCommandCatalog LoadB09(int b09id);
        public BO.x26ComboSource LoadX26(int x26id);
        public BO.a45EventRole LoadA45(int a45id);
        public IEnumerable<BO.SysDbObject> GetList_SysObjects();
        public void GenerateCreateUpdateScript(IEnumerable<BO.SysDbObject> lis);
        public IEnumerable<BO.j05Permission> GetListJ05();
    }
    class FBL:BaseBL,IFBL
    {       
        public FBL(BL.Factory mother):base(mother)
        {               
            
        }

        public IEnumerable<BO.j05Permission> GetListJ05()
        {                       
            return _db.GetList<BO.j05Permission>("SELECT * FROM j05Permission ORDER BY j05Order");
        }

        public IEnumerable<BO.GetString> GetListAutoComplete(int intO15Flag)
        {
            return _db.GetList<BO.GetString>("SELECT o15Value as Value FROM o15AutoComplete WHERE o15Flag=@flag ORDER BY o15Ordinary,o15Value", new { flag = intO15Flag });
        }

        public BO.b09WorkflowCommandCatalog LoadB09(int b09id)
        {
            return _db.Load<BO.b09WorkflowCommandCatalog>("SELECT a.*,a.b09ID as pid FROM b09WorkflowCommandCatalog a WHERE a.b09ID=@pid", new { pid = b09id });
        }
        public BO.a45EventRole LoadA45(int a45id)
        {
            return _db.Load<BO.a45EventRole>("SELECT a.*,"+_db.GetSQL1_Ocas("a45",false,false)+" FROM a45EventRole a WHERE a.a45ID=@pid", new { pid = a45id });
        }

        public BO.x26ComboSource LoadX26(int x26id)
        {
            return _db.Load<BO.x26ComboSource>("SELECT a.* FROM x26ComboSource a WHERE a.x26ID=@pid", new { pid = x26id });
        }

        public IEnumerable<BO.SysDbObject> GetList_SysObjects()
        {
            string s = "SELECT ID,name,xtype,schema_ver as version,convert(text,null) as content FROM sysobjects WHERE rtrim(xtype) IN ('V','FN','P','TR','IF') AND name not like 'dt_%' and name not like 'zzz%' and (name not like 'sys%' or name not like 'system_%') order by xtype,name";
            var lis = _db.GetList<BO.SysDbObject>(s);
            foreach(var c in lis)
            {
                string strContent = "";
                var dt = _db.GetDataTable("select colid,text FROM syscomments where id="+c.ID.ToString()+" order by colid");
                foreach(DataRow dbrow in dt.Rows)
                {
                    strContent += dbrow["text"];
                }
                c.Content = strContent;
                c.xType = c.xType.Trim();
            }
            return lis;
        }

        public void GenerateCreateUpdateScript(IEnumerable<BO.SysDbObject> lis)
        {
            var sb = new System.Text.StringBuilder();
            foreach(var c in lis)
            {
                sb.AppendLine("if exists(select 1 from sysobjects where id = object_id('" + c.Name + "') and type = '" + c.xType + "')");
                switch (c.xType)
                {
                    case "P":
                        sb.AppendLine(" drop procedure " + c.Name);
                        break;
                    case "FN":
                    case "IF":
                        sb.AppendLine(" drop function " + c.Name);
                        break;
                    case "V":
                        sb.AppendLine(" drop view " + c.Name);
                        break;
                }
                sb.AppendLine("GO");
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(c.Content);
                sb.AppendLine("");
                sb.AppendLine("GO");

                System.IO.File.WriteAllText(_mother.App.TempFolder + "\\sql_sp_funct_views.sql", sb.ToString());
            }
        }



    }
}
