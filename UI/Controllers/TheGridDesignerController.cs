using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class TheGridDesignerController : BaseController
    {
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridDesignerController(BL.TheColumnsProvider cp, BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public IActionResult Index(int j72id)
        {
            var v = new Models.TheGridDesignerViewModel();
            v.Rec = Factory.j72TheGridTemplateBL.Load(j72id);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                if (v.Rec.j72IsSystem == false && v.Rec.j03ID == Factory.CurrentUser.pid)
                {
                    v.HasOwnerPermissions = true;
                   
                    var lis = Factory.j04UserRoleBL.GetList(new BO.myQueryJ04() { j72id = j72id });
                    v.j04IDs = string.Join(",", lis.Select(p => p.pid));
                    v.j04Names = string.Join(",", lis.Select(p => p.j04Name));
                }

                v.lisJ73 = Factory.j72TheGridTemplateBL.GetList_j73(v.Rec.pid,v.Rec.j72Entity.Substring(0,3)).ToList();
                foreach (var c in v.lisJ73)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                Index_RefreshState(v);
                
                return View(v);
            }

        }

        
        [HttpPost]
        public IActionResult Index(Models.TheGridDesignerViewModel v, bool restore2factory, string oper, string guid, string j72name)    //uložení grid sloupců
        {
            Index_RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper == "saveas" && j72name != null)
            {
                var recJ72 = Factory.j72TheGridTemplateBL.Load(v.Rec.pid);
                var lisJ73 = Factory.j72TheGridTemplateBL.GetList_j73(recJ72.pid,recJ72.j72Entity.Substring(0,3)).ToList();
                recJ72.j72IsSystem = false; recJ72.j72ID = 0; recJ72.pid = 0; recJ72.j72Name = j72name; recJ72.j03ID = Factory.CurrentUser.pid;
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                List<int> j11ids = BO.BAS.ConvertString2ListInt(v.j11IDs);
                var intJ72ID = Factory.j72TheGridTemplateBL.Save(recJ72, lisJ73, j04ids,j11ids);
                return RedirectToActionPermanent("Index", new { j72id = intJ72ID });
            }
            if (oper == "rename" && j72name != null)
            {
                var recJ72 = Factory.j72TheGridTemplateBL.Load(v.Rec.pid);
                recJ72.j72Name = j72name;
                var intJ72ID = Factory.j72TheGridTemplateBL.Save(recJ72, null, null,null);
                return RedirectToActionPermanent("Index", new { j72id = intJ72ID });
            }
            if (oper == "delete" && v.HasOwnerPermissions)
            {
                if (Factory.CBL.DeleteRecord("j72", v.Rec.pid) == "1")
                {
                    v.Rec.pid = Factory.j72TheGridTemplateBL.LoadState(v.Rec.j72Entity, Factory.CurrentUser.pid, v.Rec.j72MasterEntity).pid;
                    v.SetJavascript_CallOnLoad(v.Rec.j72ID);
                    return View(v);
                }
            }
            if (oper == "changefield" && guid != null)
            {
                if (v.lisJ73.Where(p => p.TempGuid == guid).Count() > 0)
                {
                    var c = v.lisJ73.Where(p => p.TempGuid == guid).First();
                    c.j73Value = null; c.j73ValueAlias = null;
                    c.j73ComboValue = 0;
                    c.j73Date1 = null; c.j73Date2 = null;
                    c.j73Num1 = 0; c.j73Num2 = 0;
                }
                return View(v);
            }

            if (oper == "add_j73")
            {
                var c = new BO.j73TheGridQuery() { TempGuid = BO.BAS.GetGuid(), j73Column = v.lisQueryFields.First().Field };
                c.FieldType = v.lisQueryFields.Where(p => p.Field == c.j73Column).First().FieldType;
                c.FieldEntity = v.lisQueryFields.Where(p => p.Field == c.j73Column).First().SourceEntity;
                v.lisJ73.Add(c);

                return View(v);
            }
            if (oper == "delete_j73")
            {
                v.lisJ73.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "clear_j73")
            {
                v.lisJ73.Clear();
                return View(v);
            }
            if (restore2factory == true)
            {
                Factory.CBL.DeleteRecord("j72", v.Rec.pid);
                v.SetJavascript_CallOnLoad(v.Rec.pid);
                return View(v);
            }

            if (ModelState.IsValid)
            {

                var recJ72 = Factory.j72TheGridTemplateBL.Load(v.Rec.pid);
                var gridState = Factory.j72TheGridTemplateBL.LoadState(v.Rec.pid, Factory.CurrentUser.pid);
                recJ72.j72Columns = v.Rec.j72Columns;
                recJ72.j72IsPublic = v.Rec.j72IsPublic;

                gridState.j75Filter = "";   //automaticky vyčistit aktuální sloupcový filtr
                gridState.j75CurrentPagerIndex = 0;
                gridState.j75CurrentRecordPid = 0;
                
                if (gridState.j75SortDataField != null)
                {
                    if (recJ72.j72Columns.IndexOf(gridState.j75SortDataField) == -1)
                    { //vyčistit sort field, pokud se již nenachází ve vybraných sloupcích
                        gridState.j75SortDataField = "";
                        gridState.j75SortOrder = "";
                    }
                }
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                List<int> j11ids = BO.BAS.ConvertString2ListInt(v.j11IDs);
                int intJ72ID = Factory.j72TheGridTemplateBL.Save(recJ72, v.lisJ73.Where(p => p.j73ID > 0 || p.IsTempDeleted == false).ToList(), j04ids,j11ids);
                if (intJ72ID > 0)
                {
                    Factory.j72TheGridTemplateBL.SaveState(gridState,Factory.CurrentUser.pid);
                    if (recJ72.j72MasterEntity == null)
                    {
                        Factory.CBL.SetUserParam("masterview-j72id-" + recJ72.j72Entity.Substring(0, 3), intJ72ID.ToString());
                    }

                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                else
                {
                    return View(v);
                }

                //return RedirectToActionPermanent("Designer", new { j72id = v.Rec.pid });
            }


            return View(v);

        }

        private void inhale_tree(UI.Models.TheGridDesignerViewModel v)
        {
            
            v.treeNodes = new List<kendoTreeItem>();

            foreach (var rel in v.Relations)
            {
                var grp = new kendoTreeItem() { id = "group__" + rel.RelName + "__" + rel.TableName, text = rel.AliasSingular, expanded = false };

                switch (Factory.CurrentUser.j03LangIndex)
                {
                    case 1:
                        grp.text = rel.Translate1;break;
                    case 2:
                        grp.text = rel.Translate2;break;
                    default:
                        grp.text = rel.AliasSingular;break;
                }
               
                if (v.Relations.Count()==1 && v.treeNodes.Count() == 0)
                {
                    grp.expanded = true;
                }
                grp.customvalue2 = "/images/folder.png";
                grp.customvalue3 = "tree_group";
                grp.items = new List<kendoTreeItem>();

                foreach (var col in v.AllColumns.Where(p => p.Entity == rel.TableName))
                {
                    var cc = new kendoTreeItem() { id = rel.RelName + "__" + col.Entity + "__" + col.Field, text = col.Header };
                    cc.customvalue1 = rel.RelName + "__" + col.Entity;

                    switch (Factory.CurrentUser.j03LangIndex)
                    {
                        case 1:
                            cc.text = col.TranslateLang1; break;
                        case 2:
                            cc.text = col.TranslateLang2; break;
                        default:
                            cc.text = col.Header; break;
                    }
                    //cc.imageUrl = "/images/" + col.getImage();
                    cc.customvalue2 = "/images/" + col.getImage();
                    cc.customvalue3 = "tree_item";
                    if (col.IsTimestamp) cc.customvalue3 += " timestamp";
                    grp.items.Add(cc);
                }

                v.treeNodes.Add(grp);
            }


                
        }



        private void Index_RefreshState(Models.TheGridDesignerViewModel v)
        {
            var mq = new BO.myQuery(v.Rec.j72Entity);
            var ce = Factory.EProvider.ByPrefix(mq.Prefix);
            v.Relations = Factory.EProvider.getApplicableRelations(mq.Prefix); //návazné relace
            v.Relations.Insert(0, new BO.EntityRelation() { TableName = ce.TableName, AliasSingular = ce.AliasSingular, SqlFrom = ce.SqlFromGrid, RelName = "a",Translate1=ce.TranslateLang1,Translate2=ce.TranslateLang2 });   //primární tabulka a

            v.AllColumns = _colsProvider.AllColumns().ToList();
            v.AllColumns.RemoveAll(p => p.VisibleWithinEntityOnly != null && p.VisibleWithinEntityOnly.Contains(v.Rec.j72Entity.Substring(0, 3)) == false);    //nepatřičné kategorie/štítky

            v.SelectedColumns = _colsProvider.ParseTheGridColumns(mq.Prefix, v.Rec.j72Columns, Factory.CurrentUser.j03LangIndex);
           
            v.lisQueryFields = new BL.TheQueryFieldProvider(v.Rec.j72Entity.Substring(0, 3)).getPallete();
            if (Factory.CurrentUser.j03LangIndex > 0)
            {   //překlad do cizího jazyku
                foreach (var c in v.lisQueryFields)
                {
                    c.Header = Factory.tra(c.Header);
                }
            }
            
            v.lisPeriods = _pp.getPallete();
            if (v.lisJ73 == null)
            {
                v.lisJ73 = new List<BO.j73TheGridQuery>();
            }
            foreach (var c in v.lisJ73.Where(p => p.j73Column != null))
            {
                if (v.lisQueryFields.Where(p => p.Field == c.j73Column).Count() > 0)
                {
                    var cc = v.lisQueryFields.Where(p => p.Field == c.j73Column).First();
                    c.FieldType = cc.FieldType;
                    c.FieldEntity = cc.SourceEntity;
                    c.MasterPrefix = cc.MasterPrefix;
                    c.MasterPid = cc.MasterPid;
                }
            }

            inhale_tree(v);
        }
    }
}