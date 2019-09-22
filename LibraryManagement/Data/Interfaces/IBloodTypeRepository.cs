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
    public interface IBloodTypeRepository : IRepository<BloodType>
    {

        BloodType GetById(int id);
       
       PagingList<BloodType> GetAllBloodTypes(pagination pagination, sort sort, string search, string search_operator, string filter);
    }
}
