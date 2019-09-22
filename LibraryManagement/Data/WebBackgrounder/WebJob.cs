using Hangfire.Annotations;
using Hangfire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.WebBackgrounder
{
    public class WebJob : Job
    {
        public WebJob([NotNull] MethodInfo method) : base(method)
        {
        }
    }
}
