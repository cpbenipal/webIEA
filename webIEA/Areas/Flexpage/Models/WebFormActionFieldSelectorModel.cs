using System.Collections;
using System.Collections.Generic;

namespace Flexpage.Models
{

    public class WebFormActionFieldSelectorModel
    {
        public string Title { get; set; }
        public bool ReadOnly { get; set; }
        public string Value { get; set; }
        public List<string> Items { get; set; }
        public string ComboboxName { get; set; }

        public WebFormActionFieldSelectorModel(string title, bool readOnly, string comboboxName, List<string> items, string value)
        {
            Title = title;
            ReadOnly = readOnly;
            Value = value;
            Items = items;
            ComboboxName = comboboxName;
        }
    }
}