using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij02PersonBL
    {
        public BO.j02Person Load(int pid);
        public BO.j02Person LoadByEmail(string strEmail, int pid_exclude);
        public IEnumerable<BO.j02Person> GetList(BO.myQuery mq);
        public int Save(BO.j02Person rec);
        public bool ValidateBeforeSave(BO.j02Person rec);
        public BO.j02RecordSummary LoadSummary(int pid, string pagesource);

    }
    class j02PersonBL : BaseBL,Ij02PersonBL
    {
        public j02PersonBL(BL.Factory mother):base(mother)
        {
           
        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j02"));
            sb(",j04.j04Name,j07.j07Name,j03.j03Login,j03.j03ID,a02.a04ID,a04.a04City,a04.a05ID");
            sb(" FROM j02Person a");
            sb(" LEFT OUTER JOIN j03User j03 ON a.j02ID = j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID = j04.j04ID");
            sb(" LEFT OUTER JOIN j07Position j07 ON a.j07ID=j07.j07ID");
            sb(" LEFT OUTER JOIN a02Inspector a02 ON a.j02ID=a02.j02ID LEFT OUTER JOIN a04Inspectorate a04 ON a02.a04ID=a04.a04ID");
            sb(strAppend);
            return sbret();
        }
       
        public BO.j02Person Load(int intPID)
        {
            return _db.Load<BO.j02Person>(GetSQL1(" WHERE a.j02ID=@pid"), new { pid = intPID });            
        }
        public BO.j02Person LoadByEmail(string strEmail, int pid_exclude)
        {
            return _db.Load<BO.j02Person>(GetSQL1(" WHERE a.j02Email LIKE @email AND a.j02ID<>@pid_exclude"), new { email = strEmail, pid_exclude = pid_exclude });
        }

        public IEnumerable<BO.j02Person>GetList(BO.myQuery mq)
        {            
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq,_mother.CurrentUser);
            return _db.GetList<BO.j02Person>(fq.FinalSql,fq.Parameters);
        }

        
        public int Save(BO.j02Person rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j02ID);           
            p.AddInt("j07ID",rec.j07ID,true);            
            p.AddString("j02FirstName", rec.j02FirstName);
            p.AddString("j02LastName", rec.j02LastName);
            p.AddString("j02TitleBeforeName", rec.j02TitleBeforeName);
            p.AddString("j02TitleAfterName", rec.j02TitleAfterName);
            p.AddString("j02Email", rec.j02Email);      
            p.AddString("j02Mobile", rec.j02Mobile);
            p.AddString("j02Phone", rec.j02Phone);
            p.AddString("j02PID", rec.j02PID);
            p.AddBool("j02IsInvitedPerson", rec.j02IsInvitedPerson);
            p.AddString("j02Address", rec.j02Address);
            p.AddString("j02Position", rec.j02Position);
            p.AddString("j02FullText", BO.BAS.IIFS(string.IsNullOrEmpty(rec.j02TitleBeforeName),"",rec.j02TitleBeforeName + " ") + rec.j02FirstName + " " + rec.j02LastName + BO.BAS.IIFS(string.IsNullOrEmpty(rec.j02TitleAfterName),""," "+rec.j02TitleAfterName));
            return _db.SaveRecord("j02Person", p.getDynamicDapperPars(),rec);
        }

        public bool ValidateBeforeSave(BO.j02Person rec)
        {
            if (string.IsNullOrEmpty(rec.j02FirstName))
            {
                this.AddMessage("Chybí vyplnit [Jméno]."); return false;
            }
            if (string.IsNullOrEmpty(rec.j02LastName))
            {
                this.AddMessage("Chybí vyplnit [Příjmení]."); return false;
            }
            if (string.IsNullOrEmpty(rec.j02Email))
            {
                this.AddMessage("Chybí vyplnit [E-mail]."); return false;
            }
            if (!BO.BAS.IsValidEmail(rec.j02Email))
            {
                this.AddMessage("Zadaná e-mail adresa není platná"); return false;
            }
            if (LoadByEmail(rec.j02Email,rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("E-mail adresa [{0}] již je obsazena jinou osobou."), rec.j02Email));
                return false;
            }

            return true;
        }

        public BO.j02RecordSummary LoadSummary(int pid,string pagesource)
        {
            return _db.Load<BO.j02RecordSummary>("EXEC dbo._core_j02_summary @j03id,@pid,@pagesource,null", new { j03id = _mother.CurrentUser.pid, pid = pid,pagesource=pagesource });
        }
    }
}
