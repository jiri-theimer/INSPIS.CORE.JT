using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BL
{
    public interface Io27AttachmentBL
    {
        public BO.o27Attachment Load(int pid);
        public BO.o27Attachment LoadByGuid(string guid);
        
        public IEnumerable<BO.o27Attachment> GetList(BO.myQuery mq, string tempguid);
        public IEnumerable<BO.o27Attachment> GetList_f19Inf18(int f18id);
        public BO.o27Attachment InhaleFileByInfox(string strInfoxFullPath);
        public List<BO.o27Attachment> GetTempFiles( string strTempGUID);
        public int Save(BO.o27Attachment rec);
        public bool SaveChangesAndUpload(string guid, int x29id, int recpid);
        public bool SaveSingleUpload(string guid, int x29id, int recpid);
        public int UploadAndSaveOneFile(BO.o27Attachment rec, string strOrigFileName, string strSourceFullPath, string strExplicitArchiveFileName = null);
        public List<BO.o27Attachment> CopyTempFiles2Upload(string strTempGUID);
        public string GetUploadFolder(int o13id);
        public bool CopyOneTempFile2Upload(string strTempFileName, string strDestFolderName, string strDestFileName);

    }
    class o27AttachmentBL : BaseBL, Io27AttachmentBL
    {
        public o27AttachmentBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,o13.o13Name,");
            sb("case a.o27PortalPublishFlag when 1 then 'Čeká ve frontě na publikování' when 2 then 'Publikováno na portále' when 3 then 'Staženo z portálu' end as PortalPublishAlias,");
            sb(_db.GetSQL1_Ocas("o27"));
            sb(" FROM o27Attachment a LEFT OUTER JOIN o13AttachmentType o13 ON a.o13ID=o13.o13ID");
            sb(strAppend);
            return sbret();
        }
        public BO.o27Attachment Load(int pid)
        {
            return _db.Load<BO.o27Attachment>(GetSQL1(" WHERE a.o27ID=@pid"), new { pid = pid });
        }
        public BO.o27Attachment LoadByGuid(string guid)
        {
            return _db.Load<BO.o27Attachment>(GetSQL1(" WHERE a.o27DownloadGUID=@guid"), new { guid = guid });
        }


        public IEnumerable<BO.o27Attachment> GetList(BO.myQuery mq,string tempguid)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.o27ID DESC"; };
            if (tempguid != null)
            {
                mq.explicit_orderby = null;
            }
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            if (tempguid != null)
            {
                fq.Parameters.Add("guid", tempguid);
                fq.FinalSql += " AND a.o27ID NOT IN (select p85DataPID FROM p85Tempbox WHERE p85Guid=@guid)";
            }
            return _db.GetList<BO.o27Attachment>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.o27Attachment> GetList_f19Inf18(int f18id)
        {
            string s = GetSQL1(" WHERE a.x29ID=419 AND a.o27DataPID IN (SELECT f19ID FROM f19Question WHERE f18ID=@f18id)");
            return _db.GetList<BO.o27Attachment>(s, new { f18id = f18id });
        }



        public int Save(BO.o27Attachment rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.o27ID);
            p.AddInt("o13ID", rec.o13ID, true);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddInt("o27DataPID", rec.o27DataPID,true);
            

            p.AddString("o27Label", rec.o27Label);
            p.AddString("o27Description", rec.o27Description);
            p.AddString("o27PressMark", rec.o27PressMark);

            p.AddString("o27OriginalFileName", rec.o27OriginalFileName);
            p.AddString("o27OriginalExtension", rec.o27OriginalExtension);
            p.AddString("o27ArchiveFileName", rec.o27ArchiveFileName);
            p.AddString("o27ArchiveFolder", rec.o27ArchiveFolder);
            if (rec.o27OriginalExtension==null && rec.o27ArchiveFileName != null && rec.o27ArchiveFileName.Contains("."))
            {
                var arr = rec.o27ArchiveFileName.Split(".");
                rec.o27OriginalExtension = arr[arr.Length-1];
            }
            
            p.AddInt("o27FileSize", rec.o27FileSize);
            p.AddString("o27ContentType", rec.o27ContentType);
            p.AddString("o27GUID", rec.o27GUID);
            if (rec.o27DownloadGUID==null)
            {
                rec.o27DownloadGUID = BO.BAS.GetGuid();
            }
            p.AddString("o27DownloadGUID", rec.o27DownloadGUID);

            p.AddString("o27ArchivePdfFileName", rec.o27ArchivePdfFileName);

            p.AddEnumInt("o27PortalPublishFlag", rec.o27PortalPublishFlag);
            p.AddDateTime("o27PortalPublishDateUpdate", rec.o27PortalPublishDateUpdate);
            p.AddString("o27PortalPublishUserUpdate", rec.o27PortalPublishUserUpdate);
           

            int intPID = _db.SaveRecord("o27Attachment", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public int UploadAndSaveOneFile(BO.o27Attachment rec,string strOrigFileName,string strSourceFullPath,string strExplicitArchiveFileName = null)
        {
            if (rec.x29ID == 0 && rec.o13ID>0)
            {
                rec.x29ID = _mother.o13AttachmentTypeBL.Load(rec.o13ID).x29ID;
            }
            if (string.IsNullOrEmpty(strOrigFileName) == true)
            {
                rec.o27OriginalFileName = BO.BASFILE.GetFileInfo(strSourceFullPath).Name;
            }
            else
            {
                rec.o27OriginalFileName = strOrigFileName;
            }
            
            rec.o27OriginalExtension = BO.BASFILE.GetFileInfo(strSourceFullPath).Extension;
            rec.o27FileSize = Convert.ToInt32(BO.BASFILE.GetFileInfo(strSourceFullPath).Length);            
            
            if (string.IsNullOrEmpty(strExplicitArchiveFileName) == false)
            {
                rec.o27ArchiveFileName = strExplicitArchiveFileName;
            }
            if (string.IsNullOrEmpty(rec.o27ArchiveFileName))
            {
                rec.o27ArchiveFileName = rec.o27OriginalFileName ;
            }
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            rec.o27ArchiveFolder = GetUploadFolder(rec.o13ID);
            if (!System.IO.Directory.Exists(_mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder))
            {
                System.IO.Directory.CreateDirectory(_mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder);
            }
            var intO27ID = Save(rec);
            if (intO27ID > 0)
            {
                System.IO.File.Copy(strSourceFullPath, _mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder+"\\"+rec.o27ArchiveFileName, true);
            }

            return intO27ID;

        }

        public bool SaveSingleUpload(string guid, int x29id, int recpid)
        {
            using (var sc = new System.Transactions.TransactionScope()) //podléhá jedné transakci
            {
                var recs4upload = new List<BO.o27Attachment>();
                var mq = new BO.myQuery("o27");
                mq.recpid = recpid;
                mq.x29id = x29id;
                var lisO27Saved = GetList(mq,null);

                mq = new BO.myQuery("o13");
                var lisO13 = _mother.o13AttachmentTypeBL.GetList(mq).Where(p => p.x29ID == x29id);
                if (lisO13.Count() == 0)
                {
                    this.AddMessage("Nelze najít vhodný typ dokumentu.");return false;                    
                }
                if (GetTempFiles(guid).Count > 0)
                {
                    foreach (var recO27 in GetTempFiles(guid))
                    {
                        recO27.x29ID = x29id;
                        recO27.o27DataPID = recpid;
                        recO27.o13ID = lisO13.First().pid;
                        recO27.o27ArchiveFolder = GetUploadFolder(recO27.o13ID);
                        recO27.o27ArchiveFileName = recO27.o27OriginalFileName;

                        if (lisO27Saved.Count() > 0)
                        {
                            recO27.pid = lisO27Saved.First().pid;   //natvrdo přepsat stávající záznam dokumentu
                            recO27.o27ID = recO27.pid;
                        }
                        var intO27ID = Save(recO27);
                        if (intO27ID > 0)
                        {
                            recs4upload.Add(recO27);
                            
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                
                sc.Complete();

                foreach (var rec in recs4upload)
                {
                    //soubor na serveru se ukládá pod jeho originálním fyzickým názvem
                    CopyOneTempFile2Upload(guid+"_"+rec.o27OriginalFileName, rec.o27ArchiveFolder, rec.o27OriginalFileName);
                }
            }


            return true;
        }


        public bool SaveChangesAndUpload(string guid,int x29id,int recpid)
        {
            using (var sc = new System.Transactions.TransactionScope()) //podléhá jedné transakci
            {
                var recs4upload = new List<BO.o27Attachment>();
                if (GetTempFiles(guid).Count > 0)
                {
                    foreach (var recO27 in GetTempFiles(guid))
                    {
                        recO27.o27DataPID = recpid;
                        recO27.o27ArchiveFolder = GetUploadFolder(recO27.o13ID);
                        var intO27ID = Save(recO27);
                        if (intO27ID > 0)
                        {
                            recs4upload.Add(recO27);
                            //CopyOneTempFile2Upload(recO27.o27ArchiveFileName, recO27.o27ArchiveFolder, recO27.o27ArchiveFileName);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                foreach (var recTemp in _mother.p85TempboxBL.GetList(guid))  //test na odstraněné záznamy příloh
                {
                    if (_mother.CBL.DeleteRecord("o27Attachment", recTemp.p85DataPID) != "1"){
                        this.AddMessageTranslated("Error: DELETE.");
                        return false;
                    }
                }
                sc.Complete();

                foreach (var rec in recs4upload)
                {
                    CopyOneTempFile2Upload(rec.o27ArchiveFileName, rec.o27ArchiveFolder, rec.o27ArchiveFileName);
                }                
            }
            

            return true;
        }

        public bool ValidateBeforeSave(BO.o27Attachment rec)
        {
            if (rec.x29ID == 0)
            {
                this.AddMessage("Chybí druh entity."); return false;
            }
            if (rec.o27DataPID == 0)
            {
                this.AddMessage("Chybí svázaný záznam entity."); return false;
            }
            if (string.IsNullOrEmpty(rec.o27ArchiveFileName))
            {
                this.AddMessage("Chybí vyplnit [o27ArchiveFileName]."); return false;
            }
            if (string.IsNullOrEmpty(rec.o27OriginalFileName))
            {
                this.AddMessage("Chybí vyplnit [o27OriginalFileName]."); return false;
            }


            return true;
        }


        public BO.o27Attachment InhaleFileByInfox(string strInfoxFullPath)
        {
            //příklad infox: image/png|99330|2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd_2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd|1|Popisek|Číslo jednací
            if (!System.IO.File.Exists(strInfoxFullPath))
            {
                return null;
            }

            string s = System.IO.File.ReadAllText(strInfoxFullPath);
            List<string> arr = BO.BAS.ConvertString2List(s, "|");
            var rec = new BO.o27Attachment()
            {
                o27ContentType = arr[0],
                o27FileSize = BO.BAS.InInt(arr[1]),
                o27OriginalFileName = arr[2],
                o27ArchiveFileName = arr[3],
                o27GUID = arr[4],
                o13ID = BO.BAS.InInt(arr[5]),
                o27Label = arr[6],
                o27PressMark = arr[7],
                x29ID=BO.BAS.InInt(arr[8])
            };
            if (rec.o27OriginalFileName.Contains("."))
            {
                arr = BO.BAS.ConvertString2List(rec.o27OriginalFileName, ".");
                rec.o27OriginalExtension = "."+arr[arr.Count - 1];
            }
            else
            {
                rec.o27OriginalExtension = ".";
            }
            if (rec.o13ID > 0)
            {
                var recO13 = _mother.o13AttachmentTypeBL.Load(rec.o13ID);
                rec.o13Name = recO13.o13Name;
                rec.x29ID = recO13.x29ID;
            }            
            rec.FullPath = _mother.App.TempFolder + "\\" + rec.o27ArchiveFileName;
            if (rec.o27FileSize == 0 && System.IO.File.Exists(rec.FullPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(rec.FullPath);
                rec.o27FileSize = fileBytes.Length;
            }
            return rec;

        }

        public List<BO.o27Attachment> GetTempFiles(string strTempGUID)
        {
            var lisO27 = new List<BO.o27Attachment>();
            foreach (string file in System.IO.Directory.EnumerateFiles(_mother.App.TempFolder, strTempGUID + "*.infox", System.IO.SearchOption.TopDirectoryOnly))
            {
                var rec = InhaleFileByInfox(file);
                if (System.IO.File.Exists(_mother.App.TempFolder + "\\" + rec.o27ArchiveFileName) == true)
                {
                    rec.FullPath = _mother.App.TempFolder + "\\" + rec.o27ArchiveFileName;
                    lisO27.Add(rec);
                }

            }

            return lisO27;
        }

        public bool CopyOneTempFile2Upload(string strTempFileName,string strDestFolderName,string strDestFileName)
        {
            if (!System.IO.Directory.Exists(_mother.App.UploadFolder + "\\" + strDestFolderName))
            {
                System.IO.Directory.CreateDirectory(_mother.App.UploadFolder + "\\" + strDestFolderName);
            }
            string strDestFullPath = _mother.App.UploadFolder;
            if (string.IsNullOrEmpty(strDestFolderName) == false)
            {
                strDestFullPath += "\\" + strDestFolderName;
            }
            strDestFullPath+="\\"+strDestFileName;

            System.IO.File.Copy(_mother.App.TempFolder + "\\" + strTempFileName, strDestFullPath, true);

            return true;
        }
        public List<BO.o27Attachment> CopyTempFiles2Upload(string strTempGUID)
        {
            var lisO27 = new List<BO.o27Attachment>();
            foreach (string file in System.IO.Directory.EnumerateFiles(_mother.App.TempFolder, strTempGUID + "*.infox", System.IO.SearchOption.TopDirectoryOnly))
            {
                BO.o27Attachment rec = InhaleFileByInfox(file);
                if (rec.o13ID > 0)
                {
                    rec.o27ArchiveFolder = GetUploadFolder(rec.o13ID);
                }
                
                if (!System.IO.Directory.Exists(_mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder))
                {
                    System.IO.Directory.CreateDirectory(_mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder);
                }
                rec.FullPath = _mother.App.UploadFolder + "\\" + rec.o27ArchiveFolder + "\\" + rec.o27ArchiveFileName;
                System.IO.File.Copy(_mother.App.TempFolder + "\\" + rec.o27ArchiveFileName, rec.FullPath, true);

                lisO27.Add(rec);
            }
            return lisO27;
        }


        public string GetUploadFolder(int o13id)
        {
            var c = _mother.o13AttachmentTypeBL.Load(o13id);
            if (c.SharpFolder != null)
            {
                c.SharpFolder = c.SharpFolder.Replace("[YEAR]", DateTime.Now.Year.ToString(),StringComparison.OrdinalIgnoreCase);
                c.SharpFolder = c.SharpFolder.Replace("[MONTH]", BO.BAS.RightString("0"+DateTime.Now.Month.ToString(),2), StringComparison.OrdinalIgnoreCase);
                
                if (c.SharpFolder.Substring(0, 2) == "\\")
                {
                    c.SharpFolder = BO.BAS.RightString(c.SharpFolder, c.SharpFolder.Length - 2);
                }

            }

            return c.SharpFolder;
        }

    }
}
