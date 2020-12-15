using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia37InstitutionDepartmentBL: BaseInterface
    {
        public BO.a37InstitutionDepartment Load(int pid);
        public IEnumerable<BO.a37InstitutionDepartment> GetList(BO.myQuery mq);
        public int Save(BO.a37InstitutionDepartment rec);

    }
    class a37InstitutionDepartmentBL : BaseBL, Ia37InstitutionDepartmentBL
    {
        public a37InstitutionDepartmentBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,a03.a03Name,a03.a03REDIZO,a17.a17Name,a17.a17UIVCode,");            
            sb(_db.GetSQL1_Ocas("a37"));
            sb(" FROM a37InstitutionDepartment a INNER JOIN a03Institution a03 ON a.a03ID=a03.a03ID LEFT OUTER JOIN a17DepartmentType a17 ON a.a17ID=a17.a17ID");                        
            sb(strAppend);
            return sbret();
        }
        public BO.a37InstitutionDepartment Load(int pid)
        {
            return _db.Load<BO.a37InstitutionDepartment>(GetSQL1(" WHERE a.a37ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a37InstitutionDepartment> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a37InstitutionDepartment>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a37InstitutionDepartment rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a37ID);
            p.AddInt("a17ID", rec.a17ID, true);
            p.AddInt("a03ID", rec.a03ID, true);
            
            p.AddString("a37Name", rec.a37Name);
            p.AddString("a37IZO", rec.a37IZO);
            p.AddString("a37City", rec.a37City);
            p.AddString("a37Street", rec.a37Street);            
            p.AddString("a37PostCode", rec.a37PostCode);
            p.AddString("a37Phone", rec.a37Phone);
            p.AddString("a37Mobile", rec.a37Mobile);
            p.AddString("a37Fax", rec.a37Fax);
            p.AddString("a37Email", rec.a37Email);
            p.AddString("a37Web", rec.a37Web);            

            int intPID = _db.SaveRecord("a37InstitutionDepartment", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        private bool ValidateBeforeSave(BO.a37InstitutionDepartment c)
        {
            if (c.a17ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Typ činnosti]."); return false;
            }            

            if (_mother.App.Implementation !="UA" && string.IsNullOrEmpty(c.a37IZO) == true)
            {
                this.AddMessage("Chybí vyplnit [IZO]."); return false;
            }

            var mq = new BO.myQuery("a37InstitutionDepartment");
            mq.a03id = c.a03ID;
            var lis = GetList(mq);
            if (lis.Where(p=>p.a37IZO==c.a37IZO && p.pid !=c.pid).Count() > 0)
            {
                this.AddMessage("[IZO kód] nesmí být duplicitní.");return false;
            }
            if (lis.Where(p => p.a17ID == c.a17ID && p.pid != c.pid).Count() > 0)
            {
                this.AddMessage("[Typ činnosti] nesmí být duplicitní."); return false;
            }

            return true;
        }

    }
}
