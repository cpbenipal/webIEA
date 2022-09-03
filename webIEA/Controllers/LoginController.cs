using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Controllers
{
    public class LoginController : Controller
    {
        private readonly AccountInteractor _accountInteractor;
        public LoginController(AccountInteractor accountInteractor)
        {
            _accountInteractor = accountInteractor;
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountInteractor.Login(model);
                if (result != null)
                {
                    if (result.Status == (int)MemberStatusEnum.Suspended)
                    {
                        ModelState.AddModelError("", "Your account has been suspended. Please contact admin");
                        return View("Index");
                    }
                    else
                    {
                        Session.Add("Id", result.Id);
                        Session.Add("loginUserId", result.loginUserId);
                        Session.Add("Email", result.Email);
                        Session.Add("Role", result.RoleId);
                        Session.Add("FirstName", result.FirstName);
                        Session.Add("LogId", result.LogId);

                        if (result.RoleId == (int)IEARoles.Admin)
                            return RedirectToAction("IndexPage", "Member");
                        else
                            return RedirectToAction("Details", "Member");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View("Index");
                }
            }
            return View("Index");
        }
        [HttpPost]
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult UpdatePasword(UpdatePasswordDto model)
        {
            var result = _accountInteractor.UpdatePassword(model);
            return View(result);
        }
        [CustomAuthorizeAttribute("Member")]
        public ActionResult GetById(string id)
        {
            var result = _accountInteractor.GetById(id);
            return View(result);
        }
        [CustomAuthorizeAttribute("Admin", "Member")]
        public ActionResult Logout()
        {
            long logid = (long)Session["LogId"];
            _accountInteractor.Logout(logid);
            Session.Clear();
            Response.Cookies.Clear();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.Now.AddYears(-1));

            return RedirectToAction("Index", "Login");
        }

    }
}