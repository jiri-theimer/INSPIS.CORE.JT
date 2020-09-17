using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid);
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq);
        public int Save(BO.j04UserRole rec, List<int> j05ids, List<BO.j08UserRole_EventType> lisJ08);
        public bool IsA01Create(int j04id); //vrací, zda role může zakládat nějaký typ akce
        public IEnumerable<BO.j05Permission> GetListJ05(int j04id);
        public IEnumerable<BO.j08UserRole_EventType> GetListJ08(int j04id);
    }
    class j04UserRoleBL: BaseBL,Ij04UserRoleBL
    {

        public j04UserRoleBL(BL.Factory mother):base(mother)
        {
            
        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*," + _db.GetSQL1_Ocas("j04") + " FROM j04UserRole a");
            sb(strAppend);
            return sbret();
        }

        public BO.j04UserRole Load(int pid)
        {
            return _db.Load<BO.j04UserRole>(GetSQL1(" WHERE a.j04ID=@pid"), new { pid = pid });            
        }
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j04UserRole>(fq.FinalSql, fq.Parameters);
        }

        public bool IsA01Create(int j04id)
        {
            var rec = Load(j04id);
            if (rec.j04IsAllowedAllEventTypes)
            {
                return true;
            }
            if (_db.Load<BO.GetBool>("if exists(SELECT j08ID FROM j08UserRole_EventType WHERE j04id=@pid AND j08IsAllowedCreate=1) select 1 else select 0 as Value", new { pid = j04id }).Value == true)
            {
                return true;
            }
            return false;

        }

        public int Save(BO.j04UserRole rec,List<int>j05ids,List<BO.j08UserRole_EventType>lisJ08)
        {
            if (ValidateBeforeSave(rec, j05ids, lisJ08) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j04ID);
            p.AddString("j04Name", rec.j04Name);
            p.AddString("j04RoleValue", rec.j04RoleValue);
            p.AddEnumInt("j04RelationFlag", rec.j04RelationFlag);
            p.AddEnumInt("j04PortalFaceFlag", rec.j04PortalFaceFlag);
            p.AddString("j04Aspx_PersonalPage", rec.j04Aspx_PersonalPage);
            p.AddString("j04Aspx_PersonalPage_Mobile", rec.j04Aspx_PersonalPage_Mobile);
            p.AddBool("j04IsAllowedAllEventTypes", rec.j04IsAllowedAllEventTypes);
            p.AddBool("j04IsAllowInSchoolAdmin", rec.j04IsAllowInSchoolAdmin);
            p.AddInt("j04ElearningDuration", rec.j04ElearningDuration);
            p.AddBool("j04IsElearningNeeded", rec.j04IsElearningNeeded);

            int intPID= _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
            
            if (j05ids != null)
            {                               
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM j06UserRole_Permission WHERE j04ID=@pid", new { pid = intPID });
                }
                _db.RunSql("INSERT INTO j06UserRole_Permission(j04id,j05id) SELECT @pid,j05ID FROM j05Permission WHERE j05ID IN (" + string.Join(",", j05ids) + ")",new { pid = intPID });

                var lisJ05 = GetListJ05(intPID);
                string strj04RoleValue = new string('0', 50);
                foreach(var c in lisJ05)
                {
                    int x = c.j05Value;
                    strj04RoleValue = strj04RoleValue.Substring(0, x - 1) + "1" + strj04RoleValue.Substring(x, strj04RoleValue.Length - x);
                }
                _db.RunSql("UPDATE j04UserRole SET j04RoleValue=@rolevalue WHERE j04ID=@pid", new { rolevalue = strj04RoleValue, pid = intPID });
            }
            if (lisJ08 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM j08UserRole_EventType WHERE j04ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisJ08)
                {
                    _db.RunSql("INSERT INTO j08UserRole_EventType(j04ID,a10ID,j08IsAllowedCreate,j08IsAllowedRead,j08IsLeader,j08IsMember) VALUES(@pid,@a10id,@iscreate,@isread,@isleader,@ismember)", new { pid = intPID, a10id = c.a10ID, iscreate=c.j08IsAllowedCreate, isread=c.j08IsAllowedRead, isleader=c.j08IsLeader, ismember=c.j08IsMember });
                }
            }

            return intPID;
        }

        public IEnumerable<BO.j05Permission> GetListJ05(int j04id)
        {
            sb("SELECT * FROM j05Permission WHERE j05ID IN (select j05ID FROM j06UserRole_Permission WHERE j04ID=@pid)");            
            sb(" ORDER BY j05Value");
            return _db.GetList<BO.j05Permission>(sbret(), new { pid = j04id });
        }
        public IEnumerable<BO.j08UserRole_EventType> GetListJ08(int j04id)
        {
            sb("SELECT a.*,a10.a10Name FROM j08UserRole_EventType a INNER JOIN a10EventType a10 ON a.a10ID=a10.a10ID WHERE a.j04ID=@pid");
           
            return _db.GetList<BO.j08UserRole_EventType>(sbret(), new { pid = j04id });
        }

        private bool ValidateBeforeSave(BO.j04UserRole rec, List<int> j05ids, List<BO.j08UserRole_EventType> lisJ08)
        {
            if (j05ids !=null && j05ids.Count==0)
            {
                this.AddMessage("K aplikační roli musí být přiřazeno minimálně jedno oprávnění!"); return false;
            }
            if (string.IsNullOrEmpty(rec.j04Name) == true)
            {
                this.AddMessage("Název aplikační role je povinné pole."); return false;
            }
            if ((rec.j04PortalFaceFlag==BO.j04PortalFaceFlagEnum.School || rec.j04PortalFaceFlag == BO.j04PortalFaceFlagEnum.Founder) && rec.j04RelationFlag !=BO.j04RelationFlagEnum.A03)
            {
                this.AddMessage("PORTÁL role 'Škola' nebo 'Zřizovatel' musí mít vztah k instituci.");return false;
            }
            if (lisJ08 !=null && rec.j04IsAllowedAllEventTypes == false)
            {
                foreach(var c in lisJ08)
                {
                    if (c.a10ID==0 || (c.j08IsAllowedCreate == false && c.j08IsAllowedRead==false && c.j08IsLeader==false && c.j08IsMember==false))
                    {
                        this.AddMessage("Okruh přístupných typů akcí není korektně definován.");return false;
                    }
                }
            }
            

            return true;
        }
    }
}
