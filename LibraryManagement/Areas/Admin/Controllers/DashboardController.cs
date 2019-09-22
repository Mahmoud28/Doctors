using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Repository;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Models;
using Seagull.Core.Helper.Filters;
using Seagull.Doctors.Data.Interfaces;
using AutoMapper;
using Seagull.Doctors.Areas.Admin.ViewModel;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : AdminCoreBusinessController
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _globalSettings;
        private readonly ICacheServicecs _cache;
        private readonly IMapper _mapper;

        public DashboardController(IUserRepository userRepository, IGlobalSettings globalSettings, ICacheServicecs cache,
           IMapper mapper)
        {
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _cache = cache;
            _mapper = mapper;
        }
        //id is PlanId
        public IActionResult Index(int id)
        {
            //_cache.SetCache(string.Format(CacheKeys.CustomCache, _globalSettings.CurrentUser.Id), id.ToString());
            return View(id);
        }

        public class FullCalendar
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}