using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using Flexpage.Models;
using Newtonsoft.Json;
using DevExpress.Utils.Extensions;
using Flexpage.Domain.Entities;
using FlexPage2.Areas.Flexpage.Infrastructure;
using Pluritech.Contact.Abstract;
using ViewModel = Flexpage.Models.ViewModel;

namespace Flexpage.Models
{
    //
    public class WizardFormSection
    {
        public List<WebFormModel> WebForms { get; set; }
        public bool IsRepeatable { get; set; }

        public WizardFormSection()
        {
            WebForms = new List<WebFormModel>();
        }
    }

    public class WizardExtension : WizardUserViewModel
    {       
        public List<WizardFormSection> FormSections { get; set; }

        public WizardExtension()
        {
            FormSections = new List<WizardFormSection>();
        }

        public WizardExtension(WizardUserViewModel source) : base(source)
        {
            FormSections = new List<WizardFormSection>();

            AdminMode = source.AdminMode;

            SessionID = source.SessionID;
            ReadOnlyMode = source.ReadOnlyMode;
            FirstLoad = source.FirstLoad;
        }
    }
    //
}

namespace Flexpage.Controllers
{   
    public class WizardBlockController : Controller
    {
        protected readonly IFlexpageSettings _settings;
        protected readonly IFlexpageRepository _repository;
        protected readonly IWizardProvider _wizardProvider;
        protected readonly IContactProvider _contactProvider;

        // TODO move this code ot provider when WebForms was moved there
        protected readonly IFlexpage _flexpageProcessor;
        // TODO move this code ot provider when WebForms was moved there

        public WizardBlockController(IFlexpageRepository repository, IFlexpageSettings settings, IWizardProvider wizardProvider, IFlexpage flexpageProcessor, IContactProvider contactProvider)
        {
            _repository = repository;
            _settings = settings;
            _wizardProvider = wizardProvider;
            _flexpageProcessor = flexpageProcessor;
            _contactProvider = contactProvider;
        }

        public ActionResult Index()
        {
            return View();
        }

        //Partial view after sucessfully update settings of the block
        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

        public PartialViewResult Wizard(string alias)
        {
            WizardExtension model = new WizardExtension(_wizardProvider.Load(alias));
            model.IsStatic = true;
            LoadStepData(model);
            CheckWebForms(model);
            
            return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", model);
        }

        #region For edit
        [HttpPost]
        [FlexpageAdmin]
        public ActionResult UpdateWizard(WizardEditViewModel model, string command, int? parameter)
        {
            ModelState.Clear();
            model.Mode = eWizardEditMode.General;
            model.Command = command;          

            try
            {
                _wizardProvider.ProcessEditCommand(model, parameter);
                if(command == "stepSettings")
                {
                    LoadStepData(model);
                    CheckWebForms(model);
                }
                return SettingsUpdatedSuccessfully();
            }
            catch
            {
                return PartialView("~/Areas/Flexpage/Views/Flexpage/Editors/WizardForm.cshtml", model);
            }
        }
        #endregion

