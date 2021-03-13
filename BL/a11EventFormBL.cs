using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ia11EventFormBL:BaseInterface
    {
        public BO.a11EventForm Load(int pid);
        public BO.a11EventForm LoadPoll(string signature, string pin, int a11id_exclude = 0);
        public BO.a11EventForm LoadPoll(int a01id, string pin, int a11id_exclude = 0);
        public IEnumerable<BO.a11EventForm> GetList(BO.myQueryA11 mq);
        public int Save(BO.a11EventForm rec);
        public void LockUnLockPolls(int a01id, bool bolLock);
        public void LockUnLockForm(int a11id, bool bolLock);
        public void ClearF32(int a11id);
        public void AppendAccessToLog(List<BO.f32FilledValue> lisF32, string strURL);
        public IEnumerable<BO.j95FormAccessLog> GetList_J95(int a11id, int intTopRecs);
        public bool ValidateBeforeSave(BO.a11EventForm c);
        public bool TestUserAccessToEncryptedFormValues(int a11id, int f06id);
        public string GetRandomToken();
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
            sb(",a11_f06.f06Name,a11_f06.f06IsA01ClosedStrict,a11_f06.f06ValidFrom,a11_f06.f06ValidUntil,a11_f06.f06IsA01PeriodStrict,a11_f06.f06UserLockFlag");
            sb(",a11_a01.a01ValidUntil,a11_a01.a01ValidFrom,a11_a01.a01IsClosed,a11_a01.a01IsAllFormsClosed");
            sb(",a11_a01.a03ID,a37.a37IZO,a37.a37Name,a37.a37IZO,a25.a25Name,a25.a25Color,a37.a17ID,a11_a01.a08ID,a11_a01.b02ID,a11_a01.a10ID");
            sb(",k01.k01LastName+' '+k01.k01FirstName as k01FullName_Desc");
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
        public BO.a11EventForm LoadPoll(string signature,string pin,int a11id_exclude = 0)
        {
            if (a11id_exclude > 0)
            {
                return _db.Load<BO.a11EventForm>(GetSQL1(" WHERE a.a11IsPoll=1 AND a.a11AccessToken=@pin AND a11_a01.a01Signature=@signature AND a.a11ID<>@a11id_exclude"), new { signature = signature, pin = pin, a11id_exclude = a11id_exclude });                
            }
            else
            {
                return _db.Load<BO.a11EventForm>(GetSQL1(" WHERE a.a11IsPoll=1 AND a.a11AccessToken=@pin AND a11_a01.a01Signature=@signature"), new { signature = signature, pin = pin });
            }
            
        }
        public BO.a11EventForm LoadPoll(int a01id, string pin, int a11id_exclude = 0)
        {
            if (a11id_exclude > 0)
            {
                return _db.Load<BO.a11EventForm>(GetSQL1(" WHERE a.a11IsPoll=1 AND a.a11AccessToken=@pin AND a.a01ID=@a01id AND a.a11ID<>@a11id_exclude"), new { a01id = a01id, pin = pin, a11id_exclude = a11id_exclude });
            }
            else
            {
                return _db.Load<BO.a11EventForm>(GetSQL1(" WHERE a.a11IsPoll=1 AND a.a11AccessToken=@pin AND a.a01ID=@a01id"), new { a01id = a01id, pin = pin });
            }
                
        }

        public IEnumerable<BO.a11EventForm> GetList(BO.myQueryA11 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a11EventForm>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.a11EventForm rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
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

            return _db.SaveRecord("a11EventForm", p, rec, false);
        }

        public bool ValidateBeforeSave(BO.a11EventForm c)
        {
            //if (c.a01ID == 0)
            //{
            //    this.AddMessageTranslated("a01id missing."); return false;
            //}
            if (c.a11IsPoll)
            {
                if (string.IsNullOrEmpty(c.a11AccessToken.Trim())){
                    this.AddMessage("Musíte definovat anketní PIN."); return false;
                }
                if (GetList(new BO.myQueryA11() { a01id = c.a01ID }).Where(p => p.a11IsPoll == true && p.pid != c.pid).Any(p => p.a11AccessToken == c.a11AccessToken)){
                    this.AddMessage("Anketní PIN v rámci akce nesmí být duplicitní."); return false;
                }
                
            }
            if (c.f06ID == 0)
            {
                this.AddMessage("Na vstupu chybí formulář.");return false;
            }
            var recF06 = _mother.f06FormBL.Load(c.f06ID);
            if (recF06.f06IsA37Required && c.a37ID == 0)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Formulář [{0}] vyžaduje povinně vazbu na IZO školy."), recF06.f06Name)); return false;
            }

            return true;
        }

        public void LockUnLockPolls(int a01id,bool bolLock)
        {
            _db.RunSql("UPDATE a11EventForm SET a11IsLocked=@islock,a11LockDate=CASE WHEN @islock=1 THEN GETDATE() ELSE NULL END WHERE a01ID=@a01id AND a11IsPoll=1", new {islock=bolLock,a01id=a01id});
        }
        public void LockUnLockForm(int a11id, bool bolLock)
        {
            _db.RunSql("UPDATE a11EventForm SET a11IsLocked=@islock,a11LockDate=CASE WHEN @islock=1 THEN GETDATE() ELSE NULL END WHERE a11ID=@pid", new { islock = bolLock, pid = a11id });
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

        public bool TestUserAccessToEncryptedFormValues(int a11id,int f06id)
        {
            if (_mother.CurrentUser.TestPermission(BO.j05PermValuEnum.Read_Encrypted_FormValues))
            {
                return true;    //přístup k šifrovaným datům vyplývá z globální role
            }
            var lis = _mother.f06FormBL.GetListF07(f06id);
            var recJ04 = _mother.j04UserRoleBL.Load(_mother.CurrentUser.j04ID);
            switch (recJ04.j04RelationFlag)
            {
                case BO.j04RelationFlagEnum.NoRelation:     //přístup ke všem datům v systému
                    if (lis.Where(p => p.j04ID == recJ04.pid).Count() > 0)
                    {
                        return true;
                    }
                    break;
                case BO.j04RelationFlagEnum.A03:
                    var recA11 = Load(a11id);
                    var mq = new BO.myQuery("a39");
                    mq.j02id = _mother.CurrentUser.j02ID;
                    mq.a03id = recA11.a03ID;
                    var lisA39 = _mother.a39InstitutionPersonBL.GetList(mq);
                    foreach(var c in lisA39)
                    {
                        if (lis.Where(p => p.j04ID == c.j04ID_Explicit).Count() > 0)
                        {
                            return true;    //školní role má oprávnění ke čtení šifrovaných odpovědí
                        }
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }

        public string GetRandomToken()
        {
            var c = new Random();
            return BO.BAS.RightString("0000" + c.Next(1, 9999).ToString(), 4);
        }

      
    }
}
