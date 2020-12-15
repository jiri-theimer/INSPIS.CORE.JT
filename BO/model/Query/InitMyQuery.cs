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
                default:
                    return load_0(prefix, master_prefix, master_pid);
            }
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
            //switch (master_prefix)
            //{
            //    case "a10":
            //        mq.a10id = master_pid; break;
            //    case "a03":
            //        mq.a03id = master_pid; break;
            //    case "a08":
            //        mq.a08id = master_pid; break;
            //    case "a42":
            //        mq.a42id = master_pid; break;
            //    case "b02":
            //        mq.b02id = master_pid; break;
            //    case "j02":
            //        mq.j02id = master_pid; break;
                 
            //    default:
            //        break;
            //}


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
            //switch (master_prefix)
            //{
            //    case "a29":
            //        mq.a29id = master_pid;break;
            //    case "a42":
            //        mq.a42id = master_pid; break;
            //    case "a06":
            //        mq.a06id = master_pid; break;
            //    case "j02":
            //        mq.j02id = master_pid; break;
                   
            //    default:
            //        break;
            //}
            

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
            //switch (master_prefix)
            //{
            //    case "a08":
            //        mq.a08id = master_pid; break;
            //    case "b06":
            //        mq.b06id = master_pid; break;
            //    case "a01":
            //        mq.a01id = master_pid; break;
            //    case "f12":
            //        mq.f12id = master_pid; break;

            //    default:
            //        break;
            //}
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
