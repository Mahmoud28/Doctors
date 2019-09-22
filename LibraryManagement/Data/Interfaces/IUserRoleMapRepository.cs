using Seagull.Core.Models;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seagull.Core.Data.Model;

namespace Seagull.Core.Data.Interfaces
{
    
        public interface IUserRoleMapRepository : IRepository<UserRoleMap>
    {
        UserRoleMap GetById(int id);
        PagingList<UserRoleMap> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
        }
    
}
