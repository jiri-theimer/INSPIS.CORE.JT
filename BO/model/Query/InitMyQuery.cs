using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class InitMyQuery
    {
       
        public BO.baseQuery Load(string prefix, string master_prefix=null, int master_pid=0,string master_flag=null)
        {                        
            master_prefix = validate_prefix(master_prefix);
            var ret = new BO.myQuery0(prefix.Substring(0, 3));
            switch (prefix.Substring(0, 3))
            {
                case "a01":                    
                    return load_a01(master_prefix, master_pid);
                case "a03":
                    return load_a03(master_prefix, master_pid, master_flag);
                case "j02":
                    return load_j02(master_prefix, master_pid);
                case "a11":
                    return load_a11(master_prefix, master_pid);                
                case "h04":
                    return load_h04(master_prefix, master_pid);
                case "a42":
                    return load_a42(master_prefix, master_pid);
                case "f06":
                    return load_f06(master_prefix, master_pid);
                case "f21":
                    return load_f21(master_prefix, master_pid);
                case "f19":
                    return load_f19(master_prefix, master_pid);
                case "j04":
                    return load_j04(master_prefix, master_pid);
                case "x40":
                    return handle_master(new BO.myQueryX40(), master_prefix, master_pid);
                default:
                    return load_0(prefix, master_prefix, master_pid);
            }
        }

        private BO.baseQuery handle_master(BO.baseQuery mq, string master_prefix, int master_pid)
        {
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }
            return mq;
        }

        private BO.myQuery0 load_0(string prefix,string master_prefix, int master_pid)
        {
            var mq = new BO.myQuery0(prefix);
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }

        public BO.myQueryA01 LoadA01(string master_prefix = null, int master_pid = 0, string master_flag = null)
        {
            master_prefix = validate_prefix(master_prefix);
            return load_a01(master_prefix, master_pid);
        }
        public BO.myQueryH04 LoadH04(string master_prefix = null, int master_pid = 0, string master_flag = null)
        {
            master_prefix = validate_prefix(master_prefix);
            return load_h04(master_prefix, master_pid);
        }

        private BO.myQueryA01 load_a01(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryA01();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }
           
            return mq;
        }

        private BO.myQueryA03 load_a03(string master_prefix, int master_pid,string master_flag)
        {
            var mq = new BO.myQueryA03();
            if (master_pid > 0)
            {
                switch (master_flag)
                {
                    case "founder":
                        mq.a03id_founder = master_pid;
                        break;
                    case "supervisor":
                        mq.a03id_supervisory = master_pid;
                        break;
                    case "parent":
                        mq.a03id_parent = master_pid;
                        break;
                    default:
                        BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
                        break;
                }
               
            }
            
            return mq;
        }
        private BO.myQueryJ02 load_j02(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryJ02();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }


            return mq;
        }
        private BO.myQueryA11 load_a11(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryA11();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }
            
            return mq;
        }

        private BO.myQueryH04 load_h04(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryH04();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }

        private BO.myQueryA42 load_a42(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryA42();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }

        private BO.myQueryF06 load_f06(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryF06();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }
            
            return mq;
        }

        private BO.myQueryF21 load_f21(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryF21();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }
        private BO.myQueryF19 load_f19(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryF19();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }
        private BO.myQueryJ04 load_j04(string master_prefix, int master_pid)
        {
            var mq = new BO.myQueryJ04();
            if (master_pid > 0)
            {
                BO.Reflexe.SetPropertyValue(mq, master_prefix + "id", master_pid);
            }

            return mq;
        }
        
        

        private string validate_prefix(string s = null)
        {
            if (s != null)
            {
                s = s.Substring(0, 3);
            }

            return s;
        }

    }
}
