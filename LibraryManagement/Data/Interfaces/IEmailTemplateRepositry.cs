using Seagull.Core.Data.Interfaces;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seagull.Core.Data;

namespace Seagull.Doctors.Data.Interfaces
{
    public interface IEmailTemplateRepositry : IRepository<EmailTemplate>
    {
        EmailTemplate GetById(int id);
        PagingList<EmailTemplate> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
        List<EmailTemplate> GetEmailTemplates();
    }
}
