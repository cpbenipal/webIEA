using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Helpers;
using FlexPage2.Areas.Flexpage.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Flexpage.Models
{
    public class AdvertisementImageModel : ViewModel
    {
        private const string DATE_FORMAT = @"dd\/MM\/yyyy";

        public AdvertisementImageModel(IFlexpageSettings settings, IFlexpage flexpageProcessor) :base(settings, flexpageProcessor)
        {
            Periods = new List<string>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Tag { get; set; }

        public bool Active { get; set; }

        public string ImageName { get; set; }

        public string LanguageCodes { get; set; }
        public Dictionary<string, bool> SelectedLanguages { get; set; }
        public Dictionary<string, string> AvailableLanguages { get; set; }

        public List<string> Periods { get; set; }
        public string PeriodsList
        {
            get
            {
                return string.Join("; ", Periods);
            }
        }
        
        public string ImagePath { get { return PathHelper.GetBannerPath(ImageName); } }
        public string ImageFolder { get { return PathHelper.GetBannerFolder(); } }
        
        /// <summary>
        /// Loads object from repository
        /// </summary>
        /// <param name="repository">Repository to load from</param>
        /// <param name="proto">Popup content data</param>
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto,string title="", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);

            int id = proto.ID;
            AdvertisementImage banner = null;
            Dictionary<int, Language> allLanguages = GetAllLanguages(repository, _settings);
            if (proto.BlockAlias != repository.CreateNewAlias)
            {
                banner = repository.GetByID<AdvertisementImage>(id);
                Assign(banner, allLanguages);
            }
            else
            {
                AssignDefaultValues(allLanguages);
            }
        }

        public override void Assign(object source, params object[] args)
        {
            base.Assign(source, args);
            AdvertisementImage banner = source as AdvertisementImage;
            ID = banner.ID;
            Active = banner.Active;
            Description = banner.Description;
            ImageName = banner.Image;
            LanguageCodes = string.Join(";", banner.AdvertisementLanguage.Select(l => l.Language.Code));
            Name = banner.Name;
            Url = banner.Url;
            Tag = banner.Tag;
            Periods = banner.AdvertisementPeriod.OrderBy(p => p.StartDate).Select(p => p.GetText(DATE_FORMAT)).ToList();

            initLanguages(args[0] as Dictionary<int, Language>);
            banner.AdvertisementLanguage.Select(l => l.LanguageID).ToList().ForEach(l => SelectedLanguages[l.ToString()] = true);
        }

        public override void AssignDefaultValues(params object[] args)
        {
            base.AssignDefaultValues(args);
            initLanguages(args[0] as Dictionary<int, Language>);
        }

        private void initLanguages(Dictionary<int, Language> availableLangauges)
        {
            AvailableLanguages = new Dictionary<string, string>();
            availableLangauges.Keys.ToList().ForEach(id => AvailableLanguages[id.ToString()] = availableLangauges[id].Name);

            SelectedLanguages = new Dictionary<string, bool>();
            AvailableLanguages.Keys.ToList().ForEach(l => SelectedLanguages[l] = false);
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);

            AdvertisementImage banner = repository.GetByID<AdvertisementImage>(ID);
            if (banner == null)
            {
                banner = repository.CreateNewAdvertisementImage();
            }

            banner.Active = this.Active;
            banner.Description = this.Description;
            if (this.ImageName != null)
            {
                banner.Image = this.ImageName;
            }
            banner.Name = this.Name;
            banner.Url = this.Url;
            banner.Tag = this.Tag;

            // delete removed periods
            banner.AdvertisementPeriod.Where(p => !this.Periods.Contains(p.GetText(DATE_FORMAT))).ToList().ForEach(l => {
                banner.AdvertisementPeriod.Remove(l);
                repository.DeleteEntity<AdvertisementPeriod>(l.ID);
            });

            // add newly created periors
            foreach (var period in this.Periods)
            {
                if (banner.AdvertisementPeriod.All(p => p.GetText(DATE_FORMAT) != period))
                {
                    int index = period.IndexOf(" ", StringComparison.Ordinal);

                    var stringStartDate = period.Substring(0, index);
                    var stringEndDate = period.Substring(index + 3, (period.Length) - (index + 3));

                    var stratDate = DateTime.ParseExact(stringStartDate, DATE_FORMAT, CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(stringEndDate, DATE_FORMAT, CultureInfo.InvariantCulture);

                    banner.AdvertisementPeriod.Add(new AdvertisementPeriod { AdvertisementImageID = this.ID, StartDate = stratDate, EndDate = endDate });
                }
            }

            // delete unchecked languages
            banner.AdvertisementLanguage.Where(b => !this.SelectedLanguages[b.LanguageID.ToString()]).ToList().ForEach(l => {
                banner.AdvertisementLanguage.Remove(l);
                repository.DeleteEntity<AdvertisementLanguage>(l.ID);
            });

            // add newly checked languages
            foreach (var languageId in this.SelectedLanguages.Keys)
            {
                if (this.SelectedLanguages[languageId] && banner.AdvertisementLanguage.All(b => b.LanguageID.ToString() != languageId))
                {
                    repository.CreateNewAdvertisementLanguage(banner, int.Parse(languageId));
                }
            }
            return banner;
        }

        public static Dictionary<int, Language> GetAllLanguages(IFlexpageRepository repository, IFlexpageSettings settings)
        {
            List<string> allowedLanguages = settings.AllowedLanguages.ToList();
            var res = new Dictionary<int, Language>();
            repository.Languages.Where(l => allowedLanguages.Contains(l.Code)).ToList().ForEach(l => res[l.ID] = l);
            return res;
        }


    }
}