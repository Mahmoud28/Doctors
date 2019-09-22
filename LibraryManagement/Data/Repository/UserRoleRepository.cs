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
using ReflectionIT.Mvc.Paging;
using Microsoft.EntityFrameworkCore;
using Seagull.Core.Models;
using Microsoft.AspNetCore.Http;
using Seagull.Core.Helper.Authentication;

namespace Seagull.Core.Data.Repository
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private readonly IUserRepository _userRepository;
        private new readonly LibraryDbContext _context;

        public UserRoleRepository(LibraryDbContext context, IUserRepository userRepository) : base(context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public UserRole GetById(int id)
        {
            return _context.Set<UserRole>().Where(d => d.Id == id).Include(p => p.fk_UserRolePermMap).Include(p => p.fk_UserRolePermMap).FirstOrDefault();
        }
        
        public List<int> GetAllUserRolePermissionByUserRoleId(int id)
        {
            return _context.Set<UserRole>().Include(d => d.fk_UserRolePermMap).Where(d => d.Id == id).SelectMany(d => d.fk_UserRolePermMap).Select(o => o.PermId).ToList();
        }

        public PagingList<UserRole> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<UserRole> query = _context.Set<UserRole>().Where(a => a.Name != "Guest");

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
                                    var tempQuery = _context.Set<UserRole>();
                                    query = count == 0 ? query.Where<UserRole>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UserRole>(tempQuery.Where<UserRole>(
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
                        query = query.Where<UserRole>(
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
                                query = query.Where<UserRole>(
                                        (object)_search.Name, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = _context.Set<UserRole>();
                                        query = count == 0 ? query.Where<UserRole>(
                                            (object)_search.Name, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<UserRole>(tempQuery.Where<UserRole>(
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
                                //case "SectorStr":
                                //    query = query.Where(a => a.FK_Sector.Name.Contains(strFk) && a.FK_Sector.ModulType == 1);
                                //    break;
                                case "Id":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Id.ToString()) ? "" : a.Id.ToString())).Contains(strFk));
                                    break;
                                case "Name":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.Name) ? "" : a.Name)).Contains(strFk));
                                    break;
                                default:
                                    query = query.Where<UserRole>(
                                    (object)_search.Name, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<UserRole>(sort.predicate, !sort.reverse ? "asc" : "");


            return new PagingList<UserRole>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }
        
        public UserRoleMap MapUserRole(User user, UserRole userRole)
        {
            var userRoleMap = new UserRoleMap();
            userRoleMap.User = user;
            userRoleMap.UserRole = userRole;// _userRoleRepository.GetById(int.Parse(d));
            return userRoleMap;
        }

        public UserRole GetByUserRoleName(string Name)
        {
            return _context.Set<UserRole>().Where(d => d.Name == Name).Include(p => p.fk_UserRolePermMap).Include(p => p.fk_UserRolePermMap).FirstOrDefault();
        }


        //public User RemoveUserRole(User user, UserRole userRole)
        //{
        //    var userRoleMap = new UserRoleMap();
        //    userRoleMap.User = user;
        //    userRoleMap.UserRole = userRole;
        //    _context.Entry(user).State = EntityState.Modified;
        //    _userRepository.Update(user);
        //    return user;
        //}
    }
}
