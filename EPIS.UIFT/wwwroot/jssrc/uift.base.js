/* namespace UIFT */
function UIFT() { };

/* root URL webu */
UIFT.WebRoot = "";

/* predvybrana otazka po nahrani stranky/formulare */
UIFT.ActualQuestion = 0;

/* ulozene instance casto pouzivanych elementu na strance */
UIFT.Controls = {
    scrollIndicator: null,
    contentContainer: null,
    segmentsScrollbar: null
};

/* stavy odpovedi */
UIFT.AnswerState =
{
    Unchanged: 1,
    Saving: 2,
    Saved: 3
};

/* enum pro ReplyControl */
UIFT.ReplyKeyEnum =
{
    TextBox: 1,
    DropdownList: 2,
    Checkbox: 3,
    RadiobuttonList: 4,
    CheckboxList: 5,
    Button: 6,
    FileUpload: 7,
    HtmlEditor: 8,
    SummaryOverview: 9,
    EvalList: 10,
    ChessBoard: 21,
    BatteryBoard: 22
}

/* enum pro ReplyType textboxu */
UIFT.DataTypeEnum =
{
    tNone: 0,
    tInteger: 1,
    tString: 2,
    tDecimal: 3,
    tDate: 4,
    tDateTime: 5,
    tTime: 6,
    tBoolean: 7
};