using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io13AttachmentTypeBL
    {
        public BO.o13AttachmentType Load(int pid);
        public IEnumerable<BO.o13AttachmentType> GetList(BO.myQueryO13 mq);
        public int Save(BO.o13AttachmentType rec);
        

    }
    class o13AttachmentTypeBL : BaseBL, Io13AttachmentTypeBL
    {
        public o13AttachmentTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_o13_get_parent_inline(a.o13ID) as ParentPath,x29.x29Name,o13parent.o13Name as ParentName,");
            sb("case when a.o13TreeLevel>1 then replace(space(2*(a.o13TreeLevel-1)),' ','-') else '' END+a.o13Name as TreeItem,");
            sb("replace(a.o13DefaultArchiveFolder,CHAR(92),CHAR(92)+CHAR(92)) as SharpFolder,");
            sb(_db.GetSQL1_Ocas("o13"));
            sb(" FROM o13AttachmentType a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID LEFT OUTER JOIN o13AttachmentType o13parent ON a.o13ParentID=o13parent.o13ID");
            sb(strAppend);
            
            
            return sbret();
        }
        public BO.o13AttachmentType Load(int pid)
        {
            return _db.Load<BO.o13AttachmentType>(GetSQL1(" WHERE a.o13ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.o13AttachmentType> GetList(BO.myQueryO13 mq)
        {            
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.o13TreeIndex"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o13AttachmentType>(fq.FinalSql, fq.Parameters);
        }

        

        public int Save(BO.o13AttachmentType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.o13ID);
            p.AddInt("o13ParentID", rec.o13ParentID, true);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddString("o13Name", rec.o13Name);
            p.AddString("o13FilePrefix", rec.o13FilePrefix);
            p.AddString("o13Description", rec.o13Description);
            p.AddString("o13DefaultArchiveFolder", rec.o13DefaultArchiveFolder);
            p.AddBool("o13IsPortalDoc", rec.o13IsPortalDoc);
            p.AddBool("o13IsObjection", rec.o13IsObjection);

            int intPID = _db.SaveRecord("o13AttachmentType", p, rec);
            if (intPID > 0)
            {
                _db.RunSql("exec dbo._core_o13_recalc_tree");
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.o13AttachmentType rec)
        {
            
            if (string.IsNullOrEmpty(rec.o13Name) || rec.x29ID==0 || string.IsNullOrEmpty(rec.o13DefaultArchiveFolder)==true)
            {
                this.AddMessage("[Název], [Entita] a [Archiv složka] jsou povinná pole."); return false;
            }
            if (rec.o13ParentID > 0)
            {
                var recParent = Load(rec.o13ParentID);
                if (rec.o13TreeIndexFrom <= recParent.o13TreeIndex && rec.o13TreeIndexTo >= recParent.o13TreeIndex)
                {
                    if (rec.o13TreeIndexFrom > 0 || rec.o13TreeIndexTo > 0 || recParent.o13TreeIndex > 0)
                    {
                        this.AddMessage("Stromové zatřídění položky není logické.");
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
