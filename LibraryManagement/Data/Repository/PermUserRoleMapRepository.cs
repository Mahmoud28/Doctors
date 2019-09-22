using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Repository
{
    public class PermUserRoleMapRepository : Repository<PermUserRoleMap>, IPermUserRoleMapRepository
    {
        public PermUserRoleMapRepository(LibraryDbContext context) : base(context)
        {


        }
    }
}
