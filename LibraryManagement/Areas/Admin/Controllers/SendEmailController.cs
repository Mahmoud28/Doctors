using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Core.Areas.Admin.Controllers;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helpers_Extensions.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Seagull.Doctors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SendEmailController : AdminCoreBusinessController
    {
        private readonly IEmailTransferRepository _emailTransferRepositry;
        private readonly IEmailTransferUserRepository _emailTransferUserRepositry;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _localization;
        private readonly IPermissionRepository _permissionUser;

        public SendEmailController (IEmailTransferRepository emailTransferRepositry, IEmailTransferUserRepository emailTransferUserRepositry, IMapper mapper,
            IStringLocalizer localization, IPermissionRepository permissionUser)
        {
            _emailTransferRepositry = emailTransferRepositry;
            _emailTransferUserRepositry = emailTransferUserRepositry;
            _mapper = mapper;
            _localization = localization;
            _permissionUser = permissionUser;

        }

        [Route("SendEmail")]
        public IActionResult Index()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.SendEmailList))
                return AccessDeniedView();
            return View();
        }

        public IActionResult List([FromBody]PagingClass paging)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.SendEmailList))
                return AccessDeniedJson();

            var angularTable = new DataSourceAngular
            {

                data = null,
                data_count = 0,
                page_count = 0,
            };
            return Json(angularTable);
        }

        [HttpGet]
        public IActionResult PrepareEmail(int Id)
        {
            if (Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.SendEmailEdit))
                    return AccessDeniedView();
                var model = _emailTransferRepositry.GetById(Id);
                
            }
            return View(Id);
        }

        [HttpPost]
        public IActionResult CreateOrEditModel([FromBody]int Id)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.success = true;
            EmailTransferViewModel model = new EmailTransferViewModel();
            if (Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.SendEmailEdit))
                    return AccessDeniedJson();
                model = _mapper.Map<EmailTransfer, EmailTransferViewModel>(_emailTransferRepositry.GetById(Id));
                result.data = model;

            }
            else
            {
                result.data = new EmailTransferViewModel();
            }
            result.Access = true;
            result.success = true;


            return Json(result);
        }

    }
}