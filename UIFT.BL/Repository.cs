using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using UIFT;

namespace UIFT.Repository
{
    public partial class Repository
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Vraci instanci udalosti
        /// </summary>
        public BO.a11EventForm GetBLEvent(int a11id)
        {
            return this.cache.a11EventFormBLLoad(a11id);
        }

        /// <summary>
        /// Vraci instanci BL formulare
        /// </summary>
        public BO.f06Form GetBLForm(int f06id)
        {
            return this.cache.f06FormBLLoad(f06id);
        }

        /// <summary>
        /// Ulozeni informace o tom, ze uzivatel videl obsah kryptovanych otazek
        /// </summary>
        public void LogEncryptedQuestionsView(List<BO.f32FilledValue> questions, string url)
        {
            this.BL.a11EventFormBL.AppendAccessToLog(questions, url);
        }

        /// <summary>
        /// Vraci seznam Best practices pro dany segment
        /// </summary>
        public IEnumerable<BO.f30BestPracticesToSegment> GetBestPractices(int f18id)
        {
            return this.cache.f18FormSegmentBLGetList_f30(f18id);
        }

        /// <summary>
        /// Shrnuti formulare.
        /// Seznam povinnych a nevyplnenych otazek.
        /// </summary>
        public Models.ShrnutiResult GetShrnuti(int f06id)
        {
            // natahnout segmenty - sekce
            IEnumerable<BO.f18FormSegment> segmenty = cache.f18FormSegmentBLGetList(f06id);

            // seznam vsech otazek
            IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(f06id, 0);

            // seznam vsech jiz vyplnenych odpovedi ve formulari
            IEnumerable<BO.f32FilledValue> vyplneneOdpovedi = cache.f32FilledValueBLGetList(this.a11id, 0);

            List<Models.ShrnutiResultItem> nevyplnenePovinneOtazky = new List<Models.ShrnutiResultItem>();

            // projit vsechny povinne otazky
            foreach (BO.f19Question otazka in otazky.Where(t => t.f19IsRequired || !string.IsNullOrEmpty(t.f19RequiredExpression)))
            {
                bool maOdpoved;
                
                // checkboxlist - aspon jedna odpoved musi byt zaskrtnuta
                if ((otazka.ReplyControl == BO.ReplyKeyEnum.Checkbox || otazka.ReplyControl == BO.ReplyKeyEnum.Listbox) && otazka.f19IsMultiselect)
                    maOdpoved = vyplneneOdpovedi.Count(t => t.f19ID == otazka.pid && t.Value == Configuration.FT_CheckboxAnswerTrueValue) > 0;
                else
                    maOdpoved = vyplneneOdpovedi.Count(t => t.f19ID == otazka.pid && !string.IsNullOrEmpty(t.Value)) > 0;
                
                // neni zadana odpoved
                if (!maOdpoved)
                {
                    // pokud neni zadana required expr.
                    if (string.IsNullOrEmpty(otazka.f19RequiredExpression) || factory.BoolEvaluator(this.a11id, otazka.f19RequiredExpression))
                    {
                        nevyplnenePovinneOtazky.Add(new Models.ShrnutiResultItem
                        {
                            SkipExpression = otazka.f19SkipExpression,
                            Otazka = otazka.f19Name,
                            OtazkaId = otazka.pid,
                            SekceId = otazka.f18ID,
                            Sekce = segmenty.First(t => t.pid == otazka.f18ID).f18Name
                        });
                    }
                }
            }

            // projit vsechny CancelValidateExpr
            foreach (BO.f19Question otazka in otazky.Where(t => !string.IsNullOrEmpty(t.f19CancelValidateExpression)))
            {
                // spatne vyplnena odpoved
                if (factory.BoolEvaluator(this.a11id, otazka.f19CancelValidateExpression))
                {
                    nevyplnenePovinneOtazky.Add(new Models.ShrnutiResultItem
                    {
                        SkipExpression = otazka.f19SkipExpression,
                        Otazka = otazka.f19Name,
                        OtazkaId = otazka.pid,
                        SekceId = otazka.f18ID,
                        Sekce = segmenty.First(t => t.pid == otazka.f18ID).f18Name,
                        Message = otazka.f19CancelValidateExpression_Message
                    });
                }
            }

            // projit vsechny otazky s definovanou regex podminkou
            foreach (BO.f19Question otazka in otazky.Where(t => !string.IsNullOrEmpty(t.f19Regex)))
            {
                
                BO.f32FilledValue odp = vyplneneOdpovedi.FirstOrDefault(t => t.f19ID == otazka.pid);
                if (odp != null)
                {
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(otazka.f19Regex);

                    if (!regex.IsMatch(odp.Value))
                    {
                        nevyplnenePovinneOtazky.Add(new Models.ShrnutiResultItem
                        {
                            SkipExpression = otazka.f19SkipExpression,
                            Otazka = otazka.f19Name,
                            OtazkaId = otazka.pid,
                            SekceId = otazka.f18ID,
                            Sekce = segmenty.First(t => t.pid == otazka.f18ID).f18Name,
                            Message = BL.tra("Otázka nemá správný formát: ") + otazka.f19Regex
                        });
                    }
                }
            }

            // projit vsechny povinne a nevyplnene otazky a zkontrolovat, zda nemaji zadanou skip/hide podminku
            for (int i = nevyplnenePovinneOtazky.Count - 1; i >= 0; i--)
            {
                // pokud je otazka skryta, nemuze byt povinna
                if (this.BL.BoolEvaluator(this.a11id, nevyplnenePovinneOtazky[i].SkipExpression))
                {
                    nevyplnenePovinneOtazky.RemoveAt(i);
                }
            }

            // vytvoreni vysledku
            Models.ShrnutiResult result = new Models.ShrnutiResult
            {
                Success  = nevyplnenePovinneOtazky.Count == 0,
                Items = nevyplnenePovinneOtazky
            };

            // zkontrolovat workflow status
            if (result.Success)
            {
                int a01ID = cache.a11EventFormBLLoad(this.a11id).a01ID;
                int b02ID = factory.a01EventBL.Load(a01ID).b02ID;
                
                result.WorkflowMessage = factory.b02WorkflowStatusBL.Load(b02ID).b02Message4UIFT;
            }

            return result;
        }

