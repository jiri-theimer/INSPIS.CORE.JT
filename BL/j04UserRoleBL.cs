using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid);
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq);
        public int Save(BO.j04UserRole rec);
        public bool IsA01Create(int j04id); //vrací, zda role může zakládat nějaký typ akce
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
            if (_db.Load<BO.GetBool>("if exists(SELECT j08ID FROM j08UserRole_EventType WHERE j04id=@pid AND j08IsAllowedCreate=1) return 1 as Value else return 0 as Value", new { pid = j04id }).Value == true)
            {
                return true;
            }
            return false;

        }

        public int Save(BO.j04UserRole rec)
        {
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

            return _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
        }
    }
}
