using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface If32FilledValueBL
    {
        public BO.f32FilledValue Load(int pid);
        public BO.f32FilledValue Load(int a11id, int f19id, int f21id = 0);
        public IEnumerable<BO.f32FilledValue> GetList(BO.myQuery mq);
        public int Save(BO.f32FilledValue rec, bool bolDeleteCurrentAnswers);
        public bool Save(IEnumerable<BO.f32FilledValue> lisF32, bool bolDeleteCurrentAnswers);
        public bool SaveHiddenQuestionsInForm(int a11id, List<int> f19ids);
        public int SaveComment(BO.f32FilledValue rec, string strComment);
        public bool Delete(int pid);
        public bool DeleteAllInF19(int f19id, int a11id);
    }
    class f32FilledValueBL : BaseBL, If32FilledValueBL
    {
        public f32FilledValueBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,f19.f23ID,a.f32Value as Value,f21.f21Name,f19.x24ID,");
            sb(_db.GetSQL1_Ocas("f32",false,false,true));
            sb(" FROM f32FilledValue a INNER JOIN f19Question f19 ON a.f19ID=f19.f19ID INNER JOIN f21ReplyUnit f21 ON a.f21ID=f21.f21ID");
            sb(strAppend);
            return sbret();
        }
        public BO.f32FilledValue Load(int pid)
        {
            BO.f32FilledValue c= _db.Load<BO.f32FilledValue>(GetSQL1(" WHERE a.f32ID=@pid"), new { pid = pid });
            if (c !=null && c.f33ID > 0)
            {
                c.Value = GetF33Value(c.f33ID);
            }
            return c;
        }
        public BO.f32FilledValue Load(int a11id,int f19id,int f21id=0)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("a11id", a11id);
            p.AddInt("f19id", f19id);

            string s = GetSQL1(" WHERE a.a11ID=@a11id AND a.f19ID=@f19id");           
            if (f21id > 0)
            {
                s += " AND a.f21ID=@f21id";
                p.AddInt("f21id", f21id);
            }
            BO.f32FilledValue c = _db.Load<BO.f32FilledValue>(s,p.getDynamicDapperPars());
            if (c !=null && c.f33ID > 0)
            {
                c.Value = GetF33Value(c.f33ID);
            }
            return c;
        }

        public IEnumerable<BO.f32FilledValue> GetList(BO.myQuery mq)
        {
            sb("SELECT a.*,f19.f23ID,a.f32Value as Value,f21.f21Name,f19.x24ID,f31.f31IsPublished,");
            sb(_db.GetSQL1_Ocas("f32", false, false, true));
            sb(" FROM f32FilledValue a INNER JOIN f19Question f19 ON a.f19ID=f19.f19ID INNER JOIN f21ReplyUnit f21 ON a.f21ID=f21.f21ID");
            sb(" LEFT OUTER JOIN f31FilledQuestionPublishing f31 ON a.f19ID=f31.f19ID AND a.a11ID=f31.a11ID");
            if (mq.f06id > 0)
            {
                sb(" INNER JOIN f18FormSegment f18 ON f19.f18ID=f18.f18ID");
            }
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(sbret(), mq, _mother.CurrentUser);
            var lis= _db.GetList<BO.f32FilledValue>(fq.FinalSql, fq.Parameters);
            if (lis.Where(p => p.f33ID > 0).Count() > 0)    //ve výstupu jsou i ntext odpovědi - kvůli db výkonu řešíme samostatným SQL dotazem
            {
                List<int> f33ids = lis.Select(p => p.f33ID).ToList();
                var lisF33 = _db.GetList<BO.f33FilledValueMemo>("select f33ID,f33Value,f32ID FROM f33FilledValueMemo WHERE f33ID IN (" + string.Join(",", f33ids) + ")");
                foreach(var recF33 in lisF33)
                {
                    lis.Where(p => p.f33ID == recF33.f33ID).First().Value = recF33.f33Value;
                }
            }
            return lis;
        }

        private string GetF33Value(int f33id)
        {
            return _db.Load<BO.GetString>("SELECT f33Value as Value FROM f33FilledValueMemo WHERE f33ID=@pid", new { pid = f33id }).Value;
        }

        public int Save(BO.f32FilledValue rec, bool bolDeleteCurrentAnswers)
        {
            return SaveOneRec(rec, bolDeleteCurrentAnswers);
        }
        public bool Save(IEnumerable<BO.f32FilledValue> lisF32, bool bolDeleteCurrentAnswers)
        {
            using (var sc = new System.Transactions.TransactionScope())
            {
                foreach(var c in lisF32)
                {
                    if (SaveOneRec(c, bolDeleteCurrentAnswers) == 0)
                    {
                        return false;
                    }
                }
                sc.Complete();   //potvrzení transakce
            }
            
            return true;
        }
        private int SaveOneRec(BO.f32FilledValue rec,bool bolDeleteCurrentAnswers)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", rec.pid, System.Data.DbType.Int32);
            pars.Add("a11id", rec.a11ID, System.Data.DbType.Int32);
            pars.Add("f19id", rec.f19ID, System.Data.DbType.Int32);
            pars.Add("f21id", rec.f21ID, System.Data.DbType.Int32);
            pars.Add("f32isfilledbyeval", rec.f32IsFilledByEval, System.Data.DbType.Boolean);

            if (string.IsNullOrEmpty(rec.f32ValueAliasEvalList)==false || rec.f23ID == 10)
            {
                pars.Add("value", rec.Value+";" + rec.f32ValueAliasEvalList, System.Data.DbType.String);
            }
            else
            {
                pars.Add("value", rec.Value, System.Data.DbType.String);
            }
            pars.Add("is_delete_beforesave", bolDeleteCurrentAnswers, System.Data.DbType.Boolean);
            pars.Add("err_ret",null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 500);
            pars.Add("pid_ret", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);


            if (_db.RunSp("f32filledvalue_save", ref pars) == "1")
            {
                return pars.Get<int>("pid_ret");
            }
            else
            {
                return 0;
            }
           
            
        }

        
        public bool SaveHiddenQuestionsInForm(int a11id,List<int> f19ids)
        {
            string strGUID = BO.BAS.GetGuid();
            if (f19ids.Count > 0)
            {
                _db.RunSql("INSERT INTO p85TempBox(p85GUID,p85DataPID) SELECT '" + strGUID + "',f19ID FROM f19Question WHERE f19ID IN (" + String.Join(",", f19ids) + ")");
            }

            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);            
            pars.Add("a11id", a11id, System.Data.DbType.Int32);
            pars.Add("guid", strGUID, System.Data.DbType.String);
            pars.Add("err_ret", null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 500);

            if (_db.RunSp("f35filledquestionhidden_save", ref pars) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }
        public int SaveComment(BO.f32FilledValue rec, string strComment)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _mother.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", rec.pid, System.Data.DbType.Int32);
            pars.Add("a11id", rec.a11ID, System.Data.DbType.Int32);
            pars.Add("f19id", rec.f19ID, System.Data.DbType.Int32);
            pars.Add("f21id", rec.f21ID, System.Data.DbType.Int32);
            pars.Add("f32comment", strComment, System.Data.DbType.String);                        
            pars.Add("err_ret", null, System.Data.DbType.String, System.Data.ParameterDirection.Output, 500);
            pars.Add("pid_ret", null, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);

            if (_db.RunSp("f32filledvalue_save_comment", ref pars) == "1")
            {
                return pars.Get<int>("pid_ret");
            }
            else
            {
                return 0;
            }
           

        }

        public bool Delete(int pid)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("pid", pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if ( _db.RunSp("f32filledvalue_delete", ref pars) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


        public bool DeleteAllInF19(int f19id,int a11id)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("f19id", f19id, System.Data.DbType.Int32);
            pars.Add("a11id", a11id, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if ( _db.RunSp("f32filledvalue_delete_f19", ref pars) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public bool DeleteAllInF26(int f26id, int a11id)
        {
            var pars = new Dapper.DynamicParameters();
            pars.Add("j03id_sys", _db.CurrentUser.pid, System.Data.DbType.Int32);
            pars.Add("f26id", f26id, System.Data.DbType.Int32);
            pars.Add("a11id", a11id, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            if ( _db.RunSp("f32filledvalue_delete_f26", ref pars) == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }
}