        /// <summary>
        /// Vraci instanci Base tridy formulare + prilohy k formulari.
        /// </summary>
        public Models.Formular GetFormularBase(int f06id)
        {
            Models.Formular formular = new Models.Formular
            {
                Base = cache.f06FormBLLoad(f06id)
            };

            if (formular.Base != null)
            {
                // seznam priloh
                formular.Prilohy = this.cache.o27AttachmentBLGetList(Cache.IdFields.f06id, f06id);

                return formular;
            }
            else
                return null;
        }

        /// <summary>
        /// Vraci instanci formulare s naplnenymi sekcemi a otazkami
        /// </summary>
        /// <param name="f06id">ID formulare</param>
        public Models.Formular GetFormular(int f06id)
        {
            Models.Formular formular = new Models.Formular
            {
                Base = cache.f06FormBLLoad(f06id),
                Tree = new List<Models.Sekce>(),
                Sekce = new List<Models.Sekce>(),
                Otazky = new List<Models.IOtazka>()
            };

            //log.Debug("GetFormular: " + formular.ToString());

            if (formular.Base != null)
            {
                // seznam priloh k formulari
                formular.Prilohy = this.cache.o27AttachmentBLGetList(Cache.IdFields.f06id, f06id);

                // seznam vsech otazek
                IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(formular.Base.pid, 0);

                // seznam vsech jiz vyplnenych odpovedi ve formulari
                IEnumerable<BO.f32FilledValue> vyplneneOdpovedi = cache.f32FilledValueBLGetList(this.a11id, 0);
                
                // seznam vsech Jednotek odpovedi
                IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi = cache.f21ReplyUnitBLGetListJoinedF19(formular.Base.pid);

                // projit otazky a pridat do formulare
                foreach (BO.f19Question otazka in otazky.Where(t => t.f25ID == 0 && t.f26ID == 0))
                {
                    formular.Otazky.Add(Models.OtazkaFactory.Get(this, otazka, null, vyplneneOdpovedi, odpovedi, null));
                }

                #region chess a baterie
                // zpracovat sachovnici otazek
                int[] sachovniceIds = otazky.Where(t => t.f25ID != 0).Select(t => t.f25ID).Distinct().ToArray();
                if (sachovniceIds != null)
                {
                    foreach (int sachovniceId in sachovniceIds)
                    {
                        // informace o sachovnici
                        BO.f25ChessBoard chessboard = cache.f25ChessBoardBLLoad(sachovniceId);

                        formular.Otazky.Add(Models.OtazkaFactory.Get(this, chessboard, otazky, vyplneneOdpovedi, odpovedi, null, f06id, false));
                    }
                }

                // zpracovat baterii otazek
                int[] baterieIds = otazky.Where(t => t.f26ID != 0).Select(t => t.f26ID).Distinct().ToArray();
                if (baterieIds != null)
                {
                    foreach (int baterieId in baterieIds)
                    {
                        BO.f26BatteryBoard baterie = cache.f26BatteryBoardBLLoad(baterieId);

                        formular.Otazky.Add(Models.OtazkaFactory.Get(this, baterie, otazky, vyplneneOdpovedi, odpovedi, null, f06id, false));
                    }
                }
                #endregion

                // natahnout segmenty - sekce
                IEnumerable<BO.f18FormSegment> segmenty = cache.f18FormSegmentBLGetList(formular.Base.pid);
                // projit sekce, priradit jim otazky a pridat je do Modelu
                foreach (BO.f18FormSegment sekce in segmenty)
                {
                    Models.Sekce sekceDerived = new Models.Sekce
                    {
                        Base = sekce,
                        // vazba na otazky
                        Otazky = formular.Otazky.FindAll(t => t.IdSekce == sekce.pid)
                    };

                    formular.Sekce.Add(sekceDerived);
                }

                // vytvorit stromovou strukturu sekci
                foreach (Models.Sekce sekce in formular.Sekce)
                {
                    // vnorene sekce
                    sekce.SubSekce = formular.Sekce.FindAll(t => t.Base.f18ParentID == sekce.Base.pid);

                    // ma byt sekce skryta?
                    sekce.IsHidden = sekce.Otazky.Any() && sekce.Otazky.All(t => t.IsHidden) && sekce.SubSekce.Count == 0;

                    // pokud se jedna o top level node, zarad ho do formulare
                    if (sekce.Base.f18ParentID <= 0)
                        formular.Tree.Add(sekce);
                }

                // zjistit jakou otazku uzivatel naposledy upravoval
                /*Models.IOtazka posledniOtazka = formular.Otazky.Where(t => t.Base.UserUpdate == this.user.j03Login).OrderByDescending(t => t.Base.DateUpdate).First();
                if (posledniOtazka == null)
                    formular.AktualniSekce = 0;
                else
                    formular.AktualniSekce = posledniOtazka.pid;*/
                formular.AktualniSekce = 0;//!!!Remove

                return formular;
            }
            
            return formular;
        }

