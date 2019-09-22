using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using AutoMapper;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Seagull.Core.ViewModel;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using Seagull.Doctors.ViewModel;
using Seagull.Doctors.Data.Model;
using Seagull.Core.Data.Model;
using System.IO;
using Seagull.Doctors.Helper.ImageHelper;
using System.Globalization;
using Seagull.Doctors.Data;
using Seagull.Tqweemco.Scheduling;
using Seagull.Core.Helper.StaticVariables;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Hangfire;
using System.Text;
using Seagull.Doctors.Models;

namespace Seagull.Core.Controllers
{
    public class HomeController : BaseWebController
    {
       
        [Obsolete]
        public IActionResult Index(string returnUrl = null)
        {
            return View();
        }
       
    }
}
 