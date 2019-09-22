
using CodeBureau;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using Newtonsoft.Json.Linq;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtensionMethods;
using Seagull.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Seagull.Core.Helper.Authentication;
using Seagull.Doctors.Models;
using Seagull.Core.Helper;
using Seagull.Core.ViewModel;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using Seagull.Core.Helper.StaticVariables;

namespace Seagull.Core.Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IMapper _mapper;
        public UserRepository(LibraryDbContext context,IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public User GetById(int id)
        {

            var user = _context.Set<User>().Where(d => d.Id == id).Include(e => e.fk_UserRoleMap).ThenInclude(y => y.UserRole.fk_UserRoleMap).FirstOrDefault();
            return user;
        }

        public List<string> GetByUserNameOrEmail(string UserName, string Email)
        {
            List<string> errors = new List<string>();
            //if (_context.Set<User>().Where(d => d.UserName == UserName).Count() > 0)
            //errors.Add("Admin.Duplicate.UserName");
            if (_context.Set<User>().Where(d => d.Email.ToLower() == Email.ToLower() && d.IsDeleted != true).Count() > 0)
                errors.Add("Admin.Duplicate.Email");

            return errors;
        }
        public User GetByEmail(string email)
        {
            return _context.Users.Where(x => x.Email.Trim().ToLower() == email.Trim().ToLower()).Where(z=>z.IsDeleted != true).FirstOrDefault();
        }

        //type : to get User By there Roles
        public PagingList<User> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter, string type)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<User> query = _context.Set<User>().Where(x => x.IsDeleted != true).Include(e => e.fk_UserRoleMap).ThenInclude(y => y.UserRole.fk_UserRoleMap).Where(a => a.fk_UserRoleMap.Where(r => r.UserRole.Name != UserRoleName.Guest).Count() > 0);
            HashSet<int> ids = new HashSet<int>() { 7 };
            query = type != null ? (type == UserRoleName.User ? query.Where(x => x.fk_UserRoleMap.Where(u => u.UserRole.Name == UserRoleName.User).Count() > 0) : query.Where(x => x.fk_UserRoleMap.Where(u => u.UserRole.Name == type).Count() > 0)) : query;

            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = JObject.Parse(filter);
                foreach (var _filter in searchFilter)
                {
                    string fitlterstr = (string)_filter.Value;
                    var result = fitlterstr.Split(',').Where(r => !string.IsNullOrEmpty(r)).ToList();
                    if (result.Count() > 1)
                    {
                        int count = 0;
                        result.ForEach(s =>
                        {
                            string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";

                            switch ((string)_filter.Key)
                            {
                                case "":
                                    //query = query.Where(a=>a.Sector == Se)
                                    break;
                                default:
                                    var tempQuery = query;
                                    query = count == 0 ? query.Where<User>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<User>(tempQuery.Where<User>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                    break;
                            }
                        });
                    }

                    else if (!string.IsNullOrEmpty(_filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<User>(
                                    (object)_filter.Key, result.FirstOrDefault(),
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = JObject.Parse(search);
                foreach (var _search in searchFilter)
                {
                    if (!object.ReferenceEquals(null, _search.Value) && !string.IsNullOrEmpty(_search.Value.ToString()))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);

                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int i;
                            if (!(int.TryParse(checkCurrentKey.Split(',')[0].ToString(), out i)))
                                query = query.Where<User>(
                                        (object)_search.Name, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                var temp2 = query; /////3-- all 163,164,171
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = query;
                                        query = count == 0 ? query.Where<User>((object)_search.Name, (object)_tempSearchKey, (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<User>(temp2.Where<User>((object)_search.Name, (object)_tempSearchKey, (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));

                                        count = count + 1;
                                    }
                                }
                            }

                        }
                        else
                        {
                            string strFk = (string)_search.Value;
                            switch ((string)_search.Name)
                            {
                                //case "SectorStr":
                                //    query = query.Where(a => a.FK_Sector.Name.Contains(strFk) && a.FK_Sector.ModulType == 1);
                                //    break;
                                case "Name":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Name) ? "" : a.Name)).Contains(strFk));
                                    break;
                                case "Email":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Email) ? "" : a.Email)).Contains(strFk));
                                    break;
                                case "Mobile":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Mobile) ? "" : a.Mobile)).Contains(strFk));
                                    break;

                                default:
                                    query = query.Where<User>(
                                    (object)_search.Name, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<User>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagingList<User>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
            //return _context.Set<T>();
        }

        public User CheckLogin(string UserName, string Password)
        {
            var admin = _context.Users.Where(a => a.Email.ToLower() == UserName.ToLower()).Include(f => f.fk_UserRoleMap).ThenInclude(g => g.UserRole).ThenInclude(p => p.fk_UserRolePermMap).ThenInclude(h => h.Permission).FirstOrDefault();

            if(admin!= null)
            {
                using (var algorithm = MD5.Create()) //or MD5 SHA256 etc.
                {
                    var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Password));
                    Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }

                if (admin.Password == Password)
                {
                    return admin;
                }
            }
            return null;
        }
        public CheckUserAccountModel CheckUserAccount(string UserName, string Password)
        {
            CheckUserAccountModel data = new CheckUserAccountModel();
            //int fildLogIn =Convert.ToInt32(_context.SystemSettings.Where(x => x.Name == "FildLogInNumber").FirstOrDefault().Value);
            var user = _context.Users.Where(x => x.Email.ToLower() == UserName.ToLower() && x.IsDeleted != true).Include(f => f.fk_UserRoleMap).ThenInclude(g => g.UserRole).ThenInclude(p => p.fk_UserRolePermMap).ThenInclude(h => h.Permission).FirstOrDefault();
            if (user != null)
            {
                using (var algorithm = MD5.Create()) //or MD5 SHA256 etc.
                {
                    var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(Password));

                    Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }


                if (user.Password == Password)
                {
                    //user.FildLogIn = 0;
                    _context.Update(user);
                    _context.SaveChanges();

                    data.user = _mapper.Map<User, UserViewModel>(user);
                }
                else
                {
                    data.user = null;
                    data.ErrorMessage = "Admin.Please wait for account activation";
                }
            }
            else
            {
                data.user = null;
                data.ErrorMessage = "Admin.UserNotFound";
            }
            return data;
        }
        public void UpdateUserPlan(int UserId, int PlanId)
        {
            User _user = _context.Set<User>().Where(d => d.Id == UserId).Include(e => e.fk_UserRoleMap).FirstOrDefault();
            Update(_user);
        }

        public User InsertGuestUser(IHttpContextAccessor _accessor, IUserRoleRepository _userRoleRepository)
        {
            User _Guest = new User();
            _Guest.Name = "";
            _Guest.Password = "123";
            _Guest.LangId = 1;
            _Guest.Email = "Test{0}@test.com";
            _Guest.fk_UserRoleMap = new List<UserRoleMap>();

            // you can access your DBContext instance
            //_context.Add(_Guest);
            //_context.SaveChanges();
            _Guest.Email = string.Format(_Guest.Email, new Guid());
            //Update(_Guest);
            _Guest.fk_UserRoleMap.Add(_userRoleRepository.MapUserRole(_Guest, _userRoleRepository.GetById(8)));
            //Update(_Guest);
            //_context.SaveChanges();

            new SysAuthentication(_accessor).SignIn(_Guest.Id);
            return _Guest;
        }
        
        public User CreateUser(UserViewJoinUsViewModel _userModel, IUserRoleRepository _userRoleRepository)
         
        {
            User _user = new User();
            _user.Name = _userModel.Name;
            _user.Email = _userModel.Email;
            _user.Password = _userModel.Password;
            _user.Mobile = _userModel.Mobile;
            _user.fk_UserRoleMap.Add(_userRoleRepository.MapUserRole(_user, _userRoleRepository.GetById(8)));

            _context.Add(_user);
            _context.SaveChanges();

            return _user;
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Where(x => x.Email.Trim().ToLower() == email.Trim().ToLower()).FirstOrDefault();
        }

        public List<User> GetUserByProducerId(int producerId)
        {
            var data = _context.Users.Where(x => x.ProducerId == producerId).ToList();
            return data;
        }

        public List<User> GetAllRegisterUser()
        {
            List<User> query = _context.Set<User>().Where(x => x.IsDeleted != true).Include(e => e.fk_UserRoleMap).ThenInclude(y => y.UserRole.fk_UserRoleMap).Where(a => a.fk_UserRoleMap.Where(r => r.UserRoleId != 8).Count() > 0).ToList();
            query = query.Where(x => x.fk_UserRoleMap.Where(u => u.UserRoleId == 7).Count() > 0).ToList();
            return query;
        }

        //Get All Users
        public PagingList<User> GetAllUsers(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<User> query = _context.Set<User>();

            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = JObject.Parse(filter);
                foreach (var _filter in searchFilter)
                {
                    string fitlterstr = (string)_filter.Value;
                    var result = fitlterstr.Split(',').Where(r => !string.IsNullOrEmpty(r)).ToList();
                    if (result.Count() > 1)
                    {
                        int count = 0;
                        result.ForEach(s =>
                        {
                            string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";

                            switch ((string)_filter.Key)
                            {
                                case "":
                                    break;
                                default:
                                    var tempQuery = _context.Set<User>();
                                    query = count == 0 ? query.Where<User>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<User>(tempQuery.Where<User>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                    break;
                            }
                        });
                    }

                    else if (!Object.ReferenceEquals(null, _filter.Value) && !String.IsNullOrEmpty(_filter.Value.ToString()))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Name).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Name).FirstOrDefault().Value : "eq";
                        query = query.Where<User>(
                                    (object)_filter.Name, (object)result.FirstOrDefault(),
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = JObject.Parse(search);
                foreach (var _search in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _search.Value) && !String.IsNullOrEmpty(_search.Value.ToString()))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int i;
                            if (!(int.TryParse(checkCurrentKey.Split(',')[0].ToString(), out i)))
                                query = query.Where<User>(
                                        (object)_search.Name, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = _context.Set<User>();
                                        query = count == 0 ? query.Where<User>(
                                            (object)_search.Name, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<User>(tempQuery.Where<User>(
                                            (object)_search.Name, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                        count = count + 1;
                                    }
                                }
                            }

                        }
                        else
                        {
                            string strFk = (string)_search.Value;
                            switch ((string)_search.Name)
                            {
                                case "Name":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Name) ? "" : a.Name)).Contains(strFk));
                                    break;
                                default:
                                    query = query.Where<User>(
                                    (object)_search.Name, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<User>(sort.predicate, !sort.reverse ? "asc" : "");


            return new PagingList<User>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }


    }
}
