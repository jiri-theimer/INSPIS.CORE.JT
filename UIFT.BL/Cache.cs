using System;
using System.Linq;
using System.Collections.Generic;

namespace UIFT.Repository
{
    internal class Cache
    {
        /// <summary>
        /// Pocet minut platnosti cache
        /// </summary>
        private const int _cacheTimeout = 15;

        public enum IdFields
        {
            f06id,
            f18id
        }

        // ulozena instance BL factory tridy
        private BL.Factory factory;

        // set to false for disable caching
        public bool enabled;

        internal Cache(BL.Factory factory, bool enable)
        {
            this.enabled = enable;
            this.factory = factory;
        }

        #region a11
        public BO.a11EventForm a11EventFormBLLoad(int a11id)
        {
            return factory.a11EventFormBL.Load(a11id);
        }
        #endregion

        #region f06
        public BO.f06Form f06FormBLLoad(int f06id)
        {
            return factory.f06FormBL.Load(f06id);
        }
        #endregion

        #region f18
        /// <summary>
        /// Signle segment
        /// </summary>
        /// <param name="f06id">Pokud je > 0, pak je segment hledan i v kesi pro dany formular.</param>
        public BO.f18FormSegment f18FormSegmentBLLoad(int f18id, int cache_f06id = 0)
        {
            return factory.f18FormSegmentBL.Load(f18id);
        }

        /// <summary>
        /// Seznam segmentu
        /// </summary>
        public IEnumerable<BO.f18FormSegment> f18FormSegmentBLGetList(int f06id)
        {
            return factory.f18FormSegmentBL.GetList(new BO.myQuery("f18")
            {
                f06id = f06id
            });
        }
        #endregion

        #region f19
        /// <summary>
        /// Single otazka
        /// </summary>
        /// <param name="f18id">Pokud je > 0, pak je otazka hledana i v kesi pro dany segment.</param>
        public BO.f19Question f19QuestionBLLoad(int f19id, int cache_f06id = 0)
        {
            return factory.f19QuestionBL.Load(f19id);
        }

        /// <summary>
        /// Seznam otazek
        /// </summary>
        public IEnumerable<BO.f19Question> f19QuestionBLGetList(int f06id, int f18id = 0)
        {
            BO.myQuery query = null;
            if (f18id > 0)
            {
                query = new BO.myQuery("f19")
                {
                    f18id = f18id
                };
            }
            else if (f06id > 0)
            {
                query = new BO.myQuery("f19")
                {
                    f06id = f06id
                };
            }

            return factory.f19QuestionBL.GetList(query);
        }
        #endregion

        #region f21
        public IEnumerable<BO.f21ReplyUnitJoinedF19> f21ReplyUnitBLGetListJoinedF19(int f06id, int f18id = 0, int f19id = 0)
        {
            BO.myQuery query = null;
            if (f18id > 0)
            {
                query = new BO.myQuery("f21")
                {
                    f18id = f18id
                };
            }
            else if (f19id > 0)
            {
                query = new BO.myQuery("f21")
                {
                    f19id = f19id
                };
            }
            else
            {
                query = new BO.myQuery("f21")
                {
                    f06id = f06id
                };
            }

            return factory.f21ReplyUnitBL.GetListJoinedF19(query);
        }
        #endregion

        #region f25
        /// <summary>
        /// Detail sachovnicove otazky
        /// </summary>
        public BO.f25ChessBoard f25ChessBoardBLLoad(int f25id)
        {
            return factory.f25ChessBoardBL.Load(f25id);
        }
        #endregion

        #region f26
        /// <summary>
        /// Detail baterie otazed
        /// </summary>
        public BO.f26BatteryBoard f26BatteryBoardBLLoad(int f26id)
        {
            return factory.f26BatteryBoardBL.Load(f26id);
        }
        #endregion

        #region f27
        /// <summary>
        /// Seznam vsech preddefinovanych URL pro otazky typu button
        /// </summary>
        public IEnumerable<BO.f27LinkUrl> f27LinkUrlGetList()
        {
            IEnumerable<BO.f27LinkUrl> links = factory.f19QuestionBL.GetList_AllF27();
            if (links == null)
                links = new List<BO.f27LinkUrl>(0);

            return links;
        }
        #endregion

        #region f28
        /// <summary>
        /// Seznam Best practices pro dany segment
        /// </summary>
        public IEnumerable<BO.f30BestPracticesToSegment> f18FormSegmentBLGetList_f30(int f18id)
        {
            return new List<BO.f30BestPracticesToSegment>();
            /*TODO best practices
            return factory.f18FormSegmentBL.GetList_f30(new BO.myQuery
            {
                f18id = f18id
            });*/
        }
        #endregion

        public IEnumerable<BO.f31FilledQuestionPublishing> f31FilledQuestionPublishingBLGetList(int a11id, int f18id)
        {
            IEnumerable<BO.f31FilledQuestionPublishing> publikovane = factory.f31FilledQuestionPublishingBL.GetList(new BO.myQuery("f31")
            {
                a11id = a11id,
                f18id = f18id
            });

            return publikovane;
        }

        #region f32
        public IEnumerable<BO.f32FilledValue> f32FilledValueBLGetList(int a11id, int f18id = 0)
        {
            BO.myQuery query = new BO.myQuery("f32")
            {
                a11id = a11id
            };
            if (f18id > 0)
                query.f18id = f18id;

            IEnumerable<BO.f32FilledValue> odpovedi = factory.f32FilledValueBL.GetList(query);

            return odpovedi;
        }
        #endregion

        #region o27
        public List<BO.o27Attachment> o27AttachmentBLGetList(IdFields idField, int id)
        {
            BO.myQuery q = null;
            switch (idField)
            {
                case IdFields.f18id:
                    q = new BO.myQuery("o27") { f18id = id };
                    break;
                case IdFields.f06id:
                    q = new BO.myQuery("o27") { f06id = id };
                    break;
            }

            IEnumerable<BO.o27Attachment> prilohy = factory.o27AttachmentBL.GetList(q, "");
            if (prilohy == null)
                prilohy = new List<BO.o27Attachment>();

            return prilohy.ToList();
        }

        /// <summary>
        /// Vraci seznam vsech priloh pro vsechny otazky v danem segmentu
        /// </summary>
        public IEnumerable<BO.o27Attachment> o27AttachmentBLGetListSegmentQ(int f18id)
        {
            return factory.o27AttachmentBL.GetList_f19Inf18(f18id);
        }
        #endregion
    }
}