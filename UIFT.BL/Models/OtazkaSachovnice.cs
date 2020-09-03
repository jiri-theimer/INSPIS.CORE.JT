using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UIFT.Models
{
    public class OtazkaSachovnice : IOtazka
    {
        public OtazkaSachovnice(BO.f25ChessBoard chessboard)
        {
            this.Base = chessboard;
            this.IsHidden = false;
            this.IsPublished = false;

            // sloupce
            if (string.IsNullOrEmpty(this.Base.f25ColumnHeaders))
                this.Sloupce = new string[this.Base.f25ColumnCount];
            else
                this.Sloupce = this.Base.f25ColumnHeaders.Split('|');

            // radky
            if (string.IsNullOrEmpty(this.Base.f25RowHeaders))
                this.Radky = new string[this.Base.f25RowCount];
            else
                this.Radky = this.Base.f25RowHeaders.Split('|');
        }

        #region implement interface
        public int MaxAllowedSize
        {
            get { return 0; }
        }

        public int Order
        {
            get { return 0; }
        }

        public string Regex
        {
            get { return ""; }
        }

        public bool IsPreview
        {
            get;
            internal set;
        }

        public bool IsEncrypted
        {
            get
            {
                return false;
            }
        }

        public string StatID
        {
            get { return ""; }
        }

        public bool IsMultiselect
        {
            get { return false; }
        }

        public int PID
        {
            get { return this.Base.pid; }
        }

        public BO.x24DataTypeEnum ReplyType
        {
            get { return BO.x24DataTypeEnum.tNone; }
        }

        public BO.ReplyKeyEnum ReplyControl
        {
            get { return BO.ReplyKeyEnum.ChessBoard; }
        }

        public bool IsRequired
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public string Name
        {
            get { return this.Base.f25Name; }
        }

        public string SupportingText
        {
            get { return this.Base.f25Description; }
        }

        public string Hint
        {
            get { return ""; }
        }

        public int IdSekce
        {
            get { return this.Base.f18ID; }
        }

        public bool IsHidden { get; internal set; }

        public bool IsPublished { get; internal set; }

        public bool CanBePublished { get; set; }
        #endregion

        public string ViewName
        {
            get
            {
                return "ChessBoard";
            }
        }

        /// <summary>
        /// Zakladni data o otazce
        /// </summary>
        public BO.f25ChessBoard Base;

        /// <summary>
        /// Seznam otazek v sachovnici
        /// </summary>
        public List<Models.Otazka> Otazky { get; internal set; }

        /// <summary>
        /// Seznam moznych odpovedi k otazce
        /// </summary>
        public List<BO.f21ReplyUnitJoinedF19> Odpovedi { get; internal set; }

        /// <summary>
        /// Seznam uzivatelem jiz vyplnenych odpovedi k otazce
        /// </summary>
        public List<BO.f32FilledValue> VyplneneOdpovedi { get; internal set; }

        /// <summary>
        /// Seznam priloh k otazce
        /// </summary>
        public List<BO.o27Attachment> Prilohy { get; internal set; }

        /// <summary>
        /// Nazvy sloupcu
        /// </summary>
        public string[] Sloupce { get; private set; }

        /// <summary>
        /// Nazvy radku
        /// </summary>
        public string[] Radky { get; private set; }

        public string CheckAnswerValue(ref string value)
        {
            return "";
        }

        /// <summary>
        /// Validace odpovedi na danou otazku (z hlediska datovych typu)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True, pokud je odpoved validni nebo se jedna o prazdnou odpoved (value="")</returns>
        public bool Validate(ref string value)
        {
            // prazdna odpoved je vzdy validni
            if (string.IsNullOrEmpty(value))
                return true;

            bool validated = true;

            return validated;
        }

        /// <summary>
        /// Vraci seznam CSS trid pro kontajner otazky
        /// </summary>
        public string GetCssClasses()
        {
            List<string> classes = new List<string>();
            classes.Add("otazkaContainer");
            classes.Add("qControl_" + this.ViewName);
            // required
            if (this.IsRequired)
                classes.Add("isRequired");
            // otazka v preview rezimu
            if (this.IsPreview)
                classes.Add("preview");
            // readonly otazka
            if (this.ReadOnly)
                classes.Add("readOnly");

            return string.Join(" ", classes.ToArray());
        }

        /// <summary>
        /// Vraci JSON string obsahujici informace o otazce
        /// </summary>
        public string GetJsonData()
        {
            StringBuilder sb = new StringBuilder("{");
            // id otazky
            sb.AppendFormat("\"f25id\":{0},", this.Base.pid);
            // druh otazky
            sb.AppendFormat("\"control\":{0},", Convert.ToInt32(this.ReplyControl));
            // druh otazek v sachovnici
            sb.AppendFormat("\"childControl\":{0}", Convert.ToInt32(this.Otazky[0].ReplyControl));
            // pro textbox i typ prvku
            if (this.Otazky[0].ReplyControl == BO.ReplyKeyEnum.TextBox)
                sb.AppendFormat(",\"childType\":{0}", Convert.ToInt32(this.Otazky[0].ReplyType));
            // readonly
            sb.AppendFormat(",\"readonly\":{0}", this.ReadOnly.ToString().ToLower());
            sb.Append("}");

            return sb.ToString();
        }
    }
}