        /// <summary>
        /// Vraci instanci objektu sekce (seznam otazek) + dalsi info
        /// </summary>
        /// <param name="f18id">ID sekce</param>
        /// <param name="f06id">ID formulare (kvuli kesovani dat)</param>
        /// <returns></returns>
        public Models.Sekce GetSekce(int f18id, int f06id = 0, bool isPreview = false)
        {
            Models.Sekce sekce = new Models.Sekce
            {
                // instance sekce
                Base = cache.f18FormSegmentBLLoad(f18id, f06id),
                // prazdny kontajner na otazky
                Otazky = new List<Models.IOtazka>(),
                BestPractices = this.GetBestPractices(f18id)
            };
            
            if (sekce.Base != null) // sekce existuje
            {
                // pokud ma sekce nastavenou readonly expression
                if (!string.IsNullOrEmpty(sekce.Base.f18ReadonlyExpression))
                {
                    // a pokud je podminka true, nastav otazky readonly
                    sekce.IsReadOnly = this.BL.BoolEvaluator(this.a11id, sekce.Base.f18ReadonlyExpression);
                }

                // seznam priloh k sekci
                sekce.Prilohy = cache.o27AttachmentBLGetList(Cache.IdFields.f18id, sekce.Base.pid);

                // seznam priloh k otazkam v sekci
                IEnumerable<BO.o27Attachment> otPrilohy = cache.o27AttachmentBLGetListSegmentQ(sekce.Base.pid);

                // seznam vsech otazek v sekci
                IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(f06id, sekce.Base.pid);
                
                // seznam vsech jiz vyplnenych odpovedi v sekci
                IEnumerable<BO.f32FilledValue> vyplneneOdpovedi = cache.f32FilledValueBLGetList(this.a11id, sekce.Base.pid);

                // seznam vsech Jednotek odpovedi
                IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi = cache.f21ReplyUnitBLGetListJoinedF19(f06id, sekce.Base.pid);

                // zjistit stav publikovani otazek
                IEnumerable<BO.f31FilledQuestionPublishing> publikovane = cache.f31FilledQuestionPublishingBLGetList(this.a11id, sekce.Base.pid);

                // vlozit otazky, ktere nejsou sachovnici, do noveho modelu
                List<Models.IOtazka> model = new List<Models.IOtazka>();
                foreach (BO.f19Question otazka in otazky.Where(t => t.f25ID == 0 && t.f26ID == 0))
                {
                    sekce.Otazky.Add(Models.OtazkaFactory.Get(this, otazka, otPrilohy, vyplneneOdpovedi, odpovedi, publikovane, f06id, isPreview));
                }
                
                // zpracovat sachovnici otazek
                int[] sachovniceIds = otazky.Where(t => t.f25ID != 0).Select(t => t.f25ID).Distinct().ToArray();
                if (sachovniceIds != null)
                {
                    foreach (int sachovniceId in sachovniceIds)
                    {
                        // informace o sachovnici
                        BO.f25ChessBoard chessboard = cache.f25ChessBoardBLLoad(sachovniceId);

                        sekce.Otazky.Add(Models.OtazkaFactory.Get(this, chessboard, otazky, vyplneneOdpovedi, odpovedi, publikovane, f06id, isPreview));
                    }
                }

                // zpracovat baterii otazek
                int[] baterieIds = otazky.Where(t => t.f26ID != 0).Select(t => t.f26ID).Distinct().ToArray();
                if (baterieIds != null)
                {
                    foreach (int baterieId in baterieIds)
                    {
                        BO.f26BatteryBoard baterie = cache.f26BatteryBoardBLLoad(baterieId);

                        sekce.Otazky.Add(Models.OtazkaFactory.Get(this, baterie, otazky, vyplneneOdpovedi, odpovedi, publikovane, f06id, isPreview));
                    }
                }

                // seradit otazky podle poradi
                sekce.Otazky.Sort((a, b) => a.Order.CompareTo(b.Order));

                // pokud ma sekce nastavenou readonly expression a pokud je podminka true, nastav otazky readonly
                if (sekce.IsReadOnly)
                {
                    foreach (Models.IOtazka o in sekce.Otazky)
                        o.ReadOnly = true;
                }

                return sekce;
            }
            else
                return null;
        }

