using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;

namespace Flexpage.Models
{
    public class LanguageSelectorModel : ViewModel
    {
        public LanguageSelectorModel(Flexpage.Abstract.IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
        }

        public ICollection<string> LangCodes { get; set; }

        //Some models (like EnumModel/EnumValueModel) dont use LocalizedStringModel. They require to have langcodes be hidden in the language selector view.
        public bool? isLocalizedStringUsed { get; set; } 

        private string currentLangCode = null;
        public string CurrentLangCode
        {
            get
            {
                if (currentLangCode == null)
                    currentLangCode = LangCodes.FirstOrDefault();
                return currentLangCode;
            }
            set
            {
                currentLangCode = value;
            }
        }
        /// <summary>
        /// Javascript function for language changing
        /// </summary>
        public string FunctionName { get; set; }

        public override void Update()
        {
            base.Update();
        }

        /* public void SelectLanguage(string langCode)
        {
            if (!LangCodes.Contains(langCode))
                LangCodes.Add(langCode);
            CurrentLangCode = langCode;
        } */

        public void Update(string langCode, ICollection<string> codes)
        {
            LangCodes = codes;
            CurrentLangCode = langCode;
        }
    }
}