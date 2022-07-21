using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Code.Helpers;
using System.Web.Configuration;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{
    public enum LinkTarget
    {
        Blank,
        Parent,
        Search,
        Self,
        Top
    }
    public enum GalleryPagingStyle
    {
        Bullets = 1,
        Buttons = 2,
        Arrows = 3
    }
    public enum GalleryThumbnailsAlignment
    {
        LeftToRight = 0,
        RightToLeft = 1,
        Justify = 2
    }
    public enum GalleryTransitionEffects
    {
        FadeInFadeOut = 1,
        Slideshow = 2,
        None = 3,
        Auto = 4
    }

    public partial class GalleryImageModel: ViewModel
    {
        public GalleryImageModel(Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            LocalizedImageDetails = new LocalizedImageDetailsModel(settings, flexpage);
            LocalizedImageDetails.ChangeLangFunc = "fp_changeImageLanguage";
        }

        private string _url;

        public string Url
        {
            get => _url;
            set
            {
                _url = value;

                if (GallerySettings != null && GallerySettings.AddWatermark)
                    UrlWatermark = ImageUtitlity.AddWaterToImage(_url, GallerySettings.WatermarkText,
                        GallerySettings.WatermarkedImagesPath, false);
            }
        }

        public string UrlWatermark { get; set; }

        public bool IsProtected { get; set; }
        public int OrdNum { get; set; }

        public LocalizedImageDetailsModel LocalizedImageDetails { get; set; }
        public Abstract.IImageGallerySettings GallerySettings;

        public void SelectDetailsLocalization(string langCode)
        {
            LocalizedImageDetails.SelectLanguage(langCode);
        }

        private string _imageUrl => GallerySettings.AddWatermark
            ? UrlWatermark
            : Url;

        public string ThumbUrl
        {
            get
            {
                if (GallerySettings.RenderThumbnailActualSize)
                {
                    return ImageUrl;
                }
                else
                {
                    var urlToThumb = new StringBuilder();
                    urlToThumb.Append("/GetThumb.ashx?img=");
                    urlToThumb.Append(_imageUrl);
                    urlToThumb.Append("&w=");
                    urlToThumb.Append(GallerySettings.ThumbnailWidth);
                    urlToThumb.Append("&h=");
                    urlToThumb.Append(GallerySettings.ThumbnailHeight);

                    return urlToThumb.ToString();
                }
            }
        }

        public string ThumbUrlForCustom
        {
            get
            {
                
                    return ThumbUrl;
                
            }
        }

        public string ImageUrl
        {
            get
            {
                return "/flexpage/galleryimage?path=" + _imageUrl;
            }
        }

        public string AlternateText
        {
            get
            {
                var altText = LocalizedImageDetails.LocalizedAlternateText.NotEmptyLocalization;
                if(string.IsNullOrWhiteSpace(altText) && Settings.FallbackAlternateText)
                    return System.IO.Path.GetFileName(_imageUrl);
                return altText;
            }
        }
    }

    public class GalleryModel: GalleryManagerModel
    {
        public EditorType EditorType { get; set; }
        public int CurrentImageNum { get; set; } = -1;
        public short ColumnsPerPage { get; set; }
        public bool EnablePaging { get; set; }
        public short RowsPerPage { get; set; }
        public bool AllImages { get; set; }
        public bool EnableClickToEnlarge { get; set; }
        public bool EnableZoom { get; set; }
        public decimal ZoomFactor { get; set; }
        public GalleryThumbnailsAlignment AlignThumbnails { get; set; }
        public short ThumbnailWidth { get; set; }
        public short ThumbnailHeight { get; set; }
        public bool RenderThumbnailActualSize { get; set; }
        public bool FixedImages { get; set; }
        public bool RandomImages { get; set; }
        public bool RandomImageAtLoad { get; set; }
        public short ImagesChangeInterval { get; set; }
        public GalleryTransitionEffects ImageEffect { get; set; }
        public string WatermarkText { get; set; }
        public bool AddWaterMark { get; set; }
        public bool PreferActualSize { get; set; }
        public GalleryPagingStyle PagingStyles { get; set; }

        public List<GalleryImageModel> GalleryImageModels { get; set; }
        public string CurrentBlockID
        {
            get
            {
                return (this.GalleryBlockType == GalleryBlockType.Slider? "sliderBlock" : "galleryBlock") + this.ID;
            }
        }

        private string _tempUploadedImagesPath;
        public GalleryModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            ColumnsPerPage = 3;
            ThumbnailWidth = 200;
            ThumbnailHeight = 200;
            AllImages = true;
            FixedImages = true;
            RandomImages = false;
            RandomImageAtLoad = false;
            ImagesChangeInterval = 1000;
            ImageEffect = GalleryTransitionEffects.None;
            WatermarkText = "";
            AddWaterMark = false;
            PreferActualSize = false;
            GalleryImageModels = new List<GalleryImageModel>();
            EditorType = EditorType.Simple;
            AlignThumbnails = GalleryThumbnailsAlignment.Justify;
            PagingStyles = GalleryPagingStyle.Bullets;
            GalleryBlockType = GalleryBlockType.JustifiedGallery;

            _tempUploadedImagesPath = WebConfigurationManager.AppSettings["TempUploadedImagesPath"];
            Step = WidgetStep.Step2;
        }

        /// <summary>
        /// Fills model content from provided source 
        /// </summary>
        /// <param name="source">Provided source</param>
        /// <param name="args">First parameter is alias, second parameter is block name </param>
        public override void Assign(object source, params object[] args)
        {
            Gallery gallery = source as Gallery;
            base.Assign(gallery.Block);
            if(gallery != null)
            {
                processModelData(gallery);
            }
            else
            {
                EditorType = EditorType.Simple;
                Step = WidgetStep.Step1;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
        }

        // 2DO: Create ancestor interface IFlexpageBlock of all Flexpage blocks in DB and change source type accordingly
        public GalleryModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, object source, params object[] args) : base(settings, flexpage)
        {
            Assign(source, args);
        }

        public override void Delete(IFlexpageRepository repository)
        {

        }
        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);
            //LoadLanguages(repository);

            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            Gallery item = null;

            if(proto.ID == -1 && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<Gallery>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<Gallery>(ID);
            }

            if(item != null)
            {
                Assign(item);
            }
        }

        public override void Update()
        {
            base.Update();

            GallerySettings gallerySettings = getGallerySettings();
            foreach (var imageModel in GalleryImageModels)
            {
                imageModel.LocalizedImageDetails.Update();
                imageModel.GallerySettings = gallerySettings;
            }
        }

        public void processModelData(Gallery gallery)
        {
            if(gallery != null)
            {
                Step = WidgetStep.Step2;
                ID = gallery.BlockID;
                GalleryBlockType = (GalleryBlockType)gallery.GalleryType;
                ColumnsPerPage = gallery.ColumnsPerPage;
                EnablePaging = gallery.EnablePaging;
                RowsPerPage = gallery.RowsPerPage;
                AllImages = gallery.AllImages;
                EnableClickToEnlarge = gallery.EnableClickToEnlarge;
                EnableZoom = gallery.EnableZoom;
                ZoomFactor = gallery.ZoomFactor;
                AlignThumbnails = (GalleryThumbnailsAlignment)gallery.AlignThumbnails;
                ThumbnailWidth = gallery.ThumbnailWidth;
                ThumbnailHeight = gallery.ThumbnailHeight;
                RenderThumbnailActualSize = (GalleryBlockType != GalleryBlockType.JustifiedGallery ? gallery.RenderThumbnailActualSize : true);
                FixedImages = gallery.FixedImages;
                RandomImages = gallery.RandomImages;
                RandomImageAtLoad = gallery.RandomImageAtLoad;
                ImagesChangeInterval = gallery.ImagesChangeInterval;
                ImageEffect = (GalleryTransitionEffects)gallery.ImageEffect;
                WatermarkText = gallery.WatermarkText;
                AddWaterMark = gallery.AddWaterMark;
                PagingStyles = (GalleryPagingStyle)gallery.PagingStyles;

                GallerySettings gallerySettings = getGallerySettings();
                foreach (GalleryImage galleryImage in gallery.GalleryImage.ToList())
                {
                    GalleryImageModel imageModel = new GalleryImageModel(_settings, _flexpageProcessor)
                    {
                        ID = galleryImage.ID,
                        GallerySettings = gallerySettings,
                        Url = galleryImage.Url,
                        IsProtected = galleryImage.IsProtected,
                        OrdNum = galleryImage.OrdNum
                    };

                    foreach (GalleryImageLocalization loc in galleryImage.GalleryImageLocalization.ToList())
                    {
                        imageModel.LocalizedImageDetails.LocalizedAlternateText.AddText(loc.Language.Code, loc.AlternateText);
                        imageModel.LocalizedImageDetails.LocalizedDescription.AddText(loc.Language.Code, loc.Description);
                        imageModel.LocalizedImageDetails.LocalizedLinkUrl.AddText(loc.Language.Code, loc.LinkUrl);
                        imageModel.LocalizedImageDetails.LocalizedTitle.AddText(loc.Language.Code, loc.Title);
                    }

                    GalleryImageModels.Add(imageModel);
                }
            }
            else
            {
                Step = WidgetStep.Step2;
                GalleryBlockType = GalleryBlockType.JustifiedGallery;
            }
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);

            Gallery block = repository.GetByBlockID<Gallery>(ID);
            if(block == null)
            {
                if(Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                block = repository.CreateNewGallery(alias: Alias);
                repository.AddBlockToBlockList(block.Block, BlocklistID, BlockAfter);
            }

            updatePropertiesToBlock(block, repository);

            repository.ApplyChanges();

            return block;
        }

        private void updatePropertiesToBlock(Gallery gallery, IFlexpageRepository repository)
        {
            #region Gallery Props

            gallery.GalleryType = (int)GalleryBlockType;
            gallery.ColumnsPerPage = ColumnsPerPage;
            gallery.EnablePaging = EnablePaging;
            gallery.RowsPerPage = RowsPerPage;
            gallery.AllImages = AllImages;
            gallery.EnableClickToEnlarge = EnableClickToEnlarge;
            gallery.EnableZoom = EnableZoom;
            gallery.ZoomFactor = ZoomFactor;
            gallery.AlignThumbnails = (short)AlignThumbnails;
            gallery.ThumbnailWidth = ThumbnailWidth;
            gallery.ThumbnailHeight = ThumbnailHeight;
            gallery.RenderThumbnailActualSize = RenderThumbnailActualSize;
            gallery.FixedImages = FixedImages;
            gallery.RandomImages = RandomImages;
            gallery.RandomImageAtLoad = RandomImageAtLoad;
            gallery.ImagesChangeInterval = ImagesChangeInterval;
            gallery.ImageEffect = (short)ImageEffect;
            gallery.WatermarkText = WatermarkText ?? "";
            gallery.AddWaterMark = AddWaterMark;
            gallery.PagingStyles = (short)PagingStyles;
            #endregion

            string baseUploadPath = _tempUploadedImagesPath + gallery.Block.Alias;

            gallery.GalleryImage
                .Where(gi => !GalleryImageModels.Where(gim => gim.ID > 0).Select(gim => gim.ID).ToList().Contains(gi.ID))
                .ToList()
                .ForEach(gi =>
                {
                    gallery.GalleryImage.Remove(gi);
                    repository.DeleteEntity<GalleryImage>(gi.ID);
                });


            foreach (var imageModel in GalleryImageModels)
            {
                bool hasData = !string.IsNullOrEmpty(imageModel.Url);

                GalleryImage image = gallery.GalleryImage.FirstOrDefault(img => img.ID == imageModel.ID);
                if (image == null && hasData)
                {
                    image = repository.CreateNewGalleryImage(gallery);
                }
                if (image != null)
                {
                    if (hasData)
                    {
                        image.Url = imageModel.Url;
                        image.IsProtected = imageModel.IsProtected;
                        image.OrdNum = imageModel.OrdNum;

                        foreach (string lang in imageModel.LocalizedImageDetails.AllKeys)
                        {
                            Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);

                            string linkUrl = imageModel.LocalizedImageDetails.LocalizedLinkUrl.GetText(lang);
                            string title = imageModel.LocalizedImageDetails.LocalizedTitle.GetText(lang);
                            string description = imageModel.LocalizedImageDetails.LocalizedDescription.GetText(lang);
                            string alternateText = imageModel.LocalizedImageDetails.LocalizedAlternateText.GetText(lang);

                            bool hasLocData = !string.IsNullOrEmpty(linkUrl) || !string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description) || !string.IsNullOrEmpty(alternateText);

                            if (language != null)
                            {
                                GalleryImageLocalization loc = image.GalleryImageLocalization.FirstOrDefault(l => l.LanguageID == language.ID);
                                if (loc == null && hasLocData)
                                    loc = repository.CreateGalleryImageLocalization(image, language.ID);

                                if (loc != null)
                                {
                                    if (hasLocData)
                                    {
                                        loc.LinkUrl = linkUrl ?? "";
                                        loc.Title = title ?? "";
                                        loc.Description = description ?? "";
                                        loc.AlternateText = alternateText ?? "";
                                    }
                                    else
                                    {
                                        image.GalleryImageLocalization.Remove(loc);
                                        repository.DeleteEntity<GalleryImageLocalization>(loc.ID);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        gallery.GalleryImage.Remove(image);
                        repository.DeleteEntity<GalleryImage>(image.ID);
                    }
                }
            }

        }

        public void SelectImageDetailsLocalization(string langCode)
        {
            if(GalleryImageModels.Count > CurrentImageNum && GalleryImageModels[CurrentImageNum] != null)
                GalleryImageModels[CurrentImageNum].SelectDetailsLocalization(langCode);
        }

        public void AddImage(string url)
        {
            GallerySettings gallerySettings = getGallerySettings();
            GalleryImageModels.Add(new GalleryImageModel(_settings, _flexpageProcessor)
            {
                GallerySettings = gallerySettings,
                Url = url,
                OrdNum = GalleryImageModels.Count,
                ID = GalleryImageModels.DefaultIfEmpty(new GalleryImageModel(_settings, _flexpageProcessor)
                {
                    ID = 0,
                    GallerySettings = gallerySettings
                }).Min(gim => gim.ID) - 1,
            });

            CurrentImageNum = GalleryImageModels.Count - 1;
        }

        public void DeleteImage(string imgNum)
        {
            int imageNum = 0;

            if(int.TryParse(imgNum, out imageNum))
            {
                GalleryImageModels.RemoveAt(imageNum);
            }
            //GalleryImageModels.Add(new GalleryImageModel()
            //{
            //    Url = url,
            //    OrdNum = GalleryImageModels.Count,
            //});

            CurrentImageNum = GalleryImageModels.Count > 0 ? 0 : -1;
        }

        private GallerySettings getGallerySettings()
        {
            string waterMarkedImagesPath = WebConfigurationManager.AppSettings["TempWaterMarkedImagesPath"];
            if (waterMarkedImagesPath == null)
            {
                waterMarkedImagesPath = "/Content/Images/TempWaterMarkedImages/";
            }
            waterMarkedImagesPath = waterMarkedImagesPath.TrimEnd(new char[] { '\\' }) + "\\" + CurrentBlockID;
            waterMarkedImagesPath = _settings.MapPath(waterMarkedImagesPath);

            return new GallerySettings() {
                IsSlider = this.GalleryBlockType == GalleryBlockType.Slider,
                AddWatermark = this.AddWaterMark,
                WatermarkText = this.WatermarkText,
                WatermarkedImagesPath = waterMarkedImagesPath,
                RenderThumbnailActualSize = this.RenderThumbnailActualSize,
                ThumbnailWidth = this.ThumbnailWidth,
                ThumbnailHeight = this.ThumbnailHeight,
                EnableClickToEnlarge = this.EnableClickToEnlarge
            };
        }

        class GallerySettings : Abstract.IImageGallerySettings
        {
            public bool IsSlider { get; set; }
            public bool AddWatermark { get; set; }
            public string WatermarkText { get; set; }
            public string WatermarkedImagesPath { get; set; }
            public bool RenderThumbnailActualSize { get; set; }
            public short ThumbnailWidth { get; set; }
            public short ThumbnailHeight { get; set; }
            public bool EnableClickToEnlarge { get; set; }
        }
    }
}