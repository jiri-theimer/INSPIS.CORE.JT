using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Sources;

namespace BL
{
    public interface If06FormBL:BaseInterface
    {
        public BO.f06Form Load(int pid);
        public IEnumerable<BO.f06Form> GetList(BO.myQueryF06 mq);
        public IEnumerable<BO.a13AttachmentToForm> GetListA13(int f06id);
        public IEnumerable<BO.f07Form_UserRole_EncryptedPermission> GetListF07(int f06id);
        public int Save(BO.f06Form rec, List<int> j04ids, List<int> x31ids, List<BO.a13AttachmentToForm> lisA13);
        public BO.Result CloneF06(BO.f06Form recOrig, string destname, string destexportcode);

    }
    class f06FormBL : BaseBL, If06FormBL
    {
        public f06FormBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,f12.f12Name,");
            sb(_db.GetSQL1_Ocas("f06"));
            sb(" FROM f06Form a LEFT OUTER JOIN f12FormType f12 ON a.f12ID=f12.f12ID");
            sb(strAppend);
            return sbret();
        }
        public BO.f06Form Load(int pid)
        {
            return _db.Load<BO.f06Form>(GetSQL1(" WHERE a.f06ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f06Form> GetList(BO.myQueryF06 mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f06Ordinal"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f06Form>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a13AttachmentToForm> GetListA13(int f06id)
        {
            sb("SELECT a.*,o13.o13Name,");
            sb(_db.GetSQL1_Ocas("a13", false, false,false));
            sb(" FROM a13AttachmentToForm a INNER JOIN o13AttachmentType o13 ON a.o13ID=o13.o13ID");
            sb(" WHERE a.f06ID=@f06id");
            sb(" ORDER BY a.a13Ordinal");
            return _db.GetList<BO.a13AttachmentToForm>(sbret(), new { f06id = f06id });
        }


        public int Save(BO.f06Form rec, List<int> j04ids, List<int> x31ids, List<BO.a13AttachmentToForm> lisA13)
        {
            if (!ValidateBeforeSave(ref rec,lisA13))
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {   //podléhá jedné transakci
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.f06ID);
                p.AddInt("f12ID", rec.f12ID, true);
                p.AddString("f06Name", rec.f06Name);
                p.AddString("f06Description", rec.f06Description);
                p.AddString("f06ExportCode", rec.f06ExportCode);
                p.AddBool("f06IsTemplate", rec.f06IsTemplate);
                p.AddBool("f06IsPublishAllowed", rec.f06IsPublishAllowed);
                p.AddBool("f06IsReportDialog", rec.f06IsReportDialog);
                p.AddBool("f06IsWorkflowDialog", rec.f06IsWorkflowDialog);
                p.AddBool("f06IsExportToDoc", rec.f06IsExportToDoc);
                p.AddString("f06Hint", rec.f06Hint);
                p.AddString("f06RazorTemplate", rec.f06RazorTemplate);
                p.AddInt("f06Ordinal", rec.f06Ordinal);
                p.AddEnumInt("f06UserLockFlag", rec.f06UserLockFlag);
                p.AddBool("f06IsA37Required", rec.f06IsA37Required);
                p.AddBool("f06IsA01PeriodStrict", rec.f06IsA01PeriodStrict);
                p.AddBool("f06IsA01ClosedStrict", rec.f06IsA01ClosedStrict);
                p.AddInt("f06Linker_DestF06ID", rec.f06Linker_DestF06ID, true);
                p.AddString("f06Linker_DestDB", rec.f06Linker_DestDB);
                p.AddEnumInt("f06LinkerObject", rec.f06LinkerObject);
                p.AddEnumInt("f06RelationWithTeacher", rec.f06RelationWithTeacher);
                p.AddEnumInt("f06BindScopeQuery", rec.f06BindScopeQuery);
                if (rec.f06UC == null) { rec.f06UC = BO.BAS.GetGuid(); }
                p.AddString("f06UC", rec.f06UC);

                int intPID = _db.SaveRecord("f06Form", p.getDynamicDapperPars(), rec);

                if (x31ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM f08Form_Report WHERE f06ID=@pid", new { pid = intPID });
                    }
                    if (x31ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO f08Form_Report(f06ID,x31ID) SELECT @pid,x31ID FROM x31Report WHERE x31ID IN (" + string.Join(",", x31ids) + ")", new { pid = intPID });
                    }
                }
                if (j04ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM f07Form_UserRole_EncryptedPermission WHERE f06ID=@pid", new { pid = intPID });
                    }
                    if (j04ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO f07Form_UserRole_EncryptedPermission(f06ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intPID });
                    }
                }
                if (lisA13 != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM a13AttachmentToForm WHERE f06ID=@pid", new { pid = intPID });
                    }
                    foreach (var c in lisA13)
                    {
                        p = new DL.Params4Dapper();
                        p.AddInt("pid", c.a13ID, true);
                        p.AddInt("o13ID", c.o13ID, true);
                        p.AddInt("f06ID", intPID, true);
                        p.AddBool("a13IsRequired", c.a13IsRequired);
                        p.AddInt("a13Ordinal", c.a13Ordinal);
                        _db.SaveRecord("a13AttachmentToForm", p.getDynamicDapperPars(), c, false,false);
                    }
                }
                sc.Complete();   //potvrzení transakce
                return intPID;
            }                        
        }
        public bool ValidateBeforeSave(ref BO.f06Form rec, List<BO.a13AttachmentToForm> lisA13)
        {
            if (string.IsNullOrEmpty(rec.f06Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.f12ID == 0)
            {
                this.AddMessage("Chybí [Typ formuláře]."); return false;
            }
            if (rec.f06LinkerObject != BO.f06LinkerObjectENUM.None)
            {
                if (rec.f06Linker_DestF06ID == 0)
                {
                    this.AddMessage("Chybí cílový formulář pro Linker vazbu."); return false;
                }
                else
                {
                    rec.f06Linker_DestF06ID = 0; rec.f06Linker_DestDB = "";
                }

            }

            if (lisA13 != null)
            {
                if (lisA13.Where(p => p.o13ID == 0).Count() > 0)
                {
                    this.AddMessage("V rozpisu příloh formuláře je nevyplněný typ přílohy."); return false;
                }
                if (lisA13.Count() > lisA13.Select(p => p.o13ID).Distinct().Count())
                {
                    this.AddMessage("V rozpisu příloh formuláře je duplicitně typ přílohy."); return false;
                }
            }

            return true;
        }

        private class CP
        {
            public string Prefix { get; set; }
            public int OrigPid { get; set; }
            public int NewPid { get; set; }
            
        }
        public BO.Result CloneF06(BO.f06Form recOrig, string destname,string destexportcode)
        {
            int intOrigF06ID = recOrig.pid;
           
            var lisJ04 = _mother.j04UserRoleBL.GetList(new BO.myQueryJ04() { f06id = intOrigF06ID });
            
            var lisX31 = _mother.x31ReportBL.GetList(new BO.myQueryX31() { f06id = intOrigF06ID });

            var mq = new BO.myQuery("f18");
            mq.f06id = intOrigF06ID;
            var lisF18 = _mother.f18FormSegmentBL.GetList(mq);
            
            var lisF19 = _mother.f19QuestionBL.GetList(new BO.myQueryF19() { f06id = intOrigF06ID });
            var pids = new List<CP>();


            recOrig.f06ID = 0; recOrig.pid = 0;
            if (string.IsNullOrEmpty(destname) == false)
            {
                recOrig.f06Name = destname;
            }
            if (string.IsNullOrEmpty(destexportcode) == false)
            {
                recOrig.f06ExportCode = destexportcode;
            }
            int intNewF06ID = Save(recOrig, lisJ04.Select(p => p.pid).ToList(), lisX31.Select(p => p.pid).ToList(),GetListA13(intOrigF06ID).ToList());
            if (intNewF06ID == 0)
            {
                return new BO.Result(true, "Nepodařilo se založit hlavičku formuláře.");
            }
            pids.Add(new CP() { Prefix = "f06", OrigPid = intOrigF06ID, NewPid = intNewF06ID });
            foreach (var recF18 in lisF18)
            {
                mq = new BO.myQuery("f26BatteryBoard");
                mq.f18id = recF18.pid;
                var lisF26 = _mother.f26BatteryBoardBL.GetList(mq);
                mq=new BO.myQuery("f25");
                mq.f18id = recF18.pid;
                var lisF25 = _mother.f25ChessBoardBL.GetList(mq);
                recF18.f18ID = 0;recF18.pid = 0;recF18.f06ID = intNewF06ID;recF18.f18UC = recF18.f18ParentID.ToString();
                var intNewF18ID = _mother.f18FormSegmentBL.Save(recF18);
                pids.Add(new CP() { Prefix = "f18", OrigPid = mq.f18id, NewPid = intNewF18ID });
                foreach (var recF26 in lisF26)
                {
                    var intOrigF26ID = recF26.pid;
                    recF26.f18ID = intNewF18ID; recF26.pid = 0;recF26.f26ID = 0;                    
                    var intF26ID = _mother.f26BatteryBoardBL.Save(recF26);
                    pids.Add(new CP() { Prefix = "f26", OrigPid = intOrigF26ID, NewPid = intF26ID });
                }
                foreach (var recF25 in lisF25)
                {
                    var intOrigF25ID = recF25.pid;
                    recF25.f18ID = intNewF18ID; recF25.pid = 0; recF25.f25ID = 0;
                    var intF25ID = _mother.f25ChessBoardBL.Save(recF25);
                    pids.Add(new CP() { Prefix = "f25", OrigPid = intOrigF25ID, NewPid = intF25ID });
                }
            }
            foreach(var recF19 in lisF19)
            {
                recF19.f18ID = FindNP(pids, "f18", recF19.f18ID);
                recF19.f26ID = FindNP(pids, "f26", recF19.f26ID);
                recF19.f25ID = FindNP(pids, "f25", recF19.f25ID);                
               
                var lisF21 = _mother.f21ReplyUnitBL.GetList(new BO.myQueryF21() { f19id = recF19.pid });
                recF19.pid = 0; recF19.f19ID = 0;

                if (recF19.ReplyControl == BO.ReplyKeyEnum.TextBox && lisF21.Count() > 0)
                {
                    recF19.TextBox_MinValue = lisF21.First().f21MinValue;
                    recF19.TextBox_MaxValue = lisF21.First().f21MaxValue;
                    recF19.TextBox_ExportValue = lisF21.First().f21ExportValue;
                    var intF19ID = _mother.f19QuestionBL.Save(recF19, null);
                }
                else
                {
                    var intF19ID = _mother.f19QuestionBL.Save(recF19, lisF21.Select(p => p.pid).ToList());
                }

                
            }
            mq = new BO.myQuery("f18");
            mq.f06id = intNewF06ID;
            lisF18 = _mother.f18FormSegmentBL.GetList(mq);
            foreach (var recF18 in lisF18.Where(p=>p.f18ParentID>0))
            {
                recF18.f18ParentID = FindNP(pids, "f18", recF18.f18ParentID);
                _mother.f18FormSegmentBL.Save(recF18);
            }

                if (intNewF06ID > 0)
            {
                var ret= new BO.Result(false, "ok");
                ret.pid = intNewF06ID;
                return ret;
            }
            else
            {
                return new BO.Result(true, "Chyba");
            }
            
        }

        private int FindNP(List<CP> pids,string prefix,int origpid)
        {
            if (pids.Where(p => p.OrigPid == origpid && p.Prefix==prefix).Count() > 0)
            {
                return pids.Where(p => p.OrigPid == origpid && p.Prefix==prefix).First().NewPid;
            }
            return 0;
        }

        public IEnumerable<BO.f07Form_UserRole_EncryptedPermission> GetListF07(int f06id)
        {
            
            return _db.GetList<BO.f07Form_UserRole_EncryptedPermission>("select * FROM f07Form_UserRole_EncryptedPermission WHERE f06ID=@f06id", new { f06id = f06id });
        }

    }
}
