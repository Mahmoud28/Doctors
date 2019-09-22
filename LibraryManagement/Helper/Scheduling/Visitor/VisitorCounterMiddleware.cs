using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Seagull.Core.Areas.Admin.ViewModel;
using Seagull.Core.Data;
using Seagull.Core.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Seagull.Tqweemco.Scheduling
{
    public class VisitorCounterMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
    
 
        public VisitorCounterMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
  
     
        }

        public async Task Invoke(HttpContext context)
        {
            string visitorId = context.Request.Cookies["VisitorId"];
            if (visitorId == null)
            {
                //SystemSettingViewModel systemSettings = new SystemSettingViewModel();
                //systemSettings.WebSiteVisitor = systemSettings.WebSiteVisitor + 1;

                //var data = _mapper.Map<SystemSettingViewModel,SystemSetting>(systemSettings);

                //_context.SystemSettings.Update(data);
                //don the necessary staffs here to save the count by one

                context.Response.Cookies.Append("VisitorId", Guid.NewGuid().ToString(), new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = false,
                });
            }

            await _requestDelegate(context);
        }
    }
}