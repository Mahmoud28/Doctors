//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Seagull.Doctors.Data.Interfaces
//{
//    public class IPastSurgicalHistory
//    {
//    }
//}
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
using Seagull.Doctors.Data.Model;

namespace Seagull.Core.Data.Interfaces
{
    public interface IPastSurgicalHistory : IRepository<PastSurgicalHistory>
    {

        PastSurgicalHistory GetById(int id);
    }
}

