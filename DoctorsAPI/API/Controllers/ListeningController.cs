using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper.Localization;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Seagull.API.APIModels;
using Seagull.API.LocalizationApi;
using Swashbuckle.AspNetCore.Annotations;
using Seagull.API.APIHelper;
using Seagull.API.APIHelper.Authorization;
using Seagull.API.APIHelper.Mapping;
using Seagull.API.APIModels;
using Seagull.API.Controllers;
using static Seagull.API.APIModels.UserAPI;

namespace Seagull.API.Controllers
{
    [Route("Seagull")]
    public class ListeningController : ControllerBase
    {
        #region Fileds
        public string AcceptLanguage
        {
            get
            {
                return Request.Headers["Accept-Language"].ToString();
            }
        }
        private LibraryDbContext _context;
      
        string filePath = FilePath._Path;
        #endregion
        public ListeningController(LibraryDbContext context)
        {
            _context = context;
        }

       
    }
}