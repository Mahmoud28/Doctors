using Seagull.Core.Helper.Handler;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseSeagullMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SeagullMiddleWare>();
        }
    }
}
