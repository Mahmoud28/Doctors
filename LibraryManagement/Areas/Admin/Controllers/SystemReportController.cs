using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Seagull.Core.Areas.Admin.Controllers;
using Seagull.Admin.GridBuild;
using Seagull.Doctors.Data.Interfaces;
using System.Linq;
using Seagull.Core.ViewModel;
using AutoMapper;
using System.Collections.Generic;
using Seagull.Core.Data.Model;
using Seagull.Doctors.ViewModel;
using Seagull.Doctors.Data.Model;
using Seagull.Doctors.Areas.Admin.Models;
using System;
using System.Globalization;
using System.Text;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Seagull.Core.Helper.StaticVariables;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SystemReportController : AdminCoreBusinessController
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _globalSettings;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly LibraryDbContext _dbContext;
        private readonly IMapper _mapper;


        public SystemReportController(IUserRepository userRepository,
            IGlobalSettings globalSettings,
            IStringLocalizer stringLocalizer,
            LibraryDbContext dbContext,
            IMapper mapper,
            IStringLocalizer localization
            )
        {
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _stringLocalizer = stringLocalizer;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [Route("SystemReport")]
        public IActionResult Index()
        {
            return View();
        }
    }
}