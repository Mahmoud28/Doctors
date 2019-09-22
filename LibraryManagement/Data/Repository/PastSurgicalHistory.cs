//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Seagull.Doctors.Data.Repository
//{
//    public class PastSurgicalHistory
//    {
//    }
//}

using CodeBureau;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using Newtonsoft.Json.Linq;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtensionMethods;
using Seagull.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Seagull.Core.Helper.Authentication;
using Seagull.Doctors.Models;
using Seagull.Core.Helper;
using Seagull.Core.ViewModel;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using Seagull.Core.Helper.StaticVariables;
using Seagull.Doctors.Data.Model;

namespace Seagull.Core.Data.Repository
{
    public class PastSurgicalHistoryRepository : Repository<PastSurgicalHistory>, IPastSurgicalHistory
    {
        private readonly IMapper _mapper;
        public PastSurgicalHistoryRepository(LibraryDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public PastSurgicalHistory GetById(int id)
        {

            var PastSurgicalHistory = _context.Set<PastSurgicalHistory>().FirstOrDefault();
            return PastSurgicalHistory;
        }


    }
}
