using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BL
{
    public class ThePeriodProvider
    {
        private List<BO.ThePeriod> _lis;

        public ThePeriodProvider()
        {                        
            var c = new BO.CLS.ThePeriodProviderSupport();
            _lis = c.GetPallete();
        }

        
        public List<BO.ThePeriod> getPallete()
        {
            if (_lis[1].d1 != DateTime.Today)
            {
                var c = new BO.CLS.ThePeriodProviderSupport();
                _lis = c.GetPallete();
            }
            return _lis;
        }
        public BO.ThePeriod ByPid(int pid)
        {
            if (_lis.Where(p => p.pid == pid).Count() > 0)
            {
                return _lis.Where(p => p.pid == pid).First();
            }
            else
            {
                return _lis[0];
            }
        }
        

        
    }
}
