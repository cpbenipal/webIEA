using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Helpers;
using FlexPage2.Areas.Flexpage.Helpers;
using FlexPage2.Areas.Flexpage.Models.Enums;
using Flexpage.Domain.Enum;
using Flexpage.Abstract;

namespace Flexpage.Models
{
    public enum DisplayTypeAdvertisement
    {
        Random, Cycle
    }
    public class AdvertisementModel : BlockModel
    {
        public AdvertisementModel(IFlexpageSettings settings, IFlexpage flexpageProcessor) : base(settings, flexpageProcessor) { }
        public string Image { get; set; }
        public string AlternateText { get; set; }
        public string Link { get; set; }
        public string TagName { get; set; }
        public bool ConstrainProportions { get; set; } = false;
        public SizeType WidthType { get; set; } = SizeType.Auto;
        public int CustomWidth { get; set; }
        public SizeUnitType CustomWidthUnit { get; set; } = SizeUnitType.Pixel;
        public SizeType HeightType { get; set; } = SizeType.Auto;
        public int CustomHeight { get; set; }
        public SizeUnitType CustomHeightUnit { get; set; } = SizeUnitType.Pixel;
        public IEnumerable<string> AllowedTags { get; set; }
        public int ImagesPerLoad { get; set; }

        public DisplayTypeAdvertisement DisplayType { get; set; }

        public int CycleDelay { get; set; }

        private int _cycleDelay = 30;
        private int _imagesPerLoad = 3;

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto,title, needToLoadContent);
            Advertisement block = repository.GetByBlockID<Advertisement>(proto.ID);
            if(block == null &&  !string.IsNullOrEmpty(proto.BlockAlias) && proto.BlockAlias != ViewModel.NewAlias)
            {
                block = repository.GetByAlias<Advertisement>(proto.BlockAlias);
            }

            List<AdvertisementImage> allAdvertisements = repository.GetEntityList<AdvertisementImage>();

