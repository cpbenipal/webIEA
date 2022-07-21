using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Flexpage.Helpers;
using FlexPage.Helpers;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Web;
using System.Web.Hosting;
using System.ComponentModel.DataAnnotations;
using Pluritech.Contact.Abstract;

namespace Flexpage.Models
{
    public class WebFormModel : BlockModel
    {
        private static WebFormFieldValueModel CreateFieldValueModel(FormField field, IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor)
        {
            var c = System.Globalization.CultureInfo.CurrentCulture;
            var textInfo = c.TextInfo;
            return WebFormFieldValueModel.CreateFieldValue(String.Format("WebForm{0}FieldValueModel", textInfo.ToTitleCase(field.FormFieldType.Name)), field, settings, flexpageProcessor, repository);
        }

        public static List<WebFormFieldValueModel> GetFieldValues(IEnumerable<FormField> fields, IFlexpageRepository repository, IFlexpageSettings settings, IFlexpage flexpageProcessor)
        {
            return fields.Select(e => CreateFieldValueModel(e, repository, settings, flexpageProcessor)).ToList();
        }

        public delegate object CreateEntityDelegate(IFlexpageRepository repository, ViewModel model, params object[] args);
        public delegate bool ConfirmDeleteDelegate(IFlexpageRepository repository, ViewModel model, FlexpageEntity target, params object[] args);
        public delegate void DeleteDelegate(IFlexpageRepository repository, FlexpageEntity target, params object[] args);

        // public static void ApplyToCollection<S, T>(IFlexpageRepository repository, IEnumerable<S> source, ICollection<T> target, CreateEntityDelegate OnCreateEntity, Form)
        public static void ApplyToCollection<S, T>(IFlexpageRepository repository, IEnumerable<S> source, ICollection<T> target, CreateEntityDelegate OnCreateEntity, Form form, FormSection section, ConfirmDeleteDelegate OnConfirmDeletion, DeleteDelegate OnDelete)
            where S : ViewModel
            where T : FlexpageEntity
        {
            foreach (var i in source)
            {
                var e = target.FirstOrDefault(e0 => e0.ID == i.ID);
                if (i.ID < 0 || e == null)
                {
                    // e = OnCreateEntity(repository, i, args) as T;
                    e = OnCreateEntity(repository, i, form, section) as T;
                    i.ID = e.ID;

                    // f = repository.CreateNewFormField(form, field.TypeID, field.Name, section);
                }
                // i.Apply(repository, args[0], e);
                i.Apply(repository, e, form);
            }

            List<T> rl = new List<T>();

            foreach (var i in target)
            {
                var f = source.FirstOrDefault(e => e.ID == i.ID);
                var r = f == null;
                if (OnConfirmDeletion != null)
                    r = r & OnConfirmDeletion(repository, f, i, section);
                if (r)
                    rl.Add(i);
            }

            foreach (var i in rl)
            {
                target.Remove(i);
                OnDelete?.Invoke(repository, i, section);
                repository.DeleteEntity<T>(i.ID);
            }
        }

        public string Uncollapsed { get; set; } = "[]";

        public class TemplateInfo
        {
            public int TemplateID { get; set; }
            public string TemplateName { get; set; }
            public eFormTemplateType TemplateType { get; set; } = eFormTemplateType.CustomHtml;
            public bool IsInvalid { get; set; } = false;
        }

        public LocalizedStringModel Title { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }
        public int CurrentTab { get; set; }

        public int? FormID { get; set; }
        public eFormType Type { get; set; } = eFormType.Form;
        // public eFormTemplateType TemplateType { get; set; } = eFormTemplateType.None;
        public int TemplateTypeID { get; set; } = 0;

        public int Step { get; set; } = 0;

        public List<WebFormActionModel> FormActions { get; set; } = new List<WebFormActionModel>();
        public List<SubscriptionModel> Subscriptions { get; set; } = new List<SubscriptionModel>();
        public List<WebFormActionModel> ActionTemplates { get; set; }
        public List<TemplateInfo> SystemTemplates { get; set; }
        public List<TemplateInfo> UserTemplates { get; set; }
        public bool ShowFormTitle { get; set; }

        public List<WebFormSectionModel> Sections { get; set; } = new List<WebFormSectionModel>();
        public List<WebFormFieldModel> Fields { get; set; } = new List<WebFormFieldModel>();

        public List<WebFormFieldValueModel> FieldValues { get; set; } = new List<WebFormFieldValueModel>();
        //public List<WebFormFieldValueModel> fieldValues = null;
        //public List<WebFormFieldValueModel> FieldValues
        //{
        //    get
        //    {
        //        if (fieldValues == null)
        //            LoadFieldValues();
        //        return fieldValues;
        //    }
        //}

        public List<WebFormFieldModel> TemplateFields { get; set; } = new List<WebFormFieldModel>();
        public List<WebFormSectionModel> TemplateSections { get; set; } = new List<WebFormSectionModel>();
        public LocalizedStringModel SubmitButtonCaption { get; set; }
        public int SubmitButtonLayoutID { get; set; }

        public string ErrorMessages { get; set; }
        public string TemplateName { get; set; }
        public int FieldsTemplateID { get; set; }
        public bool ReadOnly
        {
            get => Type == eFormType.Form;
        }
        public LanguageSelectorModel LanguageSelector
        {
            get
            {
                return GetLanguageSelector(this.Title.CurrentLangCode,
                    new List<LocalizedStringModel>() { this.Title },
                    new List<LocalizedTextModel>() { },
                    "fp_webFormChangeLanguage");
            }
        }


        public List<HttpPostedFileBase> Files { get; set; }

        public WebFormModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            _settings = settings;
            SystemTemplates = new List<TemplateInfo>();
            UserTemplates = new List<TemplateInfo>();
        }

