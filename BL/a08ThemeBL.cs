using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia08ThemeBL: BaseInterface
    {
        public BO.a08Theme Load(int pid);
        public IEnumerable<BO.a08Theme> GetList(BO.myQuery mq);
        public int Save(BO.a08Theme rec, List<BO.a12ThemeForm> lisA12, List<BO.a14AttachmentToTheme> lisA14);
        public IEnumerable<BO.a12ThemeForm> GetListA12(int a08id);
        public IEnumerable<BO.a14AttachmentToTheme> GetListA14(int a08id);

    }
    class a08ThemeBL : BaseBL, Ia08ThemeBL
    {
        public a08ThemeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a08"));           
            sb(" FROM a08Theme a");            
            sb(strAppend);
            return sbret();
        }
        public BO.a08Theme Load(int pid)
        {
            return _db.Load<BO.a08Theme>(GetSQL1(" WHERE a.a08ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a08Theme> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a08Theme>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.a12ThemeForm> GetListA12(int a08id)
        {
            sb("SELECT a.*,f06.f06Name,f06.f06BindScopeQuery,");
            sb(_db.GetSQL1_Ocas("a12",false,false));
            sb(" FROM a12ThemeForm a INNER JOIN f06Form f06 ON a.f06ID=f06.f06ID");
            sb(" WHERE a.a08ID=@a08id");
            sb(" ORDER BY a.a12Ordinal,f06.f06Ordinal,f06.f06Name");
            return _db.GetList<BO.a12ThemeForm>(sbret(), new { a08id = a08id });
        }
        public IEnumerable<BO.a14AttachmentToTheme> GetListA14(int a08id)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a14", false, false,false));
            sb(" FROM a14AttachmentToTheme a");
            sb(" WHERE a.a08ID=@a08id");
            sb(" ORDER BY a.a14Ordinal");
            return _db.GetList<BO.a14AttachmentToTheme>(sbret(), new { a08id = a08id });
        }

        public int Save(BO.a08Theme rec, List<BO.a12ThemeForm> lisA12, List<BO.a14AttachmentToTheme> lisA14)
        {
            if (!ValidateBeforeSave(rec, lisA12, lisA14))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())  //podléhá jedné transakci
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.a08ID);
                p.AddString("a08Name", rec.a08Name);
                p.AddString("a08Ident", rec.a08Ident);
                p.AddString("a08Description", rec.a08Description);

                int intPID = _db.SaveRecord("a08Theme", p.getDynamicDapperPars(), rec);
                if (lisA12 != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM a12ThemeForm WHERE a08ID=@pid", new { pid = intPID });
                    }
                    foreach (var c in lisA12)
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("pid", c.pid, true);
                        p.AddInt("a08ID", intPID, true);
                        p.AddInt("f06ID", c.f06ID, true);
                        p.AddBool("a12IsRequired", c.a12IsRequired);
                        p.AddInt("a12Ordinal", c.a12Ordinal);
                        _db.SaveRecord("a12ThemeForm", p.getDynamicDapperPars(), c, false, true);
                    }
                    _db.RunSql("update a11EventForm set a11Ordinal=ISNULL(a12.a12Ordinal,0) from a11EventForm a INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID INNER JOIN a12ThemeForm a12 ON a01.a08ID=a12.a08ID AND a.f06ID=a12.f06ID WHERE a01.a08ID=@a08id", new { a08id = intPID });

                }
                if (lisA14 != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM a14AttachmentToTheme WHERE a08ID=@pid", new { pid = intPID });
                    }
                    foreach (var c in lisA14)
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("pid", c.pid, true);
                        p.AddInt("a08ID", intPID, true);
                        p.AddInt("o13ID", c.o13ID, true);
                        p.AddBool("a14IsRequired", c.a14IsRequired);
                        p.AddInt("a14Ordinal", c.a14Ordinal);
                        _db.SaveRecord("a14AttachmentToTheme", p.getDynamicDapperPars(), c, false, false);
                    }


                    //závěrečná SP kvůli mj. narovnání formulářů v auto-evaluačních akcích, které jsou svázány s tímto tématem                                   
                    _db.RunSql("exec dbo.a08_recovery_autoevaluation @pid", new { pid = intPID });

                }
                sc.Complete();   //potvrzení transakce
                return intPID;
            }

            

        }

        public bool ValidateBeforeSave(BO.a08Theme rec, List<BO.a12ThemeForm> lisA12, List<BO.a14AttachmentToTheme> lisA14)
        {
            if (lisA12 != null)
            {
                if (lisA12.Select(p => p.f06ID).Distinct().Count()<lisA12.Count())
                {
                    this.AddMessage("Formulář ve více řádcích."); return false;
                }
            }
            if (lisA14 != null)
            {
                if (lisA14.Exists(p => p.o13ID == 0))
                {
                    this.AddMessage("V seznamu povolených příloh je nevyplněný typ přílohy."); return false;
                }
                if (lisA14.Select(p => p.o13ID).Distinct().Count() < lisA14.Count())
                {
                    this.AddMessage("V seznamu povolených příloh je duplictní typ přílohy."); return false;
                }
            }
            if (string.IsNullOrEmpty(rec.a08Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }

            return true;
        }

    }
}