        /// <summary>
        /// Vraci instanci tridy Otazka
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <param name="f06id">ID formulare (pro kesovani)</param>
        /// <param name="fullDetail">Pokud je nastaveno na true, vrati vsechna data o otazce a vyplnenych odpovedich</param>
        /// <param name="isPreview">True pokud se jedna o preview rezim otazky.</param>
        /// <returns>Objekt otazka nebo null</returns>
        public Models.IOtazka GetOtazka(int f19id, int f06id = 0, bool fullDetail = false, bool isPreview = false)
        {
            BO.f19Question otazka = cache.f19QuestionBLLoad(f19id, f06id);

            if (otazka == null)
            {
                return null;
            }
            else
            {
                // seznam vsech Jednotek odpovedi
                IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi = cache.f21ReplyUnitBLGetListJoinedF19(f06id, 0, f19id);

                if (fullDetail)
                {
                    // seznam priloh k otazkam v sekci
                    IEnumerable<BO.o27Attachment> otPrilohy = this.BL.o27AttachmentBL.GetList(new BO.myQueryO27
                    {
                        //a11id = this.a11id,
                        f19id = f19id
                    }, "");

                    // seznam vsech jiz vyplnenych odpovedi v sekci
                    IEnumerable<BO.f32FilledValue> vyplneneOdpovedi = this.BL.f32FilledValueBL.GetList(new BO.myQueryF32
                    {
                        a11id = this.a11id,
                        f19id = f19id
                    });
                    
                    return Models.OtazkaFactory.Get(this, otazka, otPrilohy, vyplneneOdpovedi, odpovedi, null, f06id, isPreview);
                }
                else
                {
                    return Models.OtazkaFactory.Get(this, otazka, null, null, odpovedi, null, f06id, isPreview);
                }
            }
        }

