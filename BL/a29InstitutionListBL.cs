using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia29InstitutionListBL
    {
        public BO.a29InstitutionList Load(int pid);
        public IEnumerable<BO.a29InstitutionList> GetList(BO.myQuery mq);
        public int Save(BO.a29InstitutionList rec, List<int> a03ids);

    }
    class a29InstitutionListBL : BaseBL, Ia29InstitutionListBL
    {
        public a29InstitutionListBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a29"));
            sb(" FROM a29InstitutionList a");
            sb(strAppend);
            return sbret();
        }
        public BO.a29InstitutionList Load(int pid)
        {
            return _db.Load<BO.a29InstitutionList>(GetSQL1(" WHERE a.a29ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a29InstitutionList> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a29InstitutionList>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a29InstitutionList rec, List<int> a03ids)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a29ID);
            p.AddString("a29Name", rec.a29Name);
            p.AddString("a29Description", rec.a29Description);
            
            int intPID = _db.SaveRecord("a29InstitutionList", p, rec);
            if (rec.pid > 0)
            {
                _db.RunSql("DELETE FROM a43InstitutionToList WHERE a29ID=@pid", new { pid = intPID });
            }
            if (a03ids.Count > 0)
            {
                _db.RunSql("INSERT INTO a43InstitutionToList(a29ID,a03ID) SELECT @pid,a03ID FROM a03Institution WHERE a03ID IN (" + string.Join(",", a03ids) + ")", new { pid = intPID });
            }


            return intPID;
        }
        public bool ValidateBeforeSave(BO.a29InstitutionList rec)
        {
            if (string.IsNullOrEmpty(rec.a29Name))
            {
                this.AddMessage("Chybí vyplnit [Název seznamu]."); return false;
            }

            return true;
        }

    }
}
