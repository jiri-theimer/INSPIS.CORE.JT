using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum f06UserLockFlagEnum
    {
        NoLockOffer = 1,
        LockOnlyIfValid = 2,
        LockWhenever = 3,
        NotSpecified = 0,
    }
    public enum f06LinkerObjectENUM
    {
        None = 0,
        f19 = 1
    }
    public enum f06RelationWithTeacherENUM
    {
        None = 0,
        All = 1,
        OnlyNew = 2,
        OnlyBoundWithSchool = 3
    }
    public enum f06BindScopeQueryENUM
    {
        None = 0,
        DirectOnly = 1,
        PollOnly = 2
    }

    public class f06Form:BaseBO
    {
        [Key]
        public int f06ID { get; set; }
        public int f12ID { get; set; }
        public string f06Name { get; set; }
        public bool f06IsTemplate { get; set; }
        public bool f06IsPublishAllowed { get; set; }
        public string f06Description { get; set; }
        public string f06ExportCode { get; set; }
        public string f06RazorTemplate { get; set; }
        public string f06UC { get; set; }
        public f06UserLockFlagEnum f06UserLockFlag { get; set; }
        public bool f06IsA37Required { get; set; }
        public f06RelationWithTeacherENUM f06RelationWithTeacher { get; set; } = f06RelationWithTeacherENUM.None;
        public f06BindScopeQueryENUM f06BindScopeQuery { get; set; } = f06BindScopeQueryENUM.None;
        public bool f06IsA01PeriodStrict { get; set; }
        public bool f06IsA01ClosedStrict { get; set; }
        public bool f06IsReportDialog { get; set; }
        public bool f06IsWorkflowDialog { get; set; }
        public bool f06IsExportToDoc { get; set; }
        public int f06Ordinal { get; set; }
        public f06LinkerObjectENUM f06LinkerObject { get; set; } = f06LinkerObjectENUM.None;
        public int f06Linker_DestF06ID { get; set; }
        public string f06Linker_DestDB { get; set; }
        public string f06Hint { get; set; }


        public string f12Name { get; set; } //combo

    }
}
