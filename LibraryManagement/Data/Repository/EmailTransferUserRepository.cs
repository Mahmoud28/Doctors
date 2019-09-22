using CodeBureau;
using ExtensionMethods;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Data.Interfaces;
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
    public class EmailTransferUserRepository : Repository<EmailTransferUser>, IEmailTransferUserRepository
    {
        public EmailTransferUserRepository(LibraryDbContext context) : base(context)
        {


        }
        public EmailTransferUser GetById(int id)
        {
            return _context.Set<EmailTransferUser>().Where(d => d.Id == id).FirstOrDefault();
        }
        public PagingList<EmailTransferUser> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<EmailTransferUser> query = _context.Set<EmailTransferUser>();

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
                                    query = count == 0 ? query.Where<EmailTransferUser>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<EmailTransferUser>(tempQuery.Where<EmailTransferUser>(
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
                        query = query.Where<EmailTransferUser>(
                                    (object)_filter.Key, (object)result.FirstOrDefault(),
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = JObject.Parse(search);
                foreach (var _search in searchFilter)
                {
                    if (!string.IsNullOrEmpty(_search.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int i;
                            if (!(int.TryParse(checkCurrentKey.Split(',')[0].ToString(), out i)))
                                query = query.Where<EmailTransferUser>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = query;
                                        query = count == 0 ? query.Where<EmailTransferUser>(
                                            (object)_search.Key, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<EmailTransferUser>(tempQuery.Where<EmailTransferUser>(
                                            (object)_search.Key, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                        count = count + 1;
                                    }
                                }
                            }

                        }
                        else
                        {
                            string strFk = (string)_search.Value;
                            switch ((string)_search.Key)
                            {
                                default:
                                    query = query.Where<EmailTransferUser>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<EmailTransferUser>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagingList<EmailTransferUser>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }


    }
}
