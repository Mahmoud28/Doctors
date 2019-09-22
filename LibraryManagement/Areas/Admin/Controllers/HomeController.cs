using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Models;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Seagull.Core.Helper;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data.Model;
using AutoMapper;
using ReflectionIT.Mvc.Paging;
using Microsoft.Extensions.Caching.Memory;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Helper.Filters;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : AdminCoreBusinessController
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _globalSettings;
        private readonly IMapper _mapper;
        private readonly ICacheServicecs _cache;

        public HomeController(
            IUserRepository userRepository, IGlobalSettings globalSettings,
            IMapper mapper,
            ICacheServicecs cache)
        {
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _mapper = mapper;
            _cache = cache;
        }

        public IActionResult Index()
        {
            _cache.SetCache(string.Format(CacheKeys.CustomCache, _globalSettings.CurrentUser.Id), 0.ToString());
            return View();
        }
    }
}
