using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class PermUserRoleMap
    {
        public int PermId { get; set;}

        public Permission Permission { get; set; }

        public int UserRoleId { get; set;}

        public UserRole UserRole { get; set; }

    }
}
