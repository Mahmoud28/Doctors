using Seagull.Core.Helper.Localization;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Seagull.Core.Areas.Admin.Controllers.LocalizationController;

namespace Seagull.Core.Data.Interfaces
{
    public interface ILocalizationRepository
    {
        List<JsonLocalization> GetAll();
        List<JsonLocalization> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
        void AddOrUpdateLocalize(CustomLocalizeModel entity);
    }
}
