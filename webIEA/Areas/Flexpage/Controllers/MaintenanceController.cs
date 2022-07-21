using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Helpers;
using FlexPage2.Areas.Flexpage.Infrastructure;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Flexpage.Controllers
{
    /// <summary>
    /// This is a common ancestor of all controllers that support localization.
    /// </summary>
    public abstract class MaintenanceController : Controller
    {
         private Maintenance Maintenance
        {
            get
            {
                if (ConfigurationManager.AppSettings["FP:MaintenancePage"] != "")
                {
                    var maintenance = new Maintenance();
                    maintenance.MaintenancePage = ConfigurationManager.AppSettings["FP:MaintenancePage"];
                    maintenance.MaintenanceText = ConfigurationManager.AppSettings["FP:MaintenanceText"];
                    maintenance.Start = new DateTime(long.Parse(ConfigurationManager.AppSettings["FP:MaintenanceStart"]));
                    maintenance.End = new DateTime(long.Parse(ConfigurationManager.AppSettings["FP:MaintenanceEnd"]));
                    return maintenance;
                }
                return null;
            }
        }
        public MaintenanceController()
        {
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            
            base.Initialize(requestContext);
        }
        
        protected void MessageMaintenanceFillViewBag()
        {
            if (ConfigurationManager.AppSettings["FP:Maintenance"] == "True")
            {
                if (Maintenance != null && Maintenance.Start.CompareTo(DateTime.UtcNow) <= 0)
                {
                    ViewBag._Maintenance = ConfigurationManager.AppSettings["FP:MaintenanceText"];
                }
            }
        }
    }
}