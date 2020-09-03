using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BL
{
    public interface IFormValidationBL
    {       
        public List<BO.ItemValidationResult> GetValidateResult(int a11id);

    }
    class FormValidationBL : BaseBL, IFormValidationBL
    {
        public FormValidationBL(BL.Factory mother) : base(mother)
        {

        }
        private bool TryEval(int a11id, string strExpression)
        {
            if (string.IsNullOrEmpty(strExpression)) return false;
            return true;
        }
        public List<BO.ItemValidationResult> GetValidateResult(int a11id)
        {
            var OdpovediSChybou = new List<BO.ItemValidationResult>();

            var cA11 = _mother.a11EventFormBL.Load(a11id);
            if (cA11 == null)
            {
                var c = new BO.ItemValidationResult() { OtazkaId = 666, Otazka = "Není otázka", SekceId = 666, Sekce = "", Message = _mother.tra("Tato akce neobsahuje vazbu ani na jeden formulář!") };
                OdpovediSChybou.Add(c);
                return OdpovediSChybou;
            }
            //natahnout segmenty - sekce
            var mq = new BO.myQuery("f18");
            mq.f06id = cA11.f06ID;
            var segmenty = _mother.f18FormSegmentBL.GetList(mq);
            //seznam vsech otazek
            mq = new BO.myQuery("f19");
            mq.f06id = cA11.f06ID;
            var otazky = _mother.f19QuestionBL.GetList(mq);
            //seznam vsech jiz vyplnenych odpovedi ve formulari
            mq = new BO.myQuery("f32");
            mq.a11id = a11id;
            var vyplneneOdpovedi = _mother.f32FilledValueBL.GetList(mq);

            //projit vsechny povinne otazky
            foreach (var otazka in otazky.Where(p => p.f19IsRequired == true || string.IsNullOrEmpty(p.f19RequiredExpression) == false))
            {
                int intF19ID = otazka.pid;
                bool maOdpoved = false;
                if (vyplneneOdpovedi.Where(p => p.f19ID == intF19ID && string.IsNullOrEmpty(p.Value) == false).Count() > 0)
                {
                    maOdpoved = true;
                }
                if (maOdpoved == false)
                {
                    //pokud neni zadana required expr.
                    bool b = false;
                    if (otazka.f19IsRequired)
                    {
                        b = true;
                    }
                    else
                    {
                        if (TryEval(a11id, otazka.f19RequiredExpression))
                        {
                            b = true;
                        }
                    }
                    if (b && string.IsNullOrEmpty(otazka.f19SkipExpression) == false)
                    {
                        if (TryEval(a11id, otazka.f19SkipExpression))
                        {
                            b = false;  //otázka je skrytá - nebere se jako nevyplněná
                        }
                    }
                    if (b)
                    {
                        var c = new BO.ItemValidationResult() { OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = _mother.tra("Povinná otázka") };
                        OdpovediSChybou.Add(c);
                    }
                }
            }

            //projit vsechny CancelValidateExpr
            foreach (var otazka in otazky.Where(p => string.IsNullOrEmpty(p.f19CancelValidateExpression) == false))
            {
                //spatne vyplnena odpoved
                bool b = true;
                if (TryEval(a11id, otazka.f19SkipExpression))
                {
                    b = false;  //otázka je skrytá - nebere se jako nevyplněná
                }
                if (b)
                {
                    if (TryEval(a11id, otazka.f19CancelValidateExpression))
                    {
                        var c = new BO.ItemValidationResult() { OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = otazka.f19CancelValidateExpression_Message };
                        OdpovediSChybou.Add(c);
                    }
                }
            }

            //projit vsechny otazky s definovanou regex podminkou
            foreach (var otazka in otazky.Where(p => string.IsNullOrEmpty(p.f19Regex) == false))
            {
                BO.f32FilledValue odp = vyplneneOdpovedi.FirstOrDefault(p => p.f19ID == otazka.f19ID);
                if (odp != null)
                {
                    var regex = new System.Text.RegularExpressions.Regex(otazka.f19Regex);
                    if (regex.IsMatch(odp.Value) == false)
                    {
                        var c = new BO.ItemValidationResult() { OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = _mother.tra("Otázka nemá správný formát") + ": " + otazka.f19Regex };
                        OdpovediSChybou.Add(c);
                    }
                }
            }

            return OdpovediSChybou;
        }
    }
}
