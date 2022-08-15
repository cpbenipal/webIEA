﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webIEA.App_Start;
using webIEA.Dtos;
using webIEA.Interactor;

namespace webIEA.Areas.IEAdmin.Controllers
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
                    Session.Add("Id", result.Id);
                    Session.Add("Email", result.Email);
                    Session.Add("Role", result.RoleId);
                    if (result.RoleId == (int)Roles.Admin)
                        return RedirectToAction("Index", "Members");
                    else if (result.RoleId == (int)Roles.Member)
                        return RedirectToAction("Details", "BecomeMember", new { area = "Members",  id = result.loginUserId });

                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();
                }
            }
            return View();
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

    }
}