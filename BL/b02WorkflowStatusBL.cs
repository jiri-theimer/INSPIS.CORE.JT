using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ib02WorkflowStatusBL: BaseInterface
    {
        public BO.b02WorkflowStatus Load(int pid);
        public IEnumerable<BO.b02WorkflowStatus> GetList(BO.myQuery mq);
        public int Save(BO.b02WorkflowStatus rec,List<BO.b07WorkflowMessageToStatus> lisB07, List<BO.b03WorkflowReceiverToStatus> lisB03, List<BO.b10WorkflowCommandCatalog_Binding> lisB10);
        public IEnumerable<BO.b07WorkflowMessageToStatus> GetListB07(int b02id);
        public IEnumerable<BO.b03WorkflowReceiverToStatus> GetListB03(int b02id);
        public IEnumerable<BO.b10WorkflowCommandCatalog_Binding> GetListB10(int b02id);

    }
    class b02WorkflowStatusBL : BaseBL, Ib02WorkflowStatusBL
    {
        public b02WorkflowStatusBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b01.b01Name,");
            sb(_db.GetSQL1_Ocas("b02"));
            sb(" FROM b02WorkflowStatus a INNER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b02WorkflowStatus Load(int pid)
        {
            return _db.Load<BO.b02WorkflowStatus>(GetSQL1(" WHERE a.b02ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.b02WorkflowStatus> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b02Order"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b02WorkflowStatus>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b02WorkflowStatus rec, List<BO.b07WorkflowMessageToStatus> lisB07, List<BO.b03WorkflowReceiverToStatus> lisB03, List<BO.b10WorkflowCommandCatalog_Binding> lisB10)
        {
            if (ValidateBeforeSave(rec, lisB07, lisB03, lisB10) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b02ID);
            p.AddInt("b01ID", rec.b01ID,true);
            p.AddString("b02Name", rec.b02Name);
            p.AddString("b02Ident", rec.b02Ident);
            p.AddNonBlackColorString("b02Color", rec.b02Color);
            p.AddString("b02Message4UIFT", rec.b02Message4UIFT);
            p.AddInt("b02Order", rec.b02Order);
            p.AddInt("b02TimeOut_Total", rec.b02TimeOut_Total);
            p.AddInt("b02TimeOut_SLA", rec.b02TimeOut_SLA);
            p.AddBool("b02IsDefaultStatus", rec.b02IsDefaultStatus);
            p.AddBool("b02IsHoldStatus", rec.b02IsHoldStatus);
            p.AddBool("b02IsSeparateTab", rec.b02IsSeparateTab);
            p.AddBool("b02IsDurationSLA", rec.b02IsDurationSLA);
            p.AddBool("b02IsCommentForbidden", rec.b02IsCommentForbidden);
            
            if (rec.b02UC == null) { rec.b02UC = BO.BAS.GetGuid(); }
            p.AddString("b02UC", rec.b02UC);

            int intPID = _db.SaveRecord("b02WorkflowStatus", p.getDynamicDapperPars(), rec);
            if (intPID>0 && rec.b02IsDefaultStatus)
            {
                _db.RunSql("UPDATE b02WorkflowStatus SET b02IsDefaultStatus=0 WHERE b01ID=@b01id AND b02ID<>@pid", new { b01id = rec.b01ID,pid=intPID });
            }
            if (lisB07 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b07WorkflowMessageToStatus WHERE b02ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB07)
                {
                    _db.RunSql("INSERT INTO b07WorkflowMessageToStatus(b02ID,b65ID,a45ID,j04ID,j11ID) VALUES (@pid,@b65id,@a45id,@j04id,@j11id)", new { pid = intPID, b65id = c.b65ID, a45id = BO.BAS.TestIntAsDbKey(c.a45ID), j04id = BO.BAS.TestIntAsDbKey(c.j04ID), j11id = BO.BAS.TestIntAsDbKey(c.j11ID) });
                }
            }
            if (lisB03 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b03WorkflowReceiverToStatus WHERE b02ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB03)
                {
                    _db.RunSql("INSERT INTO b03WorkflowReceiverToStatus(b02ID,a45ID,j11ID) VALUES (@pid,@a45id,@j11id)", new { pid = intPID, a45id = BO.BAS.TestIntAsDbKey(c.a45ID), j11id = BO.BAS.TestIntAsDbKey(c.j11ID) });
                }
            }
            if (lisB10 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b10WorkflowCommandCatalog_Binding WHERE b02ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB10)
                {
                    _db.RunSql("INSERT INTO b10WorkflowCommandCatalog_Binding(b02ID,b09ID,b10Parameter1) VALUES (@pid,@b09id,@par1)", new { pid = intPID, b09id = c.b09ID, par1 = c.b10Parameter1 });
                }
            }



            return intPID;
        }

        public bool ValidateBeforeSave(BO.b02WorkflowStatus rec, List<BO.b07WorkflowMessageToStatus> lisB07, List<BO.b03WorkflowReceiverToStatus> lisB03, List<BO.b10WorkflowCommandCatalog_Binding> lisB10)
        {
            if (string.IsNullOrEmpty(rec.b02Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.b01ID == 0)
            {
                this.AddMessage("Chybí vazba na workflow šablonu."); return false;
            }
            if (lisB03.Where(p=>p.a45ID==0 || p.j11ID == 0).Count() > 0)
            {
                this.AddMessage("Tablka změn okruhu účastníků akce není korektně vyplněna.");return false;
            }
            if (lisB10.Count() > lisB10.Select(p => p.b09ID).Distinct().Count())
            {
                this.AddMessage("V tabulce příkazů je duplicitní příkaz."); return false;
            }
            if (lisB10.Where(p => p.b09ID == 0).Count() > 0)
            {
                this.AddMessage("V tabulce příkaznů je prázdný řádek."); return false;
            }


            return true;
        }


        public IEnumerable<BO.b07WorkflowMessageToStatus> GetListB07(int b02id)
        {
            sb("SELECT a.*,b65.b65Name,a45.a45Name,j04.j04Name,j11.j11Name,");
            sb(_db.GetSQL1_Ocas("b07", false, false, false));
            sb(" FROM b07WorkflowMessageToStatus a INNER JOIN b65WorkflowMessage b65 ON a.b65ID=b65.b65ID");
            sb(" LEFT OUTER JOIN a45EventRole a45 ON a.a45ID=a45.a45ID LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID");
            sb(" WHERE a.b02ID=@b02id");
            
            return _db.GetList<BO.b07WorkflowMessageToStatus>(sbret(), new { b02id = b02id });
        }
        public IEnumerable<BO.b03WorkflowReceiverToStatus> GetListB03(int b02id)
        {
            sb("SELECT a.*,a45.a45Name,j11.j11Name,");
            sb(_db.GetSQL1_Ocas("b03", false, false, false));
            sb(" FROM b03WorkflowReceiverToStatus a");
            sb(" LEFT OUTER JOIN a45EventRole a45 ON a.a45ID=a45.a45ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID");
            sb(" WHERE a.b02ID=@b02id");

            return _db.GetList<BO.b03WorkflowReceiverToStatus>(sbret(), new { b02id = b02id });
        }
        public IEnumerable<BO.b10WorkflowCommandCatalog_Binding> GetListB10(int b02id)
        {
            sb("SELECT a.*,b09.b09Name,b09.b09ParametersCount,");
            sb(_db.GetSQL1_Ocas("b10", false, false, false));
            sb(" FROM b10WorkflowCommandCatalog_Binding a");
            sb(" INNER JOIN b09WorkflowCommandCatalog b09 ON a.b09ID=b09.b09ID");
            sb(" WHERE a.b02ID=@b02id");
            
            return _db.GetList<BO.b10WorkflowCommandCatalog_Binding>(sbret(), new { b02id = b02id });
        }
    }
}
