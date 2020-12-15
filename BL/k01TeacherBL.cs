using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ik01TeacherBL:BaseInterface
    {
        public BO.k01Teacher Load(int pid);
        public IEnumerable<BO.k01Teacher> GetList(BO.myQuery mq);
        public BO.k02TeacherSchool LoadK02(int k01id, int a03id);

    }
    class k01TeacherBL : BaseBL, Ik01TeacherBL
    {
        public k01TeacherBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("k01"));
            sb(" FROM k01Teacher a");
            sb(strAppend);
            return sbret();
        }
        private string GetSQL_K02(string strAppend = null)
        {
            sb("SELECT a.*,a03.a03RedIZO,a03.a03Name,");
            sb(_db.GetSQL1_Ocas("k02"));
            sb(" FROM k02TeacherSchool a INNER JOIN k01Teacher k01 ON a.k01ID=k01.k01ID INNER JOIN a03Institution a03 ON a.a03ID=a03.a03ID");
            sb(strAppend);
            return sbret();
        }
        public BO.k01Teacher Load(int pid)
        {
            return _db.Load<BO.k01Teacher>(GetSQL1(" WHERE a.k01ID=@pid"), new { pid = pid });
        }
        public BO.k02TeacherSchool LoadK02(int k01id,int a03id)
        {
            return _db.Load<BO.k02TeacherSchool>(GetSQL_K02(" WHERE a.k01ID=@k01id AND a.a03ID=@a03id"), new { k01id = k01id,a03id=a03id });
        }

        public IEnumerable<BO.k01Teacher> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.k01Teacher>(fq.FinalSql, fq.Parameters);
        }




    }
}
