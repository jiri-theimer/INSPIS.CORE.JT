using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DL
{
    public class basQuery
    {
        private static void ParseJ73Query(ref List<DL.QueryRow> lis,BO.myQuery mq)
        {
            int x = 0;string ss = "";string strField = "";string strAndOrZleva = "";
            if (mq.lisJ73.Count() > 0)
            {
                mq.lisJ73.First().j73BracketLeft += "(";
                mq.lisJ73.Last().j73BracketRight += ")";
            }
            foreach (var c in mq.lisJ73)
            {                                
                x += 1;
                ss = x.ToString();
                strField = c.j73Column;
                if (c.FieldSqlSyntax != null)
                {
                    strField = c.FieldSqlSyntax;
                }
                strAndOrZleva = c.j73Op;
                
                switch (c.j73Operator)
                {
                    case "ISNULL":
                        AQ(ref lis, strField + " IS NULL","",null, strAndOrZleva, c.j73BracketLeft,c.j73BracketRight);
                        break;
                    case "NOT-ISNULL":
                        AQ(ref lis, strField + " IS NOT NULL", "", null, strAndOrZleva, c.j73BracketLeft,c.j73BracketRight);
                        break;
                    case "GREATERZERO":
                        AQ(ref lis, "ISNULL("+strField + ",0)>0", "", null, strAndOrZleva, c.j73BracketLeft,c.j73BracketRight);
                        break;
                    case "ISNULLORZERO":
                        AQ(ref lis, "ISNULL(" + strField + ",0)=0", "", null, strAndOrZleva, c.j73BracketLeft,c.j73BracketRight);
                        break;
                    case "CONTAINS":
                        //AQ(ref lis, strField + " LIKE '%'+@expr" + ss + "+'%'", "expr" + ss, c.j73Value, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        AQ(ref lis, strField + " LIKE '%" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                       
                        break;
                    case "STARTS":
                        //AQ(ref lis, strField + " LIKE @expr" + ss + "+'%'", "expr" + ss, c.j73Value, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        AQ(ref lis, strField + " LIKE '" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);

                        break;
                    case "INTERVAL":
                        if (c.FieldType == "date")
                        {
                            if (c.j73DatePeriodFlag > 0)
                            {
                                c.j73Date1 = mq.lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d1;
                                c.j73Date2 = Convert.ToDateTime(mq.lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                            }
                            if (c.j73Date1 != null && c.j73Date2 != null)
                            {
                                //AQ(ref lis, c.WrapFilter(strField + " BETWEEN @dfrom" + ss + " AND @dto" + ss), "dfrom" + ss, c.j73Date1, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight, "dto" + ss, c.j73Date2);
                                AQ(ref lis, c.WrapFilter(strField + " BETWEEN "+BO.BAS.GD(c.j73Date1)+" AND "+BO.BAS.GD(c.j73Date2)),null,null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                            }
                            else
                            {
                                if (c.j73Date1 != null)
                                {
                                    //AQ(ref lis, c.WrapFilter(strField + ">=@dfrom" + ss), "dfrom" + ss, c.j73Date1, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                    AQ(ref lis, c.WrapFilter(strField + ">=" + BO.BAS.GD(c.j73Date1)),null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                                if (c.j73Date2 != null)
                                {
                                    //AQ(ref lis, c.WrapFilter(strField + "<=@dto" + ss), "dto" + ss, c.j73Date2, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                    AQ(ref lis, c.WrapFilter(strField + "<=" + BO.BAS.GD(c.j73Date2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                            }

                        }
                        if (c.FieldType == "number")
                        {
                            //AQ(ref lis, c.WrapFilter(strField + " BETWEEN @nfrom" + ss + " AND @nto" + ss), "nfrom" + ss, c.j73Num1, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight, "nto" + ss, c.j73Num2);
                            AQ(ref lis, c.WrapFilter(strField + " BETWEEN " + BO.BAS.GN(c.j73Num1) + " AND " + BO.BAS.GN(c.j73Num2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                    case "EQUAL":
                    case "NOT-EQUAL":
                        string strOper = "=";
                        if (c.j73Operator == "NOT-EQUAL")
                        {
                            strOper = "<>";
                        }
                        if (c.FieldType == "bool" || c.FieldType == "bool1")
                        {
                            AQ(ref lis, c.WrapFilter(strField + " " + strOper+ " "+c.j73Value), "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "string")
                        {
                            //AQ(ref lis, strField + " " + strOper + " @expr" + ss, "expr" + ss, c.j73Value, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                            AQ(ref lis, strField + " "+ strOper+" '" + BO.BAS.GSS(c.j73Value)+"'", null,null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "combo")
                        {
                            //AQ(ref lis, c.WrapFilter(strField + " "+ strOper+" @combo" + ss), "combo" + ss, c.j73ComboValue, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                            AQ(ref lis, c.WrapFilter(strField + " " + strOper + " " + c.j73ComboValue.ToString()), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);

                        }                        
                        if (c.FieldType == "multi")
                        {
                            strOper = "IN";
                            if (c.j73Operator == "NOT-EQUAL")
                            {
                                strOper = "NOT IN";
                            }
                            AQ(ref lis, c.WrapFilter(strField + " "+strOper+" (" + c.j73Value+")"), "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                }
               
            }
        }
        public static DL.FinalSqlCommand ParseFinalSql(string strPrimarySql, BO.myQuery mq, BO.RunningUser ru, bool bolPrepareParam4DT = false)
        {
            var lis = new List<DL.QueryRow>();
            if (mq.lisJ73 != null)
            {
                ParseJ73Query(ref lis, mq);
            }
            if (mq.IsRecordValid == true && mq.Prefix !="a45")
            {
                AQ(ref lis, "a." + mq.Prefix + "ValidUntil>GETDATE()", "", null);
               
            }
            

            if (mq.IsRecordValid == false)
            {
                AQ(ref lis, "GETDATE() NOT BETWEEN a." + mq.Prefix + "ValidFrom AND a." + mq.Prefix + "ValidUntil", "", null);
            }
            if (mq.global_d2 != null && Convert.ToDateTime(mq.global_d2).Hour == 0)
            {
                mq.global_d2 = Convert.ToDateTime(mq.global_d2).AddDays(1).AddMinutes(-1);
            }
            if (mq.global_d1 != null)
            {
                if (mq.Prefix == "a35")
                {
                    AQ(ref lis, "a.a35PlanDate BETWEEN @gd1 AND @gd2", "gd1", mq.global_d1, "AND", null, null, "gd2", mq.global_d2);
                }
                if (mq.Prefix == "a38")
                {
                    AQ(ref lis, "a.a38PlanDate BETWEEN @gd1 AND @gd2", "gd1", mq.global_d1, "AND", null, null, "gd2", mq.global_d2);
                }
                if (mq.Prefix == "h04")
                {
                    AQ(ref lis, "a.h04Deadline BETWEEN @gd1 AND @gd2 OR a.h04CapacityPlanFrom BETWEEN @gd1 AND @gd2 OR a.h04CapacityPlanUntil BETWEEN @gd1 AND @gd2", "gd1", mq.global_d1, "AND", null, null, "gd2", mq.global_d2);
                }
                if (mq.Prefix == "a01")
                {
                    //AQ(ref lis, "(a.a01DateFrom BETWEEN @gd1 AND @gd2 OR a.a01DateUntil BETWEEN @gd1 AND @gd2 OR (a.a01DateFrom>=@gd1 AND a.a01DateUntil<=@gd2))", "gd1", mq.global_d1, "AND", null, null, "gd2", mq.global_d2);
                    //AQ(ref lis, "a.a01DateFrom>=@gd1 AND a.a01DateUntil<=@gd2", "gd1", mq.global_d1, "AND", null, null, "gd2", mq.global_d2);
                    AQ(ref lis, "a.a01DateFrom>=@gd1", "gd1", mq.global_d1);
                    AQ(ref lis, "a.a01DateUntil<=@gd2", "gd2", mq.global_d2);
                }
            }
            if (mq.MyRecordsDisponible==true && mq.CurrentUser !=null)
            {
                if (mq.Prefix == "a41")
                {
                    AQ(ref lis, "a.j02ID=@j02id_me OR a.j11ID IN (select j11ID FROM j12Team_Person WHERE j02ID=@j02id_me OR j04ID=@j04id_me) OR a.j11ID IN (select xa.j11ID FROM j12Team_Person xa INNER JOIN a02Inspector xb ON xa.a04ID=xb.a04ID WHERE xb.j02ID=@j02id_me)", "j02id_me",mq.CurrentUser.j02ID,"AND",null,null,"j04id_me",mq.CurrentUser.j04ID);
                }
                if (mq.Prefix == "h11")
                {
                    AQ(ref lis, "a.h11IsPublic=1 OR a.h11ID IN (SELECT h11ID FROM h12NoticeBoard_Permission WHERE j04ID=@j04id_me)", "j04id_me", mq.CurrentUser.j04ID);
                }
            }
            if (mq.pids != null && mq.pids.Any())
            {
                AQ(ref lis, mq.PkField + " IN (" + String.Join(",", mq.pids) + ")", "", null);
            }
            
            if (mq.j04id > 0)
            {
                if (mq.Prefix == "a39")
                {
                    AQ(ref lis, "(a.j04ID_Explicit=@j04id OR j03.j04ID=@j04id)", "j04id", mq.j04id);
                }
                else
                {
                    AQ(ref lis, "a.j04ID=@j04id", "j04id", mq.j04id);
                }                
            }
            if (mq.j72id > 0)
            {
                if (mq.Prefix == "j04") AQ(ref lis, "a.j04ID IN (select j04ID FROM j74TheGridReceiver WHERE j72ID=@j72id)", "j72id", mq.j72id);
                if (mq.Prefix == "j11") AQ(ref lis, "a.j11ID IN (select j11ID FROM j74TheGridReceiver WHERE j72ID=@j72id)", "j72id", mq.j72id);
            }
            if (mq.DateBetween != null && mq.DateBetweenDays > 0)
            {
                AQ(ref lis, string.Format("(a.p41PlanStart BETWEEN @d1 AND DATEADD(DAY,{0},@d1) OR a.p41PlanEnd BETWEEN @d1 AND DATEADD(DAY,{0},@d1) OR @d1 BETWEEN a.p41PlanStart AND a.p41PlanEnd OR DATEADD(DAY,{0},@d1) BETWEEN a.p41PlanStart AND a.p41PlanEnd)", mq.DateBetweenDays), "d1", mq.DateBetween.Value);
            }

            if (mq.a01id > 0)
            {
                if (mq.Prefix == "b05") AQ(ref lis, "a.a01ID=@a01id", "a01id", mq.a01id);
                if (mq.Prefix == "a41" || mq.Prefix == "a11" || mq.Prefix=="a35" || mq.Prefix=="a38" || mq.Prefix=="h04") AQ(ref lis, "a.a01ID=@a01id", "a01id", mq.a01id);
                if (mq.Prefix == "a01") AQ(ref lis, "(a.a01ID IN (select a01ID_Left FROM a24EventRelation WHERE a01ID_Left=@a01id OR a01ID_Right=@a01id) OR a.a01ID IN (select a01ID_Right FROM a24EventRelation WHERE a01ID_Left=@a01id OR a01ID_Right=@a01id)) AND a.a01ID<>@a01id", "a01id", mq.a01id);   //související akce
                if (mq.Prefix == "j02")
                {
                   if (mq.param1== "without_teams_and_owner")
                    {
                        AQ(ref lis, "a.j02ID IN (select j02ID FROM a41PersonToEvent WHERE a01ID=@a01id AND a45ID<>5 AND j02ID IS NOT NULL)", "a01id", mq.a01id);
                    }
                    else
                    {
                        AQ(ref lis, "a.j02ID IN (select j02ID FROM a41PersonToEvent WHERE a01ID=@a01id AND j02ID IS NOT NULL) OR a.j02ID IN (select xb.j02ID FROM a41PersonToEvent xa INNER JOIN j12Team_Person xb ON xa.j11ID=xb.j11ID WHERE xa.a01ID=@a01id AND xa.j11ID IS NOT NULL)", "a01id", mq.a01id);
                    }
                    
                }
                if (mq.Prefix == "a25") AQ(ref lis, "a.a25ID IN (select a25ID FROM a11EventForm WHERE a01ID=@a01id)", "a01id", mq.a01id);
                if (mq.Prefix == "f06") AQ(ref lis, "a.f06ID IN (select f06ID FROM a11EventForm WHERE a01ID=@a01id)", "a01id", mq.a01id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=101 AND a.o27DataPID=@a01id", "a01id", mq.a01id);
                if (mq.Prefix == "x31")
                {
                    AQ(ref lis, "(a.x31IsAllA10IDs=1 OR a.x31ID IN (SELECT a23.x31ID FROM a01Event a01 INNER JOIN a10EventType a10 ON a01.a10ID=a10.a10ID INNER JOIN a23EventType_Report a23 ON a10.a10ID=a23.a10ID WHERE a01.a01ID=@a01id)) AND (a.x31IsAllA08IDs=1 OR a.x31ID IN (SELECT a27.x31ID FROM a01Event a01 INNER JOIN a08Theme a08 ON a01.a08ID=a08.a08ID INNER JOIN a27EventTheme_Report a27 ON a08.a08ID=a27.a08ID WHERE a01.a01ID=@a01id))", "a01id", mq.a01id);
                }

            }
            if (mq.a01parentid > 0)
            {
                if (mq.Prefix == "a01") AQ(ref lis, "a.a01ParentID=@a01parentid", "a01parentid", mq.a01parentid);   //podřízené akce
            }
            if (mq.Prefix == "a45" && mq.param1 == "a45IsManual1")
            {
                AQ(ref lis, "a.a45IsManual=1", "", null);
            }
            if (mq.Prefix == "b65" && mq.param1 !=null)
            {
                AQ(ref lis, "a.x29ID=@x29id", "x29id",BO.BAS.InInt(mq.param1));
            }

            if (mq.a03id > 0)
            {
                if (mq.Prefix == "j02") AQ(ref lis, "a.j02ID IN (select j02ID FROM a39InstitutionPerson WHERE a03ID=@a03id)", "a03id", mq.a03id);
                if (mq.Prefix == "a01" || mq.Prefix == "a37" || mq.Prefix == "a39") AQ(ref lis, "a.a03ID=@a03id", "a03id", mq.a03id);
                if (mq.Prefix == "a11") AQ(ref lis, "a11_a01.a03ID=@a03id", "a03id", mq.a03id);

                if (mq.Prefix == "a19") AQ(ref lis, "a.a37ID IN (select a37ID FROM a37InstitutionDepartment WHERE a03ID=@a03id)", "a03id", mq.a03id);
                if (mq.Prefix == "k01") AQ(ref lis, "a03_k02.a03ID=@a03id", "a03id", mq.a03id);
                if (mq.Prefix == "a29") AQ(ref lis, "a.a29ID IN (select a29ID FROM a43InstitutionToList WHERE a03ID=@a03id)", "a03id", mq.a03id);
                if (mq.Prefix == "a42") AQ(ref lis, "a.a42ID IN (select a42ID FROM a01Event WHERE a03ID=@a03id)", "a03id", mq.a03id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=101 AND a.o27DataPID IN (select a01ID FROM a01Event WHERE a03ID=@a03id)", "a03id", mq.a03id);
            }
            if (mq.a03id_founder > 0)
            {
                if (mq.Prefix == "a03") AQ(ref lis, "a.a03ID_Founder=@a03id_founder", "a03id_founder", mq.a03id_founder);
            }
            if (mq.a05id > 0)
            {
                if (mq.Prefix == "j02") AQ(ref lis, "a.j02ID IN (select j02ID FROM a02Inspector WHERE a05ID=@a05id AND GETDATE() BETWEEN a02ValidFrom AND a02ValidUntil)", "a05id", mq.a05id);
                if (mq.Prefix == "a35" || mq.Prefix=="a38") AQ(ref lis, "a.j02ID IN (SELECT xa.j02ID FROM a02Inspector xa INNER JOIN a04Inspectorate xb ON xa.a04ID=xb.a04ID WHERE xb.a05ID=@a05id)", "a05id", mq.a05id);
                if (mq.Prefix == "j23") AQ(ref lis, "a.a05ID=@a05id", "a05id", mq.a05id);

            }
            if (mq.a04id > 0)
            {
                if (mq.Prefix == "j02") AQ(ref lis, "a.j02ID IN (select j02ID FROM a02Inspector WHERE a04ID=@a04id AND GETDATE() BETWEEN a02ValidFrom AND a02ValidUntil)", "a04id", mq.a04id);
                
            }
            if (mq.j23id > 0)
            {
                if (mq.Prefix == "a38") AQ(ref lis, "a.j23ID=@j23id", "j23id", mq.j23id);
            }
            if (mq.j11id > 0)
            {
                if (mq.Prefix == "j02") AQ(ref lis, "a.j02ID IN (select j02ID FROM j12Team_Person WHERE j11ID=@j11id)", "j11id", mq.j11id);
            }
            if (mq.a17id > 0)
            {
                if (mq.Prefix == "a37") AQ(ref lis, "a.a17ID=@a17id", "a17id", mq.a17id);
            }
            if (mq.h04id > 0)
            {
                if (mq.Prefix == "j02") AQ(ref lis, "a.j02ID IN (select j02ID FROM h06ToDoReceiver WHERE h04ID=@h04id AND j02ID IS NOT NULL)", "h04id", mq.h04id);
                if (mq.Prefix == "j11") AQ(ref lis, "a.j11ID IN (select j11ID FROM h06ToDoReceiver WHERE h04ID=@h04id AND j11ID IS NOT NULL)", "h04id", mq.h04id);
            }
            if (mq.a42id > 0)
            {                
                if (mq.Prefix == "a01") AQ(ref lis, "a.a42ID=@a42id", "a42id", mq.a42id);
                if (mq.Prefix == "a03") AQ(ref lis, "a.a03ID IN (select a03ID FROM a01Event WHERE a42ID=@a42id)", "a42id", mq.a42id);
                if (mq.Prefix == "x40") AQ(ref lis, "a.x29ID=101 AND a.x40DataPid IN (select a01ID FROM a01Event WHERE a42ID=@a42id)", "a42id", mq.a42id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=142 AND a.o27DataPID=@a42id", "a42id", mq.a42id);

            }
            if (mq.b01id > 0)
            {
                if (mq.Prefix == "b06") AQ(ref lis, "a.b02ID IN (select b02ID FROM b02WorkflowStatus WHERE b01ID=@b01id)", "b01id", mq.b01id);                
                if (mq.Prefix == "b02") AQ(ref lis, "a.b01ID=@b01id", "b01id", mq.b01id);                
            }
            if (mq.b02id > 0)
            {
                AQ(ref lis, "a.b02ID=@b02id", "b02id", mq.b02id);

            }
            
            if (mq.b02ids != null && mq.b02ids.Count() > 0)
            {
                AQ(ref lis, "a.b02ID IN (select b02ID FROM b02Status WHERE b02ID IN (" + string.Join(",", mq.b02ids) + "))", "", null);
            }
            if (mq.b06id > 0)
            {
                if (mq.Prefix == "f06") AQ(ref lis, "a.f06ID IN (select f06ID FROM b13WorkflowRequiredFormsToStep WHERE b06ID=@b06id)", "b06id", mq.b06id);
                if (mq.Prefix == "o13") AQ(ref lis, "a.o13ID IN (select o13ID FROM b14WorkflowRequiredAttachmentTypeToStep WHERE b06ID=@b06id)", "b06id", mq.b06id);
                if (mq.Prefix == "j04" && mq.param1=="b12") AQ(ref lis, "a.j04ID IN (select j04ID FROM b12WorkflowReceiverToHistory WHERE j04ID IS NOT NULL AND b06ID=@b06id)", "b06id", mq.b06id);
                if (mq.Prefix == "a45" && mq.param1=="b12") AQ(ref lis, "a.a45ID IN (select a45ID FROM b12WorkflowReceiverToHistory WHERE a45ID IS NOT NULL AND b06ID=@b06id)", "b06id", mq.b06id);
            }
            if (mq.f06id > 0)
            {
                if (mq.Prefix == "j04") AQ(ref lis, "a.j04ID IN (select j04ID FROM f07Form_UserRole_EncryptedPermission WHERE f06ID=@f06id)", "f06id", mq.f06id);
                if (mq.Prefix == "x31") AQ(ref lis, "a.x31ID IN (select x31ID FROM f08Form_Report WHERE f06ID=@f06id)", "f06id", mq.f06id);
                if (mq.Prefix == "f18" || mq.Prefix=="a11") AQ(ref lis, "a.f06ID=@f06id", "f06id", mq.f06id);
                if (mq.Prefix == "f19") AQ(ref lis, "a.f18ID IN (SELECT f18ID FROM f18FormSegment WHERE f06ID=@f06id)", "f06id", mq.f06id);
                if (mq.Prefix == "f32") AQ(ref lis, "a11.f06ID=@f06id", "f06id", mq.f06id);
                if (mq.Prefix == "xx1") AQ(ref lis, "f18.f06ID=@f06id", "f06id", mq.f06id); //f21ReplyUnitJoinedF19: GetListJoinedF19
                if (mq.Prefix == "f31") AQ(ref lis, "a11.f06ID=@f06id", "f06id", mq.f06id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=406 AND a.o27DataPID=@f06id", "f06id", mq.f06id);
            }
            if (mq.f06ids != null && mq.f06ids.Count() > 0)
            {
                if (mq.Prefix == "f19") AQ(ref lis, "a.f18ID IN (SELECT f18ID FROM f18FormSegment WHERE f06ID IN ("+string.Join(",",mq.f06ids)+"))","",null);
                if (mq.Prefix == "f18") AQ(ref lis, "a.f06ID IN (" + string.Join(",", mq.f06ids) + ")", "", null);
            }

            if (mq.f32id > 0)
            {
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=432 AND a.o27DataPID=@f32id", "f32id", mq.f32id);
            }
            if (mq.f18id > 0)
            {
                if (mq.Prefix == "f19" || mq.Prefix=="f26" || mq.Prefix=="f25") AQ(ref lis, "a.f18ID=@f18id", "f18id", mq.f18id);
                if (mq.Prefix == "f32") AQ(ref lis, "f19.f18ID=@f18id", "f18id", mq.f18id);
                if (mq.Prefix == "xx1") AQ(ref lis, "f19.f18ID=@f18id", "f18id", mq.f18id); //f21ReplyUnitJoinedF19: GetListJoinedF19
                if (mq.Prefix == "f31") AQ(ref lis, "f19.f18ID=@f18id", "f18id", mq.f18id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=418 AND a.o27DataPID=@f18id", "f18id", mq.f18id);
            }
            if (mq.f21id > 0)
            {
                if (mq.Prefix == "f19") AQ(ref lis, "a.f19ID IN (SELECT f19ID FROM f20ReplyUnitToQuestion WHERE f21ID=@f21id)", "f21id", mq.f21id);
                if (mq.Prefix == "f22") AQ(ref lis, "a.f22ID IN (SELECT f22ID FROM f43ReplyUnitToSet WHERE f21ID=@f21id)", "f21id", mq.f21id);
            }
            if (mq.f22id > 0)
            {
                if (mq.Prefix == "f21" || mq.Prefix=="xx1") AQ(ref lis, "a.f21ID IN (SELECT f21ID FROM f43ReplyUnitToSet WHERE f22ID=@f22id)", "f22id", mq.f22id);
            }
            if (mq.f22id == -2) //nezařazené do šablon nebo otázek
            {
                if (mq.Prefix == "xx1") AQ(ref lis, " AND a.f21ID NOT IN (select f21ID FROM f43ReplyUnitToSet) AND f21ID NOT IN (SELECT a.f21ID FROM f20ReplyUnitToQuestion a INNER JOIN f19Question b ON a.f19ID=b.f19ID)", "",null);
            }
            if (mq.f19id > 0)
            {
                if (mq.Prefix == "f21") AQ(ref lis, "a.f21ID IN (SELECT f21ID FROM f20ReplyUnitToQuestion WHERE f19ID=@f19id)", "f19id", mq.f19id);
                if (mq.Prefix == "xx1") AQ(ref lis, "f20.f19ID=@f19id", "f19id", mq.f19id); //f21ReplyUnitJoinedF19: GetListJoinedF19
                if (mq.Prefix == "f32" || mq.Prefix=="f31") AQ(ref lis, "a.f19ID=@f19id", "f19id", mq.f19id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=419 AND a.o27DataPID=@f19id", "f19id", mq.f19id);
            }
            if (mq.f19ids !=null && mq.f19ids.Count > 0)
            {
                if (mq.Prefix == "xx1") AQ(ref lis, "f20.f19ID IN ("+string.Join(",",mq.f19ids)+")", "", null); //f21ReplyUnitJoinedF19: GetListJoinedF19

            }
            if (mq.Prefix == "f21" && mq.param1 == "search")
            {
                AQ(ref lis, "ISNULL(a.f21Name,'') NOT IN ('textbox','checkbox','fileupload','')", "", null);    //filtr neprázdných jednotek odpovědi
            }
            

            if (mq.f25id > 0)
            {
                if (mq.Prefix == "f19") AQ(ref lis, "a.f25ID=@f25id", "f25id", mq.f25id);
            }
            if (mq.f26id > 0)
            {
                if (mq.Prefix == "f19") AQ(ref lis, "a.f26ID=@f26id", "f26id", mq.f26id);
            }

            if (mq.j02id > 0)
            {
                if (mq.Prefix == "a01") AQ(ref lis, "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE j02ID=@j02id)", "j02id", mq.j02id);   //je účastníkem akce
                if (mq.Prefix == "a39" || mq.Prefix == "j03" || mq.Prefix=="a35" || mq.Prefix=="a38") AQ(ref lis, "a.j02ID=@j02id", "j02id", mq.j02id);

                if (mq.Prefix == "a03") AQ(ref lis, "a.a03ID IN (select a03ID FROM a39InstitutionPerson WHERE j02ID=@j02id)", "j02id", mq.j02id);
                if (mq.Prefix == "h04")
                {
                    if (mq.h06TodoRole > 0)
                    {
                        AQ(ref lis, "a.h04ID IN (select h04ID FROM h06ToDoReceiver WHERE j02ID=@j02id AND h06TodoRole="+mq.h06TodoRole.ToString()+")", "j02id", mq.j02id);
                    }
                    else
                    {
                        AQ(ref lis, "a.h04ID IN (select h04ID FROM h06ToDoReceiver WHERE j02ID=@j02id)", "j02id", mq.j02id);
                    }                    
                }
                
                if (mq.Prefix == "j90" || mq.Prefix == "j92") AQ(ref lis, "a.j03ID IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", mq.j02id);

                if (mq.Prefix == "x40") AQ(ref lis, "a.j03ID_Creator IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", mq.j02id);
                if (mq.Prefix == "o27") AQ(ref lis, "a.x29ID=502 AND a.o27DataPID=@j02id", "j02id", mq.j02id);

            }
            if (mq.j02id_leader > 0)
            {
                if (mq.Prefix == "a01") AQ(ref lis, "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=2 AND j02ID=@j02id_leader)", "j02id_leader", mq.j02id_leader);   //je vedoucím akce
            }
            if (mq.j02id_member > 0)
            {
                if (mq.Prefix == "a01") AQ(ref lis, "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=2 AND j02ID=@j02id_member)", "j02id_member", mq.j02id_member);   //je člen akce
            }
            if (mq.j02id_issuer > 0)
            {
                if (mq.Prefix == "a01") AQ(ref lis, "a.j02ID_Issuer=@j02id_issuer", "j02id_issuer", mq.j02id_issuer);   //je zakladatelem akce
            }
            if (mq.a10id > 0)
            {
                if (mq.Prefix == "a08") AQ(ref lis, "a.a08ID IN (SELECT a08ID FROM a26EventTypeThemeScope WHERE a10ID=@a10id)", "a10id", mq.a10id);
                if (mq.Prefix == "a01") AQ(ref lis, "a.a10ID=@a10id", "a10id", mq.a10id);
            }
            if (mq.a08id > 0)
            {
                if (mq.Prefix == "a12") AQ(ref lis, "a.a08ID=@a08id", "a08id", mq.a08id);
                if (mq.Prefix == "f06") AQ(ref lis, "a.f06ID IN (select f06ID FROM a12ThemeForm WHERE a08ID=@a08id)", "a08id", mq.a08id);
            }
            if (mq.a11id > 0)
            {
                if (mq.Prefix == "f32") AQ(ref lis, "a.a11ID=@a11id", "a11id", mq.a11id);
            }
            if (mq.Prefix=="a11" && mq.a11ispoll == BO.BooleanQueryMode.TrueQuery)
            {
                AQ(ref lis, "a.a11IsPoll=1", null, null);
            }
            if (mq.HiddenQuestions == BO.BooleanQueryMode.FalseQuery)   //vyloučit skryté otázky
            {
                AQ(ref lis, "NOT EXISTS (SELECT 1 FROM f35FilledQuestionHidden f35 WHERE a.a11ID=f35.a11ID AND a.f19ID=f35.f19ID AND f35.f35IsHidden=1)", "", null);
            }
            if (mq.HiddenQuestions == BO.BooleanQueryMode.TrueQuery)   //zobrazit pouze skryté otázky
            {
                AQ(ref lis, "EXISTS (SELECT 1 FROM f35FilledQuestionHidden f35 WHERE a.a11ID=f35.a11ID AND a.f19ID=f35.f19ID AND f35.f35IsHidden=1)", "", null);
            }

            if (mq.a06id > 0)
            {
                if (mq.Prefix == "a03") AQ(ref lis, "a.a06ID=@a06id", "a06id", mq.a06id);


            }
            if (mq.a29id > 0)
            {
                if (mq.Prefix == "a03") AQ(ref lis, "a.a03ID IN (select a03ID FROM a43InstitutionToList WHERE a29ID=@a29id)", "a29id", mq.a29id);

            }
            if (mq.f12id > 0)
            {
                if (mq.Prefix == "f06") AQ(ref lis, "a.f12ID=@f12id", "f12id", mq.f12id);
            }
            if (mq.f29id > 0)
            {
                if (mq.Prefix == "a17") AQ(ref lis, "a.a17ID IN (select a17ID FROM f41PortalQuestionTab_a17Binding WHERE f29ID=@f29id)", "f29id", mq.f29id);
            }
            if (mq.h11id > 0)
            {                
                if (mq.Prefix == "j04") AQ(ref lis, "a.j04ID IN (select j04ID FROM h12NoticeBoard_Permission WHERE h11ID=@h11id)", "h11id", mq.h11id);
            }
            if (mq.x31id > 0)
            {
                if (mq.Prefix == "x32") AQ(ref lis, "a.x32ID IN (select x32ID FROM x34Report_Category WHERE x31ID=@x31id)", "x31id", mq.x31id);
                if (mq.Prefix == "j04") AQ(ref lis, "a.j04ID IN (select j04ID FROM x37ReportRestriction_UserRole WHERE x31ID=@x31id)", "x31id", mq.x31id);
                if (mq.Prefix == "a10") AQ(ref lis, "a.a10ID IN (select a10ID FROM a23EventType_Report WHERE x31ID=@x31id)", "x31id", mq.x31id);
                if (mq.Prefix == "a08") AQ(ref lis, "a.a08ID IN (select a08ID FROM a27EventTheme_Report WHERE x31ID=@x31id)", "x31id", mq.x31id);
            }
            if (mq.Prefix == "x29")
            {
                if (mq.param1 == "x29IsAttachment") { AQ(ref lis, "a.x29IsAttachment=1", "", null); };    //filtr entit x29IsAttachment=1
                if (mq.param1 == "x29IsReport") { AQ(ref lis, "a.x29IsReport=1", "", null); };    //filtr entit x29IsReport=1
            }
            if (mq.x29id > 0)
            {
                if (mq.Prefix == "o13" || mq.Prefix=="o27" || mq.Prefix == "x31") AQ(ref lis, "a.x29ID=@x29id", "x29id", mq.x29id);
                
            }
            if (mq.recpid > 0)
            {
                if (mq.Prefix == "o27") AQ(ref lis, "a.o27DataPID=@recpid", "recpid", mq.recpid);
            }

            if (mq.o53id > 0)
            {
                if (mq.Prefix == "o51") AQ(ref lis, "a.o53ID=@o53id", "o53id", mq.o53id);
            }

            if (mq.Prefix == "b02" && mq.param1 != null)
            {
                AQ(ref lis, "a.b02Entity=@prefix", "prefix", mq.param1);    //filtr seznamu stavů podle druhu entity
            }
            if (mq.Prefix == "h05" && mq.param1 == "createtodo")
            {
                AQ(ref lis, "a.h05ID IN (1,2)", "",null);    //filtr stavů pro nově zakládaný úkol
            }
            if (mq.Prefix == "j04" && mq.param1 == "j04IsAllowInSchoolAdmin")
            {
                AQ(ref lis, "a.j04IsAllowInSchoolAdmin=1", "", null);    //filtr školních rolí podle j04IsAllowInSchoolAdmin=1
            }
            if (mq.Prefix == "j02" && mq.param1 == "j02IsInvitedPerson")
            {
                AQ(ref lis, "a.j02IsInvitedPerson=1", "", null);    //filtr přizvaných osob
            }
            if (mq.Prefix == "j02" && mq.param1 == "a02Inspector")
            {
                AQ(ref lis, "a.j02ID IN (select j02ID FROM a02Inspector)", "", null);    //filtr přizvaných osob
            }
            if (mq.Prefix=="x24" && mq.param1== "textbox")
            {
                AQ(ref lis, "a.x24ID IN (1,2,3,4,5)", "", null);    //filtr v datovém typu otázky pro TEXTBOX
            }
            if (mq.Prefix=="x31" && mq.param1== "x31Is4SingleRecord=1")
            {
                AQ(ref lis, "a.x31Is4SingleRecord=1", "", null);    //pouze kontextové sestavy
            }
            if (mq.Prefix == "f06" && mq.param1 == "poll")
            {
                AQ(ref lis, "a.f06BindScopeQuery IN (0,2)", "", null);    //formuláře použitelné jako anketní
            }
            if (mq.explicit_sqlwhere != null)
            {
                AQ(ref lis, mq.explicit_sqlwhere, "", null);
            }

            if (mq.TheGridFilter != null)
            {
                ParseSqlFromTheGridFilter(mq, ref lis);  //složit filtrovací podmínku ze sloupcového filtru gridu
            }



            if (String.IsNullOrEmpty(mq.SearchString) == false && mq.SearchString.Length > 2)
            {
                string s = mq.SearchString.ToLower().Trim();
                s = s.Replace("--", "").Replace("drop", "").Replace("delete", "").Replace("truncate", "").Replace(";", " or ").Replace(",", " or ").Replace("  ", " ");
                s = s.Replace(" or ", "#or#").Replace(" and ", "#and#");
                s = s.Replace(" ", " and ");
                s = s.Replace("#or#", " or ").Replace("#and#", " and ");

                if (mq.Prefix == "a01")
                {
                    string sw = "";                    
                    if (mq.SearchImplementation == "HD")
                    {
                        sw = "Contains((a.a01Signature,a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01Description,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),@expr)";
                        sw += " OR a.a01ID IN (select a01ID FROM b05Workflow_History WHERE Contains((b05Comment),@expr))";
                    }
                    else
                    {
                        sw = string.Format("Contains((a.a01Signature,a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),'{0}')", s);
                        //sw = "Contains((a.a01Signature,a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),@expr)";
                    }
                    //sw += string.Format(" OR Contains((a03Name),'{0}')", s);

                    //if (BO.BAS.InDouble(mq.SearchString) > 0)
                    //{
                    //    //sw = "a.a01Signature LIKE '%'+@expr OR a01_a03.a03REDIZO LIKE @expr+'%'";
                    //    sw = "a.a01Signature LIKE @expr OR a01_a03.a03REDIZO LIKE @expr+'%'";

                    //}
                    //else
                    //{
                    //    //sw = "a01_a03.a03Name LIKE '%'+@expr+'%' OR a01_a03.a03City LIKE '%'+@expr+'%'";
                    //    string s = mq.SearchString.ToLower().Trim();
                    //    s = s.Replace("--", "").Replace("drop", "").Replace("delete", "").Replace("truncate", "").Replace(";", " or ").Replace(",", " or ").Replace("  ", " ");
                    //    s = s.Replace(" or ", "#or#").Replace(" and ", "#and#");
                    //    s = s.Replace(" ", " and ");
                    //    s = s.Replace("#or#", " or ").Replace("#and#", " and ");


                    //    if (mq.SearchImplementation == "HD")
                    //    {
                    //        sw = string.Format("Contains((a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01Description,a.a01InstitutionPlainText),'{0}')", s);
                    //        sw += string.Format(" OR a.a01ID IN (select a01ID FROM b05Workflow_History WHERE Contains((b05Comment),'{0}'))", s);
                    //    }
                    //    else
                    //    {
                    //        sw = string.Format("Contains((a.a01LeaderInLine,a.a01MemberInLine,a.a01CaseCode,a.a01InstitutionPlainText,a.a01InstitutionPlainTextRedizo),'{0}')", s);
                    //    }
                    //    //sw += string.Format(" OR Contains((a03Name),'{0}')", s);


                    //}
                    //AQ(ref lis, "(" + sw + ")", "expr", mq.SearchString);
                    AQ(ref lis, "(" + sw + ")", "", null);

                }
                if (mq.Prefix == "a03")
                {
                    //string sw = "Contains((a.a03REDIZO,a.a03Name,a.a03ICO,a.a03City,a.a03Street),@expr)";
                    string sw = string.Format("Contains((a.a03REDIZO,a.a03Name,a.a03ICO,a.a03City,a.a03Street),'{0}')", s);
                    if (mq.SearchString.Length == 9 && BO.BAS.InDouble(mq.SearchString)>0)
                    {
                        sw = string.Format("Contains((a.a03REDIZO),'{0}')", s);
                        sw += string.Format(" OR a.a03ID IN (SELECT a03ID FROM a37InstitutionDepartment WHERE a37IZO = '{0}')",s);
                    }

                    //if (BO.BAS.InInt(mq.SearchString) > 0)
                    //{
                    //    sw = "a.a03REDIZO LIKE '%'+@expr+'%'";
                    //    if (sw.Length == 9)
                    //    {
                    //        sw += " OR a.a03ID IN (SELECT a03ID FROM a37InstitutionDepartment WHERE a37IZO = @expr)";
                    //    }
                    //}
                    //else
                    //{
                    //    sw = "a.a03REDIZO LIKE '%'+@expr+'%' OR a.a03Name LIKE '%'+@expr+'%' OR a.a03Email LIKE '%'+@expr+'%' OR a.a03City LIKE '%'+@expr+'%' OR a.a03Street LIKE '%'+@expr+'%'";
                    //}
                    AQ(ref lis, "(" + sw + ")", "", null);

                }
                if (mq.Prefix == "j02")
                {
                    string sw = string.Format("Contains((a.j02FullText,a.j02Email,a.j02PID),'{0}')", s);
                    AQ(ref lis, "(" + sw + ")", "", null);
                    //AQ(ref lis, "(a.j02LastName LIKE '%'+@expr+'%' OR a.j02FirstName LIKE '%'+@expr+'%' OR a.j02Email LIKE '%'+@expr+'%' OR a.j02PID LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "a42")
                {
                    AQ(ref lis, "(a.a42Name LIKE '%'+@expr+'%' OR a.a42Description LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }

                if (mq.Prefix == "f06")
                {
                    AQ(ref lis, "(a.f06Name LIKE '%'+@expr+'%' OR a.f06Ident LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "f21")
                {
                    AQ(ref lis, "(a.f21Name LIKE '%'+@expr+'%' OR a.f21Description LIKE '%'+@expr+'%' OR a.f21ExportValue LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }

            }


            var ret = new DL.FinalSqlCommand();

            if (lis.Count > 0)
            {
                ret.Parameters = new Dapper.DynamicParameters();
                if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.ParName) == false))
                {
                    ret.Parameters.Add(c.ParName, c.ParValue);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.ParName, ParValue = c.ParValue });
                }
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.Par2Name) == false))
                {
                    ret.Parameters.Add(c.Par2Name, c.Par2Value);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.Par2Name, ParValue = c.Par2Value });
                }


                ret.SqlWhere = String.Join(" ", lis.Select(p =>p.AndOrZleva + " "+p.BracketLeft + p.StringWhere+p.BracketRight)).Trim();    //složit závěrčnou podmínku
                //System.IO.File.WriteAllText("c:\\temp\\hovado"+mq.Prefix+".txt", ret.SqlWhere);
            }

            if (!string.IsNullOrEmpty(ret.SqlWhere))
            {
                strPrimarySql += " WHERE " + ret.SqlWhere;
            }
            if (!string.IsNullOrEmpty(mq.explicit_orderby))
            {
                strPrimarySql += " ORDER BY " + mq.explicit_orderby;
            }

            if (strPrimarySql.Contains("@gd1"))
            {   //view s napevno navrženou podmínkou časového filtru                            
                if (mq.global_d1 == null) mq.global_d1 = new DateTime(2000, 1, 1);
                if (mq.global_d2 == null)
                {
                    mq.global_d2 = new DateTime(3000, 1, 1);
                }
               
                //if (ret.Parameters == null)
                //{
                //    ret.Parameters = new Dapper.DynamicParameters();
                //    if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                //}
                //ret.Parameters.Add("gd1", mq.global_d1, System.Data.DbType.DateTime);
                //if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = "gd1", ParValue = mq.global_d1 });
                //ret.Parameters.Add("gd2", mq.global_d2, System.Data.DbType.DateTime);
                //if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = "gd2", ParValue = mq.global_d2 });
            }
            

            //System.IO.File.WriteAllText("c:\\temp\\hovado"+mq.Prefix+".txt", strPrimarySql);
            ret.FinalSql = strPrimarySql;
            return ret;

        }

        private static void AQ(ref List<DL.QueryRow> lis, string strWhere, string strParName, object ParValue, string strAndOrZleva = "AND",string strBracketLeft=null,string strBracketRight=null,string strPar2Name=null,object Par2Value=null)
        {
            if (lis.Count == 0)
            {
                strAndOrZleva = ""; //první podmínka zleva
            }
            if (String.IsNullOrEmpty(strParName) == false && lis.Where(p => p.ParName == strParName).Count() > 0)
            {
                return; //parametr strParName již byl dříve přidán
            }
            lis.Add(new DL.QueryRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue, AndOrZleva = strAndOrZleva, BracketLeft = strBracketLeft, BracketRight = strBracketRight,Par2Name=strPar2Name,Par2Value=Par2Value });
        }
        

        private static object get_param_value(string colType, string colValue)
        {
            if (String.IsNullOrEmpty(colValue) == true)
            {
                return null;
            }
            if (colType == "num")
            {
                return BO.BAS.InDouble(colValue);
            }
            if (colType == "date")
            {
                return Convert.ToDateTime(colValue);
            }
            if (colType == "bool")
            {
                if (colValue == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return colValue;
        }
        private static void ParseSqlFromTheGridFilter(BO.myQuery mq, ref List<DL.QueryRow> lisAQ)
        {

            int x = 0;
            foreach (var filterrow in mq.TheGridFilter)
            {
                var col = filterrow.BoundColumn;
                var strF = col.getFinalSqlSyntax_WHERE();

                x += 1;
                string parName = "par" + x.ToString();

                int endIndex = 0;
                string[] arr = new string[] { filterrow.value };
                if (filterrow.value.IndexOf(";") > -1)  //v podmnínce sloupcového filtru může být středníkem odděleno více hodnot!
                {
                    arr = filterrow.value.Split(";");
                    endIndex = arr.Count() - 1;
                }

                switch (filterrow.oper)
                {
                    case "1":   //IS NULL
                        AQ(ref lisAQ, strF + " IS NULL", "", null);
                        break;
                    case "2":   //IS NOT NULL
                        AQ(ref lisAQ, strF + " IS NOT NULL", "", null);
                        break;
                    case "10":   //větší než nula
                        AQ(ref lisAQ, strF + " > 0", "", null);
                        break;
                    case "11":   //je nula nebo prázdné
                        AQ(ref lisAQ, "ISNULL(" + strF + ",0)=0", "", null);
                        break;
                    case "8":   //ANO
                        AQ(ref lisAQ, strF + " = 1", "", null);
                        break;
                    case "9":   //NE
                        AQ(ref lisAQ, strF + " = 0", "", null);
                        break;
                    case "3":   //obsahuje                
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE '%'+@{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR"); ;
                            }

                        }

                        break;
                    case "5":   //začíná na 
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE @{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "6":   //je rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " = @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "4":   //interval
                        AQ(ref lisAQ, string.Format(strF + " >= @{0}", parName + "c1"), parName + "c1", get_param_value(col.NormalizedTypeName, filterrow.c1value));
                        AQ(ref lisAQ, string.Format(strF + " <= @{0}", parName + "c2"), parName + "c2", get_param_value(col.NormalizedTypeName, filterrow.c2value));
                        break;
                    case "7":   //není rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " <> @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }
                        }

                        break;
                }

            }


            string leva_zavorka(int i, int intEndIndex)
            {
                if (intEndIndex > 0 && i == 0)
                {
                    return "(";
                }
                else
                {
                    return "";
                }
            }
            string prava_zavorka(int i, int intEndIndex)
            {
                if (intEndIndex > 0 && i == intEndIndex)
                {
                    return ")";
                }
                else
                {
                    return "";
                }
            }

        }

    }
}
