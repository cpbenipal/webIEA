using System.Collections.Generic;
using System.Linq;
using Flexpage.Code.CMS;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Code.Helpers;
using System.Web.Mvc;
using Flexpage.Domain.Enum;


namespace Flexpage.Models
{

    // 2DO: 
    // 1. Add settings to playlist model (AutoPlay, rename useControls to DisplayControls, rename useDescription to DisplayTitle
    // 2. Add public int PlaylistID to Media class therefore adding Media[] property to MediaPlaylist class 
    // 3. Remove MediaFolderUrl from VideoPlaylist
    // 4. Remove MediaPlayListLocalization property from MediaPlaylist and corresponding class from DB
    // 5. Rename MediaPlaylistCMSLocalization to MediaPlaylistInfoLocalization
    public class MediaPlaylistModel : GalleryManagerModel
    {
        public List<MediaModel> Items { get; set; }
        public MediaModel CurrentItem { get; set; }

        public int InfoID { get; set; } = -1;
        public CmsTextModel Info { get; set; }

        public bool DisplayControls { get; set; } = true;
        public bool DisplayDescription { get; set; }
        public bool DisplayTitle { get; set; } = true;
        public bool AutoPlay { get; set; }
        public GalleryBlockType DisplayType { get; set; }
        public bool ApplySizeConstraints { get; set; } = false;
        public DimensionModel Width { get; set; }
        public DimensionModel Height { get; set; }
        public MediaType MediaType { get; set; }
        public LanguageSelectorModel infoLanguageSelector = null;
        public LanguageSelectorModel InfoLanguageSelector
        {
            get
            {
                infoLanguageSelector = new LanguageSelectorModel(_settings, _flexpageProcessor) { LangCodes = Info.LocalizedTitle.Localizations.Keys, FunctionName = "fp_videoPlaylistChangeInfoLanguage" };
                return infoLanguageSelector;
            }
        }

        public short CurrentTab { get; set; } = 0;

        /// <summary>
        /// Fills model content from provided source 
        /// </summary>
        /// <param name="source">Provided source</param>
        /// <param name="args">First parameter is alias, second parameter is block name </param>
        public override void Assign(object source, params object[] args)
        {
            var vpl = source as MediaPlaylist;

            if (vpl != null)
            {
                base.Assign(vpl.Block);
                Assign(vpl);
            }
            else
            {
                DisplayType = GalleryBlockType.Slider;
                Step = WidgetStep.Step2;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
        }

        public MediaPlaylistModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpageProcessor) : base(settings, flexpageProcessor)
        {
            Width = new DimensionModel() { CustomValue = 100 };
            Height = new DimensionModel() { CustomValue = 100 };
            GalleryBlockType = GalleryBlockType.SinglePicture;
            DisplayType = GalleryBlockType.Slider;
            Items = new List<MediaModel>();
            /* CurrentItem = new VideoModel() {Index = 0, ID = -1, Title = new LocalizedStringModel("Video title"), Description = new LocalizedStringModel("Video description")  };
            Items = new List<VideoModel>() { CurrentItem }; */
            // LocalizedUrl = new LocalizedStringModel();
            Info = new CmsTextModel(settings, flexpageProcessor) { BlocklistID = -1};
            Step = WidgetStep.Step2;
        }

        public override void Delete(IFlexpageRepository repository)
        {

        }

        public void UpdateItems()
        {
            for(int i = 0; i < Items.Count; i ++)
            {
                Items[i].Index = i;
            }
        }

        public void DeleteItem()
        {

            var v = this.Items.FirstOrDefault(e => e.Index == this.CurrentItem.Index);
            if (v != null)
                this.Items.Remove(v);
            UpdateItems();
            CurrentItem = this.Items.FirstOrDefault();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            MediaPlaylist item = null;

            if (proto.ID == -1 && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<MediaPlaylist>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<MediaPlaylist>(ID);
            }

            if (item != null)
            {
                Assign(item);
            }
        }

