using CodeBureau;
using ExtensionMethods;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Data.Model;
using Seagull.Core.Data;
using Seagull.Core.Data.Repository;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Repository
{
    public class UsersNotificationRepositry : Repository<UsersNotification>, IUsersNotificationRepositry
    {
        public UsersNotificationRepositry(LibraryDbContext context) : base(context)
        {
        }
        public UsersNotification GetById(int id)
        {
            return _context.Set<UsersNotification>().Where(d => d.Id == id).FirstOrDefault();
        }
        public PagingList<UsersNotification> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<UsersNotification> query = _context.Set<UsersNotification>().Where(x => x.IsDeleted != true);

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
                                    var tempQuery = _context.Set<UsersNotification>();
                                    query = count == 0 ? query.Where<UsersNotification>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UsersNotification>(tempQuery.Where<UsersNotification>(
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
                        query = query.Where<UsersNotification>(
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
                                query = query.Where<UsersNotification>(
                                        (object)_search.Name, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = _context.Set<UsersNotification>();
                                        query = count == 0 ? query.Where<UsersNotification>(
                                            (object)_search.Name, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<UsersNotification>(tempQuery.Where<UsersNotification>(
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

                                default:
                                    query = query.Where<UsersNotification>(
                                    (object)_search.Name, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<UsersNotification>(sort.predicate, !sort.reverse ? "asc" : "");


            return new PagingList<UsersNotification>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }

        public List<UsersNotification> GetByPushNotificationId(int notificationId)
        {
            return _context.Set<UsersNotification>().Where(x => x.PushNotificationId == notificationId & x.IsDeleted != true).ToList();
        }
    }
}
