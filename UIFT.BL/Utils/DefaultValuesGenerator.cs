using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT.Repository
{
    internal class DefaultValuesGenerator
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private Repository repository;

        public DefaultValuesGenerator(Repository repository)
        {

            this.repository = repository;
        }

        /// <summary>
        /// Vytvori default odpoved pro single otazku
        /// </summary>
        /// <returns>Vztvorena odpoved nebo null</returns>
        public BO.f32FilledValue PerQuestion(BO.f19Question otazka, IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi, IEnumerable<BO.f32FilledValue> vyplneneOdpovedi, bool saveAnswerToDB = true)
        {
            // spust evaluator na default value
            /*TODO
            BL.IEvaluatorBL evaluator = this.repository.BL.get_Evaluator(this.repository.a11id);
            object ret = evaluator.TryEval(otazka.f19DefaultValue);*/
            object ret = null;

            if (ret != null) // evaluator vratil nejaky vysledek
            {
                // vraceny retezec nesmi byt prazdny
                // nebo muze byt prazdny, ale jen pro otazku typu textbox/summary/html
                if (!string.IsNullOrEmpty(ret.ToString()) 
                    || otazka.ReplyControl == BO.ReplyKeyEnum.TextBox
                    || otazka.ReplyControl == BO.ReplyKeyEnum.SummaryOverview
                    || otazka.ReplyControl == BO.ReplyKeyEnum.HtmlEditor)
                {
                    // pokud vygenerovana hodnota uz neni v DB ulozena
                    if (vyplneneOdpovedi.Count(t => t.Value == ret.ToString()) == 0)
                    {
                        // vytvorit instanci odpovedi
                        BO.f32FilledValue odpoved = new BO.f32FilledValue
                        {
                            a11ID = this.repository.a11id,
                            f19ID = otazka.pid,
                            f32IsFilledByEval = true
                        };

                        switch (otazka.ReplyControl)
                        {
                            default:
                                odpoved.Value = this.repository.BL.GlobalParams.LoadParam("FT_CheckboxAnswerTrueValue");
                                // evaluator vraci f21id odpovedi, ktera je jako vychozi
                                odpoved.f21ID = Convert.ToInt32(ret);
                                break;

                            case BO.ReplyKeyEnum.SummaryOverview:
                            case BO.ReplyKeyEnum.TextBox:
                            case BO.ReplyKeyEnum.HtmlEditor:
                                if (ret is DateTime) // jedna se o datum a cas, zkonvertuj do formatu BL
                                    odpoved.Value = Convert.ToDateTime(ret).ConvertToBL();
                                else
                                    odpoved.Value = ret.ToString();

                                // najit f21id odpovedi
                                try
                                {
                                    odpoved.f21ID = odpovedi.First(t => t.f19ID == otazka.pid).pid;
                                }
                                catch (Exception err)
                                {
                                    odpoved = null;
                                    log.Error("DefaultValuesGenerator.PerQuestion #1: {0}", err.Message);
                                }
                                break;
                        }

                        // ulozit odpoved
                        if (odpoved != null)
                        {
                            if (!saveAnswerToDB)
                                return odpoved;
                            else if (this.repository.BL.f32FilledValueBL.Save(odpoved, false) > 0)
                                return odpoved;
                            //else
                                log.Error("DefaultValuesGenerator.PerQuestion #2: {0}", this.repository.BL.CurrentUser.Messages4Notify.ToSingleLine());
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Vygeneruje default values pro vsechny otazky ve formulari
        /// </summary>
        public List<BO.f32FilledValue> PerForm(int formularId)
        {
            // seznam vsech otazek, ktere obsahuji default value expr.
            IEnumerable<BO.f19Question> otazky = this.repository.cache.f19QuestionBLGetList(formularId).Where(t => !string.IsNullOrEmpty(t.f19DefaultValue));

            // seznam odpovedi vygenerovanych pomoci default value
            List<BO.f32FilledValue> vygenerovaneOdpovedi = new List<BO.f32FilledValue>();

            if (otazky.Count() > 0) // ve formulari je alespon jedna otazka s default value expr.
            {
                // seznam vsech jiz vyplnenych odpovedi ve formulari
                IEnumerable<BO.f32FilledValue> vyplneneOdpovedi = this.repository.cache.f32FilledValueBLGetList(this.repository.a11id);

                // seznam vsech odpovedi ve formulari
                IEnumerable<BO.f21ReplyUnitJoinedF19> formOdpovedi = this.repository.cache.f21ReplyUnitBLGetListJoinedF19(formularId);

                // projit kazdou otazku s default value
                // zjistit zda ma jiz vyplnenou odpoved
                // pokud nema nebo je odpoved vyplnena automaticky (def val), tak vyhodnoti default expr a ulozi ji do databaze
                foreach (BO.f19Question otazka in otazky)
                {
                    IEnumerable<BO.f32FilledValue> odp = vyplneneOdpovedi.Where(t => t.f19ID == otazka.pid);

                    // jedna se o summary overview a ma zadanou custom odpoved - vygenerovana default value se nesmi ulozit jako odpoved
                    if (otazka.ReplyControl == BO.ReplyKeyEnum.SummaryOverview && odp.Count(t => t.f32IsFilledByEval) == 0)
                    {
                        BO.f32FilledValue o = this.PerQuestion(otazka, formOdpovedi.Where(t => t.f19ID == otazka.pid), odp, false);
                        if (o != null)
                            vygenerovaneOdpovedi.Add(o);
                    }
                    else if (odp.Count(t => !t.f32IsFilledByEval) == 0 || odp.Count() == 0)
                    {
                        // pokud otazka nema zatim zadne odpovedi nebo jsou to jen automaticky doplnene odpovedi
                        BO.f32FilledValue o = this.PerQuestion(otazka, formOdpovedi.Where(t => t.f19ID == otazka.pid), odp);
                        if (o != null)
                            vygenerovaneOdpovedi.Add(o);
                    }
                }
            }

            return vygenerovaneOdpovedi;
        }
    }
}
