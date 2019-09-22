using Seagull.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class UserRole
    {
        public UserRole()
        {
            fk_UserRoleMap = new List<UserRoleMap>();
            fk_UserRolePermMap = new List<PermUserRoleMap>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<UserRoleMap> fk_UserRoleMap { get; set; }
        public virtual List<PermUserRoleMap> fk_UserRolePermMap { get; set; }

    }
}
