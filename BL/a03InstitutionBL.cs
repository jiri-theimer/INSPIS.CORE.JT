using System.Collections.Generic;

namespace BL
{
    public interface Ia03InstitutionBL: BaseInterface
    {
        public BO.a03Institution Load(int pid);
        public BO.a03Institution LoadByRedizo(string redizo, int pid_exclude);
        public IEnumerable<BO.a03Institution> GetList(BO.myQuery mq);
        public int Save(BO.a03Institution rec);
        
    }
    class a03InstitutionBL : BaseBL,Ia03InstitutionBL
    {        
        public a03InstitutionBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend=null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a03"));
            sb(",a05.a05name,a05.a05UIVCode,a06.a06Name,a21.a21Name,a09.a09name,a09.a09UIVCode,a05.a05UIVCode,a70.a70Code,a28.a28Name");
            sb(" FROM a03Institution a");
            sb(" LEFT OUTER JOIN a05Region a05 ON a.a05id=a05.a05id LEFT OUTER JOIN a09FounderType a09 on a.a09id=a09.a09id LEFT OUTER JOIN a06InstitutionType a06 ON a.a06ID=a06.a06ID");
            sb(" LEFT OUTER JOIN a21InstitutionLegalType a21 ON a.a21ID=a21.a21ID LEFT OUTER JOIN a70SIS a70 ON a.a70ID=a70.a70ID");
            sb(" LEFT OUTER JOIN a28SchoolType a28 ON a.a28ID=a28.a28ID");
            sb(strAppend);            
            return sbret();
        }
        public BO.a03Institution Load(int pid)
        {
            return _db.Load<BO.a03Institution>(GetSQL1(" WHERE a.a03ID=@pid"), new { pid = pid });            
        }
        public BO.a03Institution LoadByRedizo(string redizo,int pid_exclude)
        {
            return _db.Load<BO.a03Institution>(GetSQL1(" WHERE a.a03REDIZO LIKE @redizo AND a.a03ID<>@pid_exclude"), new { redizo = redizo, pid_exclude= pid_exclude });
        }

        public IEnumerable<BO.a03Institution> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a03Institution>(fq.FinalSql,fq.Parameters);
        }
                
        public int Save(BO.a03Institution rec)
        {
            if (ValidateBeforeSave(ref rec) == false)
            {
                return 0;
            }            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a03ID);
            p.AddInt("a05ID", rec.a05ID,true);
            p.AddInt("a09ID", rec.a09ID, true);
            p.AddInt("a06ID", rec.a06ID, true);
            p.AddInt("a21ID", rec.a21ID, true);
            p.AddInt("a70ID", rec.a70ID, true);
            p.AddInt("a28ID", rec.a28ID, true);            
            p.AddInt("a03ID_Founder", rec.a03ID_Founder, true);
            p.AddInt("a03ID_Supervisory", rec.a03ID_Supervisory, true);
            
            p.AddBool("a03IsTestRecord", rec.a03IsTestRecord);
            p.AddEnumInt("a03SchoolPortalFlag", rec.a03SchoolPortalFlag);            

            p.AddString("a03Name", rec.a03Name);
            p.AddString("a03ShortName", rec.a03ShortName);
            p.AddString("a03ICO", rec.a03ICO);
            p.AddString("a03REDIZO", rec.a03REDIZO);            
            p.AddString("a03FounderCode", rec.a03FounderCode);
            p.AddString("a03City", rec.a03City);            
            p.AddString("a03Street", rec.a03Street);
            p.AddString("a03PostCode", rec.a03PostCode);
            p.AddString("a03Phone", rec.a03Phone);
            p.AddString("a03Mobile", rec.a03Mobile);
            p.AddString("a03Fax", rec.a03Fax);
            p.AddString("a03Email", rec.a03Email);
            p.AddString("a03Web", rec.a03Web);
            p.AddString("a03DirectorFullName", rec.a03DirectorFullName);

            p.AddEnumInt("a03ParentFlag", rec.a03ParentFlag);
            p.AddInt("a03ID_Parent", rec.a03ID_Parent, true);

            int intPID= _db.SaveRecord("a03Institution", p.getDynamicDapperPars(),rec);

           
            
            return intPID;
        }


        private bool ValidateBeforeSave(ref BO.a03Institution c)
        {
            if (c.a06ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Typ instituce]."); return false;
            }
                        
            if (string.IsNullOrEmpty(c.a03Name) == true)
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(c.a03City) == true)
            {
                this.AddMessage("Chybí vyplnit [Město]."); return false;
            }
            if (!string.IsNullOrEmpty(c.a03PostCode))
            {
                c.a03PostCode = c.a03PostCode.Replace(" ", "");
                if (BO.BAS.InInt(c.a03PostCode) == 0)
                {
                    this.AddMessage("PSČ není zadáno správně."); return false;
                }
            }
            if (c.a03ID_Founder==c.pid && c.pid != 0)
            {
                this.AddMessage("Vazba na zřizovatele není logická.");
            }
            if (c.a06ID == 3)
            {
                if (c.a03ID_Supervisory != 0)
                {
                    this.AddMessage("U typu instituce [Dohledový orgán] nemůže být vyplněna vazba na instituci typu [Dohledový orgán]."); return false;
                }
            }
            if (c.a06ID == 2)
            {
                c.a09ID = 0;
                c.a03ID_Founder = 0;
                c.a03REDIZO = "";
                if (string.IsNullOrEmpty(c.a03FounderCode) == true)
                {
                    this.AddMessage("U typu instituce 'Zřizovatel' je [Kód zřizovatele] je povinné pole."); return false;
                }
            }
            if (c.a06ID == 1)
            {
                c.a03REDIZO = c.a03REDIZO.Trim().Replace(" ", "");
                if (string.IsNullOrEmpty(c.a03REDIZO) == true)
                {
                    this.AddMessage("Chybí vyplnit REDIZO."); return false;
                }
                if (BO.BAS.InDouble(c.a03REDIZO) == 0 || c.a03REDIZO.Length != 9 || c.a03REDIZO.Substring(0,1)=="0")
                {
                    this.AddMessage("Hodnota [REDIZO] musí nesmí začínat nulou a délka musí být 9 znaků.");return false;
                }
                if (LoadByRedizo(c.a03REDIZO,c.pid) != null)
                {
                    this.AddMessageTranslated(string.Format(_mother.tra("Hodnota zadaného REDIZO kódu je již použita v jiné instituci: {0}."), LoadByRedizo(c.a03REDIZO, c.pid).a03Name));return false;
                }
            }
            if (c.a03ParentFlag==BO.a03ParentFlagEnum.Slave && c.a03ID_Parent == 0)
            {
                this.AddMessage("U podřízené školy chybí vazba na nadřízenou instituci."); return false;
            }
            if (c.a03ParentFlag == BO.a03ParentFlagEnum.Master && c.a03ID_Parent > 0)
            {
                this.AddMessage("U nadřízené školy nelze mít vazbu na nadřízenou instituci."); return false;
            }
            if (c.a03ParentFlag == BO.a03ParentFlagEnum.Slave && c.a03ID_Parent == c.pid)
            {
                this.AddMessage("Škola nemůže být podřízená sama sobě."); return false;
            }
            return true;
        }

    }
}
