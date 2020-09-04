using System;
using System.Linq;
using System.Security.Claims;

namespace UIFT.Security
{
    public class UIFTUser : ClaimsPrincipal
    {
        public UIFTUser(ClaimsIdentity identity) : base(identity)
        {
        }

        public string Name
        {
            get;
            set;
        }

        public string Jmeno
        {
            get;
            set;
        }

        public string Permissions
        {
            get;
            set;
        }

        public int[] Roles
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Vraci true, pokud je uzivatel pro aktualni formular v dane roli
        /// </summary>
        public override bool IsInRole(string role)
        {
            EventRoles evRole;
            if (Enum.TryParse(role, out evRole))
            {
                return this.Roles.Contains(Convert.ToInt32(evRole));
            }
            else
            {
                throw new Exception("Role not supported: " + role);
            }
        }

        /// <summary>
        /// Vraci true, pokud je uzivatel pro aktualni formular v dane roli
        /// </summary>
        public bool IsInRole(EventRoles role)
        {
            return this.Roles.Contains(Convert.ToInt32(role));
        }

        /// <summary>
        /// Vraci true, pokud ma uzivatel prirazene dane pravo.
        /// </summary>
        public bool HasPersmission(BO.j05PermFlagEnum permission)
        {
            return Permissions[Convert.ToInt32(permission)] == '1';
        }

        /// <summary>
        /// Vraci umele vytvorenou instanci sysUser pro vytvoreni BL repository.
        /// </summary>
        public BO.RunningUser GetSysUser()
        {
            if (this._sysUser == null)
            {
                this._sysUser = new BO.RunningUser();
                this._sysUser.j03Login = this.Name;
                this._sysUser.pid = this.ID;
            }

            return this._sysUser;
        }
        private BO.RunningUser _sysUser;
    }
}