            if (block == null)
            {
                AssignDefaultValues(allAdvertisements);
            }
            else
            {
                Assign(block, allAdvertisements);
            }
        }

        private const string _allAvailable = "(All)";

        private static IEnumerable<string> allowedTags(List<AdvertisementImage> allAdvertisements)
        {
            List<string> tags = allAdvertisements.Where(a => !string.IsNullOrWhiteSpace(a.Tag)).Select(a => a.Tag).Distinct().ToList();
            tags.Insert(0, _allAvailable);
            return tags;
        }

        public static IEnumerable<string> GetAllowedTags(IFlexpageRepository repository)
        {
            return allowedTags(repository.GetEntityList<AdvertisementImage>());
        }

        private void loadAdvertisements(object[] args, Advertisement advertisement)
        {
            List<AdvertisementImage> allAdvertisements = args.Length > 0 ? args[0] as List<AdvertisementImage> : new List<AdvertisementImage>();

            // Available tags
            AllowedTags = allowedTags(allAdvertisements);

            // Selected tag
            this.TagName = string.IsNullOrWhiteSpace(advertisement.Tag) ? _allAvailable : advertisement.Tag;
            this.ConstrainProportions = advertisement.ConstrainProportions;
            this.WidthType = advertisement.WidthType;
            this.CustomWidth = advertisement.CustomWidth;
            this.CustomWidthUnit = advertisement.CustomWidthUnit;
            this.HeightType = advertisement.HeightType;
            this.CustomHeight = advertisement.CustomHeight;
            this.CustomHeightUnit = advertisement.CustomHeightUnit;           

            if (advertisement.CycleDelay != null && advertisement.ImagesPerLoad != null)
            {
                this.DisplayType = DisplayTypeAdvertisement.Cycle;
                this.CycleDelay = advertisement.CycleDelay.Value;
                this.ImagesPerLoad = advertisement.ImagesPerLoad.Value;
            }
            else
            {
                this.DisplayType = DisplayTypeAdvertisement.Random;
                this.CycleDelay = _cycleDelay;
                this.ImagesPerLoad = _imagesPerLoad;
            }

            // Current picture selection
            var currentDate = DateTime.Today.Date;
            var r = new Random();
            var activeBanners = allAdvertisements.Where(b => b.Active && b.AdvertisementPeriod.Any(p => p.StartDate <= currentDate && p.EndDate >= currentDate));

            if (!string.IsNullOrEmpty(advertisement.Tag) && advertisement.Tag != "All" && advertisement.Tag != "(All)")
            {
                activeBanners = activeBanners.Where(a => a.Tag == advertisement.Tag);
            }

            var currentLanguageCode = _settings.GetCurrentOrDefaultLangCode();
            // Get banners that have restricted language equal current language or have no restricted languages
            activeBanners = activeBanners.Where(ab => (ab.AdvertisementLanguage.Count == 0) || ab.AdvertisementLanguage.Any(l => l.Language.Code == currentLanguageCode));

            var randomBanner = activeBanners.OrderBy(a => a.ID).Skip(r.Next(activeBanners.Count())).FirstOrDefault();
            if (randomBanner != null)
            {
                this.Image = PathHelper.GetBannerPath(randomBanner.Image);
                this.AlternateText = randomBanner.Name;

                string url = randomBanner.Url;
                if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("/") && !url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "http://" + url;
                }
                this.Link = url;
            }
        }

        public override void AssignDefaultValues(params object[] args)
        {
            base.AssignDefaultValues(args);
            loadAdvertisements(args, new Advertisement { });
        }

        public override void Assign(object source, params object[] args)
        {
            Advertisement advertisement = source as Advertisement;
            if (advertisement == null)
            {
                throw new System.Exception("Null advertisement object for assignment");
            }

            base.Assign(advertisement.Block, args);
            loadAdvertisements(args, advertisement);
        }
        public string CreateSizeString(int amount, SizeUnitType unitType, string indentString)
        {
            indentString = amount.ToString();

            switch (unitType)
            {
                case SizeUnitType.EM:
                    indentString += "em";
                    break;
                case SizeUnitType.Percentage:
                    indentString += "%";
                    break;
                case SizeUnitType.Pixel:
                    indentString += "px";
                    break;
                case SizeUnitType.REM:
                    indentString += "rem";
                    break;
            }

            return indentString;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);

            Advertisement advertisement = repository.GetByBlockID<Advertisement>(ID);
            if (advertisement == null)
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                advertisement = repository.CreateNewAdvertisement( Visible, CssClass, Alias);
                repository.AddBlockToBlockList(advertisement.Block, BlocklistID, BlockAfter);
            }

            advertisement.Tag = (this.TagName == _allAvailable || string.IsNullOrWhiteSpace(this.TagName)) ? null : this.TagName;
            advertisement.ConstrainProportions = this.ConstrainProportions;
            advertisement.WidthType = this.WidthType;
            advertisement.CustomWidth = this.CustomWidth;
            advertisement.CustomWidthUnit = this.CustomWidthUnit;
            advertisement.HeightType = this.HeightType;
            advertisement.CustomHeight = this.CustomHeight;
            advertisement.CustomHeightUnit = this.CustomHeightUnit;

            advertisement.CycleDelay = ((this.DisplayType == DisplayTypeAdvertisement.Cycle && this.CycleDelay > 0) ? this.CycleDelay : (int?) null);
            advertisement.ImagesPerLoad = ((this.DisplayType == DisplayTypeAdvertisement.Cycle && this.ImagesPerLoad > 0) ? this.ImagesPerLoad : (int?)null);

            repository.ApplyChanges();
            return advertisement;
        }

        public static List<AdvertisementModel> GenerateAdvertisementModels(BlockCommandModel proto, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository)
        {
            Advertisement source = repository.GetByBlockID<Advertisement>(proto.ID);
            string tagName = proto.Command;
            IEnumerable<AdvertisementImage> images = repository.GetEntityList<AdvertisementImage>();

            if (!String.IsNullOrEmpty(tagName) && tagName != "All" && tagName != "(All)")
            {
                images = images.Where(img => img.Tag == tagName);
            }

            DateTime currentDate = DateTime.Now.Date;

            images = images.Where(img => !String.IsNullOrEmpty(img.Image) && img.Active && img.AdvertisementPeriod.Any(p => p.StartDate <= currentDate && p.EndDate >= currentDate));

            string currentLanguageCode = settings.GetCurrentOrDefaultLangCode();

            images = images.Where(ab => !ab.AdvertisementLanguage.Any() || ab.AdvertisementLanguage.Any(l => l.Language.Code == currentLanguageCode));

            IEnumerable<AdvertisementModel> models = images.ToList()
                .Select(image => CreateWithImage(source, image, settings, flexpage));
            return models.ToList();
        }

        protected static AdvertisementModel CreateWithImage(Advertisement source, AdvertisementImage image, Abstract.IFlexpageSettings settings, IFlexpage flexpage)
        {
            AdvertisementModel model = new AdvertisementModel(settings, flexpage);
            model.Assign(source, new List<AdvertisementImage>() { image });
            return model;
        }
    }
}