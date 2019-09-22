using Seagull.Core.Models;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using Seagull.Core.ViewModel;
using Seagull.Doctors.Models;
using Seagull.Core.Data.Model;

namespace Seagull.Core.Data.Interfaces
{
    public interface IDaysRepository : IRepository<Days>
    {
       
        Days GetById(int id);
       
       PagingList<Days> GetAllDayss(pagination pagination, sort sort, string search, string search_operator, string filter);
    }
}
