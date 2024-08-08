using System;
using Medical.UI.Extentions;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Medical.UI.Filter
{
    //public class AuthFilter : IActionFilter
    //{
    //    public void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        var controller = context.Controller as ControllerBase;
    //        if (context.HttpContext.Request.Cookies["token"] == null)
    //        {
    //            context.Result = controller.RedirectToAction(
    //         actionName: "Login",
    //         controllerName: "Account",
    //         new { returnUrl = context.HttpContext.Request.Path }
    //     );
    //        }
    //    }

    //    public void OnActionExecuted(ActionExecutedContext context)
    //    {

    //    }
    //}
    public class AuthFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICrudService _crudService;

        public AuthFilter(IHttpContextAccessor httpContextAccessor, ICrudService crudService)
        {
            _httpContextAccessor = httpContextAccessor;
            _crudService = crudService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            if (token == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            var passwordResetRequired = _httpContextAccessor.HttpContext.Session.GetBool("PasswordResetRequired");
            if (passwordResetRequired)
            {
                context.Result = new RedirectToActionResult("ResetPassword", "Account", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }
    }
}