        /// <summary>
        /// Ulozeni komentare k odpovedi
        /// </summary>
        /// <param name="f19id"></param>
        /// <param name="f21id"></param>
        /// <param name="value"></param>
        public bool SaveKomentar(ref int f19id, ref int f21id, ref string value)
        {
            BO.f32FilledValue v = new BO.f32FilledValue
            {
                a11ID = this.a11id,
                f19ID = f19id,
                f21ID = f21id
            };
            bool b = this.BL.f32FilledValueBL.SaveComment(v, value) > 0;
            
            this._LastError = b ? "" : BL.CurrentUser.Messages4Notify.ToSingleLine();
            return b;
        }

        /// <summary>
        /// Odstrani vsechny dosud ulozene odpovedi k otazce
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <returns>True pokud se vycisteni povedlo</returns>
        public bool VycisteniOtazky(int f19id)
        {
            bool b = this.BL.f32FilledValueBL.DeleteAllInF19(f19id, this.a11id);

            this._LastError = b ? "" : BL.CurrentUser.Messages4Notify.ToSingleLine();
            return b;
        }

        /// <summary>
        /// Odstrani vsechny dosud ulozene odpovedi k baterii otazek
        /// </summary>
        /// <param name="f26id">ID baterie</param>
        /// <returns>True pokud se vycisteni povedlo</returns>
        public bool VycisteniBaterie(int f26id)
        {
            bool b = this.BL.f32FilledValueBL.DeleteAllInF26(f26id, this.a11id);

            this._LastError = b ? "" : BL.CurrentUser.Messages4Notify.ToSingleLine();
            return b;
        }

        /// <summary>
        /// Zapnuti / vypnuti publikovani otazky
        /// </summary>
        /// <param name="f19id">ID otazky</param>
        /// <param name="publikovat">True pokud se ma publikovat otazka</param>
        public bool PublikovaniOtazky(int f25id, int f26id, int f19id, bool publikovat)
        {
            bool b = this.BL.f31FilledQuestionPublishingBL.Save(new BO.f31FilledQuestionPublishing
            {
                a11ID = this.a11id,
                f19ID = f19id,
                f25ID = f25id,
                f26ID = f26id,
                f31IsPublished = publikovat
            }) > 0;

            this._LastError = b ? "" : BL.CurrentUser.Messages4Notify.ToSingleLine();
            return b;
        }

