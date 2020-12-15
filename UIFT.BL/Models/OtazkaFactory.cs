using System;
using System.Collections.Generic;
using System.Linq;
using UIFT.Repository;

namespace UIFT.Models
{
    public static class OtazkaFactory
    {
        /// <summary>
        /// Vraci instanci standardni otazky (Otazka class)
        /// </summary>
        internal static IOtazka Get(
            Repository.Repository repository, 
            BO.f19Question otazka, 
            IEnumerable<BO.o27Attachment> prilohy,
            IEnumerable<BO.f32FilledValue> vyplneneOdpovedi, 
            IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi, 
            IEnumerable<BO.f31FilledQuestionPublishing> publikovane,
            int f06id = 0,
            bool isPreview = false)
        {
            // pokud ma otazka template v jine otazce
            if (otazka.f19TemplateID > 0)
            {
                BO.f19Question otazkaTemplate = repository.BL.f19QuestionBL.Load(otazka.f19TemplateID);

                // vlozit data z template do soucasne otazky
                if (string.IsNullOrEmpty(otazka.f18Name))
                    otazka.f19Name = otazkaTemplate.f19Name;
                if (string.IsNullOrEmpty(otazka.f19Hint))
                    otazka.f19Hint = otazkaTemplate.f19Hint;
                if (string.IsNullOrEmpty(otazka.f19SupportingText))
                    otazka.f19SupportingText = otazkaTemplate.f19SupportingText;
                otazka.TextBox_MaxValue = otazkaTemplate.TextBox_MaxValue;
                otazka.TextBox_MinValue = otazkaTemplate.TextBox_MinValue;
                
                // ziskat odpovedi z templatu
                odpovedi = repository.cache.f21ReplyUnitBLGetListJoinedF19(f06id, 0, otazka.f19TemplateID);
            }

            // instance vlastni otazky
            Otazka otazkaDerived = new Otazka(otazka, repository.Configuration.FT_CheckboxAnswerTrueValue, repository.Configuration.FT_CheckboxAnswerFalseValue)
            {
                IsPreview = isPreview
            };

            // pokud se jedna o otazku typu button a nema custom url link
            if (otazka.f27ID > 0)
            {
                BO.f27LinkUrl linkUrl = repository.cache.f27LinkUrlGetList().FirstOrDefault(t => t.pid == otazka.f27ID);
                if (linkUrl != null)
                    otazka.f19LinkURL = linkUrl.f27URL;
            }

            // evaulovat makro, pokud je zadane pro REQUIRED expr. - pokud ano, oznacit otazku jako required
            if (!string.IsNullOrEmpty(otazka.f19RequiredExpression) && !otazka.f19IsRequired)
                otazkaDerived.IsRequired = repository.BL.BoolEvaluator(repository.a11id, otazka.f19RequiredExpression);

            // evaluovat makro, pokud je zadane a zjistit, zda nema byt otazka skryta
            if (!string.IsNullOrEmpty(otazka.f19SkipExpression))
                otazkaDerived.IsHidden = repository.BL.BoolEvaluator(repository.a11id, otazka.f19SkipExpression);

            // zjistit, zda otazka neni readonly - v preview rezimu je vzdy readonly
            if (isPreview)
                otazkaDerived.ReadOnly = true;
            else
                otazkaDerived.ReadOnly = repository.BL.BoolEvaluator(repository.a11id, otazka.f19ReadonlyExpression);

            // je publikovana?
            if (publikovane != null)
            {
                BO.f31FilledQuestionPublishing pub = publikovane.FirstOrDefault(t => t.f19ID == otazka.pid);
                if (pub != null)
                    otazkaDerived.IsPublished = pub.f31IsPublished;
            }

            // najit seznam priloh pro otazku
            if (prilohy == null)
                otazkaDerived.Prilohy = new List<BO.o27Attachment>(0);
            else
                otazkaDerived.Prilohy = prilohy.Where(t => t.o27DataPID == otazka.pid).ToList();

            // najit seznam odpovedi pro otazku (pokud je to mozne)
            if (odpovedi == null)
                otazkaDerived.Odpovedi = new List<BO.f21ReplyUnitJoinedF19>(0);
            else
                otazkaDerived.Odpovedi = odpovedi.Where(t => t.f19ID == otazka.pid || t.f19ID == otazka.f19TemplateID).ToList();

            // najit vyplnene odpovedi pro otazku
            if (vyplneneOdpovedi != null)
            {
                switch (otazka.ReplyControl)
                {
                    // nechci v te kolekci "nezaskrtnute" odpovedi
                    case BO.ReplyKeyEnum.Listbox:
                    case BO.ReplyKeyEnum.RadiobuttonList:
                    case BO.ReplyKeyEnum.DropdownList:
                        otazkaDerived.VyplneneOdpovedi = vyplneneOdpovedi.Where(t => t.f19ID == otazka.pid && t.Value != repository.Configuration.FT_CheckboxAnswerFalseValue).ToList();
                        break;
                    default:
                        otazkaDerived.VyplneneOdpovedi = vyplneneOdpovedi.Where(t => t.f19ID == otazka.pid).ToList();
                        break;
                }
            }
            else
            {
                otazkaDerived.VyplneneOdpovedi = new List<BO.f32FilledValue>();
            }

            // v pripade eval otazky, zjisti datasource
            if (otazka.ReplyControl == BO.ReplyKeyEnum.EvalList)
            {
                otazkaDerived.EvalSource = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(otazka.f19EvalListSource))
                {
                    var evaluator = new EVAL.Evaluator(repository.BL, repository.a11id);
                    object ret = evaluator.TryEval(otazka.f19EvalListSource);
                    if (ret != null)
                    {
                        if (!string.IsNullOrEmpty(ret.ToString()))
                        {
                            foreach (string str in ret.ToString().Split('|'))
                            {
                                string[] arr = str.Split(';');
                                otazkaDerived.EvalSource.Add(arr[0], arr[1]);
                            }
                        }
                    }
                }
            }

