Friend Class EvalFunctions
    ' Funkce zde mohou být použity ve výrazu evaluátoru
    ' Vstupní parametry a návratové hodnoty mohou být pouze:
    '     DOUBLE (do not use integer !)
    '     DATETIME
    '     BOOLEAN
    Private _a11ID As Integer
    Private _Factory As BL.Factory = Nothing

    Private _ServiceUser As BO.RunningUser


    Public Sub New(Factory As BL.Factory, intA11ID As Integer)
        _a11ID = intA11ID
        _Factory = Factory
        _ServiceUser = Factory.CurrentUser
        '_Factory = New BL.Factory(ServiceUser, , Nothing, Nothing, Nothing)

    End Sub

    Function now() As DateTime
        Return Microsoft.VisualBasic.Now
    End Function

    Function today() As DateTime
        Return Microsoft.VisualBasic.Today
    End Function

    Function rnd() As Double
        Microsoft.VisualBasic.Randomize()
        Return CDbl(CInt(Microsoft.VisualBasic.Rnd() * 100))
    End Function

    Dim mMatter As New Matter

    Function matter() As Matter
        Return mMatter
    End Function

    'Function mid(ByVal str As String, ByVal index As Double) As String
    '    Return Microsoft.VisualBasic.Mid(str, CInt(index))
    'End Function

    Function mid(ByVal str As String, ByVal index As Double, len As Double) As String
        Return Microsoft.VisualBasic.Mid(str, CInt(index), CInt(len))
    End Function

    Function len(ByVal str As String) As Double
        Return Microsoft.VisualBasic.Len(str)
    End Function

    Function trim(ByVal str As String) As String
        Return Microsoft.VisualBasic.Trim(str)
    End Function

    Function ifn(ByVal cond As Boolean, ByVal TrueValue As Double, ByVal FalseValue As Double) As Double
        If cond Then
            Return TrueValue
        Else
            Return FalseValue
        End If
    End Function

    Function ifd(ByVal cond As Boolean, ByVal TrueValue As Date, ByVal FalseValue As Date) As Date
        If cond Then
            Return TrueValue
        Else
            Return FalseValue
        End If
    End Function

    Function ifs(ByVal cond As Boolean, ByVal TrueValue As String, ByVal FalseValue As String) As String
        If cond Then
            Return TrueValue
        Else
            Return FalseValue
        End If
    End Function
    Function ifb(ByVal cond As Boolean) As Boolean
        If cond Then
            Return True
        Else
            Return False
        End If
    End Function
    Function ifb_in(strExpression As String, strValues As String) As Boolean
        If trim(strValues) = "" Then Return False
        Dim a() As String = Split(strValues, ",")
        For i As Integer = 0 To UBound(a)
            If strExpression = trim(a(i)) Then Return True
        Next
        Return False
    End Function
    Function ifb_regex(strExpression As String, strPattern As String) As Boolean
        Dim r As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex(strPattern)
        Return r.IsMatch(strExpression)

    End Function

    Function format(ByVal value As Object, ByVal style As String) As String
        Return Microsoft.VisualBasic.Format(value, style)
    End Function

    Function ucase(ByVal str As String) As String
        Return Microsoft.VisualBasic.UCase(str)
    End Function

    Function lcase(ByVal str As String) As String
        Return Microsoft.VisualBasic.LCase(str)
    End Function

    Function wcase(ByVal str As String) As String
        If len(str) = 0 Then Return ""
        Return Microsoft.VisualBasic.UCase(str.Substring(0, 1)) &
              Microsoft.VisualBasic.LCase(str.Substring(1))
    End Function

    Function [dateserial](ByVal year As Double, ByVal month As Double, ByVal day As Double) As Date
        Return New Date(CInt(year), CInt(month), CInt(day))
    End Function
    Function year(dat As Date) As Double
        Return Microsoft.VisualBasic.Year(dat)
    End Function
    Function month(dat As Date) As Double
        Return Microsoft.VisualBasic.Month(dat)
    End Function
    Function day(dat As Date) As Double
        Return Microsoft.VisualBasic.Day(dat)
    End Function

    Function int(ByVal value As String) As Double
        Return CInt(value)
    End Function
    Function dbl(ByVal value As String) As Double
        Return CDbl(value)
    End Function

    Function round(ByVal value As Double, digits As Double) As Double
        Return Math.Round(value, CInt(digits))
    End Function

    Function [left](ByVal str As String, x As Double) As String
        Return Microsoft.VisualBasic.Left(str, CInt(x))
    End Function


    Function rvs(f19ID As Double, Optional f21id As Double = 0, Optional strDB As String = "") As String
        Return replyvalue(f19ID, f21id, strDB)
    End Function
    Function rvd(f19ID As Double, Optional strDB As String = "") As Date
        Dim s As String = replyvalue(f19ID, 0, strDB)
        If s = "" Then Return dateserial(1900, 1, 1)

        If IsDate(s) Then
            Return CDate(s)
        Else
            Return dateserial(1900, 1, 1)
        End If
    End Function
    Function rvn(f19ID As Double, Optional strDB As String = "") As Double
        Dim s As String = replyvalue(f19ID, 0, strDB)
        If IsNumeric(s) Then
            Return CDbl(s)
        Else
            Return 0
        End If
    End Function

    Function replyvalue(f19id As Double, Optional f21id As Double = 0, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.f32FilledValueBL.ChangeDB(strDB)
        Dim cRec As BO.f32FilledValue = _Factory.f32FilledValueBL.Load(_a11ID, CInt(f19id), CInt(f21id))

        If Not cRec Is Nothing Then
            Return cRec.Value & ""
        Else
            Return ""
        End If
    End Function
    Function replyvalue_difform_ce(f19id As Double, f06id As Double, Optional f21id As Double = 0, Optional strDB As String = "") As String
        Return replyvalue_difform(f19id, f06id, f21id, strDB, True)
    End Function
    Function replyvalue_difform(f19id As Double, f06id As Double, Optional f21id As Double = 0, Optional strDB As String = "", Optional bolCurrentEvent As Boolean = False) As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If strDB <> "" Then _Factory.a01EventBL.ChangeDB(strDB)
        Dim cA01 As BO.a01Event = _Factory.a01EventBL.Load(cA11.a01ID)
        If cA01 Is Nothing Then Return ""
        Dim intA03ID As Integer = cA01.a03ID

        Dim mq As New BO.myQuery("a11")
        mq.f06id = CInt(f06id)
        If bolCurrentEvent Then
            mq.a01id = cA11.a01ID   'hledá se pouze v aktuální akci
        Else
            mq.a03id = intA03ID     'hledá se ve všech akcích dané školy
        End If

        Dim lisA11 As IEnumerable(Of BO.a11EventForm) = _Factory.a11EventFormBL.GetList(mq).OrderByDescending(Function(p) p.PID)
        If lisA11.Count > 0 Then
            Dim intA11ID As Integer = lisA11(0).pid  'a11id se bere to poslední - nejnovější
            If strDB <> "" Then _Factory.f32FilledValueBL.ChangeDB(strDB)
            Dim cRec As BO.f32FilledValue = _Factory.f32FilledValueBL.Load(intA11ID, CInt(f19id), CInt(f21id))
            If Not cRec Is Nothing Then
                Return cRec.Value & ""
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Function replyalias(f19id As Double, Optional f21id As Double = 0, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.f32FilledValueBL.ChangeDB(strDB)
        Dim cRec As BO.f32FilledValue = _Factory.f32FilledValueBL.Load(_a11ID, CInt(f19id), CInt(f21id))
        If Not cRec Is Nothing Then
            Select Case cRec.f23ID
                Case 2, 4, 3, 5, 6
                    Return cRec.f21Name
                Case Else
                    Return cRec.Value & ""
            End Select
        Else
            Return ""
        End If
    End Function

    Function replyaliases(f19id As Double, strDelimiter As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.a11EventFormBL.ChangeDB(strDB)
            _Factory.f21ReplyUnitBL.ChangeDB(strDB)
        End If
        Dim mq As New BO.myQuery("f32")
        mq.f19id = f19id
        mq.a11id = _a11ID
        mq.HiddenQuestions = BO.BooleanQueryMode.FalseQuery
        Dim lis As IEnumerable(Of BO.f32FilledValue) = _Factory.f32FilledValueBL.GetList(mq), s As New List(Of String)
        lis = lis.OrderBy(Function(f32 As BO.f32FilledValue) f32.f21Name)
        lis = lis.OrderBy(Function(f32 As BO.f32FilledValue) _Factory.f21ReplyUnitBL.Load(f32.f21ID).f21Ordinal)
        For Each c In lis
            Select Case c.f23ID
                Case 2, 4, 3, 5, 6
                    If c.Value = "1" Then s.Add(c.f21Name)
                Case Else
                    s.Add(c.Value)
            End Select
        Next
        Return String.Join(strDelimiter, s)
    End Function

    Function replycomment(f19id As Double, Optional f21id As Double = 0, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.f32FilledValueBL.ChangeDB(strDB)

        Dim cRec As BO.f32FilledValue = _Factory.f32FilledValueBL.Load(_a11ID, CInt(f19id), CInt(f21id))
        If Not cRec Is Nothing Then
            Return cRec.f32Comment & ""
        Else
            Return ""
        End If
    End Function
    Function replycomments(f19id As Double, strDelimiter As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.a11EventFormBL.ChangeDB(strDB)
            _Factory.f21ReplyUnitBL.ChangeDB(strDB)
        End If
        Dim mq As New BO.myQuery("f32")
        mq.f19id = f19id
        mq.a11id = _a11ID
        mq.HiddenQuestions = BO.BooleanQueryMode.FalseQuery
        Dim lis As IEnumerable(Of BO.f32FilledValue) = _Factory.f32FilledValueBL.GetList(mq), s As New List(Of String)
        lis = lis.OrderBy(Function(f32 As BO.f32FilledValue) f32.f21Name)
        lis = lis.OrderBy(Function(f32 As BO.f32FilledValue) _Factory.f21ReplyUnitBL.Load(f32.f21ID).f21Ordinal)
        For Each c In lis
            s.Add(c.f32Comment)
        Next
        Return String.Join(strDelimiter, s)
    End Function

    Function f19question_s(f19id As Double, strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.f19QuestionBL.ChangeDB(strDB)
        Dim cF19 As BO.f19Question = _Factory.f19QuestionBL.Load(CInt(f19id))
        If cF19 Is Nothing Then Return ""

        Dim o As Object = BO.Reflexe.GetPropertyValue(cF19, strPropertyName)
        Return o.ToString
    End Function
    Function f18formsegment_s(f19id As Double, strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.f19QuestionBL.ChangeDB(strDB)
            _Factory.f18FormSegmentBL.ChangeDB(strDB)
        End If
        Dim cF19 As BO.f19Question = _Factory.f19QuestionBL.Load(CInt(f19id))
        If cF19 Is Nothing Then Return ""
        Dim cF18 As BO.f18FormSegment = _Factory.f18FormSegmentBL.Load(cF19.f18ID)

        Dim o As Object = BO.Reflexe.GetPropertyValue(cF18, strPropertyName)
        Return o.ToString
    End Function
    Function f06form_s(f19id As Double, strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.f19QuestionBL.ChangeDB(strDB)
            _Factory.f18FormSegmentBL.ChangeDB(strDB)
            _Factory.f06FormBL.ChangeDB(strDB)
        End If
        Dim cF19 As BO.f19Question = _Factory.f19QuestionBL.Load(CInt(f19id))
        If cF19 Is Nothing Then Return ""
        Dim cF18 As BO.f18FormSegment = _Factory.f18FormSegmentBL.Load(cF19.f18ID)
        Dim cF06 As BO.f06Form = _Factory.f06FormBL.Load(cF18.f06ID)

        Dim o As Object = BO.Reflexe.GetPropertyValue(cF06, strPropertyName)
        Return o.ToString
    End Function

    Function a37institutiondepartment_s(strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a37ID = 0 Then Return ""
        Select Case lcase(strPropertyName)
            Case "a37izo" : Return cA11.a37IZO
            Case "a37name" : Return cA11.a37Name
            Case Else
                If strDB <> "" Then _Factory.a37InstitutionDepartmentBL.ChangeDB(strDB)
                Dim cA37 As BO.a37InstitutionDepartment = _Factory.a37InstitutionDepartmentBL.Load(cA11.a37ID)
                Dim o As Object = BO.Reflexe.GetPropertyValue(cA37, strPropertyName)
                Return o.ToString
        End Select
    End Function
    Function a17departmenttype_s(strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""

        If cA11.a17ID = 0 Then Return ""
        If strDB <> "" Then _Factory.a17DepartmentTypeBL.ChangeDB(strDB)
        Dim cA17 As BO.a17DepartmentType = _Factory.a17DepartmentTypeBL.Load(cA11.a17ID)

        Dim o As Object = BO.Reflexe.GetPropertyValue(cA17, strPropertyName)
        Return o.ToString
    End Function
    Function a17_ident(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a17ID = 0 Then Return ""
        If strDB <> "" Then _Factory.a17DepartmentTypeBL.ChangeDB(strDB)
        Dim cA17 As BO.a17DepartmentType = _Factory.a17DepartmentTypeBL.Load(cA11.a17ID)
        Return cA17.a17UIVCode
    End Function
    Public Function a09_ident(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)

        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""
        If strDB <> "" Then _Factory.a03InstitutionBL.ChangeDB(strDB)
        Dim cA03 As BO.a03Institution = _Factory.a03InstitutionBL.Load(cA11.a03ID)
        Return cA03.a09UIVCode
    End Function

    Function a10_ident(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If strDB <> "" Then _Factory.a10EventTypeBL.ChangeDB(strDB)
        Dim cRec As BO.a10EventType = _Factory.a10EventTypeBL.Load(cA11.a10ID)
        Return cRec.a10Ident
    End Function
    Function a08_ident(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If strDB <> "" Then _Factory.a08ThemeBL.ChangeDB(strDB)
        Dim cRec As BO.a08Theme = _Factory.a08ThemeBL.Load(cA11.a08ID)
        Return cRec.a08Ident
    End Function
    Function b02_ident(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If strDB <> "" Then _Factory.b02WorkflowStatusBL.ChangeDB(strDB)
        Dim cRec As BO.b02WorkflowStatus = _Factory.b02WorkflowStatusBL.Load(cA11.b02ID)
        Return cRec.b02Ident
    End Function

    Function a03institution_s(strPropertyName As String, Optional bolWorkWithFounder As Boolean = False, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""

        If strDB <> "" Then _Factory.a03InstitutionBL.ChangeDB(strDB)
        Dim cA03 As BO.a03Institution = _Factory.a03InstitutionBL.Load(cA11.a03ID)
        If bolWorkWithFounder Then
            'pracovat se zřizovatelem
            If cA03.a03ID_Founder <> 0 Then
                cA03 = _Factory.a03InstitutionBL.Load(cA03.a03ID_Founder)
            Else
                Return ""
            End If
        End If

        Dim o As Object = BO.Reflexe.GetPropertyValue(cA03, strPropertyName)
        Return o.ToString
    End Function

    Function teacher_s(strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.k01ID = 0 Then Return ""

        If strDB <> "" Then _Factory.k01TeacherBL.ChangeDB(strDB)
        Dim cK01 As BO.k01Teacher = _Factory.k01TeacherBL.Load(cA11.k01ID)
        If cK01 Is Nothing Or cA11.k01ID = 0 Then
            If lcase(strPropertyName) = "k01pid" Then Return cA11.a11TeacherPID
            Return "????"
        End If

        Select Case left(lcase(strPropertyName), 3)
            Case "k02"
                Dim cK02 As BO.k02TeacherSchool = _Factory.k01TeacherBL.LoadK02(cK01.PID, cA11.a03ID)
                If cK02 Is Nothing Then Return "?????"
                If lcase(strPropertyName) = "k02validfrom" Then Return format(cK02.ValidFrom, "dd.MM.yyyy")
                If lcase(strPropertyName) = "k02validuntil" Then Return format(cK02.ValidUntil, "dd.MM.yyyy")

                Return BO.Reflexe.GetPropertyValue(cK02, strPropertyName).ToString
            Case "k01"
                Dim o As Object = BO.Reflexe.GetPropertyValue(cK01, strPropertyName)
                Return o.ToString
            Case Else
                Return ""
        End Select

    End Function
    Function teacher_validfrom() As DateTime
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return Nothing
        If cA11.k01ID = 0 Then Return Nothing

        Dim cK01 As BO.k01Teacher = _Factory.k01TeacherBL.Load(cA11.k01ID)
        Dim cK02 As BO.k02TeacherSchool = _Factory.k01TeacherBL.LoadK02(cK01.PID, cA11.a03ID)
        If cK02 Is Nothing Then Return Nothing
        Return cK02.ValidFrom
    End Function
    Function teacher_validuntil() As DateTime
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return Nothing
        If cA11.k01ID = 0 Then Return Nothing

        Dim cK01 As BO.k01Teacher = _Factory.k01TeacherBL.Load(cA11.k01ID)
        Dim cK02 As BO.k02TeacherSchool = _Factory.k01TeacherBL.LoadK02(cK01.PID, cA11.a03ID)
        If cK02 Is Nothing Then Return Nothing
        Return cK02.ValidUntil
    End Function
    Function a01event_s(strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.a11EventFormBL.ChangeDB(strDB)
            _Factory.a01EventBL.ChangeDB(strDB)
        End If
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        Dim cA01 As BO.a01Event = _Factory.a01EventBL.Load(cA11.a01ID)
        If cA01 Is Nothing Then Return ""

        Dim o As Object = BO.Reflexe.GetPropertyValue(cA01, strPropertyName)
        Return o.ToString
    End Function
    Function replysummary(f19id As Double, strCommentDelimiter As String, strReplyDelimiter As String, Optional strDB As String = "") As String
        If strDB <> "" Then
            _Factory.f32FilledValueBL.ChangeDB(strDB)
            _Factory.f21ReplyUnitBL.ChangeDB(strDB)
        End If
        Dim mq As New BO.myQuery("f32")
        mq.f19id = f19id
        mq.a11id = _a11ID
        mq.HiddenQuestions = BO.BooleanQueryMode.FalseQuery
        Dim f32lis As IEnumerable(Of BO.f32FilledValue) = _Factory.f32FilledValueBL.GetList(mq)
        Dim s As New List(Of String)
        f32lis = f32lis.OrderBy(Function(f32 As BO.f32FilledValue) f32.f21Name)
        f32lis = f32lis.OrderBy(Function(f32 As BO.f32FilledValue) _Factory.f21ReplyUnitBL.Load(f32.f21ID).f21Ordinal)
        For Each c In f32lis
            Select Case c.f23ID
                Case 2, 4, 3, 5, 6
                    If c.Value = "1" Then
                        If String.IsNullOrWhiteSpace(c.f32Comment) Then
                            s.Add(c.f21Name)
                        Else
                            s.Add(c.f21Name & strCommentDelimiter & c.f32Comment)
                        End If
                    End If
                Case Else
                    If String.IsNullOrWhiteSpace(c.f32Comment) Then
                        s.Add(c.f21Name)
                    Else
                        s.Add(c.f21Name & strCommentDelimiter & c.f32Comment)
                    End If
            End Select
        Next
        Return String.Join(strReplyDelimiter, s)
    End Function
    Function a03institutionfounder_s(strPropertyName As String, Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)

        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""

        If strDB <> "" Then _Factory.a03InstitutionBL.ChangeDB(strDB)
        Dim cA03 As BO.a03Institution = _Factory.a03InstitutionBL.Load(cA11.a03ID)
        Dim founder = _Factory.a03InstitutionBL.Load(cA03.a03ID_Founder)
        Dim o As Object = BO.Reflexe.GetPropertyValue(founder, strPropertyName)
        If Not o Is Nothing Then
            Return o.ToString
        End If
        Return ""
    End Function
    Function isvalidico(text As String) As Boolean
        If text.Length <> 8 Then
            Return False
        End If
        Dim digit(7) As Integer
        Dim sum As Integer = 0
        For i As Integer = 0 To 7
            If Not Integer.TryParse(text(i), digit(i)) Then
                Return False
            End If
        Next
        For i As Integer = 0 To 6
            sum = sum + digit(i) * (8 - i)
        Next
        Dim remainder As Integer = sum Mod 11
        Dim c As Integer
        If remainder = 0 Or remainder = 10 Then
            c = 1
        ElseIf remainder = 1 Then
            c = 0
        Else
            c = 11 - remainder
        End If
        Return digit(7) = c
    End Function

    Function username() As String
        Return _ServiceUser.FullName
    End Function
    Function userid() As String
        Return _ServiceUser.j03Login
    End Function
    Function useremail() As String
        Return _ServiceUser.j02Email
    End Function

    Function evallist_enum1(strValueList As String) As String
        Dim a() As String = Split(strValueList, "|")
        For i As Integer = 0 To UBound(a)
            a(i) = a(i) & ";" & a(i)
        Next
        Return String.Join("|", a)
    End Function

    Function evallist_enum2(strValueAndTextList As String) As String
        Return strValueAndTextList
    End Function

    Function evallist_a37(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""
        Dim mq As New BO.myQuery("a37")
        mq.a03id = cA11.a03ID
        Dim lisA37 As IEnumerable(Of BO.a37InstitutionDepartment) = _Factory.a37InstitutionDepartmentBL.GetList(mq)
        If lisA37.Count = 0 Then Return ""
        Dim lisRet As New List(Of String)
        For Each c In lisA37
            lisRet.Add(c.pid.ToString & ";" & c.IzoWithName)

        Next

        Return String.Join("|", lisRet)
    End Function
    Function evallist_a18(Optional strDB As String = "") As String
        If strDB <> "" Then _Factory.a11EventFormBL.ChangeDB(strDB)
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""

        Dim mq As New BO.myQuery("a19")
        mq.a03id = cA11.a03ID

        Dim lisA19 As IEnumerable(Of BO.a19DomainToInstitutionDepartment) = _Factory.a19DomainToInstitutionDepartment.GetList(mq)
        If lisA19.Count = 0 Then Return ""
        Dim lisRet As New List(Of String)
        For Each c In lisA19.Select(Function(p) p.a18Code).Distinct
            Dim strA18Code As String = c
            Dim strA18Name As String = lisA19.Where(Function(p) p.a18Code = strA18Code)(0).a18Name
            lisRet.Add(strA18Code & " " & strA18Name & ";" & strA18Code & " " & strA18Name)
        Next

        Return String.Join("|", lisRet)
    End Function

    Function evallist_school_classes(strSchoolYear As String) As String
        If strSchoolYear = "" Then strSchoolYear = "2015/2016"
        Dim cA11 As BO.a11EventForm = _Factory.a11EventFormBL.Load(_a11ID)
        If cA11 Is Nothing Then Return ""
        If cA11.a03ID = 0 Then Return ""
        Dim cRec As BO.a03Institution = _Factory.a03InstitutionBL.Load(cA11.a03ID)
        Dim lis As IEnumerable(Of BO.iSETSchoolClass) = _Factory.FBL.GetList_SchoolClasses(cRec.a03REDIZO, strSchoolYear)
        If lis.Count = 0 Then Return ""

        Dim lisRet As New List(Of String)
        For Each c In lis
            lisRet.Add(c.ClassID.ToString & ";" & c.ClassName)
        Next
        Return String.Join("|", lisRet)
    End Function
End Class

Public Class Matter
    Function DateOpened() As DateTime
        Return #1/2/2003#
    End Function

    Function UnpaidBill() As Double
        Return 123456.78
    End Function



End Class

