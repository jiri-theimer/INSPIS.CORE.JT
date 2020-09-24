using BO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Ia01EventBL:BaseInterface
    {
        public BO.a01Event Load(int pid);
        public IEnumerable<BO.a01Event> GetList(BO.myQuery mq);
        public int Create(BO.a01Event rec, bool bolAutoInitWorkflow, List<BO.a11EventForm> lisA11, List<BO.a41PersonToEvent> lisA41, List<BO.a35PersonEventPlan> lisA35, List<int> a37ids);
        public int SaveA01Record(BO.a01Event rec, BO.a10EventType recA10);
        public BO.a01EventPermission InhalePermission(BO.a01Event rec);
        public BO.a01RecordSummary LoadSummary(int pid);
        public IEnumerable<BO.a24EventRelation> GetList_a24(int pid);
        public int SaveA24Record(BO.a24EventRelation rec);
        public string GetPageUrl(BO.a01Event rec, int pid4url=0);
    }
    class a01EventBL : BaseBL, Ia01EventBL
    {

        public a01EventBL(BL.Factory mother) : base(mother)
        {

        }

        public string GetPageUrl(BO.a01Event rec,int pid4url)
        {
            string s = "/a01/RecPage";
            if (rec.a10ViewUrl_Page != null)
            {
                s = rec.a10ViewUrl_Page;
            }
            if (pid4url == 0) pid4url = rec.pid;

            if (s.Contains("?"))
            {
                s += "&pid=" + pid4url.ToString();
            }
            else
            {
                s += "?pid=" + pid4url.ToString();
            }
            return s;

        }
        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a01", false, true));
            sb(",a01_a10.a10Name,a01_a10.b01ID,a01_a08.a08Name,a01_a03.a03Name,a01_a03.a03REDIZO,a01_a03.a05ID,a01_a06.a06Name");
            sb(",a01_a05.a05Name,a01_a09.a09Name,a01_b02.b02Name,a01_b02.b02Color,a01_b02.b02Ident,a01_a10.a10ViewUrl_Page,a01_a10.a10CoreFlag");

            sb(" FROM a01Event a INNER JOIN a10EventType a01_a10 ON a.a10ID=a01_a10.a10ID");
            sb(" LEFT OUTER JOIN a03Institution a01_a03 ON a.a03ID=a01_a03.a03ID LEFT OUTER JOIN a05Region a01_a05 ON a01_a03.a05ID=a01_a05.a05ID LEFT OUTER JOIN a09FounderType a01_a09 ON a01_a03.a09ID=a01_a09.a09ID");
            sb(" LEFT OUTER JOIN a06InstitutionType a01_a06 ON a01_a03.a06ID=a01_a06.a06ID LEFT OUTER JOIN a08Theme a01_a08 ON a.a08ID=a01_a08.a08ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus a01_b02 ON a.b02ID=a01_b02.b02ID");

            sb(strAppend);
            return sbret();
        }
        public BO.a01Event Load(int pid)
        {
            return _db.Load<BO.a01Event>(GetSQL1(" WHERE a.a01ID=@pid"), new { pid = pid });
        }
        public BO.a01RecordSummary LoadSummary(int pid)
        {
            return _db.Load<BO.a01RecordSummary>("EXEC dbo._core_a01_summary @j03id,@pid,null", new { j03id=_mother.CurrentUser.pid,pid = pid });
        }

        public IEnumerable<BO.a01Event> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a01Event>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a24EventRelation> GetList_a24(int pid)
        {
            sb("SELECT a.*,");
            sb("a01left.a01Signature as a01Signature_Left,a01right.a01Signature as a01Signature_Right,a46.a46Name,a10left.a10Name as a10Name_Left,a10right.a10Name as a10Name_Right,a46.a46DirectionFlag,");
            sb(_db.GetSQL1_Ocas("a24", true, true));
            sb(" FROM a24EventRelation a INNER JOIN a01Event a01left ON a.a01ID_Left=a01left.a01ID INNER JOIN a10EventType a10left ON a01left.a10ID=a10left.a10ID");
            sb(" INNER JOIN a01Event a01right ON a.a01ID_Right=a01right.a01ID INNER JOIN a10EventType a10right ON a01right.a10ID=a10right.a10ID");
            sb(" INNER JOIN a46EventParentType a46 ON a.a46ID=a46.a46ID");
            sb(" WHERE a.a01ID_Left=@pid OR a.a01ID_Right=@pid");
            return _db.GetList<BO.a24EventRelation>(sbret(), new { pid = pid });
        }
        public int SaveA24Record(BO.a24EventRelation rec)
        {            
            if (rec.a01ID_Left==0 || rec.a01ID_Right == 0)
            {
                this.AddMessage("Chybí vazba na vyhledanou akci.");return 0;
            }
            if (rec.a46ID == 0)
            {
                this.AddMessage("Chybí druh vazby na akci."); return 0;
            }
            if (rec.a01ID_Left == rec.a01ID_Right)
            {
                this.AddMessage("Akce nemůže být svázána sama na sebe."); return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a24ID);
            p.AddInt("a46ID", rec.a46ID, true);
            p.AddInt("a01ID_Left", rec.a01ID_Left, true);
            p.AddInt("a01ID_Right", rec.a01ID_Right, true);
            p.AddString("a24Description", rec.a24Description);          
            
            int intPID = _db.SaveRecord("a24EventRelation", p.getDynamicDapperPars(), rec);
            return intPID;
        }

        public int Create(BO.a01Event rec, bool bolAutoInitWorkflow, List<BO.a11EventForm> lisA11, List<BO.a41PersonToEvent> lisA41, List<BO.a35PersonEventPlan> lisA35, List<int> a37ids)
        {
            var recA10 = _mother.a10EventTypeBL.Load(rec.a10ID);
            if (ValidateBeforeSave(ref rec, recA10) == false)
            {
                return 0;
            }
            if (rec.a01IsTemporary==false && lisA11 !=null && recA10.a10IsUse_A08 && ValidateA11(rec,lisA11)==false)
            {
                return 0;
            }
            if (rec.a01IsTemporary == false && lisA41 != null && recA10.a10IsUse_A41 && ValidateA41(rec, lisA41) == false)
            {
                return 0;
            }
            if (lisA41 != null && recA10.a45ID_Creator > 0)   //autor akce má automaticky nějakou úvodní roli
            {
                var recA41 = new BO.a41PersonToEvent() { a45ID =(BO.EventRoleENUM) recA10.a45ID_Creator, a41IsAllocateAllPeriod=true,j02ID=rec.j02ID_Issuer };
                if (recA41.j02ID == 0) recA41.j02ID = _mother.CurrentUser.j02ID;
            }

            int intPID = SaveA01Record(rec, recA10);
            if (intPID == 0)
            {
                this.AddMessage("Záznam akce se nepodařilo založit."); return 0;
            }
            if (lisA11 != null)
            {
                AppendA11(intPID, lisA11);
            }
            if (lisA41 != null)
            {
                AppendA41(intPID, lisA41);
            }            
            if (lisA35 != null)
            {
                AppendA35(intPID, lisA35);
            }
            if (a37ids != null)
            {   //v praxi a44InstitutionDepartmentToEvent zaniklo a nemá uplatnění
                _db.RunSql("INSERT INTO a44InstitutionDepartmentToEvent(a01ID,a37ID) SELECT @pid,a37ID FROM a37InstitutionDepartment WHERE a37ID IN (" + string.Join(",", a37ids) + ")", new { pid = intPID });
            }

            if (bolAutoInitWorkflow)
            {
                _mother.WorkflowBL.CheckDefaultWorkflowStatus(intPID);
            }
            

            RunSpAfterA01Save(intPID);

            return intPID;
        }
        public int SaveA01Record(BO.a01Event rec, BO.a10EventType recA10)
        {
            if (ValidateBeforeSave(ref rec, recA10) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a01ID);
            p.AddInt("a10ID", rec.a10ID, true);
            p.AddInt("a03ID", rec.a03ID, true);
            p.AddInt("a08ID", rec.a08ID, true);
            p.AddInt("a42ID", rec.a42ID, true);
            p.AddInt("a61ID", rec.a61ID, true);
            p.AddInt("a22ID", rec.a22ID, true);
            p.AddInt("a70ID", rec.a70ID, true);
            p.AddInt("a01ParentID", rec.a01ParentID, true);
            if (rec.j03ID_Creator == 0) { rec.j03ID_Creator = _mother.CurrentUser.pid; };
            p.AddInt("j03ID_Creator", rec.j03ID_Creator, true);
            if (rec.j02ID_Issuer == 0) { rec.j02ID_Issuer = _mother.CurrentUser.j02ID; };
            p.AddInt("j02ID_Issuer", rec.j02ID_Issuer, true);

            p.AddString("a01InstitutionPlainText", rec.a01InstitutionPlainText);
            p.AddString("a01CaseCode", rec.a01CaseCode);
            p.AddString("a01CaseCodePID", rec.a01CaseCodePID);
            p.AddString("a01Name", rec.a01Name);
            p.AddString("a01Description", rec.a01Description);

            p.AddBool("a01IsTemporary", rec.a01IsTemporary);
            p.AddDateTime("a01DateFrom", rec.a01DateFrom);
            if (rec.a01DateUntil == null)
            {
                rec.a01DateUntil = new DateTime(3000, 1, 1);
            }
            p.AddDateTime("a01DateUntil", rec.a01DateUntil);

            int intPID = _db.SaveRecord("a01Event", p.getDynamicDapperPars(), rec);
            if (intPID > 0 && rec.pid>0)
            {
                RunSpAfterA01Save(intPID);  //automaticky pouštět pouze u editace již existujícího a01 záznamu. Pro založení události se volá později                
            }

            return intPID;
        }

        private void RunSpAfterA01Save(int intA01ID)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid);
            pars.Add("pid", intA01ID, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("_core_a01_after_save", ref pars);
        }

        private bool ValidateA41(BO.a01Event rec, List<BO.a41PersonToEvent> lisA41)
        {
            if (lisA41.Where(p => p.IsTempDeleted==false && p.a45ID == BO.EventRoleENUM.Vedouci).Count() == 0)
            {
                this.AddMessage("V obsazení akce musí být minimálně jeden vedoucí."); return false;
            }
            if (lisA41.Where(p => p.IsTempDeleted == false && p.j02ID==0 && p.j11ID==0).Count() > 0)
            {
                this.AddMessage("V obsazení akce je řádek bez člena."); return false;
            }
            if (lisA41.Where(p => p.IsTempDeleted == false && p.a45ID == BO.EventRoleENUM.PrizvanaOsoba && p.j02IsInvitedPerson==false).Count() > 0)
            {
                int j02id = lisA41.Where(p => p.IsTempDeleted == false && p.a45ID == BO.EventRoleENUM.PrizvanaOsoba && p.j02IsInvitedPerson == false).First().j02ID;
                this.AddMessageTranslated(string.Format(_mother.tra("V obsazení akce je osoba [{0}] zařazena jako 'Přizvaná'. Ale v nastavení osobního profilu není uvedeno, že může být přizvanou osobou."), _mother.j02PersonBL.Load(j02id).FullNameAsc)); return false;
            }
            return true;
        }
        private void AppendA35(int intA01ID, List<BO.a35PersonEventPlan> lisA35)
        {
            foreach (var c in lisA35)
            {
                c.a01ID = intA01ID;
                _mother.a35PersonEventPlanBL.Save(c);
            }
        }
        private void AppendA41(int intA01ID, List<BO.a41PersonToEvent> lisA41)
        {
            foreach (var c in lisA41)
            {
                c.a01ID = intA01ID;
                _mother.a41PersonToEventBL.Save(c,true);
            }
        }
        
        private void AppendA11(int intA01ID, List<BO.a11EventForm> lisA11)
        {
            foreach (var c in lisA11)
            {
                c.a01ID = intA01ID;
                _mother.a11EventFormBL.Save(c);
            }
        }

        private bool ValidateA11(BO.a01Event rec, List<BO.a11EventForm> lisA11)
        {
            foreach(var c in lisA11)
            {
                if (_mother.a11EventFormBL.ValidateBeforeSave(c) == false)
                {
                    return false;
                }               
            }
            var lisA12 = _mother.a08ThemeBL.GetListA12(rec.a08ID).Where(p => p.a12IsRequired == true);
            if (lisA12.Count() == 0)
            {
                return true;    //téma nemá povinné formuláře k zařazení do akce
            }
            foreach(var recA12 in lisA12)
            {
                if (lisA11.Where(p => p.f06ID == recA12.f06ID).Count() == 0)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Formulář [{0}] je povinný k zařazení do akce."), recA12.f06Name)); return false;
                }
            }           

            return true;
        }
        private bool ValidateBeforeSave(ref BO.a01Event c, BO.a10EventType recA10)
        {
            if (recA10 == null || c.a10ID == 0)
            {
                this.AddMessage("Chybí vyplnit typ akce."); return false;
            }
            if (recA10.a10IsUse_A08 && c.a08ID == 0)
            {
                this.AddMessage("Chybí vyplnit téma akce."); return false;
            }
            if (recA10.a10IsUse_A03 && c.a03ID == 0 && string.IsNullOrEmpty(c.a01InstitutionPlainText) == true)
            {
                this.AddMessage("Chybí vyplnit vazba na instituci."); return false;
            }
            if (c.j02ID_Issuer == 0)
            {
                this.AddMessage("Chybí vyplnit zadavatele akce."); return false;
            }
            if (recA10.a10IsUse_Period)
            {
                if (c.a01DateFrom == null)
                {
                    this.AddMessage("Chybí vyplnit [Datum od]."); return false;
                }
                if (c.a01DateUntil == null)
                {
                    this.AddMessage("Chybí vyplnit [Datum do]."); return false;
                }
                if (c.a01DateFrom > c.a01DateUntil)
                {
                    this.AddMessage("[Datum do] musí být větší než [Datum od]."); return false;
                }
            }
            else
            {
                if (c.a01DateFrom == null)
                {
                    c.a01DateFrom = DateTime.Today;
                }
                if (c.a01DateUntil == null)
                {
                    c.a01DateUntil = new DateTime(3000, 1, 1);
                }
            }

            if (c.a01ParentID > 0)
            {
                if (c.a01ChildsCount > 0)
                {
                    this.AddMessage("Na tuto akci se již odkazuje minimálně jedna podřízená akce."); return false;
                }
                var mother = Load(c.a01ParentID);
                if (mother.a01ParentID > 0)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Akce [{0}] nemůže být nadřízená, protože už sama je podřízená jiné akci"), mother.a01Signature)); return false;
                }
            }

            if (c.pid == 0 && recA10.a10OneSchoolInstanceLimit > 0 && c.a03ID > 0)  //typ akce má limit maximálního počtu instancí akcí u jedné instituce
            {
                var mq = new BO.myQuery("a01");
                mq.a03id = c.a03ID;mq.a10id = c.a10ID;
                var lisExist = GetList(mq);
                if (lisExist.Count() >= recA10.a10OneSchoolInstanceLimit)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("U instituce [REDIZO={0}] již existuje akce tohoto typu. Maximální počet akcí pro typ [{1}] je {2}."), lisExist.First().a03REDIZO,recA10.a10Name,recA10.a10OneSchoolInstanceLimit)); return false;
                }
            }

            return true;
        }

        public BO.a01EventPermission InhalePermission(BO.a01Event rec)
        {
            BO.a01EventPermission ret = new BO.a01EventPermission() { PermValue = BO.a01EventPermissionENUM.NoAccess };

            if (_mother.CurrentUser.TestPermission(BO.j05PermValuEnum.AdminGlobal))
            {
                ret.PermValue = BO.a01EventPermissionENUM.FullAccess;    //administrátor - automaticky nejvyšší práva k akci
                ret.IsRecordOwner = true;
            }
            else
            {
                int intPerm = _db.Load<BO.GetInteger>("select dbo._core_a01_permission(@a01id,@j03id_sys) as Value", new { a01id = rec.pid, j03id_sys = _mother.CurrentUser.pid }).Value;
                if (intPerm > 0)
                {
                    ret.PermValue = (BO.a01EventPermissionENUM)intPerm;
                    ret.IsRecordOwner = _db.Load<BO.GetBool>("select dbo._core_a01_is_user_owner(@a01id,@j03id_sys) as Value", new { a01id = rec.pid, j03id_sys = _mother.CurrentUser.pid }).Value;
                }
                else
                {
                    if (_mother.CurrentUser.TestPermission(BO.j05PermValuEnum.FormReadonlyPreview))
                    {
                        ret.PermValue = BO.a01EventPermissionENUM.ReadOnlyAccess;    //oprávnění číst všechny formulář - readonly náhled
                    }
                }

            }

            return ret;
        }

        public bool IsUserOwner(BO.a01Event rec)
        {

            return false;
        }

        
        
        
    }
}
