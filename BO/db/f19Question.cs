using System.ComponentModel.DataAnnotations;


namespace BO
{
    public enum ReplyKeyEnum
    {
        TextBox = 1,
        Checkbox = 3,
        DropdownList = 2,
        RadiobuttonList = 4,
        Listbox = 5,
        Button = 6,
        FileUpload = 7,
        HtmlEditor = 8,
        SummaryOverview = 9,
        EvalList = 10,
        ChessBoard = 21,
        BatteryBoard = 22,
    }
    public enum x24DataTypeEnum
    {
        tInteger = 1,
        tString = 2,
        tDecimal = 3,
        tDate = 4,
        tDateTime = 5,
        tTime = 6,
        tBoolean = 7,
        tNone = 0
    }
    public enum f19PortalPublishFlagENUM
    {
        None = 0,
        PublishWithSchoolApprove = 1,
        PublishAlways = 2
    }


    public class f19Question : BaseBO
    {
        [Key]
        public int f19ID { get; set; }
        public int f18ID { get; set; }
        public int f23ID { get; set; }
        public int x24ID { get; set; }
        public int f25ID { get; set; }
        public int f26ID { get; set; }
        public int f27ID { get; set; }
        public int f29ID { get; set; }
        public int f44ID { get; set; }
        public int f19TemplateID { get; set; }

        public string f19Name { get; set; }
        public string f19SupportingText { get; set; }
        public string f19Hint { get; set; }
        public string f19Ident { get; set; }
        public string f19UC { get; set; }

        public f19PortalPublishFlagENUM f19PortalPublishFlag { get; set; }
        public bool f19IsSearchablePortalField { get; set; }
        public int f19Ordinal { get; set; }

        public bool f19IsMultiselect { get; set; }
        public bool f19IsHorizontalDisplay { get; set; }
        public bool f19IsRequired { get; set; }

        public string f19ReadonlyExpression { get; set; }
        public string f19SkipExpression { get; set; }
        public string f19DefaultValue { get; set; }
        public bool f19IsDefaultValueHTML { get; set; }
        public string f19RequiredExpression { get; set; }
        public string f19StatID { get; set; }
        public string f19CancelValidateExpression { get; set; }
        public string f19CancelValidateExpression_Message { get; set; }

        public string f19LinkURL { get; set; }
        public string f19AllowedFileUploadExtensions { get; set; }
        public int f19MaxUploadFiles { get; set; }


        public int f19ChessRow { get; set; }
        public int f19ChessColumn { get; set; }

        public int f19CHLMaxAnswers { get; set; }
        public string f19Regex { get; set; }
        public string f19EvalListSource { get; set; }       //nějaká eval funkce, která vrací výstup ve struktuře id1;text1|id2;text2|id3;text3 ** příklad: evallist_enum1("1;jednička|2;dvojka|3;trojka")
        public string f19LinkerValue { get; set; }
        public string f19EntityField { get; set; }
        public bool f19IsEncrypted { get; set; }
        public int f19MaxAllowedSize { get; set; }
        public bool f19IsTextboxMultiline { get; set; }


        public string f23Name { get; set; } //combo
        public string x24Name { get; set; } //combo
        public string f18Name { get; set; } //combo
        public string f25Name { get; set; } //combo
        public string f26Name { get; set; } //combo
        public string f29Name { get; set; }//combo
        public string f44Name { get; set; }//combo
        public int f06ID;
        public string f06Name;


        public string TextBox_MinValue { get; set; }    //Load a GetList se načítá bool volbou
        public string TextBox_MaxValue { get; set; }    //Load a GetList se načítá bool volbou
        public string TextBox_ExportValue { get; set; } //Load a GetList se načítá bool volbou

        public x24DataTypeEnum ReplyType
        {
            get
            {
                switch (this.x24ID)
                {
                    case 1 :
                        return x24DataTypeEnum.tInteger;
                    case 2:
                        return x24DataTypeEnum.tString;
                    case 3:
                        return x24DataTypeEnum.tDecimal;
                    case 4:
                        return x24DataTypeEnum.tDate;
                    case 5:
                        return x24DataTypeEnum.tDateTime;
                    case 6:
                        return x24DataTypeEnum.tTime;
                    case 7:
                      return x24DataTypeEnum.tBoolean;
                    default:
                        return x24DataTypeEnum.tNone;
                }
            }
        }
        public ReplyKeyEnum ReplyControl
        {
            get
            {
                switch (this.f23ID)
                {
                    case 1: return ReplyKeyEnum.TextBox;
                    case 3: return ReplyKeyEnum.Checkbox;
                    case 2: return ReplyKeyEnum.DropdownList;
                    case 4: return ReplyKeyEnum.RadiobuttonList;
                    case 5: return ReplyKeyEnum.Listbox;
                    case 6: return ReplyKeyEnum.Button;
                    case 7: return ReplyKeyEnum.FileUpload;
                    case 8: return ReplyKeyEnum.HtmlEditor;
                    case 9: return ReplyKeyEnum.SummaryOverview;
                    case 10: return ReplyKeyEnum.EvalList;
                    default:
                        return ReplyKeyEnum.TextBox;
                }
            }
        }
        public string f21Name
        {
            get
            {
                switch (this.f23ID)
                {
                    case 1: return "textbox";
                    case 3: return "checkbox";                   
                    case 7: return "fileupload";
                    case 8: return "htmleditor";
                    case 9: return "summaryoverview";
                    case 10: return "evallist";
                    default:
                        return null;
                }
            }
        }
        public string Icon
        {
            get
            {
                switch (this.ReplyControl)
                {
                    case ReplyKeyEnum.DropdownList:
                        return "type_combo.png";
                    case ReplyKeyEnum.EvalList:
                        return "type_evallist.png";
                    case ReplyKeyEnum.Listbox:
                        return "type_listbox.png";
                    case ReplyKeyEnum.RadiobuttonList:
                        return "type_radio.png";
                    case ReplyKeyEnum.Checkbox:
                        if (this.f19IsMultiselect)
                        {
                            return "type_checkboxlist.png";
                        }
                        else
                        {
                            return "type_checkbox.png";
                        }

                    case ReplyKeyEnum.FileUpload:
                        return "type_upload.png";
                    case ReplyKeyEnum.Button:
                        return "type_button.png";
                    case ReplyKeyEnum.HtmlEditor:
                        return "type_richtext.png";
                    case ReplyKeyEnum.SummaryOverview:
                        return "type_summary.png";
                    case ReplyKeyEnum.TextBox:
                        switch (this.ReplyType)
                        {
                            case x24DataTypeEnum.tDate:
                                return "type_date.png";
                            case x24DataTypeEnum.tDateTime:
                                return "type_datetime.png";
                            case x24DataTypeEnum.tTime:
                                return "type_time.png";
                            case x24DataTypeEnum.tInteger:
                            case x24DataTypeEnum.tDecimal:
                                return "type_number.png";
                            default:
                                return "type_text.png";
                        }
                    default:
                        return "type_text.png";
                }
            }
        }


    }
}