        #region For user
        [HttpPost]
        public ActionResult SubmitWizardStep(WizardExtension model)
        {
            ModelState.Clear();
            try
            {
                // TODO: Change this processing when WebForms was moved into provider
                // probably we need only 1 service function 'ProcessCommand'

                _wizardProvider.ReloadWizardData(model);
                // TODO: Move this code to provider when WebForms was moved to providers
                // will be moved to processor 'ReloadWizardData'
                if(!model.ReadOnlyMode && model is WizardExtension)
                {
                    foreach(WizardFormSection section in ((WizardExtension)model).FormSections)
                    {
                        foreach(WebFormModel form in section.WebForms)
                            form.ReloadData(_repository, _settings);
                    }
                }
                // TODO: Move this code to provider when WebForms was moved to providers                
                switch(model.Command)
                {
                    case "refresh":
                        LoadStepData(model);
                        CheckWebForms(model);
                        break;
                    case "prev":
                        // save forms data
                        if(!model.ReadOnlyMode)
                        {
                            SaveFormsData(model, _contactProvider);
                        }
                        model.FormSections.Clear();
                        _wizardProvider.GoPrev(model);
                        LoadStepData(model);
                        CheckWebForms(model);
                        break;
                    case "next":
                        // validate forms data ONLY if not readonly
                        if(!model.ReadOnlyMode)
                        {
                            ValidateFormsData(model, ModelState);
                            if(!ModelState.IsValid)
                            {
                                // _wizardProvider.LoadStepData(model);
                                LoadStepData(model);
                                break;
                            }
                            // save forms data
                            SaveFormsData(model, _contactProvider);                            
                        }
                        model.FormSections.Clear();
                        _wizardProvider.GoNext(model);
                        LoadStepData(model);
                        if(model.CurrentStep != model.StepsCount)
                        {
                            CheckWebForms(model);
                        }
                        break;
                    case "add":
                        // _wizardProvider.LoadStepData(model);
                        LoadStepData(model);
                        AddWebForm(model, JsonConvert.DeserializeObject<WizardFormArguments>(model.Arguments));
                        break;
                    case "delete":
                        // _wizardProvider.LoadStepData(model);
                        LoadStepData(model);
                        DeleteWebForm(model, JsonConvert.DeserializeObject<WizardFormArguments>(model.Arguments));
                        break;
                }
                // TODO: Change this processing when WebForms was moved into provider
                // probably we need only 1 service function 'ProcessCommand'
            }
            catch
            {
                // model.ErrorMessages = "Error processing submitted form. Please try again later.";
            }

            return PartialView("~/Areas/Flexpage/Views/Flexpage/WizardForm.cshtml", model);
        }

        // TODO move this code ot provider when WebForms was moved there
        private void CheckWebForms(WizardBaseViewModel model)
        {
            model.Blocks.ForEach(b =>
            {
                if(b.BlockType == "WebForm")
                {
                    if(model is WizardExtension)
                    {
                        WizardFormSection section = new WizardFormSection() { IsRepeatable = model.CurrentStepInfo.RepeatableForms.Contains(b.ID) };
                        WebFormModel formModel = b as WebFormModel;
                        section.WebForms.Add(formModel);
                        ((WizardExtension)model).FormSections.Add(section);

                        if(model is WizardUserViewModel)
                            MultipleForms(model as WizardUserViewModel, section, formModel);
                    }
                    if(model is WizardEditViewModel)
                    {
                        WizardEditViewModel wModel = (WizardEditViewModel)model;
                        if(!wModel.FormSections.Any(s => s.FormID == b.ID))
                        {
                            wModel.FormSections.Add(new EditFormSection() { FormID = b.ID, IsRepeatable = model.CurrentStepInfo.RepeatableForms.Contains(b.ID) });
                        }
                    }
                }
                else
                {
                    if(model is WizardEditViewModel && model.CurrentStepInfo.IsSummary)
                    {
                        if(!((WizardEditViewModel)model).SummaryFormSections.Any(sb => sb.StepID == model.CurrentStepInfo.ID && sb.BlockID == b.ID))
                        {
                            ((WizardEditViewModel)model).SummaryFormSections.Add(new SummaryFormSection()
                            {
                                StepID = model.CurrentStepInfo.ID,
                                StepIndex = model.CurrentStep,
                                BlockID = b.ID,
                                BlockType = b.BlockType,
                                IsActive = true
                            });
                        }
                    }
                }
            });
        }

        private void MultipleForms(WizardUserViewModel model, WizardFormSection section, WebFormModel formModel)
        {
            List<FormData> datas = _repository.LoadFormDatas(formModel.FormID.Value, model.SessionID);
            if(datas != null && datas.Count > 0)
            {
                formModel.AssignFieldValues(datas[0].FieldValues);
                if(datas.Count > 1)
                {
                    for(int dataIdx = 1; dataIdx < datas.Count; dataIdx++)
                    {
                        WebFormModel newFormModel = formModel.DeepCopy();
                        newFormModel.AssignFieldValues(datas[dataIdx].FieldValues);
                        section.WebForms.Add(newFormModel);
                    }
                }
            }
        }

