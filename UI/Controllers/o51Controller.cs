using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using UI.Models.Record;

namespace UI.Controllers
{
    public class o51Controller : BaseController
    {
        ///štítky
        ///
        public IActionResult MultiSelect(string entity,string o51ids)
        {            
            var v = new TagsMultiSelect();
            v.Entity = entity;
            string prefix = v.Entity.Substring(0, 3);

            var mq = new BO.myQuery("o51Tag");
            mq.IsRecordValid = true;
            IEnumerable<BO.o51Tag> lisTags = Factory.o51TagBL.GetList(mq);
            v.ApplicableTags_Multi = lisTags.Where(p => p.o53IsMultiSelect == true && (p.o53Entities == null || p.o53Entities.Contains(prefix)));
            v.ApplicableTags_Single = lisTags.Where(p => p.o53IsMultiSelect == false && (p.o53Entities == null || p.o53Entities.Contains(prefix)));

            if (String.IsNullOrEmpty(o51ids) == false)
            {
                mq = new BO.myQuery("o51Tag");
                mq.SetPids(o51ids);
                lisTags = Factory.o51TagBL.GetList(mq);
            }
            

            mq = new BO.myQuery("o53TagGroup");
            var lisGroups = Factory.o53TagGroupBL.GetList(mq).Where(p =>p.o53IsMultiSelect==false && ( p.o53Entities == null || p.o53Entities.Contains(prefix))).ToList();
            v.SingleCombos = new List<SingleSelectCombo>();
            foreach (var group in lisGroups)
            {
                var c = new SingleSelectCombo() { o53ID = group.pid, o53Name = group.o53Name };
                if (String.IsNullOrEmpty(o51ids) == false)
                {
                    if (lisTags.Where(p => p.o53ID == group.pid).Count() > 0)
                    {
                        c.o51ID = lisTags.Where(p => p.o53ID == group.pid).First().pid;
                        c.o51Name = lisTags.Where(p => p.o53ID == group.pid).First().o51Name;
                    }
                }
                    
                v.SingleCombos.Add(c);
            }
            
            v.CheckedO51IDs = BO.BAS.ConvertString2ListInt(o51ids);
            v.SelectedO51IDs = BO.BAS.ConvertString2ListInt(o51ids);

          

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {           
            var v = new o51Record() { rec_pid = pid, rec_entity = "o51" };
            v.Rec = new BO.o51Tag();
            if (v.rec_pid > 0)
            {
                v.Rec = Factory.o51TagBL.Load(v.rec_pid);                
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }                
            }
                       
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.MakeClone();
            }
            return View(v);            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(o51Record v)
        {
            if (ModelState.IsValid)
            {
                BO.o51Tag c = new BO.o51Tag();
                if (v.rec_pid > 0) c = Factory.o51TagBL.Load(v.rec_pid);
                c.o53ID = v.Rec.o53ID;
                c.o51Code = v.Rec.o51Code;
                c.o51Name = v.Rec.o51Name;
                c.o51Ordinary = v.Rec.o51Ordinary;
                c.o51IsColor = v.Rec.o51IsColor;
                c.o51BackColor = v.Rec.o51BackColor;
                c.o51ForeColor = v.Rec.o51ForeColor;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                c.pid = Factory.o51TagBL.Save(c);
                if (c.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                }

            }

           
            this.Notify_RecNotSaved();
            return View(v);
        }


        public IActionResult Batch(int j72id, string pids)
        {
           
            var v = new TagsBatch();
            RefreshState_Batch(ref v, j72id, pids);

            return View(v);
        }
        [HttpPost]
        public IActionResult Batch(Models.TagsBatch v,string oper)
        {
            RefreshState_Batch(ref v, v.j72ID, v.Record_Pids);

            if (ModelState.IsValid)
            {
                if (oper == "postback")
                {
                    RefreshState_Batch(ref v, v.j72ID, v.Record_Pids);
                    return View(v);
                }
                if (v.SelectedO53ID == 0)
                {
                    this.AddMessage("Musíte vybrat kategorii.");
                    RefreshState_Batch(ref v, v.j72ID, v.Record_Pids);
                    return View(v);
                }


                List<int> o51ids = new List<int>();
                if (v.RecO53.o53IsMultiSelect == true)
                {
                    o51ids=v.SelectedO51IDs.Where(p => p > 0).ToList();
                }
                else
                {
                    o51ids.Add(v.SelectedRadioO51ID);
                }

                List<int> pids = BO.BAS.ConvertString2ListInt(v.Record_Pids);

                if (o51ids.Count > 0 || oper=="clear")
                {
                    string strO51IDs = string.Join(",", o51ids);
                    foreach (int pid in pids)
                    {
                        switch (oper)
                        {
                            case "replace":
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, strO51IDs, v.SelectedO53ID);
                                break;
                            case "clear":
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid,"", v.SelectedO53ID);
                                break;
                            case "append":
                                var c = Factory.o51TagBL.GetTagging(v.Record_Entity, pid);
                                if (c.TagPids == null)
                                {
                                    c.TagPids = strO51IDs;
                                }
                                else
                                {
                                    c.TagPids += ","+strO51IDs;
                                }
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, c.TagPids,v.SelectedO53ID);

                                break;
                            case "remove":
                                var d = Factory.o51TagBL.GetTagging(v.Record_Entity, pid);
                                if (d.TagPids != null)
                                {
                                    foreach(int o51id in o51ids)
                                    {
                                        d.TagPids = BO.BAS.RemoveValueFromDelimitedString(d.TagPids, o51id.ToString());
                                    }
                                    Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, d.TagPids,v.SelectedO53ID);
                                }
                                break;
                        }
                        
                    }
                    v.SetJavascript_CallOnLoad(v.j72ID);
                    return View(v);
                }
               
                
            }
            
            
           
            return View(v);
        }
        
        private void RefreshState_Batch(ref TagsBatch v,int j72id,string pids)
        {
            var gridState = Factory.j72TheGridTemplateBL.LoadState(j72id,Factory.CurrentUser.pid);            
            v.j72ID = j72id;
            v.Record_Entity = gridState.j72Entity;
            v.Record_Pids = pids;
            string prefix = v.Record_Entity.Substring(0, 3);
            
            v.lisO53 = Factory.o53TagGroupBL.GetList(new BO.myQuery("o53TagGroup")).Where(p => p.o53Entities == null || p.o53Entities.Contains(prefix));
            if (v.SelectedO53ID > 0)
            {
                v.RecO53 = Factory.o53TagGroupBL.Load(v.SelectedO53ID);
                var mq = new BO.myQuery("o51Tag");
                mq.IsRecordValid = true;
                mq.o53id = v.SelectedO53ID;
                v.ApplicableTags = Factory.o51TagBL.GetList(mq);
            }

            
        }

       

        public string GetTagHtml(string o51ids)
        {
            if (String.IsNullOrEmpty(o51ids) == true)
            {
                return "";
            }
            List<int> lis = BO.BAS.ConvertString2ListInt(o51ids);
            return Factory.o51TagBL.GetTagging(lis).TagHtml;
        }

        
    }

    
}