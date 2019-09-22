using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Seagull.Core.Helper.StaticVariables;

namespace Seagull.Core.Helper.Filters
{
    public class BaseActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // runs before action method
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // runs after action method
        }
    }

    public class BaseAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // runs before action method
            await next();
            // runs after action method
        }
    }

    public class SkipActionFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new ContentResult
            {
                Content = "I'll skip the action execution"
            };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Will not reach here
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParseParameterActionFilter : Attribute, IActionFilter
    {
        //private readonly string _Model;
        //private readonly string _continueEditing;
        //private readonly string _token;

        private Type _clazz;

        public ParseParameterActionFilter(Type clazz)//string Model, string continueEditing, string token)
        {
            //this._Model = Model;
            //this._continueEditing = continueEditing;
            //this._token = token;
            this._clazz = clazz;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Object model = Activator.CreateInstance(_clazz);
            //if (context.ActionArguments.TryGetValue("model", out model))
            if (context.ActionArguments.Any(x => x.Key.Equals("jsonData")))
            {
                JsonData g = (JsonData)context.ActionArguments.Where(x => x.Key.Equals("jsonData")).Select(o => o.Value).FirstOrDefault();
                context.ActionArguments["jsonData"] = new JsonData() { continueEditing = g.continueEditing, model = g.model.ToObject(_clazz), token = g.token , status = g.status };
            }
            //else
            //    context.ActionArguments.Add("model", "null");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParseParameterDeleteAction : Attribute, IActionFilter
    {

        private Type _clazz;

        public ParseParameterDeleteAction(Type clazz)
        {
            this._clazz = clazz;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Object model = Activator.CreateInstance(_clazz);

            if (context.ActionArguments.Any(x => x.Key.Equals("deleteModel")))
            {
                DeleteModel g = (DeleteModel)context.ActionArguments.Where(x => x.Key.Equals("deleteModel")).Select(o => o.Value).FirstOrDefault();
                context.ActionArguments["deleteModel"] = new DeleteModel() { Id = g. Id , token = g.token};
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParseParameterActionFilterModel : Attribute, IActionFilter
    {
        //private readonly string _Model;
        //private readonly string _continueEditing;
        //private readonly string _token;

        private Type _clazz;

        public ParseParameterActionFilterModel(Type clazz)//string Model, string continueEditing, string token)
        {
            //this._Model = Model;
            //this._continueEditing = continueEditing;
            //this._token = token;
            this._clazz = clazz;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Object model = Activator.CreateInstance(_clazz);
            if (context.ActionArguments.Any(x => x.Key.Equals("data")))
            {
                JsonDataModel g = (JsonDataModel)context.ActionArguments.Where(x => x.Key.Equals("data")).Select(o => o.Value).FirstOrDefault();
                context.ActionArguments["data"] = new JsonDataModel() { model = g.model.ToObject(_clazz)};
            }
           
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParseJQReportActionFilter : Attribute, IActionFilter
    {

        private Type _clazz;

        public ParseJQReportActionFilter(Type clazz)
        {
            this._clazz = clazz;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Object model = Activator.CreateInstance(_clazz);
            //if (context.ActionArguments.Any(x => x.Key.Equals("jsonData")))
            //{
            //    JsonData g = (JsonData)context.ActionArguments.Where(x => x.Key.Equals("jsonData")).Select(o => o.Value).FirstOrDefault();
            //    context.ActionArguments["jsonData"] = new JsonData() { continueEditing = g.continueEditing, model = g.model.ToObject(_clazz), token = g.token, status = g.status };
            //}
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheckPermissionFilter : ActionFilterAttribute ,IActionFilter
    {
        private string _permName;
        private string _additionalPermName;
        private string _type;
        private readonly IGlobalSettings _globalSettings;
        private readonly IPermissionRepository _permissionUser;

        public CheckPermissionFilter(string permName, string additionalPermName, string type , IGlobalSettings globalSettings, IPermissionRepository permissionUser)
        {

            _permName = permName;
            _additionalPermName = additionalPermName;
            _globalSettings = globalSettings;
            _permissionUser = permissionUser;
            _type= type;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string currentPermName = _permName;
            //Check The Status Of Model If Exist
            if (filterContext.ActionArguments.Count() > 0)
            {
                var idKey = filterContext.ActionArguments.Where(a => a.Key == "Id");
                if (idKey.Count() > 0)
                {
                    //Model State Is Add
                    if (!(int.Parse(idKey.FirstOrDefault().Value.ToString()) == 0))
                        currentPermName = _permName;
                    else
                        //Model State Is Edit
                        currentPermName = _additionalPermName;
                }
            }
            if (!_permissionUser.CheckCacheKey())
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.Headers["XRequestedWith"] == "ajax")
                {
                    // the controller action was invoked with an AJAX request
                    JsonResultHelper dataController = new JsonResultHelper
                    {
                        Access = false,
                        success = false,
                        Msg = new List<string> { "SessionTimeOut" },
                        url = "/Users/SessionTimeOut"
                    };
                    filterContext.Result = new JsonResult(new { dataController });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Users" },
                    { "action", "SessionTimeOut" }
                    });
                }
            }
            if (!_permissionUser.CheckAccess(currentPermName))
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.Headers["XRequestedWith"] == "ajax")
                {
                    // the controller action was invoked with an AJAX request
                    JsonResultHelper dataController = new JsonResultHelper
                    {
                        Access = false,
                        success = false,
                        Msg = new List<string> { "Denied" },
                        url = "/Users/AccessDenied"
                    };
                    filterContext.Result = new JsonResult(new { dataController });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Users" },
                    { "action", "AccessDenied" }
                    });
                }
            }
        }

        public void OnActionExecuted(ActionExecutingContext context)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheckPlanCachKey : ActionFilterAttribute, IActionFilter
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IPermissionRepository _permissionUser;

        public CheckPlanCachKey(IGlobalSettings globalSettings, IPermissionRepository permissionUser)
        {
            _globalSettings = globalSettings;
            _permissionUser = permissionUser;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_permissionUser.CheckCacheKey())
            {
                string[] allowNullCachController = new string[] { "PlanOperation", "Home", "Dashboard" };
                string[] allowNullCachActions = new string[] { "Index", "List" };

                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string actionName = filterContext.RouteData.Values["action"].ToString();

                if (allowNullCachController.Contains(controllerName) && allowNullCachActions.Contains(actionName))
                    return;

                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.Headers["XRequestedWith"] == "ajax")
                {
                    // the controller action was invoked with an AJAX request
                    JsonResultHelper dataController = new JsonResultHelper
                    {
                        Access = false,
                        success = false,
                        Msg = new List<string> { "SessionTimeOut" },
                        url = "/Home/Index"
                    };
                    filterContext.Result = new JsonResult(new { dataController });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Home" },
                    { "action", "Index" }
                    });
                }
            }
        }

        public void OnActionExecuted(ActionExecutingContext context)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheckGuestUser : ActionFilterAttribute, IActionFilter
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IPermissionRepository _permissionUser;

        public CheckGuestUser(IGlobalSettings globalSettings, IPermissionRepository permissionUser)
        {
            _globalSettings = globalSettings;
            _permissionUser = permissionUser;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (_globalSettings.CurrentUser.UserRoleName.Contains(UserRoleName.Guest))
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.Headers["XRequestedWith"] == "ajax")
                {
                    // the controller action was invoked with an AJAX request
                    JsonResultHelper dataController = new JsonResultHelper
                    {
                        Access = false,
                        success = false,
                        Msg = new List<string> { "SessionTimeOut" },
                        url = "/Home/Index"
                    };
                    filterContext.Result = new JsonResult(new { dataController });
                }
                else
                {
                    //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    //{ "controller", "Home" },
                    //{ "action", "Index" },
                    //{ "area ", "" }
                    //});
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        action = "Index",
                        controller = "Home",
                        area = ""
                    }));
                }
            }
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class ParseWebParameterActionFilter : Attribute, IActionFilter
        {
            //private readonly string _Model;
            //private readonly string _continueEditing;
            //private readonly string _token;

            private Type _clazz;

            public ParseWebParameterActionFilter(Type clazz)//string Model, string continueEditing, string token)
            {
                //this._Model = Model;
                //this._continueEditing = continueEditing;
                //this._token = token;
                this._clazz = clazz;
            }
            public void OnActionExecuting(ActionExecutingContext context)
            {
                Object model = Activator.CreateInstance(_clazz);
                //if (context.ActionArguments.TryGetValue("model", out model))
                if (context.ActionArguments.Any(x => x.Key.Equals("model")))
                {
                    var f = (dynamic)context.ActionArguments.Where(x => x.Key.Equals("model")).Select(o => o.Value).FirstOrDefault();
                    context.ActionArguments["model"] = f.ToObject(_clazz);
                }
                //else
                //    context.ActionArguments.Add("model", "null");
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

        }
        public void OnActionExecuted(ActionExecutingContext context)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UserAcoount : ActionFilterAttribute, IActionFilter
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IPermissionRepository _permissionUser;

        public UserAcoount(IGlobalSettings globalSettings, IPermissionRepository permissionUser)
        {
            _globalSettings = globalSettings;
            _permissionUser = permissionUser;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!(_globalSettings.CurrentUser.UserRoleName.Contains(UserRoleName.User)))
            {
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || filterContext.HttpContext.Request.Headers["XRequestedWith"] == "ajax")
                {
                    // the controller action was invoked with an AJAX request
                    // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
                    //filterContext.Result = new HttpUnauthorizedResult();
                    var routeValues = new RouteValueDictionary();
                    routeValues["controller"] = "Login";
                    routeValues["action"] = "UserLogin";
                    //Other route values if needed.
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        action = "UserLogin",
                        controller = "Login",
                        area = ""
                    }));
                }
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class ParseWebParameterActionFilter : Attribute, IActionFilter
        {
            //private readonly string _Model;
            //private readonly string _continueEditing;
            //private readonly string _token;

            private Type _clazz;

            public ParseWebParameterActionFilter(Type clazz)//string Model, string continueEditing, string token)
            {
                //this._Model = Model;
                //this._continueEditing = continueEditing;
                //this._token = token;
                this._clazz = clazz;
            }
            public void OnActionExecuting(ActionExecutingContext context)
            {
                Object model = Activator.CreateInstance(_clazz);
                //if (context.ActionArguments.TryGetValue("model", out model))
                if (context.ActionArguments.Any(x => x.Key.Equals("model")))
                {
                    var f = (dynamic)context.ActionArguments.Where(x => x.Key.Equals("model")).Select(o => o.Value).FirstOrDefault();
                    context.ActionArguments["model"] = f.ToObject(_clazz);
                }
                //else
                //    context.ActionArguments.Add("model", "null");
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }

        }
        public void OnActionExecuted(ActionExecutingContext context)
        {
        }
    }
}

