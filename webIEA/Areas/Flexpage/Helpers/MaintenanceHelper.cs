using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
namespace Flexpage.Helpers
{

    public class MaintenanceHelper
    {
        private IFlexpageRepository _repository { get; set; }
        private HttpContext _context { get; set; }
        private IFlexpageSettings _flexpageSettings { get; set; }
        private Maintenance Maintenance
        {
            get
            {
                if (ConfigurationManager.AppSettings["FP:MaintenancePage"] != "")
                {
                    var maintenance = new Maintenance();
                    maintenance.MaintenancePage = ConfigurationManager.AppSettings["FP:MaintenancePage"];
                    maintenance.MaintenanceText = ConfigurationManager.AppSettings["FP:MaintenanceText"];
                    maintenance.Start =new DateTime(long.Parse( ConfigurationManager.AppSettings["FP:MaintenanceStart"]));
                    maintenance.End = new DateTime(long.Parse(ConfigurationManager.AppSettings["FP:MaintenanceEnd"]));
                    return maintenance;
                }
                return null;
            }
        }
        public MaintenanceHelper(IFlexpageRepository repository)
        {
            _repository = repository;
        }
        public Maintenance SetMaintenance()
        {
            var maintenanceModel = _repository.GetMaintenance(_flexpageSettings.WebsiteID);
            if (maintenanceModel != null&& maintenanceModel.MaintenancePage!="")
            {
                ConfigurationManager.AppSettings["FP:Maintenance"] = "True";
                ConfigurationManager.AppSettings["FP:MaintenancePage"] = maintenanceModel.MaintenancePage;
                ConfigurationManager.AppSettings["FP:MaintenanceText"] = maintenanceModel.GetMaintenanceText();
                ConfigurationManager.AppSettings["FP:MaintenanceStart"] = maintenanceModel.Start.Ticks.ToString();
                ConfigurationManager.AppSettings["FP:MaintenanceEnd"] = maintenanceModel.End.Ticks.ToString();
            }
            else
            {
                ConfigurationManager.AppSettings["FP:Maintenance"] = "False";
            }
            return maintenanceModel;
        }
        public void RedirectMaintenance(HttpContext context, IFlexpageSettings flexpageSettings)
        {
            if (Maintenance!=null&&Maintenance.Start.CompareTo(DateTime.UtcNow)<=0)
            {
                List<string> maintenanceIgnor = new List<string>();
                maintenanceIgnor.AddRange(ConfigurationManager.AppSettings["FP:MaintenanceIgnorRedirect"].ToString().Split(','));

                var path = HttpUtility.UrlDecode(_context.Request.Url.AbsolutePath).ToString();
                if (maintenanceIgnor.FindIndex(mi => path.Contains(mi)) < 0)
                {
                    var pathAndQuery = HttpUtility.UrlEncode(_context.Request.Url.AbsoluteUri);
                    var maintenancePage = ConfigurationManager.AppSettings["FP:MaintenancePage"];
                    var isAdmin = _flexpageSettings.IsCmsAdmin();
                    if (!isAdmin && !string.IsNullOrWhiteSpace(maintenancePage) && path.CompareTo("/" + maintenancePage) != 0)
                    {
                        if (_context.Request.IsAuthenticated)
                        {
                            _context.Response.Redirect("/Account/Logout");
                        }
                        _context.Response.Redirect("/" + maintenancePage + "?MaintenanceReturnUrl=" + pathAndQuery);
                    }
                }
            }
            if (Maintenance == null || Maintenance.End.CompareTo(DateTime.UtcNow)<0)
            {
                ConfigurationManager.AppSettings["FP:Maintenance"] = "";
                ConfigureMaintenance(context, flexpageSettings);
            }
        }
        public void ConfigureMaintenance(HttpContext context, IFlexpageSettings flexpageSettings)
        {
            _context = context;
            _flexpageSettings = flexpageSettings;
            var maintenance = ConfigurationManager.AppSettings["FP:Maintenance"];
            
            if (maintenance == "")
            {
                SetMaintenance();
            }
            maintenance = ConfigurationManager.AppSettings["FP:Maintenance"];
            if (maintenance == "True")
            {
                RedirectMaintenance(_context, _flexpageSettings);
            }
            if (maintenance == "False")
            {
                var returnUrl = _context.Request.Params["MaintenanceReturnUrl"];
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    _context.Response.Redirect(returnUrl);
                }
            }
        }
    }
}
