using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ix91TranslateBL
    {
        public BO.x91Translate Load(int pid);        
        public IEnumerable<BO.x91Translate> GetList(BO.myQuery mq);
        public int Save(BO.x91Translate rec);


    }
    class x91TranslateBL : BaseBL, Ix91TranslateBL
    {
        public x91TranslateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.x91ID,a.x91Code,a.x91Orig,a.x91Lang1,a.x91Lang2,");
            sb(_db.GetSQL1_Ocas("x91",true,false,true));
            sb(" FROM x91Translate a");
            sb(strAppend);
            return sbret();
        }
        public BO.x91Translate Load(int pid)
        {
            return _db.Load<BO.x91Translate>(GetSQL1(" WHERE a.x91ID=@pid"), new { pid = pid });
        }
       
        public IEnumerable<BO.x91Translate> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x91Translate>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x91Translate rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x91ID);
            p.AddString("x91Code", rec.x91Code);
            p.AddString("x91Orig", rec.x91Orig);
            p.AddString("x91Lang1", rec.x91Lang1);
            p.AddString("x91Lang2", rec.x91Lang2);
            p.AddString("x91Lang3", rec.x91Lang3);
            p.AddString("x91Lang4", rec.x91Lang4);
            p.AddString("x91Page", rec.x91Page);
           
            int intPID = _db.SaveRecord("x91Translate", p.getDynamicDapperPars(), rec,false,true);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.x91Translate rec)
        {
            if (string.IsNullOrEmpty(rec.x91Code))
            {
                this.AddMessage("Chybí vyplnit [Originál]."); return false;
            }


            return true;
        }

    }
}
