using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper
{

    //public abstract class RazorPage<TModel> : RazorPage
    //{
    //    // now this will be available in any view @HelloWorld()
    //    public string HelloWorld()
    //    {
    //        return "Hello from the ViewBase class";
    //    }
    //}
    public class IndexModel2 : PageModel
    {
        public string Message { get; private set; } = "PageModel in C#";

        public void OnGet()
        {
            Message += $" Server time is { DateTime.Now }";
        }
    }
}
