using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface IFormValidationBL
    {       
        public List<BO.ItemValidationResult> GetValidateResult(int a11id);

    }
    class FormValidationBL : BaseBL, IFormValidationBL
    {
        public List<BO.ItemValidationResult> GetValidateResult(int a11id)
        {
            var OdpovediSChybou = new List<BO.ItemValidationResult>();

            var cA11 = _mother.a11EventFormBL.Load(a11id);
            if (cA11 == null)
            {
                var c = new BO.ItemValidationResult() { OtazkaId = 666, Otazka = "Není otázka", SekceId = 666, Sekce = "", Message = "Tato akce neobsahuje vazbu ani na jeden formulář!" };
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
            foreach (var otazka in otazky.Where(p=>p.f19IsRequired==true || string.IsNullOrEmpty(p.f19RequiredExpression) == false))
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

                }
            }

        }
    }
}