        /// <summary>
        /// Ulozeni jednotlive odpovedi na otazku
        /// </summary>
        /// <param name="otazka">Instance aktualni otazky</param>
        /// <param name="f21id"></param>
        /// <param name="value">Odpoved zadana uzivatelem</param>
        /// <param name="alias">Text odpovedi v pripade EvalListu</param>
        /// <returns>True pokud bylo mozne odpoved zvalidovat. Pokud =false, dalsi informace jsou v this.LastError</returns>
        public bool SaveOtazka(Models.IOtazka otazka, ref int f21id, ref string value, string alias, bool filledByEval)
        {
            // pokud se jedna o dropdown a zaroven byla vybrana odpoved --vyberte--, smaz ulozene odpovedi
            if ((otazka.ReplyControl == BO.ReplyKeyEnum.DropdownList && f21id == 0)
                || (otazka.ReplyControl == BO.ReplyKeyEnum.Checkbox && value == ""))
            {
                this.BL.f32FilledValueBL.DeleteAllInF19(otazka.PID, this.a11id);
                return true;
            }
            else
            {
                // vytvorit instanci odpovedi
                BO.f32FilledValue odpoved = new BO.f32FilledValue
                {
                    a11ID = this.a11id,
                    f19ID = otazka.PID,
                    f21ID = f21id,
                    f32IsFilledByEval = filledByEval
                };

                // true v pripade, ze odpoved ma spravny tvar vzhledem k nastaveni otazky
                if (!otazka.Validate(ref value))
                {
                    this._LastError = BL.tra("Odpověď nemá správný formát!");
                    return false;
                }

                // true pokud se maji smazat predchozi odpovedi - pouze pro radio buttony
                bool deleteAnswers = otazka.ReplyControl == BO.ReplyKeyEnum.RadiobuttonList
                    || otazka.ReplyControl == BO.ReplyKeyEnum.DropdownList
                    || (otazka.ReplyControl == BO.ReplyKeyEnum.Listbox && !otazka.IsMultiselect);

                // v pripade EvalListu
                if (otazka.ReplyControl == BO.ReplyKeyEnum.EvalList)
                {
                    odpoved.f32ValueAliasEvalList = alias;
                }

                // zformatovany retezec pro ulozeni do db
                odpoved.Value = otazka.CheckAnswerValue(ref value);

                // ulozeni odpovedi
                bool ret = this.BL.f32FilledValueBL.Save(odpoved, deleteAnswers) > 0;
                
                // pokud se nepovedlo ulozit, tak proc?
                this._LastError = ret ? "" : BL.CurrentUser.Messages4Notify.ToSingleLine();

                return ret;
            }
        }

        /// <summary>
        /// Vraci seznam otazek, ktere maji byt nastaveny jako readonly pro dany segment
        /// </summary>
        /// <param name="f18id">ID segmentu</param>
        /// <param name="formularId">f06id kvuli kesovani</param>
        /// <returns>Pole f19id</returns>
        public List<int> SegmentReadonlyOtazky(int f18id, int formularId)
        {
            List<int> list = new List<int>();
            // vyber vsechny otazky v danem segmentu, ktere maji zadanou readonly expr.
            IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(formularId, f18id).Where(t => !string.IsNullOrEmpty(t.f19ReadonlyExpression));

            foreach (BO.f19Question otazka in otazky)
            {
                // spust evaluator na readonly
                if (factory.BoolEvaluator(this.a11id, otazka.f19ReadonlyExpression))
                    list.Add(otazka.pid);
            }

            return list;
        }

