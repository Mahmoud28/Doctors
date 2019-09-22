using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper.Filters;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocalizationController : AdminCoreBusinessController
    {
        private readonly ILocalizationRepository _stringLocalizer;
        private readonly IMapper _mapper;
        public LocalizationController(ILocalizationRepository stringLocalizer, IMapper mapper)
        {
            _stringLocalizer = stringLocalizer;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public class CustomLocalizeModel
        {
            public string Resource { get; set; }
            public string Arabic { get; set; }
            public string English { get; set; }
        }
        [HttpPost]
        public IActionResult List([FromBody]PagingClass paging)
        {
            //var localization = _stringLocalizer.GetAll();

            var angularTable = new DataSourceAngular
            {
                data = (from a in _stringLocalizer.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter)
                        select new CustomLocalizeModel
                        {
                            Resource = a.Key,
                            Arabic = a.LocalizedValue["ar-AE"].ToString(),
                            English = a.LocalizedValue["en-US"].ToString()
                        }).ToList(),
                data_count = 0,
                page_count = 1,
            };
            //angularTable.data_count = angularTable.data.Count();
            return Json(angularTable);
        }


        [HttpPost, ParseParameterActionFilter(typeof(CustomLocalizeModel))]
        public IActionResult CreateOrEdit([FromBody]JsonData jsonData)
        {

            JsonResultHelper result = new JsonResultHelper();
            CustomLocalizeModel model = jsonData.model;
            _stringLocalizer.AddOrUpdateLocalize(model);
            result.success = true;
            return Json(result);
        }
    }
}