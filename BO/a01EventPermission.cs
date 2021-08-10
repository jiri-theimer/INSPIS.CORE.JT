using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class a01EventPermission
    {
        public List<a01EventPermissionENUM> PermValue { get; set; }

        
        public bool IsRecordOwner { get; set; }


        public bool HasPerm(a01EventPermissionENUM perm)
        {
            if (PermValue.Contains(perm))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