        private void ValidateFormsData(WizardUserViewModel model, ModelStateDictionary modelState)
        {
            if(model is WizardExtension)
            {
                for(int sectionIdx = 0; sectionIdx < ((WizardExtension)model).FormSections.Count; sectionIdx++)
                {
                    WizardFormSection section = ((WizardExtension)model).FormSections[sectionIdx] as WizardFormSection;
                    for(int formIdx = 0; formIdx < section.WebForms.Count; formIdx++)
                    {
                        WebFormModel form = section.WebForms[formIdx] as WebFormModel;
                        form.ValidateValues(modelState, $"FormSections[{sectionIdx}].WebForms[{formIdx}].");
                    }
                }
            }
        }

        private void SaveFormsData(WizardUserViewModel model, IContactProvider contactProvider)
        {
            if(model is WizardExtension)
            {
                for(int sectionIdx = 0; sectionIdx < ((WizardExtension)model).FormSections.Count; sectionIdx++)
                {
                    WizardFormSection section = ((WizardExtension)model).FormSections[sectionIdx] as WizardFormSection;
                    // delete prev saved form data
                    WebFormModel form = section.WebForms[0] as WebFormModel;
                    _repository.DeleteFormDatas(form.FormID.Value, model.SessionID);
                    for(int formIdx = 0; formIdx < section.WebForms.Count; formIdx++)
                    {
                        form = section.WebForms[formIdx] as WebFormModel;
                        form.ProcessAction(new WebFormActionModel(_settings, _flexpageProcessor)
                        {
                            Function = "savetodb",
                            Parameter = model.SessionID,
                        }, _repository, contactProvider, ModelState);
                    }
                }
            }
        }

        public void DeleteWebForm(WizardUserViewModel model, WizardFormArguments args)
        {
            if(model is WizardExtension)
            {
                WizardExtension wModel = (WizardExtension)model;
                if(wModel.FormSections.Count > args.section && wModel.FormSections[args.section].WebForms.Count > args.block)
                {
                    wModel.FormSections[args.section].WebForms.RemoveAt(args.block);
                    int formID = ((WebFormModel)wModel.FormSections[args.section].WebForms[0]).ID;
                    wModel.FormSections[args.section].IsRepeatable = model.CurrentStepInfo.RepeatableForms.Contains(formID);
                }
            }
        }

        private void AddWebForm(WizardBaseViewModel model, WizardFormArguments args)
        {
            if(model is WizardExtension)
            {
                WizardExtension wModel = (WizardExtension)model;
                if(wModel.FormSections.Count > args.section)
                {
                    wModel.FormSections[args.section].WebForms.Add(wModel.FormSections[args.section].WebForms[0].DeepCopy());
                    int formID = ((WebFormModel)wModel.FormSections[args.section].WebForms[0]).ID;
                    wModel.FormSections[args.section].IsRepeatable = model.CurrentStepInfo.RepeatableForms.Contains(formID);
                }
            }
        }

        private void LoadStepData(WizardBaseViewModel model)
        {
            // load step info
            StepInfo step = model.CurrentStepInfo;
            if(step != null)
            {
                Flexpage.Models.BlockListModel Blocklist = new Flexpage.Models.BlockListModel(_settings, _flexpageProcessor);
                Blocklist.BlocklistID = step.BlockListID;
                Blocklist.Load(_repository, new BlockCommandModel(_settings));

                // load all necessary webforms
                if(step.IsSummary)
                {
                    foreach(SummaryFormSection section in model.SummaryFormSections)
                    {
                        if(section.StepID != model.CurrentStepInfo.ID)
                        {
                            // try to create webform
                            ViewModel m = ViewModel.Create("WebFormModel", _settings, _flexpageProcessor);
                            if(m != null)
                            {
                                m.Load(_repository, new BlockCommandModel(_settings)
                                {
                                    ID = section.BlockID,
                                    IsEditor = false,
                                });
                                model.Blocks.Insert(section.Order, m as IBlockModel);
                            }
                        }
                        else
                        {
                            var notWebForm = Blocklist.Blocks.FirstOrDefault(b => b.ID == section.BlockID);
                            if(notWebForm != null)
                                model.Blocks.Insert(section.Order, notWebForm);
                        }
                    }
                }
                else
                {
                    model.Blocks = Blocklist.Blocks;
                }
            }
        }
        // TODO move this code ot provider when WebForms was moved there
        #endregion
    }
}