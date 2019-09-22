using Seagull.Core.Models;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using Seagull.Core.ViewModel;
using Seagull.Doctors.Models;

namespace Seagull.Core.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        PagingList<User> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter,string type);
        User CheckLogin(string UserName, string Password);
        CheckUserAccountModel CheckUserAccount(string UserName, string Password);
        User GetById(int id);
        void UpdateUserPlan(int UserId, int PlanId);
        List<string> GetByUserNameOrEmail(string UserName, string Email);
        User InsertGuestUser(IHttpContextAccessor _accessor , IUserRoleRepository _userRoleRepository);

        User CreateUser(UserViewJoinUsViewModel _userModel, IUserRoleRepository _userRoleRepository );
        User GetUserByEmail(string email);
        User GetByEmail(string email);
        List<User> GetUserByProducerId(int producerId);
        List<User> GetAllRegisterUser();
       PagingList<User> GetAllUsers(pagination pagination, sort sort, string search, string search_operator, string filter);
    }
}
