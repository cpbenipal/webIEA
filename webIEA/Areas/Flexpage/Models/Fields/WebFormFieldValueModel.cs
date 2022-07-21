using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{

    public class WebFormFieldValueModel : ViewModel
    {
        public enum Types
        {
            String = 1,
            Int = 2,
            Bool = 3,
            Double = 4,
            Image = 5,
            Enum = 6,
            Memo = 7,
            File = 8,
            Password = 9,
            Email = 10,
            Phone = 11,
            CAPTCHA = 12
        }

        public virtual string View { get => string.Format("_WebForm{0}Field", ((Types)TypeID).ToString()); }
        public int FieldID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FieldName { get; set; }
        public string StringValue { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public List<EnumValue> Items { get; set; }
        public bool BoolValue { get { bool v = false; bool.TryParse(StringValue, out v); return v; } set { StringValue = value.ToString(); } }

        //public bool? IsValid { get; set; }
        //public string ValidationMessage { get; set; }

        public bool IsMandatory { get; set; }
        public bool IsSystem { get; set; }
        public bool IsHidden { get; set; }
        public bool IsEncrypted { get; set; }

        public static WebFormFieldValueModel CreateFieldValue(string className, FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository)
        {
            //var m = new WebFormFieldValueModel(field, settings, repository);
            //return m;
            Type t = Type.GetType("Flexpage.Models." + className);
            if (t != null)
               return Activator.CreateInstance(t, field, settings, flexpage, repository) as WebFormFieldValueModel;
            
            return new WebFormFieldValueModel(field, settings, flexpage, repository);
        }

        public WebFormFieldValueModel() : base(null, null)
        {
        }

        public WebFormFieldValueModel(IFlexpageSettings settings, IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public WebFormFieldValueModel(FormField field, IFlexpageSettings settings, IFlexpage flexpage, IFlexpageRepository repository) : base(settings, flexpage)
        {
            Assign(field);
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

        public virtual void Setup(FormField source, IFlexpageRepository repository)
        {
        }

        // public WebFormFieldModel(IFlexpageSettings settings, List<FormFieldType> fieldTypes) : base(settings)
        public override void Assign(object source, params object[] args)
        {
            if (source is FormField)
            {
                var f = source as FormField;
                FieldID = f.ID;
                IsMandatory = f.IsMandatory;
                IsSystem = f.IsSystem;
                IsHidden = f.IsHidden;
                Title = ExtractCurrentLocalization(f.Title, _settings);
                Description = ExtractCurrentLocalization(f.Description, _settings);
                FieldName = f.Name;
                StringValue = f.Default ?? null;
                TypeID = f.TypeID;
                TypeName = f.FormFieldType.Name;
                if (args.Length > 0)
                {
                    var repo = args[0] as IFlexpageRepository;
                    if (repo != null)
                        Setup(f, repo);
                }
            }

        }

        public virtual void Validate(ModelStateDictionary modelState, string key)
        {
            var fn = key + ".StringValue";
            if(IsMandatory && String.IsNullOrEmpty(StringValue))
            {
                modelState.AddModelError(fn, "This field is required");
            }
            else
            {
                if (IsSystem)
                    return;
                if(!string.IsNullOrEmpty(this.StringValue))
                    this.StringValue = this.StringValue.Trim();               

                switch ((Types)TypeID)
                {
                    case (Types.Bool): break;
                    case (Types.Double):
                        {
                            double v;
                            if(!double.TryParse(this.StringValue, out v))
                                modelState.AddModelError(fn, "The type of this field should be double");
                            break;
                        }
                    case (Types.Enum): break;
                    case (Types.Image): break;
                    case (Types.Int):
                        {
                            int v;
                            if(!int.TryParse(this.StringValue, out v))
                                modelState.AddModelError(fn, "The type of this field should be integer");
                            break;
                        }
                    case (Types.Memo): break;
                    case (Types.String): break;
                    case (Types.Password): break;
                    case (Types.Email):
                        {
                            bool isValid = false;
                            try
                            {
                                var addr = new System.Net.Mail.MailAddress(this.StringValue);
                                isValid = addr.Address == this.StringValue;
                            }
                            catch
                            {
                                isValid = false;
                            }
                            if (!isValid)
                                modelState.AddModelError(fn, "Ths field should be a valid email");
                            break;
                        }
                        
                    case (Types.Phone):
                        {
                            System.Text.RegularExpressions.Regex regexObj = new System.Text.RegularExpressions.Regex(
        @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*" +
         @"(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$");

                            if (regexObj.IsMatch(this.StringValue))
                                modelState.AddModelError(fn, "Ths field should be a valid phone number");

                        break;
                        }
                    case (Types.CAPTCHA):
                        {
                            if (!CaptchaExtension.GetIsValid("captcha_" + FieldID))
                                modelState.AddModelError(fn, "Entered value is invalid");
                            break;
                        }
                }
            }
        }

        public void WriteToMailBody(StringBuilder sb)
        {
            if (!IsSystem)
            {
                sb.Append("<p>");
                if (!String.IsNullOrWhiteSpace(Title))
                {
                    sb.Append("<b>");
                    sb.Append(Title);
                    sb.Append("</b>");
                }

                switch (TypeName.ToLower())
                {
                    case "enum":
                        //int i;
                        //if (int.TryParse(v.StringValue, out i))
                        //{
                        //    sb.Append(v.StringValue);
                        //}
                        sb.Append(StringValue);
                        break;

                    case "memo":
                        if (!String.IsNullOrWhiteSpace(StringValue))
                        {
                            sb.Append("</p>");
                            sb.Append("<p align='justify'>");
                            sb.Append(StringValue);
                        }
                        break;

                    default:
                        if (!String.IsNullOrWhiteSpace(StringValue))
                        {
                            sb.Append(": ");
                            sb.Append(StringValue);
                        }
                        break;

                }

                sb.Append("</p>");
            }
        }

        public WebFormFieldValueModel ShallowCopy()
        {
            return (WebFormFieldValueModel)this.MemberwiseClone();
        }
    }
}