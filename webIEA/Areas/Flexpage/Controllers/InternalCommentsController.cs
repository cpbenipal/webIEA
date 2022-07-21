
using Flexpage.Business.Abstract;
using Flexpage.Domain.Abstract;
using Pluritech.Authentication.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Flexpage.Controllers
{
    public class InternalCommentsController : Controller
    {
        private readonly IUser _user;
        private readonly IInternalCommentProcessor _processor;
        public InternalCommentsController(IUser user, IInternalCommentProcessor processor)
        {
            _user = user;
            _processor = processor;
        }

        [HttpGet]
        public PartialViewResult InternalComments(int objectID)
        {
            
            var model = _user.IsAuthenticated ? _processor.LoadComments(_user.ID, objectID) : new Abstract.DTO.InternalCommentsViewModel();
            return PartialView("~/Areas/Flexpage/Views/Flexpage/InternalComments.cshtml", model);
        }

        public string SubmitComment(string text, int objectID)
        {
            _processor.SaveComment(_user.ID, objectID, text);
            return "Success";
        }
    }
}