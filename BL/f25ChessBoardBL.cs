using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If25ChessBoardBL
    {
        public BO.f25ChessBoard Load(int pid);
        public IEnumerable<BO.f25ChessBoard> GetList(BO.myQuery mq);
        public int Save(BO.f25ChessBoard rec);


    }
    class f25ChessBoardBL : BaseBL, If25ChessBoardBL
    {
        public f25ChessBoardBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,f18.f18Name,");
            sb(_db.GetSQL1_Ocas("f25"));
            sb(" FROM f25ChessBoard a INNER JOIN f18FormSegment f18 ON a.f18ID=f18.f18ID");
            sb(strAppend);

            return sbret();
        }
        public BO.f25ChessBoard Load(int pid)
        {
            return _db.Load<BO.f25ChessBoard>(GetSQL1(" WHERE a.f25ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f25ChessBoard> GetList(BO.myQuery mq)
        {
            
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f25ChessBoard>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f25ChessBoard rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f25ID);
            p.AddInt("f25ColumnCount", rec.f25ColumnCount);
            p.AddInt("f25RowCount", rec.f25RowCount);
            p.AddString("f25RowHeaders", rec.f25RowHeaders);
            p.AddString("f25ColumnHeaders", rec.f25ColumnHeaders);
            p.AddString("f25Name", rec.f25Name);
            p.AddString("f25Description", rec.f25Description);
            if (rec.f25UC==null) { rec.f25UC = BO.BAS.GetGuid(); }
            p.AddString("f25UC", rec.f25UC);


            int intPID = _db.SaveRecord("f25ChessBoard", p.getDynamicDapperPars(), rec);


            return intPID;
        }

        public bool ValidateBeforeSave(BO.f25ChessBoard rec)
        {
            if (string.IsNullOrEmpty(rec.f25Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.f18ID==0)
            {
                this.AddMessage("Chybí vyplnit [Segment formuláře]."); return false;
            }
            if (rec.f25RowCount<=0 || rec.f25RowCount > 20)
            {
                this.AddMessage("Počet řádek šachovnice musí být v rozmezí 1-20.");return false;
            }
            if (rec.f25ColumnCount <= 0 || rec.f25ColumnCount > 20)
            {
                this.AddMessage("Počet sloupců šachovnice musí být v rozmezí 1-20."); return false;
            }
            if (rec.f19ID_TemplateID>0 && _mother.f19QuestionBL.Load(rec.f19ID_TemplateID).ReplyControl != BO.ReplyKeyEnum.TextBox)
            {
                this.AddMessage("V šachovnici může fungovat pouze otázka typu [TEXTBOX].");return false;
            }

            return true;
        }

    }
}
