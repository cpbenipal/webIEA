using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.IEAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountInteractor _accountInteractor;
        public AccountController(AccountInteractor accountInteractor)
        {
            _accountInteractor = accountInteractor;
        }
        [AllowAnonymous]
        public ActionResult LoginView()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginDto model)
        {
            var result = _accountInteractor.Login(model);
            Session["Id"] = result.Id;
            Session["Email"] = result.Email;
            Session["Role"] = result.RoleId;
            return View("LoginView");
        }
        [HttpPost]
        [CustomAuthorizeAttribute("Admin,Member")]
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

    }
}