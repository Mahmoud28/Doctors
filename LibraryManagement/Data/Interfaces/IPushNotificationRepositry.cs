using Seagull.Doctors.Data.Model;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Interfaces
{
    public interface IPushNotificationRepositry : IRepository<PushNotification>
    {
        PushNotification GetById(int id);
        PagingList<PushNotification> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
    }
}
