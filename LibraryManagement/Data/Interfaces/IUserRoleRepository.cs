using Seagull.Core.Models;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seagull.Core.Data.Model;
using Microsoft.AspNetCore.Http;

namespace Seagull.Core.Data.Interfaces
{

    public interface IUserRoleRepository : IRepository<UserRole>
    {
        UserRole GetById(int id);
        PagingList<UserRole> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
        UserRoleMap MapUserRole(User user, UserRole userRole);
        List<int> GetAllUserRolePermissionByUserRoleId(int id);
        UserRole GetByUserRoleName(string Name);

    }

}
