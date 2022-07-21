using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using Flexpage.Code.CMS;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System.Web.Mvc;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{
    public enum WidgetStep
    {
        Step1 = 1,
        Step2 = 2,
        Step3 = 3,
        Step4 = 4,
        Step5 = 5,
        Step6 = 6,
        Step7 = 7,
        Step8 = 8,
        Step9 = 9,
        Step10 = 10,
        Step11 = 11,
        Step12 = 12,
        Step13 = 13,
        Step14 = 14,
        Step15 = 15,
        Step16 = 16,
        Step17 = 17,
        Step18 = 18,
        Step19 = 19,
        Step20 = 20
    }
    


    public enum AlignedDisplayPosition
    {
        [Display(Name = "Below slogan")]
        BelowSlogan = 0,
        [Display(Name = "Above slogan")]
        AboveSlogan = 1,
        [Display(Name = "Align position")]
        AlignPosition = 2,
    }

    [Flags]
    public enum GalleryEffects
    {
        None = 0x0,
        Tint = 0x1,
        Blur = 0x2,
        Shadow = 0x4,
        Glow = 0x8,
        BlackAndWhite = 0x10,
    }

    public enum TextPosition
    {
        TopLeft = 0,
        Top,
        TopRight,
        MiddleLeft,
        Middle,
        MiddleRight,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public class IndentPropertiesModel: ViewModel
    {
        public IndentPropertiesModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }
        public int Amount { get; set; }
        public SizeUnitType SizeUnitType { get; set; } = SizeUnitType.Pixel;
        public bool IsApplyIndent { get; set; }
    }

    public class TextPropertiesModel: ViewModel
    {
        public TextPropertiesModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Padding = new IndentPropertiesModel(settings, flexpage);
            Margin = new IndentPropertiesModel(settings, flexpage);
        }
        public string Text { get; set; }

        public string ForeColor { get; set; }
        public string BackColor { get; set; }
        public TextPosition Position { get; set; } = TextPosition.TopLeft;
        public IndentPropertiesModel Padding { get; set; }
        public IndentPropertiesModel Margin { get; set; }

        public TextEffect Effect { get; set; } = TextEffect.None;

        public string EffectColor { get; set; }
        public int EffectOpacity { get; set; } = 100;

    }

    public class TextWithLinkPropertiesModel: TextPropertiesModel
    {
        public TextWithLinkPropertiesModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }

        public bool UseUrl { get; set; } = true;
        public string Url { get; set; }
        public string Page { get; set; }
    }

    public class AdditionalTextWithLinkModel : TextWithLinkPropertiesModel
    {
        public AdditionalTextWithLinkModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage) { }
        public AlignedDisplayPosition DisplayPosition { get; set; } = AlignedDisplayPosition.AlignPosition;
    }

    public enum TextEffect
    {
        None = 0,
        Shadow = 1,
        Glow = 2,
    }

    public enum EditorType
    {
        Simple = 1,
        Advanced = 2
    }

    public class PictureModel: GalleryManagerModel
    {
        public LocalizedStringModel LocalizedImage { get; set; }

        public LocalizedStringModel LocalizedUrl { get; set; }
        public LocalizedStringModel LocalizedTitle { get; set; }
        public LocalizedStringModel LocalizedDescription { get; set; }
        public LocalizedStringModel LocalizedAlternateText { get; set; }
        public LocalizedStringModel LocalizedSloganText { get; set; }
        public LocalizedStringModel LocalizedCallToActionText { get; set; }

        public string AlternateText {
            get {
                var altText = LocalizedAlternateText.NotEmptyLocalization;
                if(string.IsNullOrWhiteSpace(altText) && Settings.FallbackAlternateText)
                    return System.IO.Path.GetFileName(LocalizedImage.NotEmptyLocalization);
                return altText;
            }
        }

        public EditorType EditorType { get; set; }
        public bool ConstrainProportions { get; set; } = false;
        public SizeType WidthType { get; set; } = SizeType.Auto;
        public int CustomWidth { get; set; }
        public SizeUnitType CustomWidthUnit { get; set; } = SizeUnitType.Pixel;
        public SizeType HeightType { get; set; } = SizeType.Auto;
        public int CustomHeight { get; set; }
        public SizeUnitType CustomHeightUnit { get; set; } = SizeUnitType.Pixel;

        public bool Tint { get; set; } = false;
        public bool Blur { get; set; } = false;
        public bool BlackAndWhite { get; set; } = false;

        public int TintOpacity { get; set; } = 100;
        public string TintColor { get; set; }

        public TextPropertiesModel Slogan { get; set; }
        public AdditionalTextWithLinkModel CallToAction { get; set; }

        public bool ShowSlogan { get { return !string.IsNullOrEmpty(LocalizedSloganText.CurrentLocalization); } }
        public bool ShowCallToAction { get { return LocalizedCallToActionText.Localizations.Where(u=> !string.IsNullOrEmpty(u.Value)).Count() > 0 || !string.IsNullOrEmpty(CallToAction.Url) || !string.IsNullOrEmpty(CallToAction.Page); } }

        public List<Language> Languages { get; set; }

        /// <summary>
        /// Fills model content from provided source 
        /// </summary>
        /// <param name="source">Provided source</param>
        /// <param name="args">First parameter is alias, second parameter is block name </param>
        public override void Assign(object source, params object[] args)
        {

            Picture picture = source as Picture;
            base.Assign(picture.Block);

            if(picture != null)
            {
                processModelData(picture);
            }
            else
            {
                EditorType = EditorType.Simple;
                Step = WidgetStep.Step2;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
        }

        public PictureModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            CustomWidth = 100;
            CustomHeight = 100;
            Tint = false;
            Blur = false;
            BlackAndWhite = false;
            TintOpacity = 75;
            GalleryBlockType = GalleryBlockType.SinglePicture;
            EditorType = EditorType.Simple;

            Slogan = new TextPropertiesModel(settings, flexpage);
            CallToAction = new AdditionalTextWithLinkModel(settings, flexpage);

            LocalizedImage = new LocalizedStringModel(settings, flexpage);

            LocalizedUrl = new LocalizedStringModel(settings, flexpage);
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            LocalizedDescription = new LocalizedStringModel(settings, flexpage);
            LocalizedAlternateText = new LocalizedStringModel(settings, flexpage);
            LocalizedSloganText = new LocalizedStringModel(settings, flexpage);
            LocalizedCallToActionText = new LocalizedStringModel(settings, flexpage);

            EditorType = EditorType.Simple;
            Step = WidgetStep.Step2;
        }

        // 2DO: Create ancestor interface IFlexpageBlock of all Flexpage blocks in DB and change source type accordingly
        public PictureModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, object source, params object[] args) : base(settings, flexpage)
        {
            Slogan = new TextPropertiesModel(settings, flexpage);
            CallToAction = new AdditionalTextWithLinkModel(settings, flexpage);
            LocalizedImage = new LocalizedStringModel(settings, flexpage);
            LocalizedUrl = new LocalizedStringModel(settings, flexpage);
            LocalizedTitle = new LocalizedStringModel(settings, flexpage);
            LocalizedDescription = new LocalizedStringModel(settings, flexpage);
            LocalizedAlternateText = new LocalizedStringModel(settings, flexpage);
            LocalizedSloganText = new LocalizedStringModel(settings, flexpage);
            LocalizedCallToActionText = new LocalizedStringModel(settings, flexpage);

            Assign(source, args);
        }

        public override void Delete(IFlexpageRepository repository)
        {

        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            Picture item = null;

            if(proto.ID == -1 && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<Picture>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<Picture>(ID);
            }

            if(item != null)
            {
                Assign(item);
            }
        }

        public void processModelData(Picture picture)
        {
            LocalizedImage.CurrentLangCode = 
                LocalizedTitle.CurrentLangCode = 
                LocalizedDescription.CurrentLangCode = 
                LocalizedAlternateText.CurrentLangCode = 
                LocalizedUrl.CurrentLangCode = 
                LocalizedSloganText.CurrentLangCode = 
                LocalizedCallToActionText.CurrentLangCode = _settings.GetCurrentOrDefaultLangCode();

            if (picture != null)
            {
                foreach (PictureImageLocalization loc in picture.PictureImageLocalization)
                {
                    LocalizedImage.AddText(loc.Language.Code, loc.ImageUrl);
                }

                foreach (PictureLocalization loc in picture.PictureLocalization)
                {
                    LocalizedTitle.AddText(loc.Language.Code, loc.Title);
                    LocalizedDescription.AddText(loc.Language.Code, loc.Description);
                    LocalizedAlternateText.AddText(loc.Language.Code, loc.AlternateText);
                    LocalizedUrl.AddText(loc.Language.Code, loc.LinkUrl);
                    LocalizedSloganText.AddText(loc.Language.Code, loc.SloganText);
                    LocalizedCallToActionText.AddText(loc.Language.Code, loc.CallToActionText);
                }

                Step = WidgetStep.Step2;
                ConstrainProportions = picture.ConstrainProportions;
                WidthType = (SizeType)picture.WidthType;
                CustomWidth = picture.CustomWidth;
                CustomWidthUnit = (SizeUnitType)picture.CustomWidthUnit;
                HeightType = (SizeType)picture.HeightType;
                CustomHeight = picture.CustomHeight;
                CustomHeightUnit = (SizeUnitType)picture.CustomHeightUnit;

                GalleryEffects imageEffects = (GalleryEffects)picture.ImageEffects;
                Tint = imageEffects.HasFlag(GalleryEffects.Tint);
                Blur = imageEffects.HasFlag(GalleryEffects.Blur);
                BlackAndWhite = imageEffects.HasFlag(GalleryEffects.BlackAndWhite);

                TintOpacity = picture.ImageOpacity;
                TintColor = picture.ImageColor;

                Slogan.Text = picture.SloganText;
                Slogan.ForeColor = picture.SloganForeColor;
                Slogan.BackColor = picture.SloganBackColor;
                Slogan.Position = (TextPosition)picture.SloganPosition;
                Slogan.Padding.Amount = picture.SloganPadding;
                Slogan.Padding.SizeUnitType = (SizeUnitType)picture.SloganPaddingUnit;
                Slogan.Padding.IsApplyIndent = picture.SloganApplyPadding;
                Slogan.Margin.IsApplyIndent = picture.SloganApplyMargin;
                Slogan.Margin.Amount = picture.SloganMargin;
                Slogan.Margin.SizeUnitType = (SizeUnitType)picture.SloganMarginUnit;

                GalleryEffects sloganEffects = (GalleryEffects)picture.SloganEffects;
                Slogan.Effect = sloganEffects.HasFlag(GalleryEffects.Shadow) ? TextEffect.Shadow : sloganEffects.HasFlag(GalleryEffects.Glow) ? TextEffect.Glow : TextEffect.None;

                Slogan.EffectColor = picture.SloganColor;
                Slogan.EffectOpacity = picture.SloganOpacity;

                CallToAction.Text = picture.CallToActionText;
                CallToAction.ForeColor = picture.CallToActionForeColor;
                CallToAction.BackColor = picture.CallToActionBackColor;
                CallToAction.Position = (TextPosition)picture.CallToActionPosition;
                CallToAction.Padding.Amount = picture.CallToActionPadding;
                CallToAction.Padding.SizeUnitType = (SizeUnitType)picture.CallToActionPaddingUnit;
                CallToAction.Padding.IsApplyIndent = picture.CallToActionApplyPadding;
                CallToAction.Margin.IsApplyIndent = picture.CallToActionApplyMargin;
                CallToAction.Margin.Amount = picture.CallToActionMargin;
                CallToAction.Margin.SizeUnitType = (SizeUnitType)picture.CallToActionMarginUnit;

                GalleryEffects callToActionEffects = (GalleryEffects)picture.CallToActionEffects;
                CallToAction.Effect = callToActionEffects.HasFlag(GalleryEffects.Shadow) ? TextEffect.Shadow : callToActionEffects.HasFlag(GalleryEffects.Glow) ? TextEffect.Glow : TextEffect.None;

                CallToAction.EffectColor = picture.CallToActionColor;
                CallToAction.EffectOpacity = picture.CallToActionOpacity;

                CallToAction.DisplayPosition = (AlignedDisplayPosition)picture.CallToActionDisplayPosition;
                CallToAction.Url = picture.CallToActionUrl;
                CallToAction.Page = picture.CallToActionPage;
                CallToAction.UseUrl = string.IsNullOrEmpty(picture.CallToActionPage);

            }
            else
            {
                Step = WidgetStep.Step1;
                GalleryBlockType = GalleryBlockType.NotSet;
            }
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            Picture block = repository.GetByBlockID<Picture>(ID);
            if(block == null)
            {
                if(Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }
                block = repository.CreateNewPicture(alias: Alias);
                repository.AddBlockToBlockList(block.Block, BlocklistID, BlockAfter);
            }

            base.Apply(repository);

            updatePropertiesToBlock(block, repository);

            repository.ApplyChanges();

            return null;
        }


        private void updatePropertiesToBlock(Picture pictureBlock, IFlexpageRepository repository)
        {
            pictureBlock.ConstrainProportions = this.ConstrainProportions;
            pictureBlock.WidthType = (int)this.WidthType;
            pictureBlock.CustomWidth = (int)this.CustomWidth;
            pictureBlock.CustomWidthUnit = (int)this.CustomWidthUnit;
            pictureBlock.HeightType = (int)this.HeightType;
            pictureBlock.CustomHeight = this.CustomHeight;
            pictureBlock.CustomHeightUnit = (int)this.CustomHeightUnit;

            GalleryEffects effects = GalleryEffects.None;
            if(this.Tint)
            {
                effects = effects | GalleryEffects.Tint;
            }
            if(this.Blur)
            {
                effects = effects | GalleryEffects.Blur;
            }
            if(this.BlackAndWhite)
            {
                effects = effects | GalleryEffects.BlackAndWhite;
            }
            pictureBlock.ImageEffects = (int)effects;

            pictureBlock.ImageOpacity = this.TintOpacity;
            pictureBlock.ImageColor = this.TintColor;

            pictureBlock.SloganText = this.Slogan.Text;
            pictureBlock.SloganForeColor = this.Slogan.ForeColor;
            pictureBlock.SloganBackColor = this.Slogan.BackColor;
            pictureBlock.SloganPosition = (int)this.Slogan.Position;
            pictureBlock.SloganPadding = this.Slogan.Padding.Amount;
            pictureBlock.SloganPaddingUnit = (int)this.Slogan.Padding.SizeUnitType;
            pictureBlock.SloganApplyPadding = this.Slogan.Padding.IsApplyIndent;
            pictureBlock.SloganApplyMargin = this.Slogan.Margin.IsApplyIndent;
            pictureBlock.SloganMargin = this.Slogan.Margin.Amount;
            pictureBlock.SloganMarginUnit = (int)this.Slogan.Margin.SizeUnitType;

            effects = GalleryEffects.None;
            if(this.Slogan.Effect == TextEffect.Glow)
            {
                effects = effects | GalleryEffects.Glow;
            }
            else if(this.Slogan.Effect == TextEffect.Shadow)
            {
                effects = effects | GalleryEffects.Shadow;
            }

            pictureBlock.SloganEffects = (int)effects;
            pictureBlock.SloganColor = this.Slogan.EffectColor;
            pictureBlock.SloganOpacity = this.Slogan.EffectOpacity;

            pictureBlock.CallToActionText = this.CallToAction.Text;
            pictureBlock.CallToActionForeColor = this.CallToAction.ForeColor;
            pictureBlock.CallToActionBackColor = this.CallToAction.BackColor;
            pictureBlock.CallToActionPosition = (int)this.CallToAction.Position;
            pictureBlock.CallToActionPadding = this.CallToAction.Padding.Amount;
            pictureBlock.CallToActionPaddingUnit = (int)this.CallToAction.Padding.SizeUnitType;
            pictureBlock.CallToActionApplyPadding = this.CallToAction.Padding.IsApplyIndent;
            pictureBlock.CallToActionApplyMargin = this.CallToAction.Margin.IsApplyIndent;
            pictureBlock.CallToActionMargin = this.CallToAction.Margin.Amount;
            pictureBlock.CallToActionMarginUnit = (int)this.CallToAction.Margin.SizeUnitType;

            effects = GalleryEffects.None;
            if(this.CallToAction.Effect == TextEffect.Glow)
            {
                effects = effects | GalleryEffects.Glow;
            }
            if(this.CallToAction.Effect == TextEffect.Shadow)
            {
                effects = effects | GalleryEffects.Shadow;
            }
            pictureBlock.CallToActionEffects = (int)effects;
            pictureBlock.CallToActionColor = this.CallToAction.EffectColor;
            pictureBlock.CallToActionOpacity = this.CallToAction.EffectOpacity;
            pictureBlock.CallToActionDisplayPosition = (int)this.CallToAction.DisplayPosition;
            pictureBlock.CallToActionUrl = this.CallToAction.Url;
            pictureBlock.CallToActionPage = this.CallToAction.Page;

            foreach (string lang in LocalizedImage.Localizations.Keys)
            {
                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);

                string imageUrl = LocalizedImage.GetText(lang);
                if (language != null)
                {
                    PictureImageLocalization loc = pictureBlock.PictureImageLocalization.FirstOrDefault(l => l.LanguageID == language.ID);
                    if (loc == null && !string.IsNullOrEmpty(imageUrl))
                        loc = repository.CreatePictureImageLocalization(pictureBlock, language.ID);

                    if (loc != null)
                    {
                        if(!string.IsNullOrEmpty(imageUrl))
                            loc.ImageUrl = LocalizedImage.GetText(lang);
                        else
                        {
                            pictureBlock.PictureImageLocalization.Remove(loc);
                            repository.DeleteEntity<PictureImageLocalization>(loc.ID);
                        }
                    }
                }
            }

            foreach (string lang in LocalizedTitle.Localizations.Keys
                .Union(LocalizedAlternateText.Localizations.Keys
                    .Union(LocalizedDescription.Localizations.Keys
                        .Union(LocalizedUrl.Localizations.Keys)))
                .Distinct()
                .ToList())
            {
                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);

                string linkUrl = LocalizedUrl.GetText(lang);
                string title = LocalizedTitle.GetText(lang);
                string description = LocalizedDescription.GetText(lang);
                string alternateText = LocalizedAlternateText.GetText(lang);
                string sloganText = LocalizedSloganText.GetText(lang);
                string callToActionText = LocalizedCallToActionText.GetText(lang);

                bool hasData = !string.IsNullOrEmpty(linkUrl) || !string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description) || !string.IsNullOrEmpty(alternateText) || !string.IsNullOrEmpty(sloganText) || !string.IsNullOrEmpty(callToActionText);

                if (language != null)
                {
                    PictureLocalization loc = pictureBlock.PictureLocalization.FirstOrDefault(l => l.LanguageID == language.ID);
                    if (loc == null && hasData)
                        loc = repository.CreatePictureLocalization(pictureBlock, language.ID);

                    if (loc != null)
                    {
                        if (hasData)
                        {
                            loc.LinkUrl = linkUrl ?? "";
                            loc.Title = title ?? "";
                            loc.Description = description ?? "";
                            loc.AlternateText = alternateText ?? "";
                            loc.SloganText = sloganText ?? "";
                            loc.CallToActionText = callToActionText ?? "";
                        }
                        else
                        {
                            pictureBlock.PictureLocalization.Remove(loc);
                            repository.DeleteEntity<PictureLocalization>(loc.ID);
                        }
                    }
                }
            }
        }

        public void SelectPictureLocalization(string langCode)
        {
            LocalizedImage.SelectLanguage(langCode);
        }

        public void SelectDescriptionLocalization(string langCode)
        {
            LocalizedUrl.SelectLanguage(langCode);
            LocalizedTitle.SelectLanguage(langCode);
            LocalizedDescription.SelectLanguage(langCode);
            LocalizedAlternateText.SelectLanguage(langCode);
            LocalizedSloganText.SelectLanguage(langCode);
            LocalizedCallToActionText.SelectLanguage(langCode);
        }

        public string CreateSizeString(int amount, SizeUnitType unitType, string indentString)
        {
            indentString = amount.ToString();

            switch(unitType)
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
            if(properties.IsApplyIndent)
            {
                return CreateSizeString(properties.Amount, properties.SizeUnitType, indentString);
            }

            return indentString;
        }

        public string ToRGBA(string hex, decimal alpha)
        {
            Color color = ColorTranslator.FromHtml(hex);
            return string.Format("rgba({0}, {1}, {2}, {3:0.00})", color.R, color.G, color.B, alpha);
        }

        public string GetPositionCssClass(TextPosition textPosition)
        {
            string retVal = "text-position--top-left";

            switch(textPosition)
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
            LocalizedUrl.Update();
            LocalizedImage.Update();
            LocalizedDescription.Update();
            LocalizedAlternateText.Update();
            LocalizedTitle.Update();
            LocalizedSloganText.Update();
            LocalizedCallToActionText.Update();
        }

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.LocalizedImage.CurrentLangCode,
                    new List<LocalizedStringModel>() { this.LocalizedImage },
                    new List<LocalizedTextModel>() { }, "fp_changePictureLanguage");
            }
        }

        public LanguageSelectorModel DescLanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.LocalizedTitle.CurrentLangCode,
                    new List<LocalizedStringModel>() {
                        this.LocalizedTitle,
                        this.LocalizedAlternateText,
                        this.LocalizedDescription,
                        this.LocalizedUrl,
                        this.LocalizedSloganText,
                        this.LocalizedCallToActionText },
                    new List<LocalizedTextModel>() { }, "fp_changeDescrLanguage");
            }
        }
    }
}