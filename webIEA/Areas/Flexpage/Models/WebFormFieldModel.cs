using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{
    public class WebFormFieldDropOperation
    {
        public string sourceID { get; set; }
        public string targetID { get; set; }
    }

    public class WebFormFieldModel : ViewModel
    {
        // public int ID { get; set; }
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public LocalizedStringModel Title { get; set; }
        public int Index { get; set; }

        public string StringTypeID
        {
            get => TypeID.ToString();
            set
            {
                int v;
                if (int.TryParse(value, out v))
                    TypeID = v;
            }
        }

        public string TypeName
        {
            get
            {
                if (FieldTypes == null)
                    return "";
                var f = FieldTypes.FirstOrDefault(e => e.ID == TypeID);
                return f == null ? "" : f.Name;
            }
        }

        public string EnumName
        {
            get
            {
                if (Enums == null)
                    return "";
                var ee = Enums.FirstOrDefault(e => e.ID == EnumID);
                return ee == null ? "" : ee.Name;
            }
        }

        public int TypeID { get; set; }
        public LocalizedStringModel ButtonCaption { get; set; }
        public LocalizedStringModel Description { get; set; }
        public string Default { get; set; }
        public int? SectionID { get; set; }
        public int SectionIndex { get; set; } = -1;
        // public int OrderBy { get; set; }
        public bool IsHidden { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsSystem { get; set; }
        public bool IsEncrypted { get; set; }
        public int? EnumID { get; set; }
        // public List<LocalizedStringModel> Enum { get; set; }
        public Dictionary<int, string> EnumValues { get; set; }
        /* public string CurrentEnumValueStringID
        {
            get => CurrentEnumValueID.ToString();
            set
            {
                int v;
                if (int.TryParse(value, out v))
                    CurrentEnumValueID = v;
            }
        } */

        public string CurrentEnumValue { get; set; }
        public int? CurrentEnumValueID { get; set; }
        public bool IsRadio { get; set; }
        public string RadioGroupName { get; set; }
        public List<FormFieldType> FieldTypes { get; set; }
        public List<Enum> Enums { get; set; }

        public WebFormFieldModel() : base(null, null)
        {

            Title = new LocalizedStringModel("", null, null);
            Description = new LocalizedStringModel("", null, null);
        }

        public WebFormFieldModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Title = new LocalizedStringModel("", settings, flexpage);
            Description = new LocalizedStringModel("", settings, flexpage);
        }

        // public WebFormFieldModel(FormField field, IFlexpageSettings settings, List<FormFieldType> fieldTypes) : base(settings)
        public WebFormFieldModel(FormField field, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            // assign(field, repository.GetFieldTypes());
            Assign(field, repository);
        }

        // public WebFormFieldModel(IFlexpageSettings settings, List<FormFieldType> fieldTypes) : base(settings)
        public WebFormFieldModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            //assign(field, fieldTypes);
        }

        public void SetCurrentLanguage(string langCode)
        {
            Title.SelectLanguageAndSetValue(langCode);
            Description.SelectLanguageAndSetValue(langCode);
        }

        public override void Assign(object source, params object[] args)
        {
            if (source is FormField)
            {
                var ff = source as FormField;
                // assign(ff, args[0] as List<FormFieldType>);
                Assign(ff, args[0] as IFlexpageRepository);
            }
        }

        // protected void assign(FormField source, List<FormFieldType> fieldTypes)
        protected void Assign(FormField source, IFlexpageRepository repository)
        {
            try
            {
                ID = source.ID;
                Name = source.Name;
                ///2do: remove after test finished
                //if (testRnd.Next(0, 13) == 0)
                //    throw new Exception("This is form field test exception");
                Title = LocalizedStringModel.CreateNew(source.Title, _settings, _flexpageProcessor);
                Title.SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
                TypeID = source.TypeID;
                ///2do: remove after test finished
                //if (testRnd.Next(0, 13) == 0)
                //    throw new Exception("This is form field test exception");
                Description = LocalizedStringModel.CreateNew(source.Description, _settings, _flexpageProcessor);
                Description.SelectLanguage(_settings.GetCurrentOrDefaultLangCode());
                Default = source.Default;
                Index = source.OrderBy;
                IsMandatory = source.IsMandatory;
                IsSystem = source.IsSystem;
                IsHidden = source.IsHidden;
                IsEncrypted = source.IsEncrypted;
                EnumID = source.EnumID;
                FieldTypes = repository.GetFieldTypes();
                SectionID = source.FormSectionID;
                if (TypeName == "enum")
                {
                    Enums = repository.GetEnums();
                    EnumValues = new Dictionary<int, string>();
                    if (source.Enum != null)
                        if (source.Enum.EnumValue != null)
                        {
                            foreach (var e in source.Enum.EnumValue)
                            {
                                var levm = LocalizedEnumValueModel.CreateNew(e.Text, _settings, _flexpageProcessor);
                                if (levm != null)
                                    EnumValues.Add(e.ID, levm.Current);
                            }
                        }
                }
                IsRadio = source.IsRadio;
                RadioGroupName = source.RadioGroupName;
                Enums = repository.GetEnums();
            }
            catch
            {
                // HasErrors = true;
                if (Title == null)
                    Title = new LocalizedStringModel(_settings, _flexpageProcessor, Name + ": error loading");
                if (Description == null)
                    Description = new LocalizedStringModel(_settings, _flexpageProcessor, Name + ": error loading");
            }
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            var target = args[0] as FormField;
            // target.ID = ID;
            target.Name = Name;
            target.Title = Title.ToJson();
            target.TypeID = TypeID;
            target.Description = Description.ToJson();
            target.Default = Default;
            // target.OrderBy = OrderBy;
            target.OrderBy = Index;
            target.IsHidden = IsHidden;
            target.IsMandatory = IsMandatory;
            target.IsSystem = IsSystem;
            target.IsEncrypted = IsEncrypted;
            target.EnumID = EnumID;
            target.IsRadio = IsRadio;
            target.RadioGroupName = RadioGroupName;
            // target.FormSectionID = SectionID;
            return target;
        }

        public void Update(IFlexpageRepository repository, params object[] args)
        {
            try
            {
                if (args.Length > 0)
                    if (args[0] is bool)
                        ReadOnly = (bool)args[0];
                Title.ReadOnly = ReadOnly;
                Description.ReadOnly = ReadOnly;
                Title.Update();
                Title.Localizations = Title.Localizations.Where(w => !string.IsNullOrEmpty(w.Value)).ToDictionary(x => x.Key, x => x.Value);
                Description.Update();
                Description.Localizations = Description.Localizations.Where(w => !string.IsNullOrEmpty(w.Value)).ToDictionary(x => x.Key, x => x.Value);
                FieldTypes = repository.GetFieldTypes();
                if (TypeName == "enum")
                {

                    Enums = repository.GetEnums();

                    if (EnumID.HasValue)
                    {

                        /* var ee = Enums.FirstOrDefault(e => e.ID == EnumID);
                        if (ee != null)
                        {
                            EnumValues = new Dictionary<int, string>();
                            if (ee.EnumValue != null)
                            { 
                                foreach (var e in ee.EnumValue)
                                {
                                    // var levm = LocalizedEnumValueModel.CreateNew(e.Text, _settings);
                                    var levm = new LocalizedStringModel(e.Text, _settings);
                                    if (levm != null)
                                    {
                                        EnumValues.Add(e.ID, levm.Current);
                                        if (CurrentEnumValueID == e.ID)
                                            CurrentEnumValue = levm.Current;
                                    }
                                }
                            }
                        } */
                    }
                    else
                    {
                        var ee = Enums.FirstOrDefault();
                        EnumID = ee == null ? -1 : ee.ID;
                    }
                }
            }
            catch
            {
                HasErrors = true;
            }
        }
    }

}