using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using FlexPage2.Areas.Flexpage.Infrastructure;
using FlexPage2.Models;
using Pluritech.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pluritech.Authentication.Abstract;

namespace FlexPage2.Controllers
{
    public class AccountController : Flexpage.Controllers.LocalizedController
    {
        private readonly IAuthProvider _authProvider;
        public AccountController(IFlexpageRepository repository, IAuthProvider auth, IFlexpageSettings settings, ILocalization localization)
            :base(settings, localization)
        {
            _authProvider = auth;
        }
        public ViewResult Login()
        {
            MessageMaintenanceFillViewBag();
            return View(new LoginViewModel());
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_authProvider.Authenticate(model.UserName, model.Password, model.Save))
                {
                    returnUrl = string.IsNullOrEmpty(returnUrl)|| returnUrl.Contains("Login") && returnUrl.Contains("Account") ? 
                        Url.Action("Index", "Home"): returnUrl;
                    return Redirect(returnUrl);
                }

                ModelState.AddModelError("", "Incorrect username or password");
                return View();
            }

            return View();
        }

        public ActionResult Logout(string returnUrl)
        {
            MessageMaintenanceFillViewBag();
            Session.Clear();
            Response.Cookies.Clear();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.Now.AddYears(-1));
            _authProvider.SignOut();
            HttpCookie cookie = new HttpCookie("loggedout");
            cookie.Value = "true";
            cookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Response.Cookies.Add(cookie);
            return Redirect(returnUrl ?? Url.Action("Login", "Account"));
        }

    }
}