            // pokud se jedna o fileupload a jsou nejake odpovedi, dohraj k nim informace o nahranych souborech
            if (otazka.ReplyControl == BO.ReplyKeyEnum.FileUpload && otazkaDerived.VyplneneOdpovedi.Count > 0)
            {
                otazkaDerived.VyplneneOdpovediSoubory = new List<BO.o27Attachment>();

                foreach (BO.f32FilledValue odpoved in otazkaDerived.VyplneneOdpovedi)
                {
                    IEnumerable<BO.o27Attachment> attList = repository.BL.o27AttachmentBL.GetList(new BO.myQueryO27
                    {
                        x29id = 432,
                        f32id = odpoved.pid
                    }, "");

                    if (attList != null)
                        otazkaDerived.VyplneneOdpovediSoubory.AddRange(attList);
                }
            }

            // zadne odpovedi k otazce nebyly ulozeny - zkus default value
            // pouze k nekterym typum otazek
            if (otazka.ReplyControl != BO.ReplyKeyEnum.FileUpload 
                && otazka.ReplyControl != BO.ReplyKeyEnum.BatteryBoard 
                && otazka.ReplyControl != BO.ReplyKeyEnum.ChessBoard)
            {
                if (otazkaDerived.VyplneneOdpovedi.Count(t => !t.f32IsFilledByEval) == 0 && !string.IsNullOrEmpty(otazka.f19DefaultValue))
                {
                    // v pripade preview se vygenerovane odpovedi nesmi ukladat do databaze
                    BO.f32FilledValue odp = repository.OtazkaGenerovatDefaultValues(otazka, otazkaDerived.Odpovedi, otazkaDerived.VyplneneOdpovedi, !isPreview);
                    // byla vygenerovana default odpoved
                    if (odp != null)
                        otazkaDerived.VyplneneOdpovedi.Add(odp);
                }
                // jinak pokud se jedna o Summary otazky, potrebujeme default value stejne
                else if (otazka.ReplyControl == BO.ReplyKeyEnum.SummaryOverview && otazkaDerived.VyplneneOdpovedi.Count > 0)
                {
                    BO.f32FilledValue odp = repository.OtazkaGenerovatDefaultValues(otazka, otazkaDerived.Odpovedi, otazkaDerived.VyplneneOdpovedi, false);
                    // byla vygenerovana default odpoved - vloz ji do poznamky (musim zobrazovat dve ruzne value - tu auto generovanou a tu uzivatelem napsanou)
                    if (odp != null)
                    {
                        otazkaDerived.VyplneneOdpovedi[0].f32Comment = odp.Value;
                    }
                }
            }
            
