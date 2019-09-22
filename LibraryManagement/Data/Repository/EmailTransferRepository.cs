using CodeBureau;
using ExtensionMethods;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Core.Data;
using Seagull.Core.Data.Repository;
using Seagull.Core.Helper;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Repository
{
    public class EmailTransferRepository : Repository<EmailTransfer>, IEmailTransferRepository
    {
        private readonly IGlobalSettings _globalSettings;

        public EmailTransferRepository(LibraryDbContext context, IGlobalSettings globalSettings) : base(context)
        {
            _globalSettings = globalSettings;
        }
        public EmailTransfer GetById(int id)
        {
            return _context.Set<EmailTransfer>().Where(d => d.Id == id).FirstOrDefault();
        }
        public PagingList<EmailTransfer> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<EmailTransfer> query = _context.Set<EmailTransfer>();

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
                                    query = count == 0 ? query.Where<EmailTransfer>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<EmailTransfer>(tempQuery.Where<EmailTransfer>(
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
                        query = query.Where<EmailTransfer>(
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
                    if (!Object.ReferenceEquals(null, _search.Value) && !String.IsNullOrEmpty(_search.Value.ToString()))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Name).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int i;
                            if (!(int.TryParse(checkCurrentKey.Split(',')[0].ToString(), out i)))
                                query = query.Where<EmailTransfer>(
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
                                        query = count == 0 ? query.Where<EmailTransfer>(
                                            (object)_search.Key, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<EmailTransfer>(tempQuery.Where<EmailTransfer>(
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
                            switch ((string)_search.Name)
                            {
                                case "Message":
                                    string[] statusen = { "message"};
                                    string[] statusar = { "نص الرسالة" };
                                    int[] result;
                                    if (!_globalSettings.CurrentUser.IsRtl)
                                        result = statusen.Select((x, i) => x.Contains(strFk) ? i : -1).Where(x => x != -1).ToArray();
                                    else
                                        result = statusar.Select((x, i) => x.Contains(strFk) ? i : -1).Where(x => x != -1).ToArray();
                                    if (result.Count() == 0)
                                        query = query.Except(query) ;
                                  
                                    break;

                                case "UserCounts":
                                    query = query.Where(a => ((string.IsNullOrEmpty(a.UserCounts.ToString()) ? "" : a.UserCounts.ToString())).Contains(strFk));

                                    break;
                                default:
                                    query = query.Where<EmailTransfer>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<EmailTransfer>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagingList<EmailTransfer>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
            //return _context.Set<T>();
        }




    }
}
