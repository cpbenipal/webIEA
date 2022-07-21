using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class ScriptModel
    {
        public string Container { get; set; }
        public string Command { get; set; }
    }
}