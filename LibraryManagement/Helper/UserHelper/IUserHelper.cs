using Seagull.Core.Areas.Admin.ViewModel;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper
{
    public interface IGlobalSettings
    {
        UserViewModel CurrentUser { get; }
        SystemSettingViewModel CurrentSystemSetting { get; }
        void RefreshUser();
}
}
