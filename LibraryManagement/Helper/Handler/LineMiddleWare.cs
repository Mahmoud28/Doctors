using Seagull.Core.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Seagull.Core.Helper.Handler
{
    public class SeagullMiddleWare
    {
        RequestDelegate _next;
        private IGlobalSettings _user;
        public SeagullMiddleWare(RequestDelegate next, IGlobalSettings user)
        {
            _next = next;
            _user = user;
        }
        public async Task Invoke(HttpContext context)
        {
            string[] allow = { ".js", ".css" };

            if (context.Request.Path.HasValue)
            {
                if (allow.Any(w => context.Request.Path.Value.Contains(w)))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    string culture = "ar-AE";
                    var _culture = new CultureInfo(culture);
                    var _crrentUser = _user.CurrentUser;
                    if (!_user.CurrentUser.IsRtl)
                    {
                        culture = "en-US";
                        _culture = new CultureInfo(culture);
                    }
                    Thread.CurrentThread.CurrentCulture = _culture;
                    Thread.CurrentThread.CurrentUICulture = _culture;
                    await _next.Invoke(context);
                }
            }
            else
            {

            }
        }
        //public async Task Invoke(HttpContext context)
        //{
        //    var sw = new Stopwatch();
        //    sw.Start();

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        var bodyStream = context.Response.Body;
        //        context.Response.Body = memoryStream;

        //        await _next(context);

        //        var isHtml = context.Response.ContentType?.ToLower().Contains("text/html");
        //        if (context.Response.StatusCode == 200 && isHtml.GetValueOrDefault())
        //        {
        //            {
        //                memoryStream.Seek(0, SeekOrigin.Begin);
        //                using (var streamReader = new StreamReader(memoryStream))
        //                {
        //                    var responseBody = await streamReader.ReadToEndAsync();
        //                    var newFooter = @"<footer><div id=""process"">Page processed in {0} milliseconds.</div>";
        //                    responseBody = responseBody.Replace("<footer>", string.Format(newFooter, sw.ElapsedMilliseconds));
        //                    context.Response.Headers.Add("X-ElapsedTime", new[] { sw.ElapsedMilliseconds.ToString() });
        //                    using (var amendedBody = new MemoryStream())
        //                    using (var streamWriter = new StreamWriter(amendedBody))
        //                    {
        //                        streamWriter.Write(responseBody);
        //                        amendedBody.Seek(0, SeekOrigin.Begin);
        //                        await amendedBody.CopyTo(bodyStream);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
