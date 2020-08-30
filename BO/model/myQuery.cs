using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BO
{
    public enum BooleanQueryMode
    {
        NoQuery = 0,
        FalseQuery = 2,
        TrueQuery = 1
    }
    public class myQuery
    {
        private string _prefix;
        private string _pkfield;
        private string _Entity;
        public myQuery(string strEntity)
        {
            if (String.IsNullOrEmpty(strEntity)) { strEntity = "??????"; };
            _Entity = strEntity;
            this.Refresh();
        }
        private void Refresh()
        {
            _prefix = _Entity.Substring(0, 3);
            _pkfield = "a." + _Entity.Substring(0, 3) + "ID";
        }

        public int OFFSET_PageSize { get; set; }
        public int OFFSET_PageNum { get; set; }

        public string Entity
        {
            get
            {
                return _Entity;
            }
            set
            {
                _Entity = value;
                this.Refresh();
            }
        }
        public string Prefix
        {
            get
            {
                return _prefix;
            }
        }
        public string PkField
        {
            get
            {
                return _pkfield;
            }
        }
        public List<BO.TheGridColumnFilter> TheGridFilter { get; set; }
        public IEnumerable<BO.j73TheGridQuery> lisJ73 { get; set; }
        public DateTime? global_d1;
        public DateTime? global_d2;
        public List<BO.ThePeriod> lisPeriods { get; set; }

        public List<int> pids;
        public IEnumerable<BO.TheGridColumn> explicit_columns { get; set; }
        public string explicit_orderby { get; set; }
        public string explicit_selectsql { get; set; }

        public bool? IsRecordValid;
        public BO.RunningUser CurrentUser;
        public bool MyRecordsDisponible;

        public int j04id { get; set; }


        public int a03id { get; set; }
        public int a03id_founder { get; set; }
        public int a01id { get; set; }
        public int a01parentid { get; set; }
        public int a04id { get; set; }
        public int a05id { get; set; }
        public int a06id { get; set; }

        public int j02id { get; set; }
        public int j02id_issuer { get; set; }
        public int j02id_leader { get; set; }
        public int j02id_member { get; set; }
        public int j72id { get; set; }
        public int a17id { get; set; }
        public int j11id { get; set; }
        public int j23id { get; set; }
        public int a29id { get; set; }
        public int a42id { get; set; }
        public int b01id { get; set; }
        public int b02id { get; set; }
        public List<int> b02ids { get; set; }
        public int b06id { get; set; }
        public int f06id { get; set; }
        public int f12id { get; set; }
        public int f18id { get; set; }
        public int f19id { get; set; }
        public List<int> f19ids { get; set; }
        public int f21id { get; set; }
        public int f22id { get; set; }
        public int f25id { get; set; }
        public int f26id { get; set; }
        public int f29id { get; set; }
        public int f32id { get; set; }
        public int h04id { get; set; }
        public int h11id { get; set; }
        public int a10id { get; set; }
        public int a11id { get; set; }
        public BO.BooleanQueryMode a11ispoll { get; set; }
        public int a08id { get; set; }
        public int x31id { get; set; }
        public int x29id { get; set; }
        public int recpid { get; set; }

        public int o53id { get; set; }
        public string param1;

        public BO.BooleanQueryMode HiddenQuestions { get; set; }
        public string SearchString;
        public string SearchImplementation { get; set; }    //HD nebo null
        public int TopRecordsOnly;


        public DateTime? DateBetween { get; set; }
        public int DateBetweenDays { get; set; }


        public void SetPids(string strPids)
        {
            this.pids = BO.BAS.ConvertString2ListInt(strPids);

        }



        public void InhaleMasterEntityQuery(string master_entity, int master_pid, string master_flag)
        {
            if (master_pid == 0 || master_entity == null)
            {
                return;
            }
            switch (master_entity.Substring(0, 3))
            {
                case "a01":
                    this.a01id = master_pid;
                    if (master_flag == "poll")
                    {
                        this.a11ispoll = BO.BooleanQueryMode.TrueQuery;   //pouze anketní formuláře
                    }
                    break;
                case "a03":
                    switch (master_flag)
                    {
                        case "founder":
                            this.a03id_founder = master_pid;
                            break;
                        default:
                            this.a03id = master_pid;
                            break;
                    }
                    break;
                case "a10":
                    this.a10id = master_pid;
                    break;                
                case "a08":
                    this.a08id = master_pid;
                    break;
                case "a29":
                    this.a29id = master_pid;
                    break;
                case "a42":
                    this.a42id = master_pid;
                    break;
                case "a05":
                    this.a05id = master_pid;
                    break;
                case "a04":
                    this.a04id = master_pid;
                    break;
                case "a06":
                    this.a06id = master_pid;
                    break;
                case "b01":
                    this.b01id = master_pid;
                    break;
                case "b02":
                    this.b02id = master_pid;
                    break;
                case "b06":
                    this.b06id = master_pid;
                    break;
                case "f06":
                    this.f06id = master_pid;
                    break;
                case "f18":
                    this.f18id = master_pid;
                    break;
                case "f19":
                    this.f19id = master_pid;
                    break;
                case "f21":
                    this.f21id = master_pid;
                    break;
                case "f22":
                    this.f22id = master_pid;
                    break;
                case "f25":
                    this.f25id = master_pid;
                    break;
                case "f26":
                    this.f26id = master_pid;
                    break;
                case "j02":
                    switch (master_flag)
                    {
                        case "issuer":
                            this.j02id_issuer = master_pid;
                            break;
                        case "leader":
                            this.j02id_leader = master_pid;
                            break;
                        case "member":
                            this.j02id_member = master_pid;
                            break;
                        default:
                            this.j02id = master_pid;
                            break;
                    }
                    break;


                case "x29":
                    this.x29id = master_pid;
                    break;
                case "o53":
                    this.o53id = master_pid;
                    break;
                default:
                    break;
            }

        }
    }
}
