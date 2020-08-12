using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class TheQueryFieldProvider
    {
        //private readonly BL.TheEntitiesProvider _ep;
        private string _Prefix;
        private List<BO.TheQueryField> _lis;
        private string _lastEntity;

        public TheQueryFieldProvider(string strPrefix)
        {
            _Prefix = strPrefix;
            //_ep = ep;
            _lis = new List<BO.TheQueryField>();
            SetupPallete();


        }
        public List<BO.TheQueryField> getPallete()
        {
            return _lis;
        }
        private void SetupPallete()
        {
            BO.TheQueryField of;
            switch (_Prefix)
            {
                case "a03":
                    //Instituce: a03Institution
                    AF("a03Institution", "a05ID", "a.a05ID", "Kraj", "a05Region", null, "multi");
                    AF("a03Institution", "a09ID", "a.a09ID", "Typ zřizovatele", "a09FounderType", null, "multi");
                    AF("a03Institution", "a06ID", "a.a06ID", "Typ instituce", "a06InstitutionType", null, "multi");
                    AF("a03Institution", "a21ID", "a.a21ID", "Právní forma", "a21InstitutionLegalType", null, "multi");
                    AF("a03Institution", "a70ID", "a.a70ID", "Školní IS", "a70SIS", null, "multi");
                    of=AF("a03Institution", "a03ID_Founder", "a.a03ID_Founder", "Zřizovatel", "a03Institution", null, "combo");
                    of.MasterPrefix = "a06";of.MasterPid = 2;

                    of = AF("a03Institution", "a03-a42ID", "a42ID", "INEZ", "a42Qes", null, "combo");
                    of.SqlWrapper = "a.a03ID IN (select a03ID FROM a01Event WHERE #filter#)";

                    AF("a03Institution", "a03REDIZO", "a.a03REDIZO", "REDIZO");
                    AF("a03Institution", "a03Name", "a.a03Name", "Název instituce");
                    AF("a03Institution", "a03Street", "a.a03Street", "Ulice");
                    AF("a03Institution", "a03City", "a.a03City", "Obec");
                    AF("a03Institution", "a03PostCode", "a.a03PostCode", "PSČ");
                    AF("a03Institution", "a03Email", "a.a03Email", "E-mail");
                    AF("a03Institution", "a03Web", "a.a03Web", "WWW");
                    AF("a03Institution", "a03Phone", "a.a03Phone", "TEL");
                    AF("a03Institution", "a03Mobile", "a.a03Mobile", "Mobil");
                    AF("a03Institution", "a03IsTestRecord", "a.a03IsTestRecord", "Testovací záznam", null, null, "bool");
                    AF("a03Institution", "a03DateInsert", "a.a03DateInsert", "Datum založení záznamu", null, null, "date");
                    

                    break;
                case "a01":
                    //Akce: a01Event
                    AF("a01Event","a10ID", "a.a10ID", "Typ akce", "a10EventType", null, "multi");
                    AF("a01Event", "a08ID", "a.a08ID", "Téma akce", "a08Theme", null, "multi");
                    AF("a01Event", "b02ID", "a.b02ID", "Workflow stav", "b02WorkflowStatus", null, "multi");
                    AF("a01Event", "a42ID", "a.a42ID", "INEZ", "a42Qes", null, "combo");
                    of=AF("a01Event", "a03-a05ID", "a05ID", "Kraj školy", "a05Region", null, "multi");
                    of.SqlWrapper = "a.a03ID IN (select a03ID FROM a03Institution WHERE #filter#)";
                    of = AF("a01Event", "a03-a03ID_Founder", "a03ID_Founder", "Zřizovatel školy", "a03Institution", null, "combo");
                    of.SqlWrapper = "a.a03ID IN (select a03ID FROM a03Institution WHERE #filter#)"; of.MasterPrefix = "a06"; of.MasterPid = 2;

                    AF("a01Event", "a42ID", "a.a42ID", "INEZ", "a42Qes", null, "combo");
                    AF("a01Event", "j02ID_Issuer", "a.j02ID_Issuer", "Zakladatel akce", "j02Person", null, "combo");
                    AF("a01Event", "a03ID", "a.a03ID", "Instituce", "a03Institution", null, "combo");
                    AF("a01Event", "a01DateFrom", "a.a01DateFrom", "Plán od", null, null, "date");
                    AF("a01Event", "a01DateUntil", "a.a01DateUntil", "Plán do", null, null, "date");
                    //AF("a01Event", "IsClosed", "dbo._core_a01_isclosed(a.a01IsClosed,a.a01ValidFrom,a.a01ValidUntil)", "Akce je uzavřena", null, null, "bool");
                    AF("a01Event", "IsClosed", "(case when a.a01IsClosed=1 then 1 else case when GETDATE() not between a.a01ValidFrom AND a.a01ValidUntil then 1 else 0 end end)", "Akce je uzavřena", null, null, "bool");
                    AF("a01Event", "a01IsAllFormsClosed", "a.a01IsAllFormsClosed", "Všechny formuláře uzavřeny", null, null, "bool");
                    AF("a01Event", "ExistsA11", "(case when exists(select a11ID FROM a11EventForm WHERE a01ID=a.a01ID) then 1 else 0 end)", "Obsahuje formuláře", null, null, "bool");
                    AF("a01Event", "ExistsA11Poll", "(case when exists(select a11ID FROM a11EventForm WHERE a11IsPoll=1 AND a01ID=a.a01ID) then 1 else 0 end)", "Obsahuje anketní formuláře", null, null, "bool");


                    AF("a01Event", "a01ChildsCount", "a.a01ChildsCount", "Počet podřízených akcí", null, null, "number");
                    of=AF("a01Event", "a41-j02ID","j02ID", "Účastník akce", "j02Person", null, "combo");                    
                    of.SqlWrapper = "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE #filter# OR j11ID IN (select j11ID FROM j12Team_Person WHERE #filter#))";

                    of = AF("a01Event", "a41leader-j02ID", "j02ID", "Vedoucí týmu", "j02Person", null, "combo");                    
                    of.SqlWrapper = "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=2 AND #filter#)";

                    of = AF("a01Event", "a41member-j02ID", "j02ID", "Člen týmu", "j02Person", null, "combo");                    
                    of.SqlWrapper = "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=1 AND (#filter# OR j11ID IN (select j11ID FROM j12Team_Person WHERE #filter#)))";

                    of = AF("a01Event", "a41owner-j02ID","j02ID", "Vlastník", "j02Person", null, "combo");                    
                    of.SqlWrapper = "a.a01ID IN (select a01ID FROM a41PersonToEvent WHERE a45ID=5 AND #filter#)";

                    break;
                default:
                    break;
            }




        }


        private BO.TheQueryField AF(string strEntity, string strField,string strSqlSyntax, string strHeader, string strSourceEntity = null, string strSourceSql = null, string strFieldType = "string")
        {
            if (strEntity != _lastEntity)
            {
                //zatím nic
            }

            _lis.Add(new BO.TheQueryField() { Field = strField,FieldSqlSyntax=strSqlSyntax, Entity = strEntity, Header = strHeader, FieldType = strFieldType, SourceEntity = strSourceEntity, SourceSql = strSourceSql, TranslateLang1 = strHeader, TranslateLang2 = strHeader, TranslateLang3 = strHeader });
            _lastEntity = strEntity;
            return _lis[_lis.Count - 1];
        }
    }
}
