using System;
using System.Web.Mvc;
using FlexPage;
using System.Collections.Generic;
using System.Linq;
using Flexpage.Models;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using FlexPage2.Areas.Flexpage.Infrastructure;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using FAQModel = Flexpage.Abstract.DTO.FAQModel;
using FAQQuestionModel = Flexpage.Abstract.DTO.FAQQuestionModel;

namespace Flexpage.Controllers
{
    public class FAQBlockController : Controller
    {
        private readonly IFAQBlockProvider _FAQBlockProvider;
        private readonly IFlexpageSettings _settings;
        private readonly IFlexpage _flexpage;
        Domain.Abstract.IFlexpageRepository _repository;
        public FAQBlockController(IFAQBlockProvider FAQBlockProvider, IFlexpageSettings settings, IFlexpage flexpage, Domain.Abstract.IFlexpageRepository repository)
        {
            _FAQBlockProvider = FAQBlockProvider;
            _settings = settings;
            _flexpage = flexpage;
            _repository = repository;
        }
        [HttpPost]
        [ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult UpdateFAQView(FAQModel model, string command, string parameters)
        {
            ModelState.Clear();

            string c = command.ToLower().Trim();
            try
            {
                switch (c)
                {
                    case "save":
                        _FAQBlockProvider.SaveFAQ(model);
                        break;
                }
                return GetView(model);
            }
            catch
            {
                return GetView(model);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult UpdateFAQQuestionEdit(Flexpage.Models.FAQQuestionModel model, string command, string parameters)
        {
            ModelState.Clear();

            string c = command.ToLower().Trim();
            try
            {
                switch (c)
                {
                    case "changelanguage":
                        model.ChangeLanguage(parameters);
                        return GetEditorTextsFor(model);
                }
                return GetEditorFor(model);
            }
            catch
            {
                return GetEditorFor(model);
            }
        }
        protected PartialViewResult GetEditorFor(Flexpage.Models.FAQQuestionModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionEdit.cshtml", model);
        }
        protected PartialViewResult GetEditorTextsFor(Flexpage.Models.FAQQuestionModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionEditTexts.cshtml", model);
        }
        protected PartialViewResult GetEditorFor(Flexpage.Models.FAQModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FAQ/_FAQ.cshtml", model);
        }
        protected PartialViewResult GetView(FAQModel model)
        {
            return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/FAQ.cshtml", model);
        }

        public PartialViewResult LoadQuestionItem(BlockCommandModel commandModel)
        {
            var faq = _FAQBlockProvider.LoadForEditor(commandModel.ID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);

            Flexpage.Models.FAQQuestionModel editItem;

            editItem = model.GetQuestion(commandModel.BlocklistID);
            editItem.FAQID = commandModel.ID;
            
            editItem.AllSections = model.AllSections;
            editItem.AllSubsections = model.AllSubsections;

            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionEdit.cshtml", editItem);
        }

        public PartialViewResult CreateQuestionItem(BlockCommandModel commandModel)
        {
            var faq = _FAQBlockProvider.LoadForEditor(commandModel.ID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            Flexpage.Models.FAQQuestionModel editItem;

            editItem = model.GetNewQuestion();
            editItem.FAQID = commandModel.ID;
            editItem.AllSections = model.AllSections;
            editItem.AllSubsections = model.AllSubsections;

            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionEdit.cshtml", editItem);
        }

        protected PartialViewResult GetSectionView(Models.FAQModel model)
        {
            if (model.SectionID != null)
            {
                return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_SubSectionsGrid.cshtml", model);
            }
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_SectionsGrid.cshtml", model);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FAQ(int BlockID)
        {
            var faq = _FAQBlockProvider.Load(BlockID);
            var model = new Flexpage.Models.FAQViewModel();
            model.Load(faq,_settings, _flexpage,_repository);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FAQ/_FAQ.cshtml", model);
        }
        public ActionResult FAQFilter(int ID )
        {
            string SectionName = Request.Params.Get("Section" + ID);
            string SubsectionName = Request.Params.Get("Subsection" + ID);
            string LanguageName = Request.Params.Get("Language" + ID);
            var faq = _FAQBlockProvider.Load(ID);
            var model = new Flexpage.Models.FAQViewModel();
            faq.AllSectionsSelected = SectionName == "(All)";
            faq.AllSubsectionsSelected = SubsectionName == "(All)";
            faq.Language = faq.AllLanguages.Find(l => l.Name == LanguageName);
            faq.Section = faq.AllSections.Find(l => l.Text == SectionName);
            faq.Subsection = faq.AllSubsections.Find(l => l.Text == SubsectionName);
            model.Load(faq, _settings, _flexpage, _repository);
            return PartialView("~/Areas/Flexpage/Views/Shared/DisplayTemplates/FAQ/_FAQ.cshtml", model);
        }
        public ActionResult FAQEdit(int BlockID,int? BlockAfter, int? BlockListID)
        {
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, BlockAfter, BlockListID);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_FAQ.cshtml", model);
        }
        public ActionResult FAQQuestionGrid(int BlockID)
        {
            var faq = _FAQBlockProvider.LoadForEditor(BlockID,null,null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            ViewBag.Subsections = JsonConvert.SerializeObject(model.AllSubsections);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionGrid.cshtml", model);
        }
        public ActionResult FAQSectionGrid(int BlockID)
        {
            var faq = _FAQBlockProvider.LoadForEditor(BlockID,null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_SectionsGrid.cshtml", model);
        }
        public ActionResult SubSectionsGrid(int BlockID, int? SectionID)
        {
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            faq.SectionID = SectionID;
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_SubSectionsGrid.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult FAQQuestionAdd(Flexpage.Models.FAQQuestionModel source, int BlockID)
        {
            var faq = _FAQBlockProvider.LoadForEditor(BlockID,null, null);
            try
            {
                source.Answer.Update();
                source.Question.Update();
                var modelQuestion = new FAQQuestionModel()
                {
                    ID = source.ID,
                    FAQID = BlockID,
                    QuestionNo = source.QuestionNo,
                    Description = source.Description,
                    IsVisible = source.IsVisible,
                    SectionID = source.SectionID,
                    SubsectionID = source.SubsectionID,
                    Answer=new Abstract.DTO.LocalizedTextModel() {
                        Texts =source.Answer.Texts
                    },
                    Question = new Abstract.DTO.LocalizedTextModel()
                    {
                        Texts = source.Question.Texts
                    }
                };
                
                _FAQBlockProvider.AddFAQQuestion(modelQuestion);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionGrid.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CreateQuestion(Flexpage.Models.FAQQuestionModel source, string command, string parameters)
        {
            //if (source.SectionID == 0 || source.SectionID == null || source.SubsectionID == null || source.SubsectionID == 0)
            //{
            //    throw new Exception("Both section and subsection must be selected!");
            //}
            //if (source.CountriesDropDowndit == null)
            //{
            //    throw new Exception("A country must be selected!\nIf you don\'t want to select any countries, choose \"None\".");
            //}
            var BlockID = source.FAQID;
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            try
            {
                if (source.Answer != null)
                    source.Answer.Update();
                if (source.Question != null)
                    source.Question.Update();
                var questionNo = faq?.Questions?.Max(q => q.QuestionNo);
                var modelQuestion = new FAQQuestionModel()
                {
                    ID = source.ID,
                    FAQID = BlockID,
                    QuestionNo = questionNo.HasValue ? ++questionNo : 1,
                    Description = source.Description,
                    IsVisible = source.IsVisible,
                    SectionID = source.SectionID,
                    SubsectionID = source.SubsectionID,
                    Answer = new Abstract.DTO.LocalizedTextModel()
                    {
                        Texts = source.Answer?.Texts
                    },
                    Question = new Abstract.DTO.LocalizedTextModel()
                    {
                        Texts = source.Question?.Texts
                    }
                };
                if (source.ID > 0)
                {
                    _FAQBlockProvider.EditFAQQuestion(modelQuestion);
                }
                else
                {
                    _FAQBlockProvider.AddFAQQuestion(modelQuestion);
                }
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            //faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            //model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            //model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_Items.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        [FlexpageAdmin]

        public ActionResult FAQQuestionEdit(Flexpage.Models.FAQQuestionModel source, int BlockID)
        {
            if (source.SectionID == 0 || source.SubsectionID == 0)
            {
                throw new Exception("Both section and subsection must be selected!");
            }
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            try
            {
                source.Answer.Update();
                source.Question.Update();
                var modelQuestion = new FAQQuestionModel()
                {
                    ID = source.ID,
                    FAQID = BlockID,
                    QuestionNo = source.QuestionNo,
                    Description = source.Description,
                    IsVisible = source.IsVisible,
                    SectionID = source.SectionID,
                    SubsectionID = source.SubsectionID,
                    Answer = new Abstract.DTO.LocalizedTextModel()
                    {
                        Texts = source.Answer.Texts
                    },
                    Question = new Abstract.DTO.LocalizedTextModel()
                    {
                        Texts = source.Question.Texts
                    }
                };

                _FAQBlockProvider.EditFAQQuestion(modelQuestion);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionGrid.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        [FlexpageAdmin]
        public ActionResult FAQQuestionDelete(int BlockID, int ID)
        {
            try
            {
                _FAQBlockProvider.DeleteFAQQuestion(BlockID, ID);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            model.Load(faq);
            return PartialView("~/Areas/Flexpage/Views/Shared/EditorTemplates/FAQ/_QuestionGrid.cshtml", model);
        }

        [FlexpageAdmin]
        public ActionResult FAQSectionAdd(FAQSectionModel source, int BlockID, int? SectionID)
        {
            try
            {
                source.SectionID = SectionID;
                _FAQBlockProvider.AddFAQSection(source);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            faq.SectionID = source.SectionID;
            model.Load(faq);
            return GetSectionView(model);
        }
        [FlexpageAdmin]
        public ActionResult FAQSectionEdit(FAQSectionModel source, int BlockID, int? SectionID)
        {
            try
            {
                source.SectionID = SectionID;
                _FAQBlockProvider.EditFAQSection(source);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            faq.SectionID = source.SectionID;
            model.Load(faq);
            return GetSectionView(model);
        }
        [FlexpageAdmin]
        public ActionResult FAQSectionDelete(int ID, int BlockID, int? SectionID)
        {
            try
            {
                _FAQBlockProvider.DeleteFAQSection(ID);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var faq = _FAQBlockProvider.LoadForEditor(BlockID, null, null);
            var model = new Flexpage.Models.FAQModel(_settings, _flexpage, _repository);
            faq.SectionID = SectionID;
            model.Load(faq);
            return GetSectionView(model);
        }
        [FlexpageAdmin]
        public JsonResult FAQSectionAddCombo(string text, int? sectionID)
        {
            FAQSectionModel result;
            try
            {
                result = _FAQBlockProvider.AddFAQSection(new FAQSectionModel() {Text = text, Value = default(int), SectionID = sectionID});
            }
            catch (Exception e)
            {
                return Json("{\"success\": false,\"error\": { \"title\" : \"Error import\",\"message\" : \"" + e.Message + "\"}}");
            }

            return Json("{\"success\": true, \"ID\" : " + result.ID + ", \"Text\" : \"" + result.Text + "\"}");
        }

    }
}