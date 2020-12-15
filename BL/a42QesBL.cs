using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ia42QesBL
    {
        public BO.a42Qes Load(int pid);
        public BO.a42Qes LoadByName(string strA42Name, int intExcludePID);
        public BO.a42Qes LoadByGuid(string strGUID, int intExcludePID);
        public IEnumerable<BO.a42Qes> GetList(BO.myQueryA42 mq);
        public int Save(BO.a42Qes rec);
        public int PrepareTempData(BO.a42Qes rec, BO.a01Event recA01Template, List<BO.a12ThemeForm> lisA12, List<int> a03ids, BO.x40MailQueue recX40, List<BO.a12ThemeForm> lisA12Poll);
        public void UpdateJobState(int a42id, BO.a42JobState stateflag);
        public bool DeleteWithA01(int pid);
        public int ChangePeriod(BO.a42Qes rec);
    }
    class a42QesBL : BaseBL, Ia42QesBL
    {
        
        public a42QesBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,a08.a08Name,j40.j40SmtpEmail,");
            sb(_db.GetSQL1_Ocas("a42"));
            sb(" FROM a42Qes a INNER JOIN a08Theme a08 ON a.a08ID=a08.a08ID LEFT OUTER JOIN j40MailAccount j40 ON a.j40ID=j40.j40ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a42Qes Load(int pid)
        {
            return _db.Load<BO.a42Qes>(GetSQL1(" WHERE a.a42ID=@pid"), new { pid = pid });
        }
        public BO.a42Qes LoadByName(string strA42Name, int intExcludePID)
        {
            return _db.Load<BO.a42Qes>(GetSQL1(" WHERE a.a42Name LIKE @name AND a.a42ID<>@excludepid"), new { name = strA42Name, excludepid = intExcludePID });
        }
        public BO.a42Qes LoadByGuid(string strGUID, int intExcludePID)
        {
            return _db.Load<BO.a42Qes>(GetSQL1(" WHERE a.a42JobGuid LIKE @guid AND a.a42ID<>@excludepid"), new { guid = strGUID, excludepid = intExcludePID });
        }

        public IEnumerable<BO.a42Qes> GetList(BO.myQueryA42 mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a42ID DESC"; };
            DL.FinalSqlCommand fq = DL.basQuerySupport.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a42Qes>(fq.FinalSql, fq.Parameters);
        }

        public void UpdateJobState(int a42id,BO.a42JobState stateflag)
        {
            _db.RunSql("UPDATE a42Qes set a42JobState=@x WHERE a42ID=@pid", new { x =(int) stateflag, pid = a42id });
        }
        public bool DeleteWithA01(int pid)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
           
            if (_db.RunSp("a42qes_delete_with_events", ref pars, 240) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }

           
        }
        public int ChangePeriod(BO.a42Qes rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            _db.RunSql("UPDATE a01Event SET a01DateFrom=@d1,a01DateUntil=@d2 WHERE a42ID=@pid", new { d1 = rec.a42DateFrom, d2 = rec.a42DateUntil, pid = rec.pid });
            return Save(rec);
        }

        public int Save(BO.a42Qes rec)
        {

            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a42ID);
            p.AddInt("j40ID", rec.j40ID,true);
            p.AddInt("a08ID", rec.a08ID,true);
            p.AddInt("a42TestFlag", rec.a42TestFlag);
            p.AddString("a42Name", rec.a42Name);
            p.AddDateTime("a42DateFrom", rec.a42DateFrom);
            p.AddDateTime("a42DateUntil", rec.a42DateUntil);
            p.AddString("a42Description", rec.a42Description);
            p.AddInt("a42TempRowsA01", rec.a42TempRowsA01);
            p.AddInt("a42TempRowsX40", rec.a42TempRowsX40);
            p.AddString("a42JobGuid", rec.a42JobGuid);
            p.AddString("a42UploadGuid", rec.a42UploadGuid);
            int intPID = _db.SaveRecord("a42Qes", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a42Qes rec)
        {
            if (string.IsNullOrEmpty(rec.a42Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.a08ID==0)
            {
                this.AddMessage("Chybí vyplnit [Téma akce]."); return false;
            }
            if (rec.a42DateFrom==null || rec.a42DateUntil==null)
            {
                this.AddMessage("Chybí vyplnit období od-do."); return false;
            }
            if (LoadByName(rec.a42Name,rec.pid) != null)
            {
                this.AddMessage("INEZ s tímto názvem již existuje."); return false;
            }


            return true;
        }

        public int PrepareTempData(BO.a42Qes rec,BO.a01Event recA01Template,List<BO.a12ThemeForm> lisA12, List<int> a03ids,BO.x40MailQueue recX40, List<BO.a12ThemeForm> lisA12Poll)
        {   //vrací a42ID založené hlavičky INEZU
            string strJobGuid = rec.a42JobGuid;
            string strUploadGUID = rec.a42UploadGuid;

            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            if (a03ids.Count == 0)
            {
                this.AddMessage("Chybí výběr cílových institucí.");return 0;
            }
            if (string.IsNullOrEmpty(recX40.x40Subject) == true || string.IsNullOrEmpty(recX40.x40Body) == true)
            {
                this.AddMessage("Předmět a obsah e-mail zprávy je povinný."); return 0;
            }
            if (recA01Template.a08ID==0 || recA01Template.a10ID == 0)
            {
                this.AddMessage("Chybí téma akce."); return 0;
            }
            foreach(var recA12 in lisA12)
            {
                if (_mother.f06FormBL.Load(recA12.f06ID).f06IsA37Required)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Formulář {0} vyžaduje povinně vazbu na IZO školy."),recA12.f06Name)); return 0;
                }
            }
            
            
            var mq = new BO.myQuery("a39");
            mq.j04id = _mother.GlobalParams.LoadParamInt("j04ID_SchoolDirector",35);
            var lisA39 = _mother.a39InstitutionPersonBL.GetList(mq).Where(p => p.j02Email != null);
            var lisX43 = new List<BO.x43MailQueue_Recipient>();
            var lisO27 = _mother.o27AttachmentBL.GetTempFiles(strUploadGUID);

            foreach (int a03id in a03ids)
            {
                var recA03 = _mother.a03InstitutionBL.Load(a03id);
                if (recA03.isclosed)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Záznam instituce '{0}' není časově platný."),recA03.NamePlusRedizo)); return 0;
                }
                if (recX40.x40RecipientFlag==BO.RecipientFlagEnum.SchoolAdress || recX40.x40RecipientFlag == BO.RecipientFlagEnum.SchoolPlusDirector)
                {
                    if (string.IsNullOrEmpty(recA03.a03Email.Trim()) == false && lisX43.Where(p => p.x43Email.ToLower() == recA03.a03Email.Trim().ToLower()).Count() == 0)
                    {
                        lisX43.Add(new BO.x43MailQueue_Recipient() { x29ID = 103, x43DataPID = recA03.pid, x43Email = recA03.a03Email.Trim(), x43DisplayName=recA03.a03Name, TempA03ID=a03id });
                    }                    
                }
                if (recX40.x40RecipientFlag == BO.RecipientFlagEnum.SchoolPlusDirector || recX40.x40RecipientFlag == BO.RecipientFlagEnum.DirectorAddress)
                {
                    foreach(var recA39 in lisA39.Where(p => p.a03ID == a03id))
                    {
                        if (lisX43.Where(p => p.x43Email.ToLower() == recA39.j02Email.Trim().ToLower()).Count() == 0)
                        {
                            lisX43.Add(new BO.x43MailQueue_Recipient() { x29ID = 502, x43DataPID = recA39.j02ID, x43Email = recA39.j02Email.Trim(),x43DisplayName=recA39.Person, TempA03ID=a03id });
                        }
                    }
                }

                
            }            
                        
            var recTemp = new BO.p85Tempbox() {p85Prefix="a01", p85GUID = strJobGuid, p85OtherKey1 = recA01Template.a10ID };
            _mother.p85TempboxBL.Save(recTemp);
            foreach(var a03id in a03ids)
            {
                var recA03 = _mother.a03InstitutionBL.Load(a03id);
                recTemp = new BO.p85Tempbox() {p85Prefix="a03", p85GUID = strJobGuid, p85OtherKey1 = a03id,p85OtherKey2=recA01Template.a10ID, p85FreeText01=BO.BAS.OM2(recA03.NamePlusRedizo,90) };
                _mother.p85TempboxBL.Save(recTemp);
            }
            foreach (var recA12 in lisA12)
            {
                recTemp = new BO.p85Tempbox() { p85Prefix = "a12", p85GUID = strJobGuid, p85OtherKey1 = recA12.a12ID, p85FreeText01 = recA12.f06Name,p85OtherKey2=recA12.f06ID };
                _mother.p85TempboxBL.Save(recTemp);
            }
            if (lisA12Poll != null)
            {
                foreach (var recA12 in lisA12Poll.Where(p=>p.TempCount>0))  //p85FreeBoolean01=true: anketní formulář, p85FreeNumber01: počet anketních formulářů v akci
                {
                    recTemp = new BO.p85Tempbox() { p85Prefix = "a12",p85FreeBoolean01=true, p85GUID = strJobGuid, p85OtherKey1 = recA12.a12ID,p85OtherKey2=recA12.f06ID, p85FreeText01 = recA12.f06Name,p85FreeNumber01=recA12.TempCount };
                    _mother.p85TempboxBL.Save(recTemp);
                }
            }
            _mother.MailBL.SaveMailJob2Temp(strJobGuid, recX40, strUploadGUID, lisX43);

            rec.a42TempRowsA01 = _mother.p85TempboxBL.GetList(strJobGuid, false, "a03").Count();
            rec.a42TempRowsX40= _mother.p85TempboxBL.GetList(strJobGuid, false, "x40").Count();
            int intA42ID = Save(rec);
            
            return intA42ID;

        }

    }
}
