using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Flexpage.Models
{
    public class FAQQuestionModel
    {
        public int ID { get; set; }

        public int FAQID { get; set; }

        public int? QuestionNo { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        public int? SectionID { get; set; }

        public List<FAQSectionModel> AllSections { get; set; }

        public int? SubsectionID { get; set; }

        public List<FAQSectionModel> AllSubsections { get; set; }


        public LanguageSelectorModel LanguageSelector { get; set; }

        public LocalizedTextModel Answer { get; set; }

        public LocalizedTextModel Question { get; set; }

        public string CountriesView { get; set; }

        public string CountriesDropDowndit { get; set; }

        public string SectionView { get; set; }

        public string SubsectionView { get; set; }

        public void ChangeLanguage(string code)
        {
            LanguageSelector.CurrentLangCode = code;
            Answer.Update();
            Answer.SelectLanguage(code);
            Question.Update();
            Question.SelectLanguage(code);
        }
        public void Load(Flexpage.Abstract.DTO.FAQQuestionModel source, Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage,Domain.Abstract.IFlexpageRepository repository)
        {
            ID = source.ID;
            FAQID = source.FAQID;
            QuestionNo = source.QuestionNo;
            LastUpdatedDate = source.LastUpdatedDate;
            Description = source.Description;
            IsVisible = source.IsVisible;
            AllSections = source.AllSections.Where(s=>s.SectionID==null).ToList();
            SectionID = source.SectionID;
            SubsectionID = source.SubsectionID;
            AllSubsections = source.AllSubsections;
            Answer = new LocalizedTextModel(settings,flexpage);
            Answer.Load(repository,new BlockCommandModel(settings) { ID=ID });
            Answer.Texts = source.Answer?.Texts??new Dictionary<string, LocalizedTextsModel>();
            if (!Answer.Texts.Keys.Contains(settings.GetCurrentOrDefaultLangCode()))
            {
                Answer.Texts.Add(settings.GetCurrentOrDefaultLangCode(), new LocalizedTextsModel());
            }
            Answer.CurrentLanguage = settings.GetCurrentOrDefaultLangCode();
            Answer.CurrentText = Answer.Texts[settings.GetCurrentOrDefaultLangCode()].Text;
            Answer.ParentModelFieldName = "Answer";
            Question = new LocalizedTextModel(settings, flexpage);
            Question.Load(repository, new BlockCommandModel(settings) { ID = ID });
            Question.Texts = source.Question?.Texts ?? new Dictionary<string, LocalizedTextsModel>();
            if (!Question.Texts.Keys.Contains(settings.GetCurrentOrDefaultLangCode()))
            {
                Question.Texts.Add(settings.GetCurrentOrDefaultLangCode(), new LocalizedTextsModel());
            }
            Question.CurrentLanguage = settings.GetCurrentOrDefaultLangCode();
            Question.CurrentText = Question.Texts[settings.GetCurrentOrDefaultLangCode()].Text;
            Question.ParentModelFieldName = "Question";
            SectionView = AllSections.FirstOrDefault(s => SectionID.HasValue && s.ID == SectionID)?.Text;
            SubsectionView = AllSubsections.FirstOrDefault(s => SubsectionID.HasValue && s.ID == SubsectionID)?.Text;
        }
    }
}