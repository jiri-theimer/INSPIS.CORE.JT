using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Fidoo
{

    //Volání uživatelského účtu podle e-mail adresy
    public class UserByEmail
    {
        public string email { get; set; }
    }


    //vstupní filtr pro seznam středisek a projektů
    public class QueryRequestRoot
    {
        public Queryrequest queryRequest { get; set; }
    }

    public class Queryrequest
    {
        public int limit { get; set; } = 100;
        public int offset { get; set; }
        public Sort[] sort { get; set; }
    }

    public class Sort
    {
        public bool ascendant { get; set; }
        public string property { get; set; }
    }


    //vstupní filtr pro seznam výdajů

    public class QueryRequestExpenses
    {
        public DateTime from { get; set; }
        public DateTime to { get; set; }
        public DateTime lastModifyFrom { get; set; }
        public int limit { get; set; }
    }


    




}
