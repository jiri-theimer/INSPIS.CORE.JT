using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace BO
{
    public class InitMyQuery
    {
        private List<string> _mqi { get; set; }
        private string _master_prefix { get; set; }
        private int _master_pid { get; set; }

        private void handle_myqueryinline_input(string myqueryinline)
        {
            if (string.IsNullOrEmpty(myqueryinline))
            {
                return;
            }
            _mqi = BO.BAS.ConvertString2List(myqueryinline, "@");
        }
        public BO.baseQuery Load(string prefix, string master_prefix = null, int master_pid = 0, string myqueryinline = null)
        {
            handle_myqueryinline_input(myqueryinline);

            _master_prefix = validate_prefix(master_prefix);
            _master_pid = master_pid;

            var ret = new BO.myQuery(prefix.Substring(0, 3));
            switch (prefix.Substring(0, 3))
            {
                case "a01":
                    return LoadA01(master_prefix, master_pid, myqueryinline);
                case "h04":
                    return LoadH04(master_prefix, master_pid);
                case "a03":
                    return load_a03();

                case "a10":
                    return handle_myquery_reflexe(new BO.myQueryA10());
                case "a11":
                    return handle_myquery_reflexe(new BO.myQueryA11());
                case "a35":
                    return handle_myquery_reflexe(new BO.myQueryA35());
                case "a38":
                    return handle_myquery_reflexe(new BO.myQueryA38());
                case "a41":
                    return handle_myquery_reflexe(new BO.myQueryA41());
                case "a42":
                    return handle_myquery_reflexe(new BO.myQueryA42());
                case "f06":
                    return handle_myquery_reflexe(new BO.myQueryF06());
                case "f21":
                    return handle_myquery_reflexe(new BO.myQueryF21());
                case "f19":
                    return handle_myquery_reflexe(new BO.myQueryF19());
                case "f32":
                    return handle_myquery_reflexe(new BO.myQueryF32());
                case "f31":
                    return handle_myquery_reflexe(new BO.myQueryF31());
                case "j02":
                    return handle_myquery_reflexe(new BO.myQueryJ02());
                case "j04":
                    return handle_myquery_reflexe(new BO.myQueryJ04());
                case "o27":
                    return handle_myquery_reflexe(new BO.myQueryO27());
                case "x31":
                    return handle_myquery_reflexe(new BO.myQueryX31());
                case "z01":
                    return handle_myquery_reflexe(new BO.myQueryZ01());
                case "x40":
                    return handle_myquery_reflexe(new BO.myQueryX40());
                case "xx1":
                    return handle_myquery_reflexe(new BO.myQueryXX1());
                default:
                    return handle_myquery_reflexe(new BO.myQuery(prefix.Substring(0, 3)));
            }
        }

        

        private T handle_myquery_reflexe<T>(T mq)
        {
            if (_mqi != null)   
            {   //na vstupu je explicitní myquery ve tvaru název@typ@hodnota
                for(int i = 0; i < _mqi.Count; i+=3)
                {
                    switch (_mqi[i+1])
                    {
                        case "int":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], Convert.ToInt32(_mqi[i+2]));
                            break;
                        case "date":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], BO.BAS.String2Date(_mqi[i + 2]));
                            break;
                        case "bool":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], BO.BAS.BG(_mqi[i + 2]));
                            break;
                        default:
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], _mqi[i + 2]);
                            break;
                    }
                }
            }   
            else
            {   //filtr podle master_prefix+master_pid
                if (_master_pid > 0 && _master_prefix != null)
                {
                    BO.Reflexe.SetPropertyValue(mq, _master_prefix + "id", _master_pid);
                }
            }
            
            return mq;
        }


        public BO.myQueryA01 LoadA01(string master_prefix = null, int master_pid = 0, string myqueryinline = null)
        {
            handle_myqueryinline_input(myqueryinline);
            _master_prefix = validate_prefix(master_prefix);
            _master_pid = master_pid;

            var mq = new BO.myQueryA01();
            mq = handle_myquery_reflexe(mq);

            return mq;

        }

        public BO.myQueryH04 LoadH04(string master_prefix = null, int master_pid = 0, string myqueryinline = null)
        {
            handle_myqueryinline_input(myqueryinline);
            _master_prefix = validate_prefix(master_prefix);
            _master_pid = master_pid;

            var mq = new BO.myQueryH04();
            mq = handle_myquery_reflexe(mq);
            return mq;
        }

      
        private BO.myQueryA03 load_a03()
        {
            

            var mq = new BO.myQueryA03();
            mq = handle_myquery_reflexe(mq);

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
