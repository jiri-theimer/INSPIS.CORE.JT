using System;
using System.Collections.Generic;
using System.Text;

namespace BO.Ginis
{
    public class GinisCJ
    {
        public string DenikCJ { get; set; }
        public string RokCJ { get; set; }
        public int PoradoveCisloCJ { get; set; }

        public GinisCJ(string strFullCJ)
        {

            var a = strFullCJ.Split('-');

            this.DenikCJ = a[0];
            if (a.Length <= 1)
                return;

            a = a[1].Split('/');

            int intTest = 0;
            if (int.TryParse(a[0], out intTest))
            {
                this.PoradoveCisloCJ = intTest;
            }



            this.RokCJ = BO.BAS.RightString("20" + a[1], 4);


        }
    }
}