        public void Assign(MediaPlaylist playlist)
        {
            base.Assign(playlist.Block);
            var lc = _settings.GetCurrentOrDefaultLangCode();

            if (playlist != null)
            {                
                this.DisplayTitle = playlist.DisplayTitle;
                this.ApplySizeConstraints = playlist.ApplySizeConstraints;
                this.DisplayDescription = playlist.DisplayDescription;
                this.AutoPlay = playlist.AutoPlay;
                this.DisplayControls = playlist.DisplayControls;
                this.MediaType = playlist.MediaType;
                Items.Clear();

                foreach (var v in playlist.Media)
                {
                    if (v.MediaType.Name == MediaTypeName.Audio.GetDisplay())
                    {
                        Items.Add(new AudioModel(_settings, _flexpageProcessor, v) { Index = Items.Count });
                    }
                    else
                    {
                        Items.Add(new VideoModel(_settings, _flexpageProcessor, v) { Index = Items.Count });
                    }
                    
                }

                CurrentItem = Items.FirstOrDefault();

                InfoID = playlist.InfoID == null ? -1 : playlist.InfoID.Value;
                Info = new CmsTextModel(playlist.Info, _settings, _flexpageProcessor) {BlocklistID = -1};
                Info.LocalizedTitle.CurrentLangCode = lc;
                Info.FullText.CurrentLanguage = lc;

                InfoLanguageSelector.Update(lc, Info.LocalizedTitle.Localizations.Keys);

                Step = WidgetStep.Step2;
                // ApplySizeConstraints = playlist.ApplySizeConstraints;
                Width = new DimensionModel() { Type = (SizeType)playlist.WidthType, CustomValue = (int)playlist.CustomWidth, CustomValueUnit = (SizeUnitType)playlist.CustomWidthUnit };
                Height = new DimensionModel() { Type = (SizeType)playlist.HeightType, CustomValue = (int)playlist.CustomHeight, CustomValueUnit = (SizeUnitType)playlist.CustomHeightUnit };
            }
            else
            {
                Step = WidgetStep.Step1;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
            Update();
        }

        public void SelectInfoLanguage(string langCode)
        {
                Info.SelectLanguage(langCode);
                InfoLanguageSelector.Update(langCode, Info.LocalizedTitle.Localizations.Keys);
        }

        public void SelectItem(int index)
        {
            CurrentItem = Items.FirstOrDefault(e => e.Index == index);
        }

        public virtual void AddItem()
        {
            
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);
            return null;
        }


        public virtual void ApplyPlaylist(MediaPlaylist playlist, IFlexpageRepository repository)
        {
            playlist.ApplySizeConstraints = this.ApplySizeConstraints;
            playlist.WidthType = (short)this.Width.Type;
            playlist.CustomWidth = (short)this.Width.CustomValue;
            playlist.CustomWidthUnit = (short)this.Width.CustomValueUnit;
            playlist.HeightType = (short)this.Height.Type;
            playlist.CustomHeight = (short)this.Height.CustomValue;
            playlist.CustomHeightUnit = (short)this.Height.CustomValueUnit;
            playlist.DisplayTitle = this.DisplayTitle;
            playlist.DisplayDescription = this.DisplayDescription;
            playlist.AutoPlay = this.AutoPlay;
            playlist.DisplayControls = this.DisplayControls;
            playlist.MediaType =repository.GetMediaType(this.MediaType);
        }

        public void SelectInfoLocalization(string langCode)
        {
            InfoLanguageSelector.CurrentLangCode = langCode;
            Info.SelectLanguage(langCode); 
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
            if (CurrentItem == null)
                return;
            CurrentItem.Update();
            // LocalizedVideo.Update();
            Info.Update();
            InfoLanguageSelector.CurrentLangCode = Info.LocalizedTitle.CurrentLangCode;
            var i = Items.FirstOrDefault(e => e.Index == CurrentItem.Index);
            if (i != null)
            {
                i.Assign(CurrentItem);
            }
        }

    }
}