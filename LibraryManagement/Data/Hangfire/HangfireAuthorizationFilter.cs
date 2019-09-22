using Hangfire.Annotations;
using Hangfire.Dashboard;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Helper.StaticVariables;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        protected readonly IGlobalSettings _user;
        //private readonly LibraryDbContext _context;
        public HangfireAuthorizationFilter(IGlobalSettings user)
        {
            _user = user;
        }
        public bool Authorize([NotNull] DashboardContext context)
        {
            UserViewModel us = _user.CurrentUser;
            if (us == null)
                return false;
            if (us.UserRoleName.FirstOrDefault() == UserRoleName.Admin)
                return true;
            return false;
        }
    }
}