        public WebFormModel(FormBlock source, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            _settings = settings;
            // assign(source, repository.GetFieldTypes());
            Assign(source, repository);
        }
        public void AssignLight(object source, params object[] args)
        {
            if (source is FormBlock)
            {
                FormBlock formBlock = source as FormBlock;
                if (formBlock.Block != null)
                    base.Assign(formBlock.Block, args);
                FormID = formBlock?.FormID;
                Name = formBlock?.Form?.Name;
                ShowFormTitle = formBlock?.Form?.ShowTitle ?? true;
                Title = Models.LocalizedStringModel.CreateNew(formBlock?.Form?.Title, _settings, _flexpageProcessor);
            }
        }
        public override void Assign(object source, params object[] args)
        {
            if (source is FormBlock)
            {
                Step = 1;
                FormBlock formBlock = source as FormBlock;
                this.ID = formBlock.BlockID;
                if (formBlock.Block != null)
                    base.Assign(formBlock.Block, args);
                var isEditor = (bool)args[1];
                var r = args[0] as IFlexpageRepository;
                AssignFormData(formBlock.Form, r, isEditor);
                if (isEditor)
                    Update(r);
                // assign(formBlock, args[0] as List<FormFieldType>);
                // Assign(formBlock, args[0] as IFlexpageRepository);

            }
        }

        public void Apply(IFlexpageRepository repository, Form target)
        {
            // ??? target.FormType = ;
            target.Name = Name;
            target.ShowTitle = ShowFormTitle;
            target.SubmitButtonCaption = SubmitButtonCaption.ToJson();
            target.SubmitButtonLayout = SubmitButtonLayoutID;
            target.TemplateTypeID = TemplateTypeID;
            target.Title = Title.ToJson();
            target.TypeID = (int)Type;

            ApplyToCollection(repository, FormActions, target.FormActions, (r, m, args) =>
            {
                var frm = args[0] as Form;
                var am = m as WebFormActionModel;
                return r.CreateFormActionLink(frm, am.ActionID);
            }, target, null, null, null);


            ApplyToCollection(repository, Fields, target.FormField, (r, m, args) =>
            {
                var frm = args[0] as Form;
                var fld = m as WebFormFieldModel;
                var sec = args[1] as FormSection;
                return r.CreateNewFormField(frm, fld.TypeID, fld.Name, sec);
            }, target, null, (r, m, t, args) =>
            {
                var fm = m as WebFormFieldModel;
                var te = t as FormField;
                // return fm.SectionID == te.FormSectionID;
                return te.FormSectionID == null;
            }, null);

            ApplyToCollection(repository, Sections, target.FormSection, (r, m, args) =>
            {
                var frm = args[0] as Form;
                var sec = m as WebFormSectionModel;
                return r.CreateNewFormSection(frm, sec.Name, sec.Index);
            }, target, null, null, (r, t, args) =>
            {
                var sec = t as FormSection;
                var c = sec.FormField.Count;
                for (int i = 0; i < c; i++)
                    r.DeleteEntity<FormField>(sec.FormField.FirstOrDefault().ID);
            });
        }

        public void Apply(IFlexpageRepository repository, FormBlock target)
        {
            target.FormID = FormID.Value;
            target.Block.Alias = this.Alias;
            //target.Block.Visible = Visible;
            target.Block.CssClass = CssClass;

            if (Type == eFormType.Template)
            {
                // Form f = null;
                // if (FormID.HasValue)
                //    f = repository.GetByID<Form>(FormID.Value);
                // if (f == null)
                //    target.Form = repository.CreateNewForm(Name, (int)eFormType.Template, TemplateTypeID);
                // target.Form = repository.CreateNewForm(Name, eFormType.Template, TemplateTypeID);
                Apply(repository, target.Form);
            }
        }

        /// <summary>
        /// Applies changes made to view model to repository
        /// </summary>
        /// <param name="repository">Repository</param>
        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            FormBlock formBlock = repository.GetByBlockID<FormBlock>(ID);
            if (formBlock == null || Alias == repository.CreateNewAlias) // ???
            {
                if (Alias == repository.CreateNewAlias)
                {
                    Alias = null;
                }

                //if (FormID.HasValue)
                //{
                Form template = null;
                if (FormID.HasValue)
                    template = repository.GetByID<Form>(FormID.Value);
                formBlock = repository.CreateNewFormBlock(template, Visible, CssClass);
                if (!FormID.HasValue)
                    FormID = formBlock.Form.ID;
                if (BlocklistID > -1)
                    repository.AddBlockToBlockList(formBlock.Block, BlocklistID, BlockAfter);
                //}
            }

            base.Apply(repository, formBlock.Block);
            Apply(repository, formBlock);

            repository.ApplyChanges();