        /// <summary>
        /// Vraci seznam otazek, ktere maji byt nastaveny jako required pro dany segment
        /// </summary>
        /// <param name="f18id">ID segmentu</param>
        /// <param name="formularId">f06id kvuli kesovani</param>
        /// <returns>Pole f19id</returns>
        public List<int> SegmentRequiredOtazky(int f18id, int formularId)
        {
            List<int> list = new List<int>();
            // vyber vsechny otazky v danem segmentu, ktere maji zadanou required expr.
            IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(formularId, f18id).Where(t => !string.IsNullOrEmpty(t.f19RequiredExpression));

            foreach (BO.f19Question otazka in otazky)
            {
                // spust evaluator na readonly
                if (factory.BoolEvaluator(this.a11id, otazka.f19RequiredExpression))
                    list.Add(otazka.pid);
            }

            return list;
        }

        /// <summary>
        /// Vraci vsechny otazky ve formulari jejichz f19SkipExpr vratilo true, tedy maji byt skryte.
        /// </summary>
        public List<int> FormularSkryteOtazky(int formularId, out int[] skryteSegmenty) {
            // vsechny otazky ve formulari
            IEnumerable<BO.f19Question> otazky = cache.f19QuestionBLGetList(formularId);
            
            // parovani otazka -> segment
            // vytvori slovnik s klicem <f19id, pocet_otazek_v_segmentu>
            Dictionary<int, int> pocetOtazekVsegmentu = new Dictionary<int, int>();
            foreach (BO.f19Question otazka in otazky)
            {
                if (pocetOtazekVsegmentu.ContainsKey(otazka.f18ID))
                    pocetOtazekVsegmentu[otazka.f18ID] += 1;
                else
                    pocetOtazekVsegmentu.Add(otazka.f18ID, 1);
            }

            List<int> skryteOtazky = new List<int>();

            foreach (BO.f19Question otazka in otazky.Where(t => !string.IsNullOrEmpty(t.f19SkipExpression)))
            {
                // spust evaluator na default value
                if (factory.BoolEvaluator(this.a11id, otazka.f19SkipExpression))
                {
                    skryteOtazky.Add(otazka.pid);
                    // odebrat otazku se segmentu
                    pocetOtazekVsegmentu[otazka.f18ID] -= 1;
                }
            }

            skryteSegmenty = pocetOtazekVsegmentu.Where(t => t.Value == 0).Select(t => t.Key).ToArray();
            return skryteOtazky;
        }

        /// <summary>
        /// Funkce generuje default odpovedi pro danou otazku.
        /// </summary>
        /// <returns>Odpoved, pro ktere byla vygenerovana nova default value.</returns>
        public BO.f32FilledValue OtazkaGenerovatDefaultValues(BO.f19Question otazka, IEnumerable<BO.f21ReplyUnitJoinedF19> odpovedi, IEnumerable<BO.f32FilledValue> vyplneneOdpovedi, bool saveAnswerToDB = true)
        {
            DefaultValuesGenerator generator = new DefaultValuesGenerator(this);
            return generator.PerQuestion(otazka, odpovedi, vyplneneOdpovedi, saveAnswerToDB);
        }

        /// <summary>
        /// Funkce projde vsechny otazky ve formulari, zjisti pouze ty s nastavenou Default Value Expr, vyhodnoti a ulozi default value.
        /// Pouze pro otazky, ktere jiz nemaji odpoved ulozenou clovekem.
        /// </summary>
        /// <returns>Seznam odpovedi, pro ktere byla vygenerovana nova default value.</returns>
        public List<BO.f32FilledValue> FormularGenerovatDefaultValues(int formularId)
        {
            DefaultValuesGenerator generator = new DefaultValuesGenerator(this);
            return generator.PerForm(formularId);
        }

        /// <summary>
        /// Vraci true, pokud je otazka ReadOnly
        /// </summary>
        public bool IsQuestionReadOnly(Models.IOtazka otazka)
        {
            if (otazka is Models.Otazka)
            {
                Models.Otazka ot = otazka as Models.Otazka;
                if (!string.IsNullOrEmpty(ot.Base.f19ReadonlyExpression))
                {
                    return factory.BoolEvaluator(this.a11id, ot.Base.f19ReadonlyExpression);
                }
            }
            return false;
        }
    }
}