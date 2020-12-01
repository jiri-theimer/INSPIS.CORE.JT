﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ix55WidgetBL
    {
        public BO.x55Widget Load(int pid);
        public BO.x55Widget LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq);
        public int Save(BO.x55Widget rec, List<int> j04ids);

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
        public BO.x55Widget LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.x55Widget>(GetSQL1(" WHERE a.x55Code LIKE @code AND a.x55ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }

        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x55Widget>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x55Widget rec, List<int> j04ids)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x55ID);
            p.AddString("x55Name", rec.x55Name);
            p.AddString("x55Code", rec.x55Code);
            p.AddString("x55TableSql", rec.x55TableSql);
            p.AddString("x55TableColHeaders", rec.x55TableColHeaders);
            p.AddString("x55TableColTypes", rec.x55TableColTypes);
            p.AddString("x55Content", rec.x55Content);
            p.AddInt("x55Ordinal", rec.x55Ordinal);
            p.AddBool("x55IsSystem", rec.x55IsSystem);
            p.AddEnumInt("x55TypeFlag", rec.x55TypeFlag);
            p.AddString("x55Image", rec.x55Image);
            p.AddString("x55Description", rec.x55Description);
            int intPID = _db.SaveRecord("x55Widget", p.getDynamicDapperPars(), rec);
            if (intPID > 0)
            {
                if (j04ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM x57WidgetRestriction WHERE x55ID=@pid", new { pid = intPID });
                    }
                    if (j04ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO x57WidgetRestriction(x55ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intPID });
                    }
                }
            }


            return intPID;
        }

        public bool ValidateBeforeSave(BO.x55Widget rec)
        {
            if (string.IsNullOrEmpty(rec.x55Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x55Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            if (LoadByCode(rec.x55Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V systému již existuje jiný dashboard box s kódem: {0}."), rec.x55Code)); return false;
            }

            return true;
        }

        public BO.x56WidgetBinding LoadState(int j03id)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x56"));
            sb(" FROM x56WidgetBinding a WHERE a.j03ID=@j03id");
            
            var rec= _db.Load<BO.x56WidgetBinding>(sbret(), new { j03id = j03id });

            if (rec == null && j03id>0)
            {
                rec = new BO.x56WidgetBinding() { j03ID = j03id };
                if (SaveState(rec) > 0)
                {
                    return LoadState(j03id);
                }
                
            }

            return rec;
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