            return formBlock;
        }

        private void AssignDefaultValues(IFlexpageRepository repository, string predefinedAlias)
        {
            Title = new LocalizedStringModel(_settings, _flexpageProcessor);
            SubmitButtonCaption = new LocalizedStringModel("Submit", _settings, _flexpageProcessor);
            this.ID = -1;
            this.Alias = string.IsNullOrWhiteSpace(predefinedAlias) ? repository.CreateNewAlias : predefinedAlias;
            // int i = 0;
            // FormActions = repository.GetFormActionsList().Select(e => new WebFormActionModel(e, _settings) { Index = i++ }).ToList();
            FormActions = new List<WebFormActionModel>();
        }

        public override void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
            base.Load(repository, proto, title, needToLoadContent);

            if (proto.IsEditor)
            {
                LoadCommonLists(repository);
                var template = SystemTemplates.FirstOrDefault();
                if (template != null)
                    FieldsTemplateID = SystemTemplates.FirstOrDefault().TemplateID;
                try
                {
                    TemplateFields = repository
                        .GetTemplateFields(FieldsTemplateID)
                        .Where(e => !e.FormSectionID.HasValue)
                        .OrderBy(e => e.OrderBy)
                        .Select(e => new WebFormFieldModel(e, _settings, _flexpageProcessor, repository))
                        .ToList();
                }
                catch 
                {

                }

                try
                {
                    TemplateSections = repository
                        .GetTemplateSections(FieldsTemplateID).OrderBy(e => e.OrderNum).Select(e => new WebFormSectionModel(e, _settings, _flexpageProcessor, repository, false)).ToList();
                }
                catch
                {
                }
            }

            this.Alias = proto.BlockAlias;
            ID = proto.ID;
            FormBlock item = null;

            if (proto.ID == BlockModel.NewStaticBlockID && !string.IsNullOrEmpty(proto.BlockAlias))
            {
                item = repository.GetByAlias<FormBlock>(Alias);
            }
            else
            {
                item = repository.GetByBlockID<FormBlock>(ID);
            }

            if (item == null)
            {
                AssignDefaultValues(repository, proto.BlockAlias);
            }
            else
            {
                Assign(item, repository, proto.IsEditor);
                // Assign(item, proto.BlockAlias, repository);
                if (!proto.IsEditor && item.Form != null && item.Form.FormField.Count > 0)
                {
                    //var c = System.Globalization.CultureInfo.CurrentCulture;
                    //var textInfo = c.TextInfo;
                    //FieldValues = item.Form.FormField.Select(e => WebFormFieldValueModel.CreateFieldValue(String.Format("WebForm{0}FieldValueModel", textInfo.ToTitleCase(e.FormFieldType.Name)), e, _settings, repository)).ToList();
                    // FieldValues = GetFieldValues(item.Form.FormField, repository, _settings);
                }

            }

        }

        // protected void assignFormData(Form form, List<FormFieldType> fieldTypes)
        protected void AssignFormData(Form form, IFlexpageRepository repository, bool isEditor)
        {
            //Subscriptions = repository.GetSubscriptions().Select(s =>
            //{
            //    s.ShortDescription = LocalizedStringModel.CreateNew(s.ShortDescription, _settings, _flexpageProcessor).NotEmptyLocalization;
            //    return s;
            //}).ToList();
            try
            {
                Subscriptions = repository.GetSubscriptions().Select(s =>
                {
                    var desc = s.SubscriptionText.LastOrDefault();
                    CmsTextModel cmsText = null;
                    if (desc == null)
                        cmsText = new CmsTextModel(_settings, _flexpageProcessor);
                    else
                        cmsText = new CmsTextModel(desc.CmsText, _settings, _flexpageProcessor);
                    //s.ShortDescription = 
                    return new SubscriptionModel(_settings, _flexpageProcessor)
                    {
                        Code = s.Code,
                        ShortDescription = LocalizedStringModel.CreateNew(cmsText.FullText.ToLocalizedStringModel(true, 2048).ToJson(), _settings, _flexpageProcessor)
                    };
                }).ToList();
            }
            catch
            {
                HasErrors = true;
            }
            FormID = form.ID;
            Name = form.Name;
            TemplateName = form.Name;
            // Type = (eFormType)form.TypeID;
            Type = eFormType.Form;
            Title = LocalizedStringModel.CreateNew(form.Title, _settings, _flexpageProcessor);
            Title.SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
            ShowFormTitle = form.ShowTitle;
            TemplateTypeID = form.TemplateTypeID;
            SubmitButtonCaption = LocalizedStringModel.CreateNew(form.SubmitButtonCaption, _settings, _flexpageProcessor);
            SubmitButtonCaption.SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
            SubmitButtonLayoutID = form.SubmitButtonLayout;
            Sections = new List<WebFormSectionModel>();
            int i = 0;


            if (form.FormSection != null)
                foreach (var section in form.FormSection.OrderBy(ff => ff.OrderNum).ToList())
                {
                    Sections.Add(new WebFormSectionModel(section, _settings, _flexpageProcessor, repository, isEditor));
                }

            Fields = new List<WebFormFieldModel>();

            i = 0;
            if (form.FormField != null)
                foreach (var field in form.FormField.Where(ff => !ff.FormSectionID.HasValue).OrderBy(ff => ff.OrderBy).ToList())
                {
                    // Fields.Add(new WebFormFieldModel(field, _settings, fieldTypes) { Index = i });
                    Fields.Add(new WebFormFieldModel(field, _settings, _flexpageProcessor, repository) { Index = i });
                    i++;
                }
            // Display  fields in editor's  order
            if (isEditor)
            {
                try
                {
                    LoadTemplateFields(repository);
                }
                catch
                {
                    HasErrors = true;
                }
                i = 0;
                // FormActions = repository.GetFormActionsList().Select(e => new WebFormActionModel(e, _settings) { Index = i++ }).ToList();

                try
                {
                    FormActions = form.FormActions.OrderBy(e => e.Order).Select(e => new WebFormActionModel(e, _settings, _flexpageProcessor) { Index = i++ }).ToList();
                    FormActions.Sort((a, b) => (a.Index - b.Index));
                }
                catch
                {
                    HasErrors = true;
                }

                //if (FormActions != null)
                //{
                //    foreach (var e in form.FormActions)
                //    {
                //        var e0 = FormActions.FirstOrDefault(e1 => e1.ActionID == e.ActionID);
                //        if (e0 != null)
                //        {
                //            e0.Selected = true;
                //            e0.ID = e.ID;
                //            e0.Index = e.Order;
                //        }
                //    }
                //    FormActions.Sort((a, b) => (a.Index - b.Index));
                //}
            }
            else
                FieldValues = GetFieldValues(form.FormField.Where(ff => !ff.FormSectionID.HasValue).OrderBy(e => e.OrderBy), repository, _settings, _flexpageProcessor);


        }

        public WebFormFieldModel AddField(IFlexpageRepository repository, int templateFieldID, string targetIndex, string sectionIndex)
        {
            var f = repository.GetByID<FormField>(templateFieldID);
            int i, j;
            WebFormFieldModel fm;
            if (f == null)
                fm = new WebFormFieldModel(_settings, _flexpageProcessor) { FieldTypes = repository.GetFieldTypes(), Name = "New field", ID = -1 };
            else
                fm = new WebFormFieldModel(f, _settings, _flexpageProcessor, repository);
            // fm = new WebFormFieldModel(f, _settings, repository.GetFieldTypes()) { ID = -1 };
            bool ti = int.TryParse(targetIndex, out i);
            if (int.TryParse(sectionIndex, out j))
            {
                if (j > -1 && j < Sections.Count)
                {
                    var s = Sections[j];
                    if (s != null)
                    {
                        fm.SectionID = s.ID;
                        fm.SectionIndex = j;
                        if (ti)
                            s.Fields.Insert(i, fm);
                        else
                            s.Fields.Add(fm);
                    }
                }
            }
            else
            if (ti)
                Fields.Insert(i, fm);
            else
                Fields.Add(fm);

            ReorderFields();
            return fm;
        }

        public WebFormSectionModel AddSection(IFlexpageRepository repository, int templateSectionID, string targetIndex)
        {
            var s = repository.GetByID<FormSection>(templateSectionID);
            int i;
            WebFormSectionModel sm;
            if (s == null)
                sm = new WebFormSectionModel(_settings, _flexpageProcessor) { Name = "New section", ID = -1 };
            else
                sm = new WebFormSectionModel(s, _settings, _flexpageProcessor, repository, false);
            // fm = new WebFormFieldModel(f, _settings, repository.GetFieldTypes()) { ID = -1 };

            if (int.TryParse(targetIndex, out i))
                Sections.Insert(i, sm);
            else
                Sections.Add(sm);

            ReorderSections();
            return sm;
        }

        public WebFormActionModel AddAction(IFlexpageRepository repository, int actionID, string targetIndex)
        {
            var a = repository.GetByID<FormAction>(actionID);
            int i;
            WebFormActionModel am;
            if (a == null)
                am = new WebFormActionModel(_settings, _flexpageProcessor) { Description = "Unknown action", ID = -1 };
            else
                am = new WebFormActionModel(a, _settings, _flexpageProcessor);
            if (int.TryParse(targetIndex, out i))
                FormActions.Insert(i, am);
            else
                FormActions.Add(am);

            ReorderActions();
            return am;
        }

        private void CreateValues(IFlexpageRepository repository, FormData data, IEnumerable<WebFormFieldValueModel> models)
        {
            foreach (var v in models)
            {
                if (v.TypeID == (int)WebFormFieldValueModel.Types.File)
                {
                    if (!String.IsNullOrEmpty(v.StringValue))
                    {
                        if (v.StringValue.ToLower().Contains("/temp/"))
                        {
                            string fn = String.Format("/Content/Files/User/{0}", System.IO.Path.GetFileName(v.StringValue));
                            System.IO.File.Move(HostingEnvironment.MapPath(v.StringValue), HostingEnvironment.MapPath(fn));
                            v.StringValue = fn;
                        }
                    }
                }
                var fv = repository.CreateNewFormFieldValue(data, v.StringValue, v.FieldID);
            }
        }

        public bool SaveToDB(IFlexpageRepository repository, string parameter, ModelStateDictionary state, string keyPrefix = "")
        {
            try
            {
                var data = repository.CreateNewFormData(FormID.Value, parameter);
                CreateValues(repository, data, FieldValues);
                foreach (var s in Sections)
                {
                    CreateValues(repository, data, s.FieldValues);
                }
                repository.ApplyChanges();
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool ValidateSubscription(WebFormActionModel action, IFlexpageRepository repository, ModelStateDictionary state, string keyPrefix = "")
        {
            bool r = true;
            bool correctJSON;
            dynamic p = action.ParseParameter(new { Subscription = "" }, out correctJSON);

            Subscription subscription = repository.GetSubscription(p.Subscription);
            if (subscription == null)
            {
                state.AddModelError("", "Subscription not found!");
                r = false;
            }
            var emailField = FieldValues.FirstOrDefault(fv => fv.FieldName.ToLower().Trim() == "e-mail");
            if (emailField == null)
            {
                state.AddModelError("", "Subscriber email not found! Please Check if form contais field named 'E-Mail'.");
                r = false;
            }

            var email = emailField?.StringValue;
            var firstNameField = FieldValues.FirstOrDefault(fv => fv.FieldName.ToLower().Trim() == "firstname");
            var lastNameField = FieldValues.FirstOrDefault(fv => fv.FieldName.ToLower().Trim() == "lastname");
            if (firstNameField == null && lastNameField == null)
            {
                state.AddModelError("", "Subscriber name not found! Please check if form contais field named 'First name' or 'Last name'.");
                r = false;
            }
            return r;
        }

        public async Task<bool> SaveSubscription(WebFormActionModel action, IFlexpageRepository repository, ModelStateDictionary state, string keyPrefix = "")
        {
            bool correctJSON;
            dynamic p = action.ParseParameter(new { Subscription = "" }, out correctJSON);


            Subscription subscription = repository.GetSubscription(p.Subscription);
            if (subscription == null)
            {
                throw new Exception("Subscription not found!");
            }
            var email = FieldValues.FirstOrDefault(fv => fv.FieldName == "E-Mail")?.StringValue;
            var name = FieldValues.FirstOrDefault(fv => fv.FieldName == "First name").StringValue + " " + FieldValues.FirstOrDefault(fv => fv.FieldName == "Last name").StringValue;
            var subscriber = new Subscriber()
            {
                Name = name,
                Email = email,
                Language = _settings.GetCurrentOrDefaultLangCode(),
                SubscriptionID = subscription.ID
            };
            subscription.Subscriber.Add(subscriber);
            repository.ApplyChanges();

            var mh = new MailHelper();
            var body = GenerateMailBody();
            return await mh.SendMail(ConfigurationManager.AppSettings["FromAddress"], email, "subscription mailing", body); ;
        }

        private string SafeValue(object value, string defaultValue)
        {
            var v = value as string;
            return String.IsNullOrWhiteSpace(v) ? defaultValue : v;
        }

        private void AddModelStateError(WebFormFieldValueModel fieldValue, ModelStateDictionary state, string message, string keyPrefix = "")
        {
            int i = FieldValues.IndexOf(fieldValue);
            var key = String.Format("{0}FieldValues[{1}].StringValue", keyPrefix, i);
            state.AddModelError(key, message);
        }

        public bool ValidateRegister(WebFormActionModel action, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix)
        {
            bool r = true;
            bool correctJSON;
            dynamic p = action.ParseParameter(new { LoginField = "", PasswordField = "", FirstNameField = "", LastNameField = "", EMailField = "", ConfirmPasswordField = "", CompanyNameField = "" }, out correctJSON);
            var loginFieldName = SafeValue(p.LoginField, "Login");
            var passwordFieldName = SafeValue(p.PasswordField, "Password");
            var confirmPasswordFieldName = SafeValue(p.ConfirmPasswordField, "Confirm Password");
            var companyNameFieldName = SafeValue(p.CompanyNameField, "Company name");

            var loginFieldValue = FieldValues.FirstOrDefault(fv => fv.FieldName == loginFieldName);
            if (loginFieldValue == null)
            {
                state.AddModelError("", String.Format("Unable to find login field. Please check if form fild named '{0}' exists", loginFieldName));
                r = false;
            }
            if (String.IsNullOrWhiteSpace(loginFieldValue.StringValue))
            {
                AddModelStateError(loginFieldValue, state, "Login must not be empty or whitespace", keyPrefix);
                r = false;
            }

            var passwordFieldValue = FieldValues.FirstOrDefault(fv => fv.FieldName == passwordFieldName);
            var confirmPasswordFieldValue = FieldValues.FirstOrDefault(fv => fv.FieldName == confirmPasswordFieldName);
            if (passwordFieldValue == null)
            {
                state.AddModelError("", "Incorrect action setup. Check password field name parameter");
                r = false;
            }
            if (confirmPasswordFieldValue == null)
            {
                state.AddModelError("", "Incorrect action setup. Check confirm password field name parameter");
                r = false;
            }

            var password = FieldValues.FirstOrDefault(fv => fv.FieldName == passwordFieldName)?.StringValue;
            var confirmPassword = FieldValues.FirstOrDefault(fv => fv.FieldName == confirmPasswordFieldName)?.StringValue;
            if (password != confirmPassword)
            {
                AddModelStateError(passwordFieldValue, state, "Passwords do not match", keyPrefix);
                AddModelStateError(confirmPasswordFieldValue, state, "Passwords do not match", keyPrefix);
                r = false;
            }
            if (contactProvider.ExistWebLogin(loginFieldValue.StringValue))
            {
                AddModelStateError(loginFieldValue, state, "Provided login already present in database. Try enter another one", keyPrefix);
                r = false;
            }
            return r;
        }

        public async Task<bool> SaveRegister(WebFormActionModel action, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix)
        {
            bool correctJSON;
            dynamic p = action.ParseParameter(new { LoginField = "", PasswordField = "", FirstNameField = "", LastNameField = "", EMailField = "", ConfirmPasswordField = "", CompanyNameField = "" }, out correctJSON);
            var emailFieldName = SafeValue(p.EMailField, "E-Mail");
            var lastNameFieldName = SafeValue(p.LastNameField, "Last name");
            var firstNameFieldName = SafeValue(p.FirstNameField, "First name");
            var loginFieldName = SafeValue(p.LoginField, "Login");
            var passwordFieldName = SafeValue(p.PasswordField, "Password");
            var confirmPasswordFieldName = SafeValue(p.ConfirmPasswordField, "Confirm Password");
            var companyNameFieldName = SafeValue(p.CompanyNameField, "Company name");

            var email = FieldValues.FirstOrDefault(fv => fv.FieldName == emailFieldName)?.StringValue;
            var name1 = FieldValues.FirstOrDefault(fv => fv.FieldName == lastNameFieldName)?.StringValue;
            var name2 = FieldValues.FirstOrDefault(fv => fv.FieldName == firstNameFieldName)?.StringValue;
            var login = FieldValues.FirstOrDefault(fv => fv.FieldName == loginFieldName)?.StringValue;
            var passwordFieldValue = FieldValues.FirstOrDefault(fv => fv.FieldName == passwordFieldName);
            var confirmPasswordFieldValue = FieldValues.FirstOrDefault(fv => fv.FieldName == confirmPasswordFieldName);
            if (passwordFieldValue == null)
                throw new Exception("Incorrect action setup. Check password field name parameter)");
            if (confirmPasswordFieldValue == null)
                throw new Exception("Incorrect action setup. Check confirm password field name parameter)");

            var password = FieldValues.FirstOrDefault(fv => fv.FieldName == passwordFieldName)?.StringValue;
            var confirmPassword = FieldValues.FirstOrDefault(fv => fv.FieldName == confirmPasswordFieldName)?.StringValue;
            var company = FieldValues.FirstOrDefault(fv => fv.FieldName == companyNameFieldName)?.StringValue;
            if (password != confirmPassword)
            {
                AddModelStateError(passwordFieldValue, state, "Passwords do not match", keyPrefix);
                AddModelStateError(confirmPasswordFieldValue, state, "Passwords do not match", keyPrefix);
                // throw new Exception("Passwords do not match");
            }
            Task<bool> mailTask = null;
            try
            {
                var result = contactProvider.SaveNewContact(new Pluritech.Contact.Abstract.DTO.ContactInfoModel()
                {
                    Name1 = name1,
                    Name2 = name2,
                    Type = Pluritech.Contact.Abstract.DTO.eContactType.Person,
                });
                if (!string.IsNullOrWhiteSpace(login))
                    contactProvider.SaveWebLogin(result.ID, login, password);
                contactProvider.AddContactTelecom(new Pluritech.Contact.Abstract.DTO.TelecomView() { Value = email, IsDefault = true, TypeID = 3 },
                    Pluritech.Contact.Abstract.DTO.eContactType.Person, result.ID, result.ShortcutID);
                if (!string.IsNullOrWhiteSpace(company))
                {
                    contactProvider.SaveGeneralInfoPerson(new Pluritech.Contact.Abstract.DTO.PersonViewModel()
                    {
                        Name1 = name1,
                        Name2 = name2,
                        ID = result.ID,
                        ShortcutID = result.ShortcutID,
                        Language = "",
                        Notes = company
                    });
                }
                var mh = new MailHelper();
                var body = GenerateMailBody();
                mailTask = mh.SendMail(ConfigurationManager.AppSettings["FromAddress"], email, "subscription mailing", body);
            }
            catch
            {

            }
            return mailTask == null ? false : await mailTask;
        }

        public string GenerateMailBody()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var v in FieldValues)
            {
                // SYSTEM fieldS - НЕ ПОПАДАЮТ В ТЕЛО ПИСЬМА 
                // HIDDEN FIELDS - ПОПАДАЮТ
                v.WriteToMailBody(sb);
            }
            foreach (var s in Sections)
            {
                // SYSTEM fieldS - НЕ ПОПАДАЮТ В ТЕЛО ПИСЬМА 
                // HIDDEN FIELDS - ПОПАДАЮТ
                s.WriteToMailBody(sb);
            }
            return sb.ToString();
        }

        public async Task<bool> SendMailToAdmin(ModelStateDictionary state, string keyPrefix = "")
        {
            var mh = new MailHelper();
            var body = GenerateMailBody();
            return await mh.SendMail(ConfigurationManager.AppSettings["FromAddress"], ConfigurationManager.AppSettings["ToAddress"], "automatic message", body);
        }

        public bool ValidateSendMailBack(WebFormActionModel action, ModelStateDictionary state, string keyPrefix = "")
        {
            bool correctJSON;
            dynamic p = action.ParseParameter(new { AddressField = "" }, out correctJSON);
            var mh = new MailHelper();
            var fv = FieldValues.FirstOrDefault(e => e.FieldName == p.AddressField);
            if (fv == null)
                fv = FieldValues.FirstOrDefault(e => e.FieldName == "E-Mail");
            if (fv == null)
                state.AddModelError("", "Unable to find mailback address field");
            return true;
        }

        public async Task<bool> SendMailBack(WebFormActionModel action, ModelStateDictionary state, string keyPrefix = "")
        {
            bool correctJSON;
            dynamic p = action.ParseParameter(new { AddressField = "" }, out correctJSON);

            var mh = new MailHelper();
            var fv = FieldValues.FirstOrDefault(e => e.FieldName == p.AddressField);
            if (fv == null)
            {
                fv = FieldValues.FirstOrDefault(e => e.FieldName == "E-Mail");
            }
            var body = GenerateMailBody();
            return await mh.SendMail(ConfigurationManager.AppSettings["FromAddress"], fv.StringValue, "feedback", body);
        }

        public async Task<bool> SendMailList(string addressList, ModelStateDictionary state, string keyPrefix = "")
        {
            var mh = new MailHelper();
            var body = GenerateMailBody();
            return await mh.SendMail(ConfigurationManager.AppSettings["FromAddress"], addressList, "mailing", body);
        }

        public bool ValidateAction(WebFormActionModel a, IFlexpageRepository repository, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix = "")
        {
            bool r = true;
            switch (a.Function.ToLower())
            {
                case "mailtoadmin":
                    break;
                case "mailback":
                    r = ValidateSendMailBack(a, state, keyPrefix);
                    break;
                case "maillist":
                    break;
                case "customaction":
                    break;
                case "inplaceredirect":
                    break;
                case "globalredirect":
                    break;
                case "savetodb":
                    break;
                case "subscribe":
                    r = ValidateSubscription(a, repository, state, keyPrefix);
                    break;
                case "register":
                    r = ValidateRegister(a, contactProvider, state, keyPrefix);
                    break;
            }
            return r;
        }

        public bool ValidateActions(IFlexpageRepository repository, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix = "")
        {
            if (FormActions == null || FormActions.Count < 1)
            {
                var form = repository.GetByID<Form>(this.FormID.Value);
                int i = 0;
                FormActions = form.FormActions.OrderBy(e => e.Order).Select(e => new WebFormActionModel(e, _settings, _flexpageProcessor) { Index = i++ }).ToList();
            }
            bool r = true;
            foreach (var a in FormActions)
            {
                r &= ValidateAction(a, repository, contactProvider, state, keyPrefix);
            }
            return r;
        }

        public object ProcessAction(WebFormActionModel a, IFlexpageRepository repository, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix = "")
        {
            object r = null;
            switch (a.Function.ToLower())
            {
                case "mailtoadmin":
                    r = this.SendMailToAdmin(state, keyPrefix);
                    break;
                case "mailback":
                    r = this.SendMailBack(a, state, keyPrefix);
                    break;
                case "maillist":
                    r = this.SendMailList(a.Parameter, state, keyPrefix);
                    break;
                case "customaction":
                    r = this.CustomAction(a.Parameter, state, keyPrefix);
                    break;
                case "inplaceredirect":
                    r = this.InplaceRedirect(a.Parameter, state, keyPrefix);
                    break;
                case "globalredirect":
                    r = this.GlobalRedirect(a.Parameter, state, keyPrefix);
                    break;
                case "savetodb":
                    r = this.SaveToDB(repository, a.Parameter, state, keyPrefix);
                    break;
                case "subscribe":
                    r = this.SaveSubscription(a, repository, state, keyPrefix);
                    break;
                case "register":
                    var registerTask = this.SaveRegister(a, contactProvider, state, keyPrefix);
                    if (registerTask.Status == TaskStatus.Faulted)
                        throw new Exception(registerTask.Exception.InnerException?.Message);

                    r = registerTask;
                    break;
            }
            return r;
        }

        public ActionResult ProcessActions(IFlexpageRepository repository, IContactProvider contactProvider, ModelStateDictionary state, string keyPrefix = "")
        {
            if (FormActions == null || FormActions.Count < 1)
            {
                var form = repository.GetByID<Form>(this.FormID.Value);
                int i = 0;
                FormActions = form.FormActions.OrderBy(e => e.Order).Select(e => new WebFormActionModel(e, _settings, _flexpageProcessor) { Index = i++ }).ToList();
            }
            object r = null;
            foreach (var a in FormActions)
            {
                r = ProcessAction(a, repository, contactProvider, state, keyPrefix);
                if (Object.Equals(r, false))
                {
                    // state.AddModelError("", "Error submitting form");
                    r = null;
                    break;
                }
            }
            if (!(r is ActionResult))
            {
                return new HTMLResult("Congratulations! Form is successfully submitted");
            }
            else
                return r as ActionResult;

        }

        public ActionResult CustomAction(string url, ModelStateDictionary state, string keyPrefix = "")
        {

            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    var serializedForm = URLSerializer.Serialize(this);
                    HttpResponseMessage response = client.PostAsync(url, serializedForm).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    return new PartialHTMLResult(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
            return new PartialHTMLResult("");
        }
        public ActionResult InplaceRedirect(string url, ModelStateDictionary state, string keyPrefix = "")
        {
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    //// HttpResponseMessage response = await client.GetAsync(url);
                    //HttpResponseMessage response = client.GetAsync(url).Result;
                    //response.EnsureSuccessStatusCode();
                    //// string responseBody = await response.Content.ReadAsStringAsync();
                    //string responseBody = response.Content.ReadAsStringAsync().Result;
                    //return new PartialHTMLResult(responseBody);
                    return new RedirectResult(url);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
            return new PartialHTMLResult("");
        }

        public ActionResult GlobalRedirect(string url, ModelStateDictionary state, string keyPrefix = "")
        {
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    // return new HTMLResult(String.Format("redirect {0}", url));
                    var r = new JsonResult();
                    var _u = url;
                    r.Data = new { url = _u };
                    return r;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                return null;
            }
        }

        //public WebFormFieldModel AddField(IFlexpageRepository repository)
        //{
        //    int i;
        //    var fm = new WebFormFieldModel(f, _settings, repository.GetFieldTypes());
        //    Fields.Add(fm);

        //    ReorderFields();
        //    return fm;
        //}


        public void MoveField(int sourceIndex, int targetIndex)
        {
            var item = Fields[sourceIndex];

            Fields.RemoveAt(sourceIndex);

            if (targetIndex > sourceIndex)
                targetIndex--;
            // the actual index could have shifted due to the removal

            Fields.Insert(targetIndex, item);
            ReorderFields();
        }

        public void DeleteField(int index)
        {
            Fields.RemoveAt(index);
            ReorderFields();
        }

        public void DeleteAction(int index)
        {
            FormActions.RemoveAt(index);
            ReorderActions();
        }

        public void DeleteSection(int index)
        {
            Sections.RemoveAt(index);
            ReorderSections();
        }

        public void ReorderFields()
        {
            for (int i = 0; i < Fields.Count; i++)
                Fields[i].Index = i;
            foreach (var s in Sections)
                s.ReorderFields();
        }

        public void ReorderSections()
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                Sections[i].Index = i;
            }
        }

        public void ReorderActions()
        {
            for (int i = 0; i < FormActions.Count; i++)
            {
                FormActions[i].Index = i;
            }
        }

        public void LoadTemplate(IFlexpageRepository repository)
        {
            if (FormID.HasValue)
            {
                Form form = repository.GetByID<Form>(FormID.Value);
                if (form != null)
                {
                    // assignFormData(form, repository.GetFieldTypes());
                    AssignFormData(form, repository, true);
                }
            }
        }

        public void SetCurrentTemplate(IFlexpageRepository repository)
        {
            Step = 1;

            if (FormID.HasValue)
            {
                LoadTemplate(repository);
                Type = eFormType.Form;
            }
        }

        public void SetCurrentLanguage(IFlexpageRepository repository, string langCode)
        {
            Title.Update();
            Title.SelectLanguageAndSetValue(langCode);
            SubmitButtonCaption.Update();
            SubmitButtonCaption.SelectLanguageAndSetValue(langCode);

            foreach (var f in Fields)
            {
                f.SetCurrentLanguage(langCode);
                f.Update(repository, ReadOnly);
            }
        }

        public void EditTemplate(string type)
        {
            Type = eFormType.Template;
            if (type == "1")
            {
                TemplateTypeID = (int)eFormTemplateType.UserTemplate;
                FormID = null;
            }
        }

        private void LoadTemplateFields(IFlexpageRepository repository)
        {
            TemplateFields = repository.GetTemplateFields(FieldsTemplateID)
                .Where(e => !e.FormSectionID.HasValue)
                .OrderBy(e => e.OrderBy)
                .Select(e => new WebFormFieldModel(e, _settings, _flexpageProcessor, repository))
                .ToList();
            TemplateFields.Insert(0, new WebFormFieldModel(_settings, _flexpageProcessor) { FieldTypes = repository.GetFieldTypes(), Name = "New field" });
            TemplateSections = repository.GetTemplateSections(FieldsTemplateID)
                .OrderBy(e => e.OrderNum)
                .Select(e => new WebFormSectionModel(e, _settings, _flexpageProcessor, repository, false))
                .ToList();
            TemplateSections.Insert(0, new WebFormSectionModel(_settings, _flexpageProcessor) { Name = "New section" });
        }

        public void Update(IFlexpageRepository repository)
        {
            Update();
            Subscriptions = repository.GetSubscriptions().Select(s =>
            {
                var desc = s.SubscriptionText.LastOrDefault();
                CmsTextModel cmsText = null;
                if (desc == null)
                    cmsText = new CmsTextModel(_settings, _flexpageProcessor);
                else
                    cmsText = new CmsTextModel(desc.CmsText, _settings, _flexpageProcessor);
                return new SubscriptionModel(_settings, _flexpageProcessor)
                {
                    Code = s.Code,
                    ShortDescription = LocalizedStringModel.CreateNew(cmsText.FullText.ToLocalizedStringModel(true, 2048).ToJson(), _settings, _flexpageProcessor)
                };
            }).ToList();
            Title.ReadOnly = ReadOnly;
            SubmitButtonCaption.ReadOnly = ReadOnly;
            Title.Update();
            SubmitButtonCaption.Update();
            FormActions.Sort((a, b) => (a.Index - b.Index));
            FormActions.ForEach(e => { e.ReadOnly = ReadOnly; e.Form = this; });
            Fields.Sort((a, b) => (a.Index - b.Index));
            Sections.Sort((a, b) => (a.Index - b.Index));
            LoadTemplateFields(repository);
            LoadActionTemplates(repository);
            foreach (var f in Fields)
                f.Update(repository, ReadOnly);
            foreach (var s in Sections)
                s.Update(repository, ReadOnly);
        }

        public void LoadActionTemplates(IFlexpageRepository repository)
        {
            int i = 0;
            ActionTemplates = repository.GetFormActionsList()
                .Where(e => !e.IsVoid)
                .Select(e => new WebFormActionModel(e, _settings, _flexpageProcessor) { Index = i++ }).ToList();
        }

        protected void LoadCommonLists(IFlexpageRepository repository)
        {
            SystemTemplates = repository.GetFormTemplatesList()
                .Select(f => new TemplateInfo() { TemplateID = f.ID, TemplateName = f.Name, TemplateType = (eFormTemplateType)f.TemplateTypeID })
                // .Where(w => w.TemplateType == eFormTemplateType.ContactForm)
                .ToList();
            UserTemplates = repository.GetFormTemplatesList(true)
                .Select(f => new TemplateInfo() { TemplateID = f.ID, TemplateName = f.Name, TemplateType = (eFormTemplateType)f.TemplateTypeID })
                .ToList();
            LoadActionTemplates(repository);
        }

        //поговорить с Лехой насчет того, какие еще констрайнты наложить на значения полей, типа кол-во знаков,  
        //    *-mandatory
        //    *-соответствие типу
        //    -длина строки ?
        //в случае формы контактов должно быть два атомарных экшена
        //    -валидация
        //    -отсылка письма админу
        //попробовать переименовать FlexpageController в FrontendContoller
        public void ValidateValues(ModelStateDictionary modelState, string keyPrefix = "")
        {
            for (int i = 0; i < Sections.Count; i++)
            {
                var s = Sections[i];
                s.ValidateValues(modelState, String.Format("{1}Sections[{0}]", i, keyPrefix));
            }
            for (int i = 0; i < FieldValues.Count; i++)
            {
                var fv = FieldValues[i];
                fv.Validate(modelState, String.Format("{1}FieldValues[{0}]", i, keyPrefix));
            }
        }

        public WebFormModel DeepCopy()
        {
            WebFormModel newModel = (WebFormModel)this.MemberwiseClone();

            newModel.FieldValues = new List<WebFormFieldValueModel>();
            this.FieldValues.ForEach(v =>
            {
                WebFormFieldValueModel newValue = v.ShallowCopy();
                newValue.StringValue = string.Empty;
                newModel.FieldValues.Add(newValue);
            });

            return newModel;
        }
        private string ExtractCurrentLocalization(string text, IFlexpageSettings settings)
        {

            var lt = LocalizedStringModel.FromJSON<LocalizedStringModel>(text);
            if (lt != null)
            {
                lt.CurrentLangCode = settings.GetCurrentOrDefaultLangCode();
                return lt.GetCurrentLocalizaion();
            }
            return text;
        }
        public void ReloadData(IFlexpageRepository repository, IFlexpageSettings settings)
        {
            // reload form fields to get enum values
            for (int i = 0; i < FieldValues.Count; i++)
            {
                FormField field = repository.GetByID<FormField>(FieldValues[i].FieldID);
                FieldValues[i].Title = ExtractCurrentLocalization(field.Title, _settings);
                FieldValues[i].Description = ExtractCurrentLocalization(field.Description, _settings);
                FieldValues[i].TypeID = field.TypeID;
                FieldValues[i].TypeName = field.FormFieldType.Name;
                FieldValues[i].IsMandatory = field.IsMandatory;
                FieldValues[i].IsSystem = field.IsSystem;
                FieldValues[i].IsHidden = field.IsHidden;
                FieldValues[i].IsEncrypted = field.IsEncrypted;
                FieldValues[i].FieldName = field.Name;

                if (FieldValues[i].TypeID == (int)WebFormFieldValueModel.Types.Enum)
                {
                    WebFormFieldValueModel newValueModel = CreateFieldValueModel(field, repository, settings, _flexpageProcessor);
                    newValueModel.StringValue = FieldValues[i].StringValue;
                    FieldValues[i] = newValueModel;
                }

            }
        }

        public void LoadValues(IFlexpageRepository repository, string parameter)
        {
            List<FormData> values = repository.LoadFormDatas(FormID.Value, parameter);
            if (values != null && values.Count > 0)
            {
                AssignFieldValues(values[0].FieldValues);
            }
        }

        public void AssignFieldValues(ICollection<FormFieldValue> sourceValues)
        {
            AssignFieldValues(sourceValues, FieldValues);
            foreach (WebFormSectionModel section in Sections)
            {
                AssignFieldValues(sourceValues, section.FieldValues);
            }
        }

        private void AssignFieldValues(ICollection<FormFieldValue> source, List<WebFormFieldValueModel> dest)
        {
            foreach (WebFormFieldValueModel valueModel in dest)
            {
                FormFieldValue value = source.FirstOrDefault(fv => fv.FiedlID == valueModel.FieldID);
                if (value != null)
                {
                    valueModel.StringValue = value.Value;
                    if (valueModel.TypeID == (int)WebFormFieldValueModel.Types.Bool)
                        valueModel.BoolValue = value.Value == "True";
                }
            }
        }
    }
}