using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;
using Seagull.Core.Helper.Localization;
using System.IO;

namespace Seagull.Core.ViewEngines.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class WebViewEngine<TModel> : RazorPage<TModel>
    {
        private readonly IStringLocalizer _localizerResource;
        private JsonLocalization _localizer;
        /// <summary>
        /// Get a localized resources
        /// </summary>
        public JsonLocalization T
        {
            get
            {
                return _localizer;
            }
        }
    }
}