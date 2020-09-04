using System;
using System.Collections.Generic;

namespace UIFT.Models
{
    public interface IOtazka
    {
        /// <summary>
        /// ID otazky. f19id / f25id
        /// </summary>
        int PID { get; }

        /// <summary>
        /// Poradi otazky v ramci sekce
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Upresneni druhu otazky - v pripade textboxu
        /// </summary>
        BO.x24DataTypeEnum ReplyType { get; }

        /// <summary>
        /// Druh otazky
        /// </summary>
        BO.ReplyKeyEnum ReplyControl { get; }

        /// <summary>
        /// Staticke ID otazky
        /// </summary>
        string StatID { get; }

        /// <summary>
        /// Maximalni povolena delka odpovedi (textbox, file)
        /// </summary>
        int MaxAllowedSize { get; }

        /// <summary>
        /// Muze byt vybrano vice polozek najednou? Rozliseni checkboxu a radiobuttonu
        /// </summary>
        bool IsMultiselect { get; }

        /// <summary>
        /// Je vyzadovana odpoved?
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Odpovedi se maji zobrazovat jen povolanym uzivatelum
        /// </summary>
        bool IsEncrypted { get; }

        /// <summary>
        /// Otazka je jen pro cteni
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Validacni regex vyraz nebo prazdny string
        /// </summary>
        string Regex { get; }

        /// <summary>
        /// f19hint
        /// </summary>
        string Hint { get; }

        /// <summary>
        /// f19supporting_text
        /// </summary>
        string SupportingText { get; }

        /// <summary>
        /// f19name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// f18id
        /// </summary>
        int IdSekce { get; }

        /// <summary>
        /// Je otazka zaskrtnuta jako publikovana?
        /// </summary>
        bool IsPublished { get; }

        /// <summary>
        /// True pokud je otazka v preview rezimu
        /// </summary>
        bool IsPreview { get; }

        /// <summary>
        /// True pokud otazka ma obsahovat checkbox pro publikovani
        /// </summary>
        bool CanBePublished { get; set; }

        /// <summary>
        /// Vraci nazev (filename) view pro dany typ otazky
        /// </summary>
        string ViewName { get; }

        /// <summary>
        /// Seznam moznych odpovedi k otazce
        /// </summary>
        List<BO.f21ReplyUnitJoinedF19> Odpovedi { get; }

        /// <summary>
        /// Seznam uzivatelem jiz vyplnenych odpovedi k otazce
        /// </summary>
        List<BO.f32FilledValue> VyplneneOdpovedi { get; }

        /// <summary>
        /// Seznam priloh k otazce
        /// </summary>
        List<BO.o27Attachment> Prilohy { get; }

        /// <summary>
        /// Ma byt otazka skryta?
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Validace odpovedi na danou otazku (z hlediska datovych typu)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True, pokud je odpoved validni nebo se jedna o prazdnou odpoved (value="")</returns>
        bool Validate(ref string value);

        /// <summary>
        /// Vraci odpoved na otazku jako string naformatovany pro ulozeni do databaze
        /// </summary>
        /// <param name="value">Odpoved na otazku</param>
        /// <returns>varchar pro ulozeni do databaze</returns>
        string CheckAnswerValue(ref string value);

        /// <summary>
        /// Vraci seznam CSS trid pro kontajner otazky
        /// </summary>
        string GetCssClasses();

        /// <summary>
        /// Vraci JSON string obsahujici informace o otazce
        /// </summary>
        string GetJsonData();
    }
}