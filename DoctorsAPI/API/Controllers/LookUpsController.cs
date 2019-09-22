﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Seagull.API.APIHelper;
using Seagull.API.Controllers;
using Seagull.Core.Data;
using Seagull.Core.Data.Model;
using Seagull.Doctors.Data.Model;
using Seagull.Doctors.Helper.ImageHelper;
using Swashbuckle.AspNetCore.Annotations;

namespace Doctors.API.Controllers
{
    [Route("Lists")]
    public class LookUpsController : ControllerBase
    {
        private LibraryDbContext _context;
        private readonly DbSet<MedicinesLookUp> _medicinesLookUp;
        private readonly DbSet<Days> _days;

        public Paths _paths = new Paths();
        public static IConfiguration Configuration { get; set; }
        public LookUpsController(LibraryDbContext context)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _context = context;
            _medicinesLookUp = _context.Set<MedicinesLookUp>();
            _days = _context.Set<Days>();
            Configuration = builder.Build();
        }

        [HttpGet("Days")]
        public ActionResult Days()
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            var data = _days.ToList();
            result.data = data;
            return Ok(result);
        }

        [HttpGet("MedicinesList")]
        public ActionResult MedicinesList(PagingModel paging)
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            paging.PageNumber = paging.PageNumber == 0 ? 0 : paging.PageNumber;
            paging.Count = paging.Count == 0 ? 100 : paging.Count;
            var data = _medicinesLookUp.ToList().Skip((paging.PageNumber * paging.Count)).Take(paging.Count).ToList();
            result.data = data;
            return Ok(result);
        }
    }
}