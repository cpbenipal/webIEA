using System.Collections.Generic;
using System.Linq;
using Flexpage.Code.CMS;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;
using Flexpage.Domain.Enum;
using Flexpage.Abstract;

namespace Flexpage.Models
{

    public class MediaModel : GalleryManagerModel
    {
        public List<LocalizedMediaModel> Localizations { get; set; }
        public LocalizedMediaModel CurrentLocalization { get; set; }

        // public LocalizedStringModel LocalizedUrl { get; set; }
        public LocalizedStringModel Title { get; set; }
        public LocalizedStringModel Description { get; set; }

        public bool DisplayTitle { get; set; }
        public bool AutoPlay { get; set; }
        public bool Loop { get; set; }
        public bool DisplayControls { get; set; } = true;
        public string Color { get; set; }
        public short Opacity { get; set; }
        public EditorType EditorType { get; set; }
        public bool ApplySizeConstraints { get; set; } = false;
        public DimensionModel Width { get; set; }
        public DimensionModel Height { get; set; }
        public MediaType MediaType { get; set; }

        /// <summary>
        /// In case of using video model as child element, index among parent children   
        /// </summary>
        public int Index { get; set; }

        public LanguageSelectorModel videoLanguageSelector = null;
        public LanguageSelectorModel VideoLanguageSelector
        {
            get
            {
                videoLanguageSelector = new LanguageSelectorModel(_settings, _flexpageProcessor)
                {
                    LangCodes = Localizations.Select(e => e.Language).ToList(),
                    FunctionName = "fp_videoChangeVideoLanguage"
                };
                return videoLanguageSelector;
            }
        }

        public LanguageSelectorModel infoLanguageSelector = null;
        public LanguageSelectorModel InfoLanguageSelector
        {
            get
            {
                infoLanguageSelector = new LanguageSelectorModel(_settings, _flexpageProcessor)
                {
                    CurrentLangCode = Title.CurrentLangCode,
                    LangCodes = Title.Localizations.Keys,
                    FunctionName = "fp_videoChangeInfoLanguage"
                };
                return infoLanguageSelector;
            }
        }

        public short CurrentTab { get; set; } = 0;

        public LocalizedMediaModel NonEmptyLocalization {
            get {
                //try to find first not empty string
                if(CurrentLocalization == null || string.IsNullOrEmpty(CurrentLocalization.MediaUrl))
                {
                    var tmpLoc = Localizations.FirstOrDefault(l => l.Language == _settings.DefaultLangCode);
                    if(tmpLoc != null && !string.IsNullOrEmpty(tmpLoc.MediaUrl))
                        return tmpLoc;
                    return Localizations.FirstOrDefault(l => !string.IsNullOrEmpty(l.MediaUrl));
                }
                return CurrentLocalization;
            }
        }

        /// <summary>
        /// Fills model content from provided source 
        /// </summary>
        /// <param name="source">Provided source</param>
        /// <param name="args">First parameter is alias, second parameter is block name </param>
        public override void Assign(object source, params object[] args)
        {

            Media v = source as Media;

            if (v != null)
            {
                base.Assign(v.Block);
                Assign(v);
            }
            else
            {
                EditorType = EditorType.Simple;
                Step = WidgetStep.Step2;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
        }

        public void Assign(MediaModel source)
        {
            Width = source.Width;
            Height = source.Height;
            GalleryBlockType = source.GalleryBlockType;
            EditorType = source.EditorType;
            AutoPlay = source.AutoPlay;
            CurrentLocalization = source.CurrentLocalization;
            Localizations = source.Localizations;
            // LocalizedUrl = new LocalizedStringModel();
            Title = source.Title;
            Description = source.Description;
            Color = source.Color;
            Opacity = source.Opacity;
            Loop = source.Loop;
            DisplayControls = source.DisplayControls;
        }

        public MediaModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
            Width = new DimensionModel() { CustomValue = 100 };
            Height = new DimensionModel() { CustomValue = 100 };
            GalleryBlockType = GalleryBlockType.SinglePicture;
            EditorType = EditorType.Simple;

            CurrentLocalization = new LocalizedMediaModel(_settings, _flexpageProcessor)
            {
                ID = -1,
                Language = _settings.GetCurrentOrDefaultLangCode()
            };

            Localizations = new List<LocalizedMediaModel>() { CurrentLocalization };
            // LocalizedUrl = new LocalizedStringModel();
            Title = new LocalizedStringModel(_settings, _flexpageProcessor, true);
            Description = new LocalizedStringModel(_settings, _flexpageProcessor, true);

            EditorType = EditorType.Simple;
            Step = WidgetStep.Step2;
        }


