using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia10EventTypeBL:BaseInterface
    {
        public BO.a10EventType Load(int pid);
        public IEnumerable<BO.a10EventType> GetList(BO.myQuery mq);
        public IEnumerable<BO.a20EventType_UserRole_PersonalPage> GetListA20(int a10id);
        public int Save(BO.a10EventType rec, List<int> a08ids, List<BO.a20EventType_UserRole_PersonalPage> lisA20);

    }
    class a10EventTypeBL : BaseBL, Ia10EventTypeBL
    {
        public a10EventTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b01.b01Name,");
            sb(_db.GetSQL1_Ocas("a10"));
            sb(" FROM a10EventType a LEFT OUTER JOIN b01WorkflowTemplate b01 ON a.b01ID=b01.b01ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a10EventType Load(int pid)
        {
            return _db.Load<BO.a10EventType>(GetSQL1(" WHERE a.a10ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a10EventType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a10EventType>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a20EventType_UserRole_PersonalPage> GetListA20(int a10id)
        {
            sb("SELECT a.*,j04.j04Name");            
            sb(" FROM a20EventType_UserRole_PersonalPage a INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID");
            sb(" WHERE a.a10ID=@a10id");
           
            return _db.GetList<BO.a20EventType_UserRole_PersonalPage>(sbret(), new { a10id = a10id });
        }
        


        public int Save(BO.a10EventType rec, List<int> a08ids,List<BO.a20EventType_UserRole_PersonalPage> lisA20)
        {
            if (!ValidateBeforeSave(rec, lisA20))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a10ID);
            p.AddInt("b01ID", rec.b01ID,true);
            p.AddInt("a45ID_Creator", rec.a45ID_Creator, true);
            p.AddString("a10Name", rec.a10Name);
            p.AddString("a10Aspx_Insert", rec.a10Aspx_Insert);
            p.AddString("a10Aspx_Framework", rec.a10Aspx_Framework);
            p.AddString("a10Ident", rec.a10Ident);
            p.AddString("a10Description", rec.a10Description);

            p.AddBool("a10IsUse_A08", rec.a10IsUse_A08);
            p.AddBool("a10IsUse_A03", rec.a10IsUse_A03);
            p.AddBool("a10IsUse_A41", rec.a10IsUse_A41);
            p.AddBool("a10IsUse_CaseCode", rec.a10IsUse_CaseCode);
            p.AddBool("a10IsUse_DeadLine", rec.a10IsUse_DeadLine);
            p.AddBool("a10IsUse_A41_J11", rec.a10IsUse_A41_J11);
            p.AddBool("a10IsUse_A41_Leader", rec.a10IsUse_A41_Leader);
            p.AddBool("a10IsUse_Name", rec.a10IsUse_Name);
            p.AddBool("a10IsUse_Period", rec.a10IsUse_Period);
            p.AddBool("a10IsUse_Poll", rec.a10IsUse_Poll);
            p.AddBool("a10IsEpis2", rec.a10IsEpis2);
            p.AddBool("a10IsSupportCloning", rec.a10IsSupportCloning);
            
            p.AddInt("a10OneSchoolInstanceLimit", rec.a10OneSchoolInstanceLimit);
            p.AddBool("a10IsUse_K01", rec.a10IsUse_K01);
            p.AddBool("a10IsUse_ReChangeForms", rec.a10IsUse_ReChangeForms);

            p.AddInt("a10Linker_a10ID", rec.a10Linker_a10ID);
            p.AddInt("a10Linker_a08ID", rec.a10Linker_a08ID);
            p.AddString("a10LinkerDB", rec.a10LinkerDB);

            p.AddString("a10ViewUrl_Insert", rec.a10ViewUrl_Insert);
            p.AddString("a10ViewUrl_Page", rec.a10ViewUrl_Page);
            p.AddString("a10CoreFlag", rec.a10CoreFlag);

            int intPID = _db.SaveRecord("a10EventType", p, rec);
            if (a08ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM a26EventTypeThemeScope WHERE a10ID=@pid", new { pid = intPID });
                }
                if (a08ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO a26EventTypeThemeScope(a10ID,a08ID) SELECT @pid,a08ID FROM a08Theme WHERE a08ID IN (" + string.Join(",", a08ids) + ")", new { pid = intPID });
                }
            }
            if (lisA20 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM a20EventType_UserRole_PersonalPage WHERE a10ID=@pid", new { pid = intPID });
                }
                foreach(var c in lisA20)
                {
                    _db.RunSql("INSERT INTO a20EventType_UserRole_PersonalPage(a10ID,j04ID,a20Aspx_Framework) VALUES(@pid,@j04id,@page)", new { pid = intPID, j04id = c.j04ID, page = c.a20Aspx_Framework });
                }
            }
            


            return intPID;
        }
        private bool ValidateBeforeSave(BO.a10EventType rec, List<BO.a20EventType_UserRole_PersonalPage> lisA20)
        {
            if (string.IsNullOrEmpty(rec.a10Name))
            {
                this.AddMessage("Chybí vyplnit [Název typu akce]."); return false;
            }
            if (rec.b01ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Workflow šablona]."); return false;
            }
            if (rec.a10IsUse_Period && rec.a10IsUse_A41_J11)
            {
                this.AddMessage("Nelze kombinovat možnost plánování s možností definovat řešitele akce přes pojmenované seznamy."); return false;
            }
            if (rec.a10IsUse_A41==false && rec.a10IsUse_A41_Leader)
            {
                this.AddMessage("Volba [Povinný vedoucí akce] je aplikovatelná pouze s volbou [Vyplňovat Personální obsazení akce]."); return false;
            }

            if (lisA20 != null)
            {
                if (lisA20.Where(p => string.IsNullOrEmpty(p.a20Aspx_Framework) == true || p.j04ID==0).Count() > 0)
                {
                    this.AddMessage("V rozpisu Web stránek je nevyplněná role nebo url stránky."); return false;
                }
                if (lisA20.Count()>lisA20.Select(p=>p.j04ID).Distinct().Count())
                {
                    this.AddMessage("V rozpisu Web stránek je duplicitně aplikační role."); return false;
                }
            }

            return true;
        }

    }
}
