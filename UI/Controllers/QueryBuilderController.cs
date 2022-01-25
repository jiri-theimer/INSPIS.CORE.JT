using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class QueryBuilderController : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public QueryBuilderController(BL.ThePeriodProvider pp)
        {            
            _pp = pp;
        }
        public IActionResult Index(int j76id,string entity,bool create)
        {
            var v = new QueryBuilderViewModel() { SelectedJ76ID = j76id,Entity=entity,IsCreate=create };
            if (v.SelectedJ76ID > 0)
            {
                InhaleRecAndList(v);
            }
            else
            {
                if (string.IsNullOrEmpty(v.Entity))
                {
                    return this.StopPage(true, "entity missing");
                }                
                if (v.IsCreate)
                {
                    v.Rec = new BO.j76NamedQuery() { j76Entity = v.Entity };
                }
            }

            

            RefreshState(v);

            return View(v);
        }

        private void InhaleRecAndList(QueryBuilderViewModel v)
        {
            v.Rec = Factory.j76NamedQueryBL.Load(v.SelectedJ76ID);
            v.Entity = v.Rec.j76Entity;
            v.lisJ77 = Factory.j76NamedQueryBL.GetList_j77(v.Rec.pid, v.Entity.Substring(0, 3)).ToList();
            foreach (var c in v.lisJ77)
            {
                c.TempGuid = BO.BAS.GetGuid();
            }
        }

        private void RefreshState(QueryBuilderViewModel v)
        {
            v.lisJ76 = Factory.j76NamedQueryBL.GetList(v.Entity, Factory.CurrentUser.pid).OrderBy(p => p.j76Name);
            if (!Factory.IsUserAdmin())
            {
                v.lisJ76 = v.lisJ76.Where(p => p.j03ID == Factory.CurrentUser.pid);
            }
            if (!v.IsCreate && v.lisJ76.Count()>0 && (v.SelectedJ76ID == 0 || !v.lisJ76.Any(p=>p.pid==v.SelectedJ76ID)))
            {
                v.SelectedJ76ID = v.lisJ76.First().pid;
                InhaleRecAndList(v);
            }
            if (v.Rec == null)
            {
                v.Rec = new BO.j76NamedQuery() { j76Entity = v.Entity };
                v.IsCreate = true;
            }
            if (v.Rec.pid==0 || v.Rec.j03ID==Factory.CurrentUser.pid || Factory.IsUserAdmin())
            {
                v.HasOwnerPermissions = true;
            }
           
            v.lisQueryFields = new BL.TheQueryFieldProvider(v.Rec.j76Entity.Substring(0, 3)).getPallete();
            if (Factory.CurrentUser.j03LangIndex > 0)
            {   //překlad do cizího jazyku
                foreach (var c in v.lisQueryFields)
                {
                    c.Header = Factory.tra(c.Header);
                }
            }

            v.lisPeriods = _pp.getPallete();
            if (v.lisJ77 == null)
            {
                v.lisJ77 = new List<BO.j77NamedQueryRow>();
            }
            foreach (var c in v.lisJ77.Where(p => p.j77Column != null))
            {
                if (v.lisQueryFields.Where(p => p.Field == c.j77Column).Count() > 0)
                {
                    var cc = v.lisQueryFields.Where(p => p.Field == c.j77Column).First();
                    c.FieldType = cc.FieldType;
                    c.FieldEntity = cc.SourceEntity;
                    //c.MasterPrefix = cc.MasterPrefix;
                    //c.MasterPid = cc.MasterPid;
                }
            }
        }


        [HttpPost]
        public IActionResult Index(Models.QueryBuilderViewModel v, string oper, string guid, string j76name)
        {
            RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "create")
            {
                return RedirectToActionPermanent("Index", new {entity=v.Entity, create = true });
            }
            if (oper == "saveas" && !string.IsNullOrEmpty(j76name))
            {
                var recJ76 = Factory.j76NamedQueryBL.Load(v.Rec.pid);
                var lisJ77 = Factory.j76NamedQueryBL.GetList_j77(recJ76.pid, recJ76.j76Entity.Substring(0, 3)).ToList();
                recJ76.j76ID = 0; recJ76.pid = 0; recJ76.j76Name = j76name;
                recJ76.j03ID = Factory.CurrentUser.pid;
                
                var intJ76ID = Factory.j76NamedQueryBL.Save(recJ76, lisJ77);
                return RedirectToActionPermanent("Index", new { j76id = intJ76ID });
            }
            
            if (oper == "delete" && v.HasOwnerPermissions)
            {
                if (Factory.CBL.DeleteRecord("j76", v.Rec.pid) == "1")
                {
                    return RedirectToActionPermanent("Index", new { entity = v.Entity });
                    
                }
            }

            if (oper == "changefield")
            {
                if (guid == null)
                {
                    this.AddMessageTranslated("guid missing");
                    return View(v);
                }
                if (v.lisJ77.Where(p => p.TempGuid == guid).Count() > 0)
                {
                    var c = v.lisJ77.Where(p => p.TempGuid == guid).First();
                    c.j77Value = null; c.j77ValueAlias = null;
                    c.j77ComboValue = 0;
                    c.j77Date1 = null; c.j77Date2 = null;
                    c.j77Num1 = 0; c.j77Num2 = 0;
                }
                return View(v);
            }
            if (oper == "add_j77")
            {
                var c = new BO.j77NamedQueryRow() { TempGuid = BO.BAS.GetGuid(), j77Column = v.lisQueryFields.First().Field };
                c.FieldType = v.lisQueryFields.Where(p => p.Field == c.j77Column).First().FieldType;
                c.FieldEntity = v.lisQueryFields.Where(p => p.Field == c.j77Column).First().SourceEntity;
                v.lisJ77.Add(c);

                return View(v);
            }
            if (oper == "delete_j77")
            {
                v.lisJ77.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "clear_j77")
            {
                v.lisJ77.Clear();
                return View(v);
            }


            if (ModelState.IsValid)     //to je uložit změny
            {
                var c = new BO.j76NamedQuery() { j03ID = Factory.CurrentUser.pid,j76Entity=v.Entity };
                if (v.Rec.pid > 0)
                {
                    c= Factory.j76NamedQueryBL.Load(v.Rec.pid);
                }
                c.j76Name = v.Rec.j76Name;
                c.j76IsPublic = v.Rec.j76IsPublic;
                var intJ76ID = Factory.j76NamedQueryBL.Save(c, v.lisJ77.Where(p => p.j77ID > 0 || !p.IsTempDeleted).ToList());
                if (intJ76ID > 0)
                {
                    Factory.CBL.SetUserParam("masterview-j76id-" +v.Entity.Substring(0, 3), intJ76ID.ToString());

                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                else
                {
                    return View(v);
                }
            }



                return View(v);
        }

    }

    
}