        public MediaModel(Flexpage.Abstract.IFlexpageSettings settings, IFlexpage flexpageProcessor, object source, params object[] args) 
            : base(settings, flexpageProcessor)
        {
            Localizations = new List<LocalizedMediaModel>();
            // LocalizedUrl = new LocalizedStringModel();
            Title = new LocalizedStringModel(settings, flexpageProcessor);
            Description = new LocalizedStringModel(settings, flexpageProcessor);
            Assign(source, args);
        }

        public override void Delete(IFlexpageRepository repository)
        {

        }

        public void DeleteMedia()
        {
            this.CurrentLocalization.MediaUrl = null;

        }

        public void DeleteThumb()
        {
            this.CurrentLocalization.ThumbUrl = null;
        }

        public void SelectMediaLanguage(string langCode)
        {
            var loc = Localizations.FirstOrDefault(e => e.Language == langCode);
            if (loc == null)
            {
                loc = new LocalizedMediaModel(_settings, _flexpageProcessor) { Language = langCode, MediaID = this.ID, ID = -1 };
                Localizations.Add(loc);
            }
            CurrentLocalization = loc;
            VideoLanguageSelector.Update(langCode, Localizations.Select(e => e.Language).ToList());
        }

        public void SelectInfoLanguage(string langCode)
        {
            Title.SelectLanguage(langCode);
            Description.SelectLanguage(langCode);
            InfoLanguageSelector.Update(langCode, Title.Localizations.Keys);
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            Media item = null;

            if (proto.ID == -1 && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<Media>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<Media>(ID);
            }

            if (item != null)
            {
                Assign(item);
            }
        }

        public void Assign(Media media)
        {
            base.Assign(media.Block);
            var lc = _settings.GetCurrentOrDefaultLangCode();
            VideoLanguageSelector.CurrentLangCode = lc;
            InfoLanguageSelector.CurrentLangCode = lc;

            Title.CurrentLangCode = lc;
            Description.CurrentLangCode = lc;

            // LocalizedUrl.CurrentLangCode = lc;


            if (media != null)
            {
                if (media.MediaPlaylist != null)
                {
                    this.AutoPlay = media.MediaPlaylist.AutoPlay; //bah changed
                    this.DisplayControls = media.MediaPlaylist.DisplayControls;
                }
                else
                {
                    this.AutoPlay = media.AutoPlay;
                    this.DisplayControls = media.DisplayControls;
                }
                this.DisplayTitle = media.DisplayTitle;
                this.Loop = media.Loop;
                this.Color = media.Color;
                this.MediaType = media.MediaType;
                this.Opacity = media.Opacity ?? 0;
                

                if (media.MediaMovieLocalization.Count > 0)
                {
                    Localizations.Clear();

                    foreach(var loc in media.MediaMovieLocalization)
                    {
                        Localizations.Add(new LocalizedMediaModel(_settings, FlexpageProcessor) { ID = loc.ID, Language = loc.Language.Code, LanguageID = loc.LanguageID, MediaID = this.ID, MediaUrl = loc.MediaUrl, ThumbUrl = loc.ThumbUrl });
                    }
                    CurrentLocalization = Localizations.FirstOrDefault(e => e.Language == lc);                    
                }
                if (CurrentLocalization == null || string.IsNullOrEmpty(CurrentLocalization.Language)) {
                    CurrentLocalization = new LocalizedMediaModel(_settings, _flexpageProcessor) { ID = -1, Language = lc };
                }

                foreach (var loc in media.MediaLocalization)
                {
                    Title.AddText(loc.Language.Code, loc.Title);
                    Description.AddText(loc.Language.Code, loc.Description);
                    // LocalizedUrl.AddText(loc.Language.Code, loc.);
                }
                Title.CurrentLangCode = lc;
                Description.CurrentLangCode = lc;

                InfoLanguageSelector.Update(lc, Title.Localizations.Keys);
                VideoLanguageSelector.Update(lc, Localizations.Select(e => e.Language).ToList());

                Step = WidgetStep.Step2;
                ApplySizeConstraints = media.ApplySizeConstraints;
                Width = new DimensionModel() { Type = (SizeType)media.WidthType, CustomValue = (media.CustomWidth==null?100: (int)media.CustomWidth), CustomValueUnit =(media.CustomWidthUnit == null ? SizeUnitType.Pixel : (SizeUnitType)media.CustomWidthUnit) };
                Height = new DimensionModel() { Type = (SizeType)media.HeightType, CustomValue = (media.CustomHeight == null ? 100 : (int)media.CustomHeight), CustomValueUnit = (media.CustomHeightUnit == null ? SizeUnitType.Pixel : (SizeUnitType)media.CustomHeightUnit) };
            }
            else
            {
                Step = WidgetStep.Step1;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
            Update();
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);
            return null;
        }


