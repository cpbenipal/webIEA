using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{
    public class WebFormSectionModel : ViewModel
    {
        public int Index { get; set; }
        public int FormID { get; set; }
        public string Name { get; set; }
        public LocalizedStringModel Title { get; set; }
        public string CurrentTitle { get; set; }
        public bool ShowTitle { get; set; }
        // public int OrderNum { get; set; }
        public List<WebFormFieldModel> Fields { get; set; } = new List<WebFormFieldModel>();
        public List<WebFormFieldValueModel> FieldValues { get; set; }
        public bool ReadOnly { get; set; }

        public void ReorderFields()
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].Index = i;
            }
        }

        public WebFormSectionModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Title = new LocalizedStringModel("{\"en\":\"New section\"}", settings, flexpage);
        }

        // public WebFormSectionModel(FormSection section, IFlexpageSettings settings, List<FormFieldType> fieldTypes) : base(settings)
        public WebFormSectionModel(FormSection section, IFlexpageSettings settings, Abstract.IFlexpage flexpage, IFlexpageRepository repository, bool isEditor)
            : base(settings, flexpage)
        {
            // assign(section, fieldTypes);
            AssignInternal(section, repository, isEditor);
        }

        public override void Assign(object source, params object[] args)
        {
            if (source is FormSection)
            {
                var ff = source as FormSection;
                // assign(ff, args[0] as List<FormFieldType>);
                AssignInternal(ff, args[0] as IFlexpageRepository, (bool)args[1]);
            }
        }

        // protected void assign(FormSection source, List<FormFieldType> fieldTypes)
        protected void AssignInternal(FormSection source, IFlexpageRepository repository, bool isEditor)
        {
            try
            {
                ID = source.ID;
                Name = source.Name;
                Title = LocalizedStringModel.CreateNew(source.Title, _settings, _flexpageProcessor);
                CurrentTitle = Title.Current;
                ShowTitle = source.ShowTitle;
                // OrderNum = source.OrderNum;
                Index = source.OrderNum;
                FormID = source.FormID;
                //2do: must be removed after test
                //if (testRnd.Next(0, 7) == 0)
                //    throw new Exception("This is form field test exception");
                Fields = new List<WebFormFieldModel>();
                foreach (var field in source.FormField)
                {
                    Fields.Add(new WebFormFieldModel(field, _settings, _flexpageProcessor, repository) { SectionIndex = Index });
                }
                if (!isEditor)
                    FieldValues = WebFormModel.GetFieldValues(source.FormField.OrderBy(e => e.OrderBy), repository, _settings, _flexpageProcessor);

            }
            catch
            {
                if (Fields == null)
                    Fields = new List<WebFormFieldModel>();
                if (FieldValues == null)
                    FieldValues = new List<WebFormFieldValueModel>();
                HasErrors = true;
            }
        }

        public void DeleteField(int index)
        {
            var f = Fields.FirstOrDefault(e => e.Index == index);
            if (f != null)
                Fields.Remove(f);
        }

        public FormSection Apply(IFlexpageRepository repository, FormSection target, Form form)
        {
            target.ID = ID;
            target.Name = Name;
            target.Title = Title.ToJson();
            target.ShowTitle = ShowTitle;
            // target.OrderNum = OrderNum;
            target.OrderNum = Index;

            WebFormModel.ApplyToCollection(repository, Fields, target.FormField, (r, m, args) =>
            {
                var frm = args[0] as Form;
                var fld = m as WebFormFieldModel;
                var sec = args[1] as FormSection;
                return r.CreateNewFormField(frm, fld.TypeID, fld.Name, sec);
            }, form, target, null, null);
            return target;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            return Apply(repository, args[0] as FormSection, args[1] as Form);
        }

        public void Update(IFlexpageRepository repository, params object[] args)
        {
            if (args.Length > 0)
                if (args[0] is bool)
                    ReadOnly = (bool)args[0];
            Title.ReadOnly = ReadOnly;
            Title.Update();
            CurrentTitle = Title.Current;
            foreach (var f in Fields)
            {
                f.Update(repository, ReadOnly);
                f.SectionIndex = Index;
            }
        }

        public void ValidateValues(ModelStateDictionary modelState, string key)
        {
            for (int i = 0; i < FieldValues.Count; i++)
            {
                var f = FieldValues[i];
                f.Validate(modelState, String.Format("{0}.FieldValues[{1}]", key, i));
            }
        }

        public void WriteToMailBody(StringBuilder sb)
        {
            if (!String.IsNullOrWhiteSpace(CurrentTitle))
            {
                sb.Append("<h2>");
                sb.Append(CurrentTitle);
                sb.Append("</h2>");
            }
            sb.Append("<div style='padding-left: 16px'>");
            foreach (var f in FieldValues)
                f.WriteToMailBody(sb);
            sb.Append("</div>");
        }

    }

}

