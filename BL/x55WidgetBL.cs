using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ix55WidgetBL
    {
        public BO.x55Widget Load(int pid);
        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq);
        public int Save(BO.x55Widget rec);

        public BO.x56WidgetBinding LoadState(int j03id);
        public int SaveState(BO.x56WidgetBinding rec);

    }
    class x55WidgetBL : BaseBL, Ix55WidgetBL
    {
        public x55WidgetBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x55"));
            sb(" FROM x55Widget a");
            sb(strAppend);
            return sbret();
        }
        public BO.x55Widget Load(int pid)
        {
            return _db.Load<BO.x55Widget>(GetSQL1(" WHERE a.x55ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x55Widget>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x55Widget rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x55ID);
            p.AddString("x55Name", rec.x55Name);
            p.AddString("x55Code", rec.x55Code);
            p.AddString("x55Sql", rec.x55Sql);            
            p.AddString("x55Content", rec.x55Content);
            p.AddInt("x55Ordinal", rec.x55Ordinal);
            p.AddBool("x55IsSystem", rec.x55IsSystem);
            p.AddEnumInt("x55TypeFlag", rec.x55TypeFlag);
            p.AddString("x55Image", rec.x55Image);

            int intPID = _db.SaveRecord("x55Widget", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.x55Widget rec)
        {
            if (string.IsNullOrEmpty(rec.x55Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

        public BO.x56WidgetBinding LoadState(int j03id)
        {
            return _db.Load<BO.x56WidgetBinding>(GetSQL1(" WHERE a.j03ID=@j03id"), new { j03id = j03id });
        }

        public int SaveState(BO.x56WidgetBinding rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("j03ID", rec.j03ID,true);
            p.AddString("x56Skin", rec.x56Skin);
            p.AddString("x56Boxes", rec.x56Boxes);
            p.AddString("x56DockState", rec.x56DockState);
           
            return _db.SaveRecord("x56WidgetBinding", p.getDynamicDapperPars(), rec);

        }
    }
}
