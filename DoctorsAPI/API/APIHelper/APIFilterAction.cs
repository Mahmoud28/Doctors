
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Seagull.API.APIHelper;

namespace Seagull.Core.Helper.Filters
{
    public class BaseAPIActionFilter : IActionFilter
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

    public class BaseAPIAsyncActionFilter : IAsyncActionFilter
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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheckAPIRequest : ActionFilterAttribute, IActionFilter
    {
        public CheckAPIRequest()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string authHeader = filterContext.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer"))
            {
                APIJsonResult dataController = new APIJsonResult
                {
                    Access = false,
                    success = false,
                    Msg = new List<string> { "SessionTimeOut" },
                    url = "/Home/Index",
                    token = string.Empty
                };
                filterContext.Result = new JsonResult(new { dataController });
            }
        }

        public void OnActionExecuted(ActionExecutingContext context)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParseParameterActionFilterAPI : Attribute, IActionFilter
    {
        //private readonly string _Model;
        //private readonly string _continueEditing;
        //private readonly string _token;

        private Type _clazz;

        public ParseParameterActionFilterAPI(Type clazz)//string Model, string continueEditing, string token)
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
            var y = context.ActionArguments;
            //if (context.ActionArguments.Any(x => x.Key.Equals("jsonData")))
            //{
            //    JsonData g = (JsonData)context.ActionArguments.Where(x => x.Key.Equals("jsonData")).Select(o => o.Value).FirstOrDefault();
            //    context.ActionArguments["jsonData"] = new JsonData() { continueEditing = g.continueEditing, model = g.model.ToObject(_clazz), token = g.token, status = g.status };
            //}
            //else
            //    context.ActionArguments.Add("model", "null");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

    }

}

