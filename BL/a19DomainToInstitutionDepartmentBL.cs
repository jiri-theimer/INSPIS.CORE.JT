using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia19DomainToInstitutionDepartmentBL
    {
        public BO.a19DomainToInstitutionDepartment Load(int pid);
        public IEnumerable<BO.a19DomainToInstitutionDepartment> GetList(BO.myQuery mq);
        public int Save(BO.a19DomainToInstitutionDepartment rec);

    }
    class a19DomainToInstitutionDepartmentBL : BaseBL, Ia19DomainToInstitutionDepartmentBL
    {
        public a19DomainToInstitutionDepartmentBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,a37.a37Name,a37.a37IZO,a18.a18Name,a18.a18Code,a18.a18Code+' - '+a18.a18Name as a18CodePlusName,a37.a37IZO+' - '+a37.a37Name as a37IzoPlusName,a37.a03ID,");
            sb(_db.GetSQL1_Ocas("a19",false,false,false));
            sb(" FROM a19DomainToInstitutionDepartment a INNER JOIN a37InstitutionDepartment a37 ON a.a37ID=a37.a37ID INNER JOIN a18DepartmentDomain a18 ON a.a18ID=a18.a18ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a19DomainToInstitutionDepartment Load(int pid)
        {
            return _db.Load<BO.a19DomainToInstitutionDepartment>(GetSQL1(" WHERE a.a19ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a19DomainToInstitutionDepartment> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a19DomainToInstitutionDepartment>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a19DomainToInstitutionDepartment rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a19ID);
            p.AddInt("a18ID", rec.a18ID, true);
            p.AddInt("a37ID", rec.a37ID, true);

            p.AddBool("a19IsShallEnd", rec.a19IsShallEnd);
            p.AddInt("a19StudyCapacity", rec.a19StudyCapacity);
            p.AddInt("a19StudyDuration", rec.a19StudyDuration);
            p.AddInt("a19StudyLanguage", rec.a19StudyLanguage);
            p.AddInt("a19StudyPlatform", rec.a19StudyPlatform);            

            int intPID = _db.SaveRecord("a19DomainToInstitutionDepartment", p.getDynamicDapperPars(), rec,false,false);



            return intPID;
        }

        private bool ValidateBeforeSave(BO.a19DomainToInstitutionDepartment c)
        {
            if (c.a18ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Vzdělávací obor]."); return false;
            }
            if (c.a37ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Činnost školy]."); return false;
            }

            return true;
        }

    }
}
