using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ij23NonPersonBL
    {
        public BO.j23NonPerson Load(int pid);
        public IEnumerable<BO.j23NonPerson> GetList(BO.myQuery mq);
        public int Save(BO.j23NonPerson rec);

    }
    class j23NonPersonBL : BaseBL, Ij23NonPersonBL
    {
        public j23NonPersonBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j23"));
            sb(",j24.j24Name,a05.a05name,j24.j24IsDriver");
            sb(" FROM j23NonPerson a INNER JOIN j24NonPersonType j24 ON a.j24ID=j24.j24ID");
            sb(" LEFT OUTER JOIN a05Region a05 ON a.a05id=a05.a05id");
            sb(strAppend);
            return sbret();
        }
        public BO.j23NonPerson Load(int pid)
        {
            return _db.Load<BO.j23NonPerson>(GetSQL1(" WHERE a.j23ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.j23NonPerson> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j23NonPerson>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j23NonPerson rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j23ID);
            p.AddInt("a05ID", rec.a05ID, true);
            p.AddInt("j24ID", rec.j24ID, true);
            p.AddString("j23Name", rec.j23Name);
            p.AddString("j23Code", rec.j23Code);
            p.AddString("j23Description", rec.j23Description);
            
            int intPID = _db.SaveRecord("j23NonPerson", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.j23NonPerson rec)
        {
            if (string.IsNullOrEmpty(rec.j23Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.j24ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Typ zdroje]."); return false;
            }


            return true;
        }

    }
}
