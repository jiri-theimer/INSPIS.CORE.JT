﻿using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ia11EventFormBL
    {
        public BO.a11EventForm Load(int pid);
        public IEnumerable<BO.a11EventForm> GetList(BO.myQuery mq);
        public int Save(BO.a11EventForm rec);
        public void LockUnLockPolls(int a01id, bool bolLock);
        public void LockUnLockForm(int a11id, bool bolLock);
        public void ClearF32(int a11id);
        public void AppendAccessToLog(List<BO.f32FilledValue> lisF32, string strURL);
        public IEnumerable<BO.j95FormAccessLog> GetList_J95(int a11id, int intTopRecs);
        public bool ValidateBeforeSave(BO.a11EventForm c);
    }
    class a11EventFormBL : BaseBL, Ia11EventFormBL
    {

        public a11EventFormBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a11", false, false,true));
            sb(",a11_f06.f06Name,a11_f06.f06IsA01ClosedStrict,a11_f06.f06ValidFrom,a11_f06.f06ValidUntil");
            sb(",a11_a01.a01ValidUntil,a11_a01.a01ValidFrom,a11_a01.a01IsClosed,a11_a01.a01IsAllFormsClosed");
            sb(",a37.a37IZO,a37.a37Name,a37.a37IZO,a25.a25Name,a25.a25Color");
            sb(" FROM a11EventForm a INNER JOIN f06Form a11_f06 ON a.f06ID=a11_f06.f06ID INNER JOIN a01Event a11_a01 ON a.a01ID=a11_a01.a01ID");
            sb(" LEFT OUTER JOIN a25EventFormGroup a25 ON a.a25ID=a25.a25ID");
            sb(" LEFT OUTER JOIN a37InstitutionDepartment a37 ON a.a37ID=a37.a37ID");
            sb(" LEFT OUTER JOIN k01Teacher k01 ON a.k01ID=k01.k01ID");
            
            sb(strAppend);
            return sbret();
        }
        public BO.a11EventForm Load(int pid)
        {
            return _db.Load<BO.a11EventForm>(GetSQL1(" WHERE a.a11ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a11EventForm> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a11EventForm>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.a11EventForm rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a11ID);            
            p.AddInt("a01ID", rec.a01ID, true);
            p.AddInt("f06ID", rec.f06ID, true);
            p.AddInt("a37ID", rec.a37ID, true);
            p.AddInt("a25ID", rec.a25ID, true);
            p.AddInt("a22ID", rec.a22ID, true);
            p.AddInt("k01ID", rec.k01ID, true);
            p.AddString("a11AccessToken", rec.a11AccessToken);                        
            p.AddBool("a11IsPoll", rec.a11IsPoll);
            p.AddBool("a11IsSimulation", rec.a11IsSimulation);
            p.AddString("a11IzoOffice", rec.a11IzoOffice);
            p.AddString("a11Description", rec.a11Description);
            p.AddInt("a11Ordinal", rec.a11Ordinal);
            p.AddDateTime("a11LockDate", rec.a11LockDate);
            p.AddBool("a11IsLocked", rec.a11IsLocked);
            p.AddBool("a11IsLockedByWorkflow", rec.a11IsLockedByWorkflow);
            p.AddString("a11TeacherPID", rec.a11TeacherPID);

            return _db.SaveRecord("a11EventForm", p.getDynamicDapperPars(), rec, false);
        }

        public bool ValidateBeforeSave(BO.a11EventForm c)
        {
            var recF06 = _mother.f06FormBL.Load(c.f06ID);
            if (recF06.f06IsA37Required && c.a37ID == 0)
            {
                this.AddMessage(string.Format("Formulář [{0}] vyžaduje povinně vazbu na IZO školy.", recF06.f06Name)); return false;
            }

            return true;
        }

        public void LockUnLockPolls(int a01id,bool bolLock)
        {
            _db.RunSql("UPDATE a11EventForm SET a11IsLocked=@islocked,a11LockDate=CASE WHEN @islocked=1 THEN GETDATE() ELSE NULL END WHERE a01ID=@a01id AND a11IsPoll=1", new {islock=bolLock,a01id=a01id});
        }
        public void LockUnLockForm(int a11id, bool bolLock)
        {
            _db.RunSql("UPDATE a11EventForm SET a11IsLocked=@islocked,a11LockDate=CASE WHEN @islocked=1 THEN GETDATE() ELSE NULL END WHERE a11ID=@pid", new { islock = bolLock, pid = a11id });
        }

        public void ClearF32(int a11id)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid);
            pars.Add("pid", a11id, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("a11eventform_clearf32", ref pars);
        }
        public void AppendAccessToLog(List<BO.f32FilledValue> lisF32,string strURL)
        {
            sbinit();           
            foreach (var c in lisF32)
            {                
                sb("INSERT INTO j95FormAccessLog(f32ID,a11ID,f19ID,j03ID,j95Date,j95URL) VALUES(");
                sb(BO.BAS.IIFS(c.pid == 0, "NULL", c.pid.ToString()));
                sb(","+c.a11ID.ToString()+","+c.f19ID.ToString()+","+_mother.CurrentUser.pid.ToString()+",GETDATE()");
                sb(","+BO.BAS.GS(strURL));
                sb("); ");               
            }
            _db.RunSql(sbret());
        }
        public IEnumerable<BO.j95FormAccessLog> GetList_J95(int a11id,int intTopRecs)
        {
            string s = string.Format("SELECT TOP {0} a.*,j03.j03Login,f19.f19Name FROM j95FormAccessLog a INNER JOIN j03User j03 ON a.j03ID=j03.j03ID LEFT OUTER JOIN f19Question f19 ON a.f19ID=f19.f19ID LEFT OUTER JOIN f32FilledValue f32 ON a.f32ID=f32.f32ID WHERE a.a11ID=@pid ORDER BY a.j95ID DESC", intTopRecs);
            return _db.GetList<BO.j95FormAccessLog>(s, new { pid = a11id });
        }
    }
}
