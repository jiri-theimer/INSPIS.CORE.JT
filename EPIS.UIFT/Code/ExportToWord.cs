using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFT
{
    public class ExportToWord
    {
        // nazvy stylu v sablone wordu
        private const string StyleName_NazevOtazky = "QuestionName";
        private const string StyleName_NapovedaOdpovedi = "AnswerHint";
        private const string StyleName_SupportingText = "SupportingText";// char style
        private const string StyleName_TableHeader = "TableHeader";// char style
        private const string StyleName_Comment = "Comment";
        private const string StyleName_SegmentText = "SegmentText";
        private const string StyleName_Hint = "Hint";// char style
        private const string StyleName_SegmentTitle = "SegmentTitle";
        private const string StyleName_SegmentSupportingText = "SegmentSupportingText";

        /// <summary>
        /// Bookmark s nazev formulare
        /// </summary>
        private const string Bookmark_FormName = "nazev";

        private WordprocessingDocument wordprocessingDocument;
        private Repository.Repository _Repository;

        public ExportToWord(Repository.Repository repository)
        {
            _Repository = repository;
        }

        private Paragraph CreatePar(string style)
        {
            Paragraph p1 = new Paragraph();
            p1.PrependChild<ParagraphProperties>(new ParagraphProperties(new ParagraphStyleId() { Val = style }));
            wordprocessingDocument.MainDocumentPart.Document.Body.AppendChild(p1);
            return p1;
        }

        private Paragraph CreatePar(string style, string text)
        {
            var p1 = CreatePar(style);
            p1.AppendChild(new Run(new Text(text)));
            return p1;
        }

        private void CreateRun(Paragraph par, string style, string text, bool addBreak = true)
        {
            Run run = new Run(new Text(text));
            run.PrependChild<RunProperties>(new RunProperties(new RunStyle() { Val = style }));
            par.AppendChild(run);
            if (addBreak)
                par.AppendChild(new Break());
        }

        public void Run(Models.Formular formular, bool showAnswers, System.IO.Stream stream)
        {
            this.wordprocessingDocument = WordprocessingDocument.Open(stream, true);

            wordprocessingDocument.MainDocumentPart.Document.Body.RemoveAllChildren();
            
            // projit sekce
            foreach (Models.Sekce sekce in formular.Sekce)
            {
                CreatePar(StyleName_SegmentTitle, sekce.Base.f18Name);

                // popis sekce
                if (!string.IsNullOrEmpty(sekce.Base.f18Text))
                    CreatePar(StyleName_SegmentText, sekce.Base.f18Text);

                // supporting text
                if (!string.IsNullOrEmpty(sekce.Base.f18SupportingText))
                    CreatePar(StyleName_SegmentSupportingText, sekce.Base.f18SupportingText);

                // najit otazky pro sekci
                foreach (UIFT.Models.IOtazka otazka in formular.Otazky.FindAll(t => t.IdSekce == sekce.Base.pid))
                {
                    // otazky, ktere se netisknou
                    if (otazka.ReplyControl == BO.ReplyKeyEnum.Button)
                        continue;

                    // hlavicka otazky
                    BuildQuestionHeader(otazka);
                    
                    // telo otazky - tj. odpovedi
                    BuildQuestionBody(otazka, showAnswers);

                    // pokud otazka obsahuje moznost komentare
                    if (otazka.Odpovedi != null)
                    {
                        if (otazka.Odpovedi.Exists(t => t.f21IsCommentAllowed))
                        {
                            BO.f32FilledValue fillComm = otazka.VyplneneOdpovedi.FirstOrDefault(t => t.f21ID == otazka.Odpovedi.Find(t2 => t2.f21IsCommentAllowed).pid);
                            if (fillComm != null && showAnswers)
                                BuildComment(fillComm.f32Comment);
                            else
                                BuildComment();
                        }
                    }

                    wordprocessingDocument.MainDocumentPart.Document.Body.AppendChild(new Paragraph());
                }
            }

            wordprocessingDocument.Close();
        }

        /// <summary>
        /// Vytvori telo otazky, tj. jednotlive odpovedi
        /// </summary>
        private void BuildQuestionBody(Models.IOtazka otazka, bool showAnswers)
        {
            switch (otazka.ReplyControl)
            {
                case BO.ReplyKeyEnum.FileUpload:
                    if (otazka.VyplneneOdpovedi.Count > 0 && showAnswers)
                        BuildTextArea(1, otazka.VyplneneOdpovedi[0].Value);
                    else
                        BuildTextArea(1);
                    break;

                case BO.ReplyKeyEnum.RadiobuttonList:
                case BO.ReplyKeyEnum.DropdownList:
                    BuildAnswerHint("Zaškrtněte jednu odpověď.");
                    // jednotlive odpovedi
                    BuildListAnswers(otazka.Odpovedi, showAnswers ? otazka.VyplneneOdpovedi : null);
                    break;

                case BO.ReplyKeyEnum.Listbox:
                case BO.ReplyKeyEnum.Checkbox:
                    if (((Models.Otazka)otazka).Base.f19IsMultiselect) // checkboxy
                    {
                        BuildAnswerHint("Můžete zaškrtnout i více odpovědí najednou.");
                    }
                    else // radio boxy
                    {
                        BuildAnswerHint("Zaškrtněte jednu odpověď.");
                    }

                    // jednotlive odpovedi
                    BuildListAnswers(otazka.Odpovedi, showAnswers ? otazka.VyplneneOdpovedi : null);
                    break;

                case BO.ReplyKeyEnum.TextBox:
                    Models.Otazka otazka2 = (Models.Otazka)otazka;

                    // slozit napovedu pro min a max hodnoty
                    #region slozit napovedu pro min a max hodnoty
                    if (!string.IsNullOrEmpty(otazka2.Base.TextBox_MinValue) || !string.IsNullOrEmpty(otazka2.Base.TextBox_MaxValue))
                    {
                        string hint = "", minVal = "", maxVal = "";
                        switch (otazka.ReplyType)
                        {
                            case BO.x24DataTypeEnum.tDecimal:
                            case BO.x24DataTypeEnum.tInteger:
                                if (!string.IsNullOrEmpty(otazka2.Base.TextBox_MinValue))
                                    minVal = otazka2.Base.TextBox_MinValue.ConvertFromBL().ToString(_Repository.BL.GlobalParams.LoadParam("UIFT_DateFormat"));
                                if (!string.IsNullOrEmpty(otazka2.Base.TextBox_MaxValue))
                                    maxVal = otazka2.Base.TextBox_MaxValue.ConvertFromBL().ToString(_Repository.BL.GlobalParams.LoadParam("UIFT_DateFormat"));
                                break;
                            default:
                                minVal = otazka2.Base.TextBox_MinValue;
                                maxVal = otazka2.Base.TextBox_MaxValue;
                                break;
                        }

                        if (!string.IsNullOrEmpty(minVal))
                            hint = "Min. hodnota: " + minVal;
                        if (!string.IsNullOrEmpty(maxVal))
                        {
                            if (hint != "")
                                hint += ", ";
                            hint = "Max. hodnota: " + maxVal;
                        }
                        BuildAnswerHint(hint);
                    }
                    #endregion

                    if (otazka.VyplneneOdpovedi.Count > 0 && showAnswers)
                    {
                        string txt = otazka.VyplneneOdpovedi[0].Value;
                        switch (otazka.ReplyType)
                        {
                            case BO.x24DataTypeEnum.tDateTime:
                                txt = txt.ConvertFromBL().ToString(_Repository.BL.GlobalParams.LoadParam("UIFT_DateTimeFormat"));
                                break;
                            case BO.x24DataTypeEnum.tDate:
                                txt = txt.ConvertFromBL().ToString(_Repository.BL.GlobalParams.LoadParam("UIFT_DateFormat"));
                                break;
                        }
                        BuildTextArea(1, txt);
                    }
                    else
                        BuildTextArea(1);
                    break;

                // html
                case BO.ReplyKeyEnum.HtmlEditor:
                    if (otazka.VyplneneOdpovedi.Count > 0 && showAnswers)
                        BuildTextArea(defaultText: otazka.VyplneneOdpovedi[0].Value);
                    else
                        BuildTextArea();
                    break;
            }
        }

        /// <summary>
        /// Vytvori seznam odpovedi v Bullet listu
        /// </summary>
        private void BuildListAnswers(List<BO.f21ReplyUnitJoinedF19> odpovedi, List<BO.f32FilledValue> vyplneneOdpovedi)
        {
            Paragraph par = new Paragraph();
            par.PrependChild<ParagraphProperties>(new ParagraphProperties()
            {
                ContextualSpacing = new ContextualSpacing() { Val = OnOffValue.FromBoolean(true) }
            });

            var runList = new List<Run>();
            foreach (var odpoved in odpovedi)
            {
                var newRun = new Run();
                    
                if (vyplneneOdpovedi != null)
                {
                    BO.f32FilledValue val = vyplneneOdpovedi.FirstOrDefault(t => t.f21ID == odpoved.pid);
                    if (val != null)
                    {
                        if (val.Value == _Repository.BL.GlobalParams.LoadParam("FT_CheckboxAnswerTrueValue"))
                            newRun.PrependChild<RunProperties>(new RunProperties() { Bold = new Bold() { Val = new OnOffValue(true) }, Color = new Color() { Val = "green" } });
                    }
                }

                newRun.AppendChild(new Text(odpoved.f21Name));
                runList.Add(newRun);
            }

            AddBulletList(runList);
        }

        public void AddBulletList(List<Run> runList)
        {
            // Introduce bulleted numbering in case it will be needed at some point
            NumberingDefinitionsPart numberingPart = wordprocessingDocument.MainDocumentPart.NumberingDefinitionsPart;
            if (numberingPart == null)
            {
                numberingPart = wordprocessingDocument.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>("NumberingDefinitionsPart001");
                Numbering element = new Numbering();
                element.Save(numberingPart);
            }

            // Insert an AbstractNum into the numbering part numbering list.  The order seems to matter or it will not pass the 
            // Open XML SDK Productity Tools validation test.  AbstractNum comes first and then NumberingInstance and we want to
            // insert this AFTER the last AbstractNum and BEFORE the first NumberingInstance or we will get a validation error.
            var abstractNumberId = numberingPart.Numbering.Elements<AbstractNum>().Count() + 1;
            var abstractLevel = new Level(new NumberingFormat() { Val = NumberFormatValues.Bullet }, new LevelText() { Val = "·" }) { LevelIndex = 0 };
            var abstractNum1 = new AbstractNum(abstractLevel) { AbstractNumberId = abstractNumberId };

            if (abstractNumberId == 1)
            {
                numberingPart.Numbering.Append(abstractNum1);
            }
            else
            {
                AbstractNum lastAbstractNum = numberingPart.Numbering.Elements<AbstractNum>().Last();
                numberingPart.Numbering.InsertAfter(abstractNum1, lastAbstractNum);
            }

            // Insert an NumberingInstance into the numbering part numbering list.  The order seems to matter or it will not pass the 
            // Open XML SDK Productity Tools validation test.  AbstractNum comes first and then NumberingInstance and we want to
            // insert this AFTER the last NumberingInstance and AFTER all the AbstractNum entries or we will get a validation error.
            var numberId = numberingPart.Numbering.Elements<NumberingInstance>().Count() + 1;
            NumberingInstance numberingInstance1 = new NumberingInstance() { NumberID = numberId };
            AbstractNumId abstractNumId1 = new AbstractNumId() { Val = abstractNumberId };
            numberingInstance1.Append(abstractNumId1);

            if (numberId == 1)
            {
                numberingPart.Numbering.Append(numberingInstance1);
            }
            else
            {
                var lastNumberingInstance = numberingPart.Numbering.Elements<NumberingInstance>().Last();
                numberingPart.Numbering.InsertAfter(numberingInstance1, lastNumberingInstance);
            }

            Body body = wordprocessingDocument.MainDocumentPart.Document.Body;

            foreach (Run runItem in runList)
            {
                // Create items for paragraph properties
                var numberingProperties = new NumberingProperties(new NumberingLevelReference() { Val = 0 }, new NumberingId() { Val = numberId });
                var spacingBetweenLines1 = new SpacingBetweenLines() { After = "0" };  // Get rid of space between bullets
                var indentation = new Indentation() { Left = "720", Hanging = "360" };  // correct indentation 

                ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
                RunFonts runFonts1 = new RunFonts() { Ascii = "Symbol", HighAnsi = "Symbol" };
                paragraphMarkRunProperties1.Append(runFonts1);

                // create paragraph properties
                var paragraphProperties = new ParagraphProperties(numberingProperties, spacingBetweenLines1, indentation, paragraphMarkRunProperties1);

                // Create paragraph 
                var newPara = new Paragraph(paragraphProperties);

                // Add run to the paragraph
                newPara.AppendChild(runItem);

                // Add one bullet item to the body
                body.AppendChild(newPara);
            }
        }

        /// <summary>
        /// Vlozi informativni hlasku pro upresneni zpusobu vyplneni odpovedi
        /// </summary>
        /// <param name="text">Text hlasky</param>
        private void BuildAnswerHint(string text)
        {
            CreatePar(StyleName_NapovedaOdpovedi, text);
        }

        /// <summary>
        /// Vytvoreni hlavicky otazky
        /// </summary>
        private void BuildQuestionHeader(Models.IOtazka otazka)
        {
            // nazev otazky
            var par = CreatePar(StyleName_NazevOtazky, otazka.Name);

            // supporting text
            if (!string.IsNullOrEmpty(otazka.SupportingText))
                CreateRun(par, otazka.SupportingText, StyleName_SupportingText);

            // hint
            if (!string.IsNullOrEmpty(otazka.Hint))
                CreateRun(par, otazka.Hint, StyleName_Hint);
        }

        /// <summary>
        /// Vytvari misto pro komentar k otazce
        /// </summary>
        private void BuildComment(string defaultText = "")
        {
            CreatePar(StyleName_Comment, "Komentář k otázce:");
            
            BuildTextArea(2, defaultText);
        }

        /// <summary>
        /// Vytvori prazdnou radku s teckovanym bottom border
        /// </summary>
        /// <param name="rows">Pocet radek k vytvoreni</param>
        private void BuildTextArea(int rows = 3, string defaultText = "")
        {
            Paragraph p1 = new Paragraph();
            p1.PrependChild<ParagraphProperties>(new ParagraphProperties()
            {
                ContextualSpacing = new ContextualSpacing() { Val = OnOffValue.FromBoolean(true) },
                ParagraphBorders = new ParagraphBorders(
                    new BottomBorder() { Val = BorderValues.Dotted }
                )
            });

            if (!string.IsNullOrEmpty(defaultText))
            {
                var p = add();
                p.AppendChild(new Run(new Text(defaultText)));
                wordprocessingDocument.MainDocumentPart.Document.Body.AppendChild(p);
            }

            // vlozit odpovidajici pocet radku
            for (int i = 0; i < rows; i++)
                wordprocessingDocument.MainDocumentPart.Document.Body.AppendChild(add());

            Paragraph add()
            {
                Paragraph p1 = new Paragraph();
                p1.PrependChild<ParagraphProperties>(new ParagraphProperties()
                {
                    ContextualSpacing = new ContextualSpacing() { Val = OnOffValue.FromBoolean(true) },
                    ParagraphBorders = new ParagraphBorders(
                        new BottomBorder() { Val = BorderValues.Dotted }
                    )
                });
                return p1;
            }
        }
    }
}