            return otazkaDerived;
        }

        /// <summary>
        /// Vraci instanci otazky typu Sachovnice
        /// </summary>
        internal static IOtazka Get(
            Repository.Repository repository, 
            BO.f25ChessBoard chessboard,
            IEnumerable<BO.f19Question> otazky,
            IEnumerable<BO.f32FilledValue> vyplneneOdpovedi,
            IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi,
            IEnumerable<BO.f31FilledQuestionPublishing> publikovane,
            int f06id,
            bool isPreview)
        {
            // instance sachovnice
            OtazkaSachovnice sachovnice = new OtazkaSachovnice(chessboard);

            // je publikovana?
            if (publikovane != null)
            {
                BO.f31FilledQuestionPublishing pub = publikovane.FirstOrDefault(t => t.f25ID == chessboard.pid);
                if (pub != null)
                    sachovnice.IsPublished = pub.f31IsPublished;
            }

            // readonly
            sachovnice.ReadOnly = isPreview;

            // doplnit otazky v sachovnici
            sachovnice.Otazky = new List<Otazka>();
            foreach (BO.f19Question otazka in otazky)
            {
                sachovnice.Otazky.Add((Otazka)Get(repository, otazka, null, vyplneneOdpovedi, odpovedi, null, f06id, isPreview));
            }

            // pokud jsou vsechny otazky v baterii skryte, skryj i baterii
            sachovnice.IsHidden = sachovnice.Otazky.All(t => t.IsHidden);

            return sachovnice;
        }

        /// <summary>
        /// Vraci instanci otazky typu Baterie
        /// </summary>
        internal static IOtazka Get(
            Repository.Repository repository,
            BO.f26BatteryBoard batteryBoard,
            IEnumerable<BO.f19Question> otazky,
            IEnumerable<BO.f32FilledValue> vyplneneOdpovedi,
            IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi,
            IEnumerable<BO.f31FilledQuestionPublishing> publikovane,
            int f06id,
            bool isPreview)
        {
            // instance sachovnice
            OtazkaBaterie baterie = new OtazkaBaterie(batteryBoard);
            
            // je publikovana?
            if (publikovane != null)
            {
                BO.f31FilledQuestionPublishing pub = publikovane.FirstOrDefault(t => t.f26ID == batteryBoard.pid);
                if (pub != null)
                    baterie.IsPublished = pub.f31IsPublished;
            }

            // readonly
            baterie.ReadOnly = isPreview;

            // doplnit otazky v sachovnici
            baterie.Otazky = new List<Otazka>();
            foreach (BO.f19Question otazka in otazky.Where(t => t.f26ID == batteryBoard.pid))
            {
                baterie.Otazky.Add((Otazka)Get(repository, otazka, null, vyplneneOdpovedi, odpovedi, null, f06id, isPreview));
            }

            // pokud jsou vsechny otazky v baterii skryte, skryj i baterii
            baterie.IsHidden = baterie.Otazky.All(t => t.IsHidden);

            return baterie;
        }
    }
}