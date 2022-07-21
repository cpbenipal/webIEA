using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.ComponentModel;

namespace Flexpage.Code.Helpers
{
    public class RolesCheckList : CheckBoxList
    {
        protected override void OnDataBound(EventArgs e)
        {
            foreach(ListItem item in Items)
            {
                item.Value = item.Value.ToLower();
            }

            if(_text != null)
            {
                this.ClearSelection();
                foreach(string itemValue in _text.Split(Separator))
                {
                    ListItem item = this.Items.FindByValue(itemValue);
                    if(item != null)
                    {
                        item.Selected = true;
                    }
                }
            }

            base.OnDataBound(e);
        }

        public char Separator
        {
            get
            {
                object o = ViewState["Separator"];
                if(o != null)
                    return (char)o;
                else
                    return ',';
            }
            set
            {
                ViewState["Separator"] = value;
                ViewState.SetItemDirty("Separator", true);
            }
        }

        private string _text = null;
        [Bindable(true, BindingDirection.TwoWay)]
        public override string Text
        {
            get
            {
                string text = "";
                foreach(ListItem item in this.Items)
                {
                    if(item.Selected)
                    {
                        text += item.Value + Separator;
                    }
                }
                return text.TrimEnd(Separator);
            }
            set
            {
                _text = value;
            }
        }
    }
}