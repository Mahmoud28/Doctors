
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
    public class AllergyRepository : Repository<Allergy>, IAllergyRepository
    {
        private readonly IMapper _mapper;
        public AllergyRepository(LibraryDbContext context,IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public Allergy GetById(int id)
        {
            var Allergy = _context.Set<Allergy>().Where(d => d.Id == id).FirstOrDefault();
            return Allergy;
        }

        public List<AllergyViewModel> GetAllByUserId(int UserId)
        {
            var queryMedicationAllergyMap = _context.Set<MedicationAllergyMap>().Where(a => a.UserId == UserId).Select(x => x.Id);
            List<AllergyViewModel> queryAllergy = _mapper.Map<List<Allergy>,List<AllergyViewModel>>(_context.Set<Allergy>().ToList());
            if (queryMedicationAllergyMap.Count() > 0)
            {
                queryAllergy.Where(s => queryMedicationAllergyMap.Contains(s.Id)).ToList().ForEach(a => {
                    a.Selected = true;
                });
            }
            return queryAllergy;
        }

        public void SaveAllaergytList(int UserId , List<int> AllergyListIds)
        {
            AllergyListIds.ForEach(a => {
                MedicationAllergyMap map = new MedicationAllergyMap
                {
                    UserId = UserId,
                    AllergyId = a,
                };
                _context.Set<MedicationAllergyMap>().Add(map);
                _context.SaveChanges();
            });
        }



        //Get All Allergys
        public PagingList<Allergy> GetAllAllergys(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            IQueryable<Allergy> query = _context.Set<Allergy>();

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
                                    var tempQuery = _context.Set<Allergy>();
                                    query = count == 0 ? query.Where<Allergy>(
                                        (object)_filter.Key, (object)s,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<Allergy>(tempQuery.Where<Allergy>(
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
                        query = query.Where<Allergy>(
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
                                query = query.Where<Allergy>(
                                        (object)_search.Name, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = _context.Set<Allergy>();
                                        query = count == 0 ? query.Where<Allergy>(
                                            (object)_search.Name, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<Allergy>(tempQuery.Where<Allergy>(
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
                                    query = query.Where<Allergy>(
                                    (object)_search.Name, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }

                    }
                }
            }
            query = query.OrderBy<Allergy>(sort.predicate, !sort.reverse ? "asc" : "");


            return new PagingList<Allergy>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }


    }
}
