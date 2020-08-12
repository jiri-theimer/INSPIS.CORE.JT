using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum o27PortalPublishFlagENUM
    {
        InPublishQueue=1,   //Čeká ve frontě na publikování
        Published =2,       //Publikováno na portále
        WithDrawn =3,       //Staženo z portálu
        DefaultState = 0      
    }
    public class o27Attachment:BaseBO
    {
        [Key]
        public int o27ID { get; set; }    
        public int o13ID { get; set; }
        public int x29ID { get; set; }
        public int o27DataPID { get; set; }
        public string o27Label { get; set; }
        public string o27OriginalFileName { get; set; }
        public string o27OriginalExtension { get; set; }
        public string o27ArchiveFileName { get; set; }
        public string o27ArchiveFolder { get; set; }
        public int o27FileSize { get; set; }
        public string o27ContentType { get; set; }
        public string o27GUID { get; set; }

        public string o27PressMark { get; set; }
        public string o27GinisDocPID { get; set; }
        public string o27GinisFilePID { get; set; }

        public string o27Description { get; set; }

        public o27PortalPublishFlagENUM o27PortalPublishFlag { get; set; }
        public DateTime? o27PortalPublishDateUpdate { get; set; }
        public string o27PortalPublishUserUpdate { get; set; }
        public string o27ArchivePdfFileName { get; set; }

        public string FullPath { get; set; }

        public string PortalPublishAlias;

        public string o27DownloadGUID { get; set; }

        public string o13Name;

    }
}
