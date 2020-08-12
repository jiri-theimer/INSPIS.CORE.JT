using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia39InstitutionPersonBL
    {
        public BO.a39InstitutionPerson Load(int pid);
        public IEnumerable<BO.a39InstitutionPerson> GetList(BO.myQuery mq);
        public int Save(BO.a39InstitutionPerson rec);

    }
    class a39InstitutionPersonBL : BaseBL, Ia39InstitutionPersonBL
    {
        public a39InstitutionPersonBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_j02_fullname_desc(j02.j02TitleBeforeName,j02.j02FirstName,j02.j02LastName,j02.j02TitleAfterName) as Person,j02.j02Email,a03.a03Name,a03.a03REDIZO,a03.a03ICO,a06.a06Name");
            sb(",isnull(j04_school.j04Name,j04_system.j04Name) as RoleName,");
            sb(_db.GetSQL1_Ocas("a39"));            
            sb(" FROM a39InstitutionPerson a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN a03Institution a03 ON a.a03ID=a03.a03ID LEFT OUTER JOIN a06InstitutionType a06 ON a03.a06ID=a06.a06ID");
            sb(" LEFT OUTER JOIN j04UserRole j04_school ON a.j04ID_Explicit=j04_school.j04ID");
            sb(" LEFT OUTER JOIN j03User j03 ON j02.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04_system ON j03.j04ID=j04_system.j04ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a39InstitutionPerson Load(int pid)
        {
            return _db.Load<BO.a39InstitutionPerson>(GetSQL1(" WHERE a.a39ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a39InstitutionPerson> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a39InstitutionPerson>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a39InstitutionPerson rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a39ID);
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddInt("a03ID", rec.a03ID, true);
            p.AddInt("j04ID_Explicit", rec.j04ID_Explicit, true);
            p.AddBool("a39IsAllowInspisWS", rec.a39IsAllowInspisWS);
            p.AddString("a39Description", rec.a39Description);
          
            int intPID = _db.SaveRecord("a39InstitutionPerson", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        private bool ValidateBeforeSave(BO.a39InstitutionPerson c)
        {
            if (c.a03ID == 0)
            {
                this.AddMessage("Na vstupu chybí vazba na školu."); return false;
            }
            if (c.j02ID == 0)
            {
                this.AddMessage("Na vstupu chybí vazba na osobní profil."); return false;
            }
            var mq = new BO.myQuery("a39InstitutionPerson");
            mq.a03id = c.a03ID;            
            if (GetList(mq).Where(p=>p.j02ID==c.j02ID && p.pid != c.pid).Count()>0)
            {
                this.AddMessage("Osoba již existuje jako kontaktní u této instituce.");return false;
            }

            return true;
        }

    }

}
