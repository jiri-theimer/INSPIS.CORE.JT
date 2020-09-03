using System;
using System.Collections.Generic;
using System.Text;
using UIFT.Repository;

namespace UIFT.Models
{
    public class Otazka : IOtazka
    {
        public Otazka(BO.f19Question otazka, string checkboxTrueValue, string checkboxFalseValue)
        {
            this.Base = otazka;
            this.IsHidden = false;
            this.ReadOnly = false;
            this.IsPublished = false;
            this.CanBePublished = otazka.f19PortalPublishFlag == BO.f19PortalPublishFlagENUM.PublishWithSchoolApprove;
            this.CheckboxFalseValue = checkboxFalseValue;
            this.CheckboxTrueValue = checkboxTrueValue;
        }
        private string CheckboxTrueValue;
        private string CheckboxFalseValue;

        #region implement interface
        public int Order
        {
            get { return this.Base.f19Ordinal; }
        }

        public string Regex
        {
            get { return this.Base.f19Regex; }
        }

        public int MaxAllowedSize
        {
            get { return this.Base.f19MaxAllowedSize; }
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
                return this.Base.f19IsEncrypted;
            }
        }

        public string StatID
        {
            get { return this.Base.f19StatID; }
        }

        public int PID
        {
            get { return this.Base.pid; }
        }

        public bool IsMultiselect
        {
            get 
            {
                if (this.ReplyControl == BO.ReplyKeyEnum.TextBox && this.ReplyType == BO.x24DataTypeEnum.tString)
                    return this.Base.f19IsTextboxMultiline;
                else
                    return this.Base.f19IsMultiselect; 
            }
        }

        public BO.x24DataTypeEnum ReplyType
        {
            get { return this.Base.ReplyType; }
        }

        public BO.ReplyKeyEnum ReplyControl
        {
            get 
            {
                if (this.Base.ReplyControl == BO.ReplyKeyEnum.Checkbox && this.Base.f19IsMultiselect)
                    return BO.ReplyKeyEnum.Listbox;
                else
                    return this.Base.ReplyControl; 
            }
        }

        public bool IsRequired
        {
            get { return _IsRequired.HasValue ? _IsRequired.Value : this.Base.f19IsRequired; }
            set { _IsRequired = value; }
        }
        private bool? _IsRequired;

        public bool ReadOnly { get; set; }

        public string Name
        {
            get { return this.Base.f19Name; }
        }

        public string SupportingText
        {
            get { return this.Base.f19SupportingText; }
        }

        public string Hint
        {
            get { return this.Base.f19Hint; }
        }

        public int IdSekce
        {
            get { return this.Base.f18ID; }
        }

        public bool IsHidden { get; internal set; }

        public bool IsPublished { get; internal set; }

        public bool CanBePublished { get; set; }
        #endregion

        #region ViewName property
        public string ViewName
        {
            get
            {
                // jmeno editoru pro aktualni typ otazky
                string controlName = this.Base.ReplyControl.ToString();

                switch (this.Base.ReplyControl)
                {
                    case BO.ReplyKeyEnum.EvalList:
                        controlName = this.Base.f19IsMultiselect ? "EvalListMulti" : "EvalList";
                        break;

                    case BO.ReplyKeyEnum.TextBox: // pokud se jedna o Textbox, resit podtyp
                        switch (this.ReplyType)
                        {
                            case BO.x24DataTypeEnum.tDecimal: // Decimal je stejny jako Integer
                                controlName += "_tInteger";
                                break;
                            case BO.x24DataTypeEnum.tDateTime: // Datetime je stejny jako Date
                                controlName += "_tDate";
                                break;
                            default:
                                controlName += "_" + this.Base.ReplyType.ToString();
                                break;
                        }
                        break;

                    case BO.ReplyKeyEnum.Checkbox:
                        controlName = this.Base.f19IsMultiselect ? "CheckboxList" : "Checkbox";
                        break;

                    case BO.ReplyKeyEnum.Listbox:
                        // pokud se jedna o Listbox, vyber Checkbox list nebo radio button list podle zaskrtnuteho parametru IsMultiselect
                        // Listbox jako takovy (multi select) neexistuje
                        controlName = this.Base.f19IsMultiselect ? "CheckboxList" : "RadioButtonList";
                        break;
                }

                return controlName;
            }
        }
        #endregion

        /// <summary>
        /// Zdroj dat pro EvalList otazky, jinak null
        /// </summary>
        public Dictionary<string, string> EvalSource { get; internal set; }

        /// <summary>
        /// Zakladni data o otazce
        /// </summary>
        public BO.f19Question Base;

        /// <summary>
        /// Seznam moznych odpovedi k otazce
        /// </summary>
        public List<BO.f21ReplyUnitJoinedF19> Odpovedi { get; internal set; }

        /// <summary>
        /// Seznam priloh k otazce
        /// </summary>
        public List<BO.o27Attachment> Prilohy { get; internal set; }

        /// <summary>
        /// Seznam uzivatelem jiz vyplnenych odpovedi k otazce
        /// </summary>
        public List<BO.f32FilledValue> VyplneneOdpovedi { get; internal set; }

        /// <summary>
        /// Soubory uploadovane uzivatelem k odpovedi
        /// [f32id, soubor]
        /// </summary>
        public List<BO.o27Attachment> VyplneneOdpovediSoubory { get; internal set; }

