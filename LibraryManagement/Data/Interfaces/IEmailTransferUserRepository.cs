using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Interfaces
{
    public interface IEmailTransferUserRepository : IRepository<EmailTransferUser>
    {
        EmailTransferUser GetById(int id);
        PagingList<EmailTransferUser> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
    }


}
