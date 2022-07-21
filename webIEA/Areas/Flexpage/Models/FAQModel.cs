using Flexpage.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class FAQModel : BlockModel
    {
        public bool ShowSectionsFilter { get; set; }

        public bool ShowSubsectionsFilter { get; set; }

        public bool ShowCountriesFilter { get; set; }

        public bool ShowLanguagesFilter { get; set; }

        public int? SectionID { get; set; }

        public int? SubsectionID { get; set; }

        public List<FAQSectionModel> AllSections { get; set; }

        public List<FAQSectionModel> AllSubsections { get; set; }

        public List<FAQSectionModel> Sections { get; set; }

        public List<FAQSectionModel> SubSections { get; set; }

        public List<Flexpage.Models.FAQQuestionModel> Questions { get; set; }

        public List<Language> AllLanguages { get; set; }

        public List<Country> AllCountries { get; set; }

        Abstract.IFlexpage _flexpage;
        Domain.Abstract.IFlexpageRepository _repository;

        public LanguageSelectorModel LanguageSelector { get; set; }

       public FAQModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage,Domain.Abstract.IFlexpageRepository repository) :base(settings, flexpage)
        {
            _settings = settings;
            _flexpage = flexpage;
            _repository = repository;
            LanguageSelector = GetLanguageSelectorModel();
        }

        public void Load(Flexpage.Abstract.DTO.FAQModel source)
        {
            ID = source.ID;
            ShowCountriesFilter = source.ShowCountriesFilter;
            ShowLanguagesFilter = source.ShowLanguagesFilter;
            ShowSectionsFilter = source.ShowSectionsFilter;
            ShowSubsectionsFilter = source.ShowSubsectionsFilter;
            SectionID = source.SectionID;
            SubsectionID = source.SubsectionID;

            //AllSubsections = source.AllSubsections ?? new List<FAQSectionModel>();
            AllCountries = source.AllCountries ?? new List<Country>();
            AllLanguages = source.AllLanguages?.Where(l => _settings.AllowedLanguages.Contains(l.Code))?.ToList() ?? new List<Language>();
            Sections = source.AllSections?.Where(c => c.SectionID == null).ToList() ?? new List<FAQSectionModel>();
            AllSections = source?.AllSections?.Where(s => s.SectionID == null).ToList() ?? new List<FAQSectionModel>();
            AllSubsections = source?.AllSections?.Where(s => s.SectionID.HasValue).ToList() ?? new List<FAQSectionModel>();
            SubSections = AllSubsections.Where(s => s.SectionID == SectionID).ToList();

            Questions = new List<FAQQuestionModel>();
            source.Questions.ForEach(q =>
            {
                var model = new FAQQuestionModel();
                q.AllSections = AllSections;
                q.AllSubsections = AllSubsections;
                q.AllCountries = AllCountries;
                model.Load(q, _settings, _flexpage, _repository);
                model.LanguageSelector = LanguageSelector;
                Questions.Add(model);
            });

        }
        public FAQQuestionModel GetNewQuestion()
        {
            var faq = new Abstract.DTO.FAQQuestionModel()
            {
                FAQID = ID,
                IsVisible = true
            };
            var model = new FAQQuestionModel();
            model.Load(faq, _settings, _flexpage, _repository);
            model.LanguageSelector = LanguageSelector ?? GetLanguageSelectorModel(model);
            return model;
        }
        public FAQQuestionModel GetQuestion(int idQuestion)
        {
            var model = new FAQQuestionModel();
            model = Questions.FirstOrDefault(i => i.ID == idQuestion);
            return model;
        }
        protected LanguageSelectorModel GetLanguageSelectorModel(FAQQuestionModel source)
        {
            return new LanguageSelectorModel(_settings,_flexpage)
            {
                LangCodes = GetModelLangCodes(source),
                FunctionName = "fp_changeQuestionLanguage",
                CurrentLangCode = source.Question.CurrentLanguage
            };
        }
        protected LanguageSelectorModel GetLanguageSelectorModel()
        {
            return new LanguageSelectorModel(_settings, _flexpage)
            {
                LangCodes = new List<string>(),
                FunctionName = "fp_changeQuestionLanguage",
                CurrentLangCode = _settings.GetCurrentOrDefaultLangCode()
            };
        }
        protected List<string> GetModelLangCodes(FAQQuestionModel model)
        {
            HashSet<string> langcodes = new HashSet<string>();
            langcodes.UnionWith(model.Question.Texts.Keys);
            langcodes.UnionWith(model.Answer.Texts.Keys);
            return langcodes.ToList();
        }
        
    }
    public class FAQViewModel 
    {
        public int ID { get; set; }

        public bool AllSectionsSelected { get; set; }

        public bool AllSubsectionsSelected { get; set; }

        public bool ShowSectionsFilter { get; set; }

        public bool ShowSubsectionsFilter { get; set; }

        public bool ShowCountriesFilter { get; set; }

        public bool ShowLanguagesFilter { get; set; }

        public FAQSectionModel Section { get; set; }

        public FAQSectionModel Subsection { get; set; }

        public Language Language { get; set; }

        public Country Country { get; set; }

        public List<Flexpage.Models.FAQQuestionModel> Questions { get; set; }

        public List<FAQSectionModel> AllSections { get; set; }

        public List<FAQSectionModel> AllSubsections { get; set; }

        public List<Language> AllLanguages { get; set; }

        public List<Country> AllCountries { get; set; }

        public LanguageSelectorModel LanguageSelector { get; set; }

        Flexpage.Abstract.IFlexpageSettings _settings;
        Abstract.IFlexpage _flexpage;
        Domain.Abstract.IFlexpageRepository _repository;

        public void Load(Flexpage.Abstract.DTO.FAQViewModel source,Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage, Domain.Abstract.IFlexpageRepository repository)
        {
            _settings = settings;
            _flexpage = flexpage;
            _repository = repository;
            ID = source.ID;
            AllSectionsSelected = source.AllSectionsSelected;
            AllSubsectionsSelected = source.AllSubsectionsSelected || AllSectionsSelected;
            ShowCountriesFilter = source.ShowCountriesFilter;
            ShowLanguagesFilter = source.ShowLanguagesFilter;
            ShowSectionsFilter = source.ShowSectionsFilter;
            ShowSubsectionsFilter = source.ShowSubsectionsFilter;
            AllSections = source.AllSections ?? new List<FAQSectionModel>();
            AllSubsections = source.AllSubsections ?? new List<FAQSectionModel>();
            AllCountries = source.AllCountries ?? new List<Country>();
            AllLanguages = source.AllLanguages?.Where(l => _settings.AllowedLanguages.Contains(l.Code))?.ToList() ?? new List<Language>();
            Language = source.Language ?? source.AllLanguages?.FirstOrDefault(l => l.Code == _settings.GetCurrentOrDefaultLangCode());
            var SectionAll = new Abstract.DTO.FAQSectionModel() {Text = "(All)"};

            Section = AllSectionsSelected ? SectionAll : source.Section;
            Subsection = AllSectionsSelected || AllSubsectionsSelected ? SectionAll : source.Subsection;
            Country = source.Country ?? source.AllCountries?.FirstOrDefault(c => c.ShortName == "BE");

            if (Section != null && Section.ID != 0 && !source.AllSectionsSelected)
            {
                AllSubsections = AllSubsections.Where(s => s.SectionID == Section.ID).ToList();
            }
            AllSections.Add(SectionAll);
            AllSubsections.Add(SectionAll);

            Questions = new List<FAQQuestionModel>();
            source.Questions.Where(q=> 
            (!ShowSectionsFilter || AllSectionsSelected || Section==null|| q.SectionID==Section.ID)
            && (!ShowSubsectionsFilter || AllSubsectionsSelected || Subsection==null|| q.SubsectionID == Subsection.ID)
            && (!ShowCountriesFilter  || Country == null || q.Countries.Exists(c=>c.CountryID== Country.ID))
            ).ToList().ForEach(q =>
            {
                var model = new FAQQuestionModel();
                model.Load(q, _settings, _flexpage, _repository);
                model.LanguageSelector = LanguageSelector;
                model.Answer.SelectLanguageAndSetValue(Language?.Code?? _settings.GetCurrentOrDefaultLangCode());
                model.Question.SelectLanguageAndSetValue(Language?.Code ?? _settings.GetCurrentOrDefaultLangCode());
                Questions.Add(model);
            });
        }
    }
}