        public void Apply(Media media, IFlexpageRepository repository)
        {
            media.ApplySizeConstraints = this.ApplySizeConstraints;

            media.WidthType = (short)this.Width.Type;
            media.CustomWidth = (short)this.Width.CustomValue;
            media.CustomWidthUnit = (short)this.Width.CustomValueUnit;
            media.HeightType = (short)this.Height.Type;
            media.CustomHeight = (short)this.Height.CustomValue;
            media.CustomHeightUnit = (short)this.Height.CustomValueUnit;
            media.DisplayTitle = this.DisplayTitle;
            media.AutoPlay = this.AutoPlay;
            media.Loop = this.Loop;  //bah uncommented
            media.DisplayControls = this.DisplayControls;
            media.Color = this.Color;
            media.Opacity = this.Opacity;
            media.MediaType = repository.GetMediaType(this.MediaType); ;

            foreach (var l in Localizations)
            {
                string u = l.MediaUrl;
                var loc = media.MediaMovieLocalization.FirstOrDefault(e => e.ID == l.ID);
                Language language = repository.Languages.FirstOrDefault(e => l.Language == e.Code);

                if (loc == null && !string.IsNullOrEmpty(u))
                    loc = repository.CreateMediaMovieLocalization(media, language.ID);

                if (loc != null)
                {
                    if (!string.IsNullOrEmpty(u))
                    {
                        loc.MediaUrl = l.MediaUrl;
                        loc.ThumbUrl = l.ThumbUrl;
                    }
                    else
                    {
                        media.MediaMovieLocalization.Remove(loc);
                        repository.DeleteEntity<MediaMovieLocalization>(loc.ID);
                    }
                }
            }


            var ls = Title.Localizations.Keys.Union(Description.Localizations.Keys.Distinct().ToList());
            foreach (var lang in ls)
            {
                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);
                // string linkUrl = LocalizedUrl.GetText(lang);
                var t = Title.GetText(lang);
                var d = Description.GetText(lang);

                bool hasData = !string.IsNullOrEmpty(t) || !string.IsNullOrEmpty(d);

                if (language != null)
                {
                    var loc = media.MediaLocalization.FirstOrDefault(e => e.LanguageID == language.ID);
                    if (loc == null && hasData)
                        loc = repository.CreateMediaLocalization(media, language.ID);

                    if (loc != null)
                    {
                        if (hasData)
                        {
                            loc.Title = t ?? "";
                            loc.Description = d ?? "";
                        }
                        else
                        {
                            media.MediaLocalization.Remove(loc);
                            repository.DeleteEntity<MediaLocalization>(loc.ID);
                        }
                    }
                }
            }
        }

        public void SelectLocalization(string langCode)
        {
            VideoLanguageSelector.CurrentLangCode = langCode;
            var l = Localizations.FirstOrDefault(e => e.Language == langCode);
            if (l != null)
            {
                CurrentLocalization = l;
            }
        }

        public void SelectInfoLocalization(string langCode)
        {
            Title.SelectLanguage(langCode);
            Description.SelectLanguage(langCode);
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

        public string CreateIndentString(IndentPropertiesModel properties, string indentString)
        {
            if (properties.IsApplyIndent)
            {
                return CreateSizeString(properties.Amount, properties.SizeUnitType, indentString);
            }

            return indentString;
        } 

        public string GetPositionCssClass(TextPosition textPosition)
        {
            string retVal = "text-position--top-left";

            switch (textPosition)
            {
                case TextPosition.Bottom:
                    retVal = "text-position--bottom";
                    break;
                case TextPosition.BottomLeft:
                    retVal = "text-position--bottom-left";
                    break;
                case TextPosition.BottomRight:
                    retVal = "text-position--bottom-right";
                    break;
                case TextPosition.Middle:
                    retVal = "text-position--centered";
                    break;
                case TextPosition.MiddleLeft:
                    retVal = "text-position--center-left";
                    break;
                case TextPosition.MiddleRight:
                    retVal = "text-position--center-right";
                    break;
                case TextPosition.Top:
                    retVal = "text-position--top";
                    break;
                case TextPosition.TopLeft:
                    retVal = "text-position--top-left";
                    break;
                case TextPosition.TopRight:
                    retVal = "text-position--top-right";
                    break;

            }

            return retVal;
        }

        public override void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository, string Title = "")
        {
            base.FillViewData(viewData, repository);

            List<string> pages = repository.GetList<Page>().Select(page => page.Block.Alias).ToList();
            pages.Add("");

            viewData["PagesList"] = pages.OrderBy(s => s).ToList();
        }

        public override void Update() 
        {
            base.Update();
            if (CurrentLocalization == null)
                return;
            // LocalizedVideo.Update();
            Description.Update();
            Title.Update();
            VideoLanguageSelector.CurrentLangCode = CurrentLocalization.Language;
            InfoLanguageSelector.CurrentLangCode = Title.CurrentLangCode;
            var c = Localizations.FirstOrDefault(e => e.Language == CurrentLocalization.Language);
            if (c != null)
            {
                c.Assign(CurrentLocalization);
            }
            
        }

    }
}