using Seagull.Core.Data.Model;
using Seagull.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Permission GetById(int id);
        List<PermissionRecordModelAngular> GetAllPermissionRecordForTree();
        bool CheckAccess(string PermName);
        bool CheckCacheKey();
    }
}