        /// <summary>
        /// Vraci odpoved na otazku jako string naformatovany pro ulozeni do databaze
        /// </summary>
        /// <param name="value">Odpoved na otazku</param>
        /// <returns>varchar pro ulozeni do databaze</returns>
        public string CheckAnswerValue(ref string value)
        {
            switch (this.ReplyControl)
            {
                case BO.ReplyKeyEnum.Checkbox:
                case BO.ReplyKeyEnum.RadiobuttonList:
                case BO.ReplyKeyEnum.Listbox:
                    return value == "true" || value == CheckboxTrueValue ? CheckboxTrueValue : CheckboxFalseValue;

                case BO.ReplyKeyEnum.DropdownList:
                    return CheckboxTrueValue;

                case BO.ReplyKeyEnum.TextBox:
                    if (string.IsNullOrEmpty(value))
                    {
                        return "";
                    }
                    else
                    {
                        /* formaty odpovedi jsou zavisle na subtypu otazky */
                        switch (this.ReplyType)
                        {
                            case BO.x24DataTypeEnum.tDecimal:
                                return Convert.ToDouble(value).ToString();
                            case BO.x24DataTypeEnum.tDate:
                            case BO.x24DataTypeEnum.tDateTime:
                                return Convert.ToDateTime(value).ConvertToBL();
                            default:
                                return value.Trim();
                        }
                    }

                default:
                    if (string.IsNullOrEmpty(value))
                        return "";
                    else
                        return value.Trim();
            }
        }

        /// <summary>
        /// Validace odpovedi na danou otazku (z hlediska datovych typu)
        /// </summary>
        /// <param name="value">Hodnota odpovedi</param>
        /// <returns>True, pokud je odpoved validni nebo se jedna o prazdnou odpoved (value="")</returns>
        public bool Validate(ref string value)
        {
            // prazdna odpoved je vzdy validni
            if (string.IsNullOrEmpty(value))
                return true;

            bool validated = true;

            switch (this.Base.ReplyControl)
            {
                case BO.ReplyKeyEnum.TextBox:
                    BO.f21ReplyUnitJoinedF19 odpoved = this.Odpovedi[0];

                    /* formaty odpovedi jsou zavisle na subtypu otazky */
                    switch (this.Base.ReplyType)
                    {
                        case BO.x24DataTypeEnum.tDecimal:
                            double x1;
                            validated = double.TryParse(value, out x1);
                            if (validated)
                            {
                                // pokud se ma vstup validovat - min a max hodnota
                                if (!string.IsNullOrEmpty(odpoved.f21MinValue))
                                    validated = x1 >= Convert.ToDouble(odpoved.f21MinValue);
                                if (!string.IsNullOrEmpty(odpoved.f21MaxValue) && validated)
                                    validated = x1 <= Convert.ToDouble(odpoved.f21MaxValue);
                            }
                            break;

                        case BO.x24DataTypeEnum.tInteger:
                            int x2;
                            validated = int.TryParse(value, out x2);
                            if (validated)
                            {
                                // pokud se ma vstup validovat - min a max hodnota
                                if (!string.IsNullOrEmpty(odpoved.f21MinValue))
                                    validated = x2 >= Convert.ToInt32(odpoved.f21MinValue);
                                if (!string.IsNullOrEmpty(odpoved.f21MaxValue) && validated)
                                    validated = x2 <= Convert.ToInt32(odpoved.f21MaxValue);
                            }
                            break;

                        case BO.x24DataTypeEnum.tDate:
                        case BO.x24DataTypeEnum.tDateTime:
                            DateTime x3;
                            validated = DateTime.TryParse(value, out x3);
                            if (validated)
                            {
                                // pokud se ma vstup validovat - min a max hodnota
                                if (!string.IsNullOrEmpty(odpoved.f21MinValue))
                                    validated = x3 >= odpoved.f21MinValue.ConvertFromBL();
                                if (!string.IsNullOrEmpty(odpoved.f21MaxValue) && validated)
                                    validated = x3 <= odpoved.f21MaxValue.ConvertFromBL();
                            }
                            break;
                    }
                    break;
            }

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
            // readonly otazka
            if (this.ReadOnly)
                classes.Add("readOnly");
            // stat id
            if (!string.IsNullOrEmpty(this.StatID))
                classes.Add("statId_" + this.StatID);
            // otazka v preview rezimu
            if (this.IsPreview)
                classes.Add("preview");

            return string.Join(" ", classes.ToArray());
        }

        /// <summary>
        /// Vraci JSON string obsahujici informace o otazce
        /// </summary>
        public string GetJsonData()
        {
            StringBuilder sb = new StringBuilder("{");
            // id otazky
            sb.AppendFormat("\"f19id\":{0},", this.Base.pid);
            // druh otazky
            sb.AppendFormat("\"control\":{0}", Convert.ToInt32(this.ReplyControl));
            // pro textbox i typ prvku
            sb.AppendFormat(",\"type\":{0}", Convert.ToInt32(this.ReplyType));
            // readonly
            sb.AppendFormat(",\"readonly\":{0}", this.ReadOnly.ToString().ToLower());
            sb.Append("}");

            return sb.ToString();
        }
    }
}