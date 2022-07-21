using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System;

namespace Flexpage.Models
{
    public class DisclaimerModel : ViewModel
    {
        public bool Enabled { get; set; }

        public int CmsTextID { get; set; }

        public LocalizedTextModel CmsText { get; set; }

        public bool ResetCookie { get; set; }

        public string CookieToken { get; set; }

        public string NewLanguageCode { get; set; }

        public string AcceptCookie => GetResourseValue("AcceptCookie");

        public string DeclineCookie => GetResourseValue("DeclineCookie");

        private int websiteID { get; set; }

        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.CmsText.CurrentLanguage,
                    new List<LocalizedStringModel>() { },
                    new List<LocalizedTextModel>() { this.CmsText });
            }
        }

        public DisclaimerModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            CmsText = new LocalizedTextModel(settings, flexpage);
            CmsText.ParentModelFieldName = "CmsText";
            NewLanguageCode = _settings.GetCurrentOrDefaultLangCode();
            websiteID = settings.WebsiteID;
        }

        public string GenerateNewToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title="", bool needToLoadContent = true)
        {
            base.Load(repository,proto);

            var disclaimer =  repository.GetDisclaimer(websiteID);

            if (disclaimer != null)
            {
                CookieToken = disclaimer.CookieToken;
                ID = disclaimer.ID;
                Enabled = disclaimer.Enabled;
                ResetCookie = false;
                CmsTextID = disclaimer.CmsTextID;
                LoadCmsText(repository);
            }
            else
            {
                CmsText.AddText(CmsText.CurrentLanguage, GetResourseValue("CookieMessage"));
                CmsText.CurrentText = GetResourseValue("CookieMessage");
            }
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            base.Apply(repository, args);

            if (ID == 0)
            {
                repository.CreateDisclaimer(Enabled, SaveCmsLocalization(CmsText, repository, CmsTextID), CookieToken, websiteID);
            }
            else
            {
                var disclaimer = repository.GetByID<Disclaimer>(ID);
                
                if (ResetCookie) // if reset cookie checkbox is marked upon saving, then a new token is generated; otherwise it stays the same.
                {
                    CookieToken = GenerateNewToken();
                }
                disclaimer.CookieToken = CookieToken;
                disclaimer.Enabled = Enabled;
                disclaimer.CmsTextID = CmsTextID;
                CmsText.Texts[NewLanguageCode] =new Abstract.DTO.LocalizedTextsModel(CmsText.CurrentText);
                disclaimer.CmsText = SaveCmsLocalization(CmsText, repository, CmsTextID);

 
            }

            return this;
        }

        public override void Delete(IFlexpageRepository repository)
        {
            base.Delete(repository);
        }

        private CmsText SaveCmsLocalization(LocalizedTextModel model, IFlexpageRepository repository, int ID)
        {
            CmsText cmstext = repository.GetByBlockID<CmsText>(ID);
            if (cmstext == null)
            {
                cmstext = repository.CreateNewCmsText();
            }

            foreach (string lang in model.Texts.Keys)
            {
                if (string.IsNullOrWhiteSpace(lang))
                    continue;

                Language language = repository.Languages.FirstOrDefault(l => l.Code == lang);
                CmsTextLocalization loc = cmstext.CmsTextLocalizations.FirstOrDefault(l => l.Language.Code == lang);
                if (loc == null)
                {
                    loc = repository.CreateNewLocalization(cmstext, language.ID);
                }
                loc.FullText = model.Texts[lang]?.Text;
            }
            return cmstext;
        }

        public void LoadCmsText(IFlexpageRepository repository)
        {
            CmsTextModel cms = new CmsTextModel(_settings, _flexpageProcessor);
            cms.Load(repository, new BlockCommandModel(_settings)
            {
                ID = CmsTextID,
            });

            CmsText = cms.FullText;
            CmsText.ParentModelFieldName = "CmsText";
        }

        public override void Update()
        {
            base.Update();
            CmsText.Update();
        }

        public void SelectLocalization()
        {
            CmsText.SelectLanguageAndSetValue(NewLanguageCode);
        }

        protected string GetResourseValue(string faleValue)
        {
            return HttpContext.GetGlobalResourceObject("Resource", faleValue, System.Globalization.CultureInfo.CurrentUICulture).ToString();
        }
    }
}