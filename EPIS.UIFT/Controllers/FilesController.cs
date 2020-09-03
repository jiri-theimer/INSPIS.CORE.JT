using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UIFT;
using NLog;
using System.Threading.Tasks;

namespace UIFT.Controllers
{
    public class FilesController : BaseController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ulozeni souboru uploadovanych ve formulari
        /// </summary>
        /// <param name="id">f19id</param>
        /// <returns>JSON seznam ulozenych souboru</returns>
        public async Task<ActionResult> Save(int id)
        {
            // nastavit ve web.config
            //this.Server.ScriptTimeout = 300;

            List<ViewDataUploadFilesResult> statuses = new List<ViewDataUploadFilesResult>();

            // model otazky
            Models.IOtazka otazka = this.UiRepository.GetOtazka(id, this.PersistantData.f06id);

            // otazka neexistuje
            if (otazka == null)
            {
                statuses.Add(new ViewDataUploadFilesResult()
                {
                    success = false,
                    f19id = otazka.PID,
                    message = UiRepository.BL.tra("Otázka neexistuje")
                });
            }
            else
            {
                // otazka je readonly
                if (this.UiRepository.IsQuestionReadOnly(otazka))
                {
                    statuses.Add(new ViewDataUploadFilesResult()
                    {
                        success = false,
                        f19id = otazka.PID,
                        message = UiRepository.BL.tra("Otázka je pouze pro čtení")
                    });
                }
                else
                {
                    if (Request.Form.Files.Count == 0)
                    {
                        statuses.Add(new ViewDataUploadFilesResult()
                        {
                            success = false,
                            f19id = otazka.PID,
                            message = UiRepository.BL.tra("Nebyl přiložen žádný soubor")
                        });
                    }

                    // odpoved je prilis velka
                    if (otazka.MaxAllowedSize > 0 && Request.Form.Files.Count > 0)
                    {
                        bool sizeOK = true;
                        foreach (IFormFile file in Request.Form.Files)
                        {
                            if (file.Length > otazka.MaxAllowedSize)
                            {
                                sizeOK = false;
                                break;
                            }
                        }

                        if (!sizeOK)
                        {
                            statuses.Add(new ViewDataUploadFilesResult()
                            {
                                success = false,
                                f19id = otazka.PID,
                                message = UiRepository.BL.tra("Nahrávaný soubor je příliš velký, maximální velikost je ") + Convert.ToInt32((double)otazka.MaxAllowedSize / 1000d) + " kB."
                            });
                        }
                    }
                }
            }

            if (statuses.Count == 0)
            {
                // ma otazka uz ulozenou odpoved?
                BO.f32FilledValue odpoved = this.UiRepository.BL.f32FilledValueBL.Load(this.PersistantData.a11id, id);

                foreach (IFormFile file in Request.Form.Files)
                {
                    FileInfo fi = new FileInfo(file.FileName);

                    // ulozit odpoved na otazku do databaze
                    if (odpoved == null)
                    {
                        odpoved = new BO.f32FilledValue
                        {
                            a11ID = this.PersistantData.a11id,
                            f19ID = id,
                            f21ID = otazka.Odpovedi[0].pid,
                            Value = fi.Name
                        };
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(odpoved.Value))
                            odpoved.Value += "|";
                        odpoved.Value += fi.Name;
                    }

                    // ulozit odpoved
                    int f32id = this.UiRepository.BL.f32FilledValueBL.Save(odpoved, false);
                    if (f32id > 0)
                    {
                        // vygenerovat guid souboru
                        string guid = Guid.NewGuid().ToString();
                        // jmeno souboru pro ulozeni
                        //string fileName = guid + "_" + fi.Name;

                        // ziskat cestu pro ulozeni souboru do tempu
                        string fullPath = this.UiRepository.BL.GlobalParams.LoadParam("TempFolder");
                        if (!fullPath.EndsWith("\\"))
                            fullPath += "\\";
                        fullPath += guid + fi.Extension;

                        // ulozit soubor do tempu
                        using (var stream = System.IO.File.Create(fullPath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // ulozit do DB
                        BO.o27Attachment priloha = new BO.o27Attachment();
                        priloha.o27ContentType = file.ContentType;
                        priloha.o27OriginalFileName = fi.Name;
                        priloha.o27DataPID = f32id;
                        priloha.x29ID = 432;
                        priloha.o27GUID = guid;
                        priloha.o13ID = Convert.ToInt32(UiRepository.BL.GlobalParams.LoadParam("UIFT_FileUploadType_o13id"));

                        // ulozit soubor do databaze
                        int o27id = this.UiRepository.BL.o27AttachmentBL.UploadAndSaveOneFile(priloha, fi.Name, fullPath);
                        if (o27id > 0)
                        {
                            statuses.Add(new ViewDataUploadFilesResult()
                            {
                                success = true,
                                f19id = otazka.PID,
                                name = file.FileName,
                                html = HtmlHelpers.HtmlHelpers.FileUploadRow(this.HttpContext, this.PersistantData.a11id, priloha.o27DownloadGUID, Convert.ToInt32(file.Length), priloha.o27OriginalFileName).ToString()
                            });
                        }
                        else
                        {
                            statuses.Add(new ViewDataUploadFilesResult()
                            {
                                success = false,
                                f19id = otazka.PID,
                                message = UiRepository.BL.CurrentUser.Messages4Notify.ToSingleLine()
                            }); ;
                        }
                    }
                    else
                    {
                        statuses.Add(new ViewDataUploadFilesResult()
                        {
                            success = false,
                            f19id = otazka.PID,
                            message = UiRepository.BL.CurrentUser.Messages4Notify.ToSingleLine()
                        });
                    }
                }
            }

            // kvuli zasranymu explorerovi se musi posilat text, nikoliv json
            if (statuses.Count > 0)
            {
                string str = string.IsNullOrEmpty(statuses[0].html) ? "" : statuses[0].html.Replace("\"", "'");
                //str = str.Replace("\r\n", "");
                return Content("[{" + string.Format("\"success\":{0},\"message\":\"{3}\",\"name\":\"{1}\",\"f19id\":{2},\"html\":\"\"", statuses[0].success.ToString().ToLower(), statuses[0].name, statuses[0].f19id, statuses[0].message) + "}]" + str);
            }
            else
                return Content("[]");

            //return Json(statuses);
        }

        private class ViewDataUploadFilesResult
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string name { get; set; }
            public string html { get; set; }
            public int f19id { get; set; }
        }

