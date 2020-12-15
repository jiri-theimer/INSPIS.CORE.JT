using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class FormValidation
    {
        private BL.Factory _Factory { get; set; }
        
        public FormValidation(BL.Factory f)
        {
            _Factory = f;
            
        }

        private bool TryEval(int a11id, string strExpression)
        {
            if (string.IsNullOrEmpty(strExpression)) return false;

            var evaluator = new EVAL.Evaluator(_Factory, a11id);
            if (Convert.ToBoolean(evaluator.TryEval(strExpression)))
            {
                return true;
            }

            return false;
        }

        public List<BO.ItemValidationResult> GetValidateResult(int a11id)
        {
            var OdpovediSChybou = new List<BO.ItemValidationResult>();

            var cA11 = _Factory.a11EventFormBL.Load(a11id);
            if (cA11 == null)
            {
                var c = new BO.ItemValidationResult() { OtazkaId = 666, Otazka = "Není otázka", SekceId = 666, Sekce = "", Message ="Tato akce neobsahuje vazbu ani na jeden formulář!" };
                OdpovediSChybou.Add(c);
                return OdpovediSChybou;
            }
            //natahnout segmenty - sekce
            var mq = new BO.myQuery("f18");
            mq.f06id = cA11.f06ID;
            var segmenty = _Factory.f18FormSegmentBL.GetList(mq);
            //seznam vsech otazek
            
            var otazky = _Factory.f19QuestionBL.GetList(new BO.myQueryF19() { f06id = cA11.f06ID });
            //seznam vsech jiz vyplnenych odpovedi ve formulari                       
            var vyplneneOdpovedi = _Factory.f32FilledValueBL.GetList(new BO.myQueryF32() { a11id = a11id });

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
                        var c = new BO.ItemValidationResult() {a11ID=a11id,f06ID=cA11.f06ID,f06Name=cA11.f06Name, OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = _Factory.tra("Povinná otázka") };
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
                        var c = new BO.ItemValidationResult() { a11ID = a11id, f06ID = cA11.f06ID, f06Name = cA11.f06Name, OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = otazka.f19CancelValidateExpression_Message };
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
                        var c = new BO.ItemValidationResult() { a11ID = a11id, f06ID = cA11.f06ID, f06Name = cA11.f06Name, OtazkaId = otazka.pid, Otazka = otazka.f19Name, SekceId = otazka.f18ID, Sekce = otazka.f18Name, Message = _Factory.tra("Odpověď nemá správný formát") + ": " + otazka.f19Regex };
                        OdpovediSChybou.Add(c);
                    }
                }
            }

            return OdpovediSChybou;
        }

    }
}
