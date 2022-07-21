
var _cb_allowedProperties = [];
var _cb_allowedEnums = [];
var _cb_langCode = "";

function cb_Init(obj, editMode) {
	var dialog = $(obj.GetMainElement()).parents('.custom-property-edit-form');
	if(_cb_allowedProperties.length === 0) {		
		_cb_allowedProperties = eval(`[${dialog.find('#cb_allowedproperties').val()}]`);
		dialog.find('#cb_allowedproperties').detach();		
	}
    if (_cb_allowedEnums.length === 0) {
        var cb_allowedenums = dialog.find('#cb_allowedenums').val().replace(/\w"\w/gm, "\\\"").replace(/"{/gm, "{").replace(/}"/gm, "}");
        try {
			if (cb_allowedenums != "") {
				const parsedEnums = JSON.parse(cb_allowedenums);
				if (Array.isArray(parsedEnums))
					_cb_allowedEnums = parsedEnums;
				else
					_cb_allowedEnums = [cb_allowedenums];
			}
        } catch (e) {

        }

        dialog.find('#cb_allowedenums').detach();
    }
    
    if (editMode) {
        var $ID = dialog.find('input[type=hidden][name=ID]');
        var ID = $ID.val();
        cb_ShowEditor(dialog, ID);
	}
}

function cb_ShowEditor(dialog, propId) {
	dialog.find('.cb_editor').addClass("hidden");
	var prop = _cb_allowedProperties.filter(p => p.id == propId).pop();
	if(prop) {
		dialog.find('input[type=hidden][name=ID]').val(prop.id);
		dialog.find(`.cb_editor.${prop.type}`).removeClass("hidden");
    }
	if (prop && prop.type) {
		if (prop.type === 'Enum') {
			cb_Property_SetEnumValues(dialog, propId);
		}
		else if (prop.type === 'Tag') {
			cb_Property_SetTagValues(dialog, propId);
        }
	}
}

function cb_Property_Selected(s, e) {
    var dialog = $(s.GetMainElement()).parents('.custom-property-edit-form');
	cb_ShowEditor(dialog, s.GetValue());
}

function cb_Property_Read(s) {
	var dialog = $(s.GetMainElement()).parents('.custom-property-edit-form');
	var editor = dialog.find('.cb_editor:visible');
	var value = dialog.find('input[type=hidden][name=Value]');
	value.val('');
	if(editor.hasClass('String')) {
		value.val(editor.find('input').val());
	}
	if(editor.hasClass('DateTime')) {
		var date = CB_Editor_Date.GetValue();
		// format 2008-01-27T05:40:40
		value.val(date.toISOString());
	}
	if(editor.hasClass('Enum')) {
		value.val(CB_Editor_Enum.GetValue());
    }
    if (editor.hasClass('Float')) {
        value.val(CB_Editor_Float.GetValue());
    }
    if (editor.hasClass('Bool')) {
        value.val(CB_Editor_Bool.GetValue());
    }
    if (editor.hasClass('Html')) {
        value.val(CB_Editor_Html.GetHtml());
	}
	if (editor.hasClass('Tag')) {
		value.val(CB_Editor_Tag.GetText());
	}
	if (editor.hasClass('File')) {
		value.val(CB_Editor_File.GetText());
    }
}

function cb_Property_SetDateEditorValue(s, dateStr) {
	if(dateStr) {
		var date = new Date(dateStr);
		if(!isNaN(date)) {
			s.SetValue(date);
		}
	}
}

function cb_Property_SetEnumValues(dialog,propID) {
    var enumV = _cb_allowedEnums.find(e => e.id == propID);
	CB_Editor_Enum.ClearItems();
    if (enumV) {	
        enumV.values.forEach(v => {	
            if (v.value.Localizations) {
                _cb_langCode = dialog.find('#cb_langCode').val();
                if ((v.value.Localizations[_cb_langCode] || v.value.Localizations["en"])) {
                    v.value = v.value.Localizations[_cb_langCode] || v.value.Localizations["en"];
                } else {
                    v.value = "(none)";
                }
            }
			CB_Editor_Enum.AddItem(v.value, v.id);
		});		
	}
}

function cb_Property_SetEnumValue(val) {
	if(val !== undefined) {
		CB_Editor_Enum.SetValue(val);
	}
}

function cb_Property_SetTagValues(dialog, propID) {
	var enumV = _cb_allowedEnums.find(e => e.id == propID);
	CB_Editor_Tag.ClearItems();
	if (enumV) {
		enumV.values.forEach(v => {
			CB_Editor_Tag.AddItem(v.value);
		});
	}
}

function cb_Property_SetTagValue(val) {
	if (val !== undefined) {
		CB_Editor_Tag.SetText(val);
	}
}

function fp_OpenFileSelectPopup() {
	fp_popupControlOpen({ command: 'browserchoose', blockType: 'BrowserSelector', action: 'BrowserSelectorNew', controller: 'FlexPage', alwaysCallOnClose: true, parameters: '{}'}, function () { });
}

window.flexpage = window.flexpage || {};
window.flexpage.fp_afterObjectSelected = function (obj) {
	if (obj && obj[0]) {
		$('input[name=CB_Editor_FileView]').val(fp_selectedFolderName + obj[0].name + obj[0].ext);
		
		$('input[name=CB_Editor_File]').val(obj[0].id);
	}
}