        /// <summary>
        /// Smazat soubor/prilohu
        /// </summary>
        /// <param name="guid">Guid prilohy</param>
        public ActionResult Delete(string id)
        {
            // instance prilohy
            BO.o27Attachment priloha = this.UiRepository.BL.o27AttachmentBL.LoadByGuid(id);

            // instance odpovedi
            BO.f32FilledValue odpoved = this.UiRepository.BL.f32FilledValueBL.Load(priloha.o27DataPID);

            // instance otazky
            Models.IOtazka otazka = this.UiRepository.GetOtazka(odpoved.f19ID, this.PersistantData.f06id);

            // otazka je readonly
            if (this.UiRepository.IsQuestionReadOnly(otazka))
            {
                return Json(new
                {
                    success = false,
                    message = UiRepository.BL.tra("Otázka je pouze pro čtení")
                });
            }

            // smazat soubor - prilohu
            this.UiRepository.BL.CBL.DeleteRecord("o27", priloha.pid);
            
            // smazat odpoved na otazku
            List<string> list = odpoved.Value.Split('|').ToList();
            list.RemoveAt(list.IndexOf(priloha.o27OriginalFileName));
            odpoved.Value = string.Join("|", list);

            int newId = this.UiRepository.BL.f32FilledValueBL.Save(odpoved, false);

            return Json(new { success = true, f19id = otazka.PID });
        }

        /// <summary>
        /// Vraci v binarni podobe soubor dle zadaneho ID
        /// </summary>
        /// <param name="id">o27id</param>
        public ActionResult Get(string id)
        {
            BO.o27Attachment attachment = this.UiRepository.BL.o27AttachmentBL.LoadByGuid(id);

            // soubor/ID neexistuje
            if (attachment == null)
            {
                log.Warn("FilesController.Get: file not found, GUID {0}", id);
                return Content("FILE NOT FOUND IN DB");
            }

            // nazev souboru. V pripade, ze je vyplnene PDFFILENAME, pouzij toto pole (jedna se o predgenerovane pdf misto puvodniho souboru).
            string finalFilename = string.IsNullOrEmpty(attachment.o27ArchivePdfFileName) ? attachment.o27OriginalFileName : attachment.o27ArchivePdfFileName;

            // cesta k ulozenemu souboru
            string path = UiRepository.BL.GlobalParams.LoadParam("UploadFolder");
            if (!path.EndsWith("\\"))
                path += "\\";
            if (!string.IsNullOrEmpty(attachment.o27ArchiveFolder))
            {
                path += attachment.o27ArchiveFolder + "\\";
            }
            path += attachment.o27ArchiveFileName;

            // soubor neni na disku
            if (!System.IO.File.Exists(path))
            {
                log.Warn("FilesController.Get: file doesn't exist, path: {1}; GUID {0}", id, path);
                return Content("FILE NOT FOUND");
            }

            FileInfo fInfo = new FileInfo(path);

            // nazev souboru osekany o nepovolene znaky
            string origName = MakeValidFileName(finalFilename);
            // content type
            string contentType = string.IsNullOrEmpty(attachment.o27ContentType) ?
                "application/" + fInfo.Extension.Replace(".", "") : attachment.o27ContentType;

            // log
            log.Info("FilesController.Get #3: {0}; ID: {1}; PATH: {2}; CONTENT TYPE: {3};", origName, id, path, contentType);

            return PhysicalFile(path, contentType, origName);
        }

        /// <summary>
        /// Vraci jmeno souboru bez nepovolenych znaku
        /// </summary>
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()) + ",");
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
