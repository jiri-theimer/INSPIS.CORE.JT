using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia38NonPersonEventPlanBL
    {
        public BO.a38NonPersonEventPlan Load(int pid);
        public IEnumerable<BO.a38NonPersonEventPlan> GetList(BO.myQueryA38 mq);
        public IEnumerable<BO.a38TimeLine> GetListTimeLine(BO.myQueryA38 mq);
        public IEnumerable<BO.a38TimeLinePerson> GetListPersonTimeLine(BO.myQueryA38 mq);
        public int Save(BO.a38NonPersonEventPlan rec);
        public bool ValidateBeforeSave(BO.a38NonPersonEventPlan rec);
    }
    class a38NonPersonEventPlanBL : BaseBL, Ia38NonPersonEventPlanBL
    {

        public a38NonPersonEventPlanBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a38", false, false));
            sb(",j23.j23Name,j23.j23Code,j25.j25Name");
            sb(",j02.j02FirstName,j02.j02LastName,j02.j02TitleBeforeName,j02.j02TitleAfterName");
            sb(" FROM a38NonPersonEventPlan a INNER JOIN j23NonPerson j23 ON a.j23ID=j23.j23ID");
            sb(" LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j25NonPersonPlanReason j25 ON a.j25ID=j25.j25ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a38NonPersonEventPlan Load(int pid)
        {
            return _db.Load<BO.a38NonPersonEventPlan>(GetSQL1(" WHERE a.a38ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a38NonPersonEventPlan> GetList(BO.myQueryA38 mq)
        {
            
            DL.FinalSqlCommand fq = DL.basQuerySupport.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a38NonPersonEventPlan>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a38TimeLinePerson> GetListPersonTimeLine(BO.myQueryA38 mq)
        {
            sb("SELECT a.j02ID,a.a38PlanDate,COUNT(*) as Krat");
            sb(" FROM a38NonPersonEventPlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID");

            DL.FinalSqlCommand fq = DL.basQuerySupport.GetFinalSql(sbret(), mq, _mother.CurrentUser);

            var lis = _db.GetList<BO.a38TimeLinePerson>(fq.FinalSql + " GROUP BY a.j02ID,a.a38PlanDate", fq.Parameters);

            return lis;
        }
        public IEnumerable<BO.a38TimeLine> GetListTimeLine(BO.myQueryA38 mq)
        {
            sb("SELECT a.j23ID,a.a01ID,a.a38PlanDate,COUNT(*) as Krat");
            sb(" FROM a38NonPersonEventPlan a INNER JOIN j23NonPerson j23 ON a.j23ID=j23.j23ID");

            DL.FinalSqlCommand fq = DL.basQuerySupport.GetFinalSql(sbret(), mq, _mother.CurrentUser);

            var lis = _db.GetList<BO.a38TimeLine>(fq.FinalSql + " GROUP BY a.j23ID,a.a01ID,a.a38PlanDate", fq.Parameters);

            return lis;
        }

        public int Save(BO.a38NonPersonEventPlan rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a38ID);
            p.AddInt("a01ID", rec.a01ID, true);
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddInt("j23ID", rec.j23ID, true);
            p.AddInt("j25ID", rec.j25ID, true);
            p.AddBool("a38IsDriver", rec.a38IsDriver);
            p.AddString("a38Location", rec.a38Location);
            p.AddDateTime("a38PlanDate", rec.a38PlanDate);

            int inta38ID = _db.SaveRecord("a38NonPersonEventPlan", p.getDynamicDapperPars(), rec, false);

            return inta38ID;
        }


        public bool ValidateBeforeSave(BO.a38NonPersonEventPlan rec)
        {
            if (rec.j23ID==0)
            {
                this.AddMessage("Na vstupu chybí nepersonální zdroj."); return false;
            }
            if (rec.j02ID == 0 || rec.a01ID==0)
            {
                this.AddMessage("Na vstupu chybí ID osoby nebo ID akce."); return false;
            }
            
            var lisA38 = GetList(new BO.myQueryA38() { a01id = rec.a01ID, j23id = rec.j23ID });
            if (lisA38.Where(p=>p.j02ID==rec.j02ID && p.a38PlanDate==rec.a38PlanDate).Count() > 0)
            {
                string strPerson = _mother.j02PersonBL.Load(rec.j02ID).FullNameAsc;
                string strCar = _mother.j23NonPersonBL.Load(rec.j23ID).NamePlusCode;
                this.AddMessageTranslated(string.Format("Pro osobu [{0}], den [{1}] a zdroj [{2}] již existuje rezervace.", strPerson, rec.a38PlanDate, strCar));
                return false;
            }
            if (lisA38.Count() > 0 && rec.a38IsDriver==true)
            {
                if (lisA38.Where(p => p.a38PlanDate == rec.a38PlanDate && p.a38IsDriver==true).Count() > 0)
                {
                    string strCar = _mother.j23NonPersonBL.Load(rec.j23ID).NamePlusCode;
                    this.AddMessageTranslated(string.Format("Pro den [{0}] a zdroj [{1}] již je přiřazen řidič.", rec.a38PlanDate, strCar));
                    return false;
                }
            }
            return true;
        }


    }
}
