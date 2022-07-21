using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class NavigationBarModel
    {
        public bool AdminMode { get; set; }
        public string UserName { get; set; }
        public string PageName { get; set; }
        public int BlockID { get; set; }
        public bool ShowAdminControls { get; set; } = true;
        public bool IsPWAdmin { get; set; } = false;
    }
}