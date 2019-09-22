using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class Permission
    {
        public Permission()
        {
            
            fk_PermUserRoleMap = new List<PermUserRoleMap>();
        }
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Category { get; set; }

        public virtual List<PermUserRoleMap> fk_PermUserRoleMap { get; set; }
    }
}
