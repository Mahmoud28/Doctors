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
    public interface IUsersNotificationRepositry : IRepository<UsersNotification>
    {
        UsersNotification GetById(int id);
        PagingList<UsersNotification> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter);
        List<UsersNotification> GetByPushNotificationId(int notificationId);
    }
}
