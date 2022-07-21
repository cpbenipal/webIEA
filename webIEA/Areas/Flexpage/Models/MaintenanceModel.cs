using Flexpage.Abstract;
using Flexpage.Code.StructureManagement;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Flexpage.Helpers;
using Flexpage.Helpers.StructureManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Flexpage.Models
{


    public class MaintenanceModel : ViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public MaintenanceRedirectionType RedirectionType { get; set; }
        public string MaintenanceText { get; set; }
        public string MaintenancePage { get; set; }
        public bool Active { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public List<string> AvailablePages { get; set; }
        public bool Layout { get; set; }
        public string DisplayMaintenanceText { get; set; }

        private int websiteID { get; set; }
        public MaintenanceModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Layout = true;
            websiteID = settings.WebsiteID;
        }
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            AvailablePages = new List<string>();
            repository.GetList<Page>().ForEach(p => { AvailablePages.Add(p.PageName); });
            repository.SetMaintenance(websiteID);
            var maintenance = repository.GetMaintenance(websiteID);
            if(maintenance != null)
            {
                this.Active = true;
            }
            else 
            {
                maintenance = new Maintenance()
                {
                    Start = DateTime.UtcNow,
                    End = DateTime.UtcNow,
                    RedirectionType = MaintenanceRedirectionType.Standard,
                    MaintenanceText = "We are currently undergoing scheduled maintenance. We will come back {MaintenanceEndDate} at {MaintenanceEndTime}. Thank you for your patience.",
                    MaintenancePage = "Flexpage/Maintenance",
                    WebsiteID = websiteID,
                };
                maintenance.End.AddHours(1);
            }
            maintenance = repository.CreateMaintenance(maintenance);
            this.Assign(maintenance);
        }
        
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);
            var oldMaintenance = repository.GetMaintenance(websiteID);
            if (this.Active == false && oldMaintenance == null)
            {
                return null;
            }
            if (oldMaintenance!=null)
            {
                oldMaintenance.ArchivedDate = DateTime.UtcNow;
            }
            
            if (this.Active == true)
            {
                var newMaintenance = new Maintenance() { };
                newMaintenance.Start = this.Start;
                newMaintenance.End = this.End;
                newMaintenance.MaintenanceText = this.MaintenanceText;
                newMaintenance.RedirectionType = this.RedirectionType;
                newMaintenance.MaintenancePage = this.MaintenancePage;
                newMaintenance.WebsiteID = websiteID;
                repository.CreateMaintenance(newMaintenance);
            }

            repository.ApplyChanges();
            return null;
        }

        public override void Update()
        {
            if (this.RedirectionType == MaintenanceRedirectionType.Standard||string.IsNullOrWhiteSpace(MaintenancePage))
            {
                this.MaintenancePage = "Flexpage/Maintenance";
            }
           this.Start= this.Start.ToUniversalTime();
           this.End = this.End.ToUniversalTime();
        }
        public void Assign(Maintenance source)
        {
            base.Assign(source);
            this.Start = source.Start.ToLocalTime();
            this.End = source.End.ToLocalTime();
            this.RedirectionType = source.RedirectionType;
            this.MaintenanceText = source.MaintenanceText;
            if (this.RedirectionType == MaintenanceRedirectionType.Standard || string.IsNullOrWhiteSpace(source.MaintenancePage))
            {
                this.MaintenancePage = "Flexpage/Maintenance";
            }
            else
            {
                this.MaintenancePage = source.MaintenancePage;
            }
            this.DisplayMaintenanceText = source.GetMaintenanceText();
        }
    }
}