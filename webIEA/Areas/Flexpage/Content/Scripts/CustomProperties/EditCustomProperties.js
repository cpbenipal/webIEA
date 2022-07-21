var StatusProperty = {
    New: 0,
    Removed: 1,
    Existing: 2
};

//if (typeof fp_onItemSelected === 'undefined') {
//    var fp_onItemSelected = function (key, name) {
        
//        var cbo = controls.GetByName('FileTextBox' + key);
//        cbo.SetText(o.name);
//        $("#FileIDHidden" + key).val(o.id);
//    };
//}

if (typeof fp_selectedfileId === 'undefined') {
    var fp_selectedFileId = null;
}

if (typeof fp_selectedFileName === 'undefined') {
    var fp_selectedFileName = null;
}

var ObjectProperties = function () {
    this.id = -1;
    this.value = "";
    this.status = StatusProperty.New;
    this.enabled = true;
}

var ObjectPropertiesRequest = function () {
    this.objectId = -1;
    this.properties = [];
}

var AddEditCustomPropertyService = function () {
    var elemId = '#EditProperties';
    var objectPropertiesRequest = new ObjectPropertiesRequest();

    function addId(id, status) {
     var prop = objectPropertiesRequest.properties;

        for (var i = 0; i < prop.length; i++) {
            if (prop[i].id == id) {
                if (prop[i].status == StatusProperty.Removed) {
                    prop[i].status = StatusProperty.New;
                }
                return;
            }
        }

        var property = new ObjectProperties();

        property.id = id;
        property.status = (status ? status : StatusProperty.New);

        objectPropertiesRequest.properties.push(property);
        setInputEditProperties();
    }

    function removeId(id) {
        var prop = objectPropertiesRequest.properties;

        for (var i = 0; i < prop.length; i++) {
            if (prop[i].id == id) {
                prop[i].status = StatusProperty.Removed;
                return;
            }
        }

        addId(id, StatusProperty.Removed);
    }

    function setInputEditProperties() {
        objectPropertiesRequest.objectId = objectPropertiesRequest.objectId > 0
            ? objectPropertiesRequest.objectId
            : $("#Properties_ObjectID").val();

        var prop = objectPropertiesRequest.properties;

        $(".EditInput").each(function () {
            var propertyId = $(this).data("id");
            var propertyValue;
            var enabled = true;
            try {
                enabled = eval("EnabledCheckBox" + propertyId).GetValue();
            } catch (e) {

            }
            if ($(this).children().first().attr("role") == "checkbox") {
                switch ($(this).find("input").val()) {
                    case "C":
                        propertyValue = "true";
                        break;
                    case "U":
                        propertyValue = "false";
                        break;
                }

            } else if ($(this).find("#html" + propertyId).length>0) {
                propertyValue = eval("html" + propertyId).GetHtml();
                propertyValue = encodeURI(propertyValue);
            } else if ($(this).length > 0 && $(this)[0].id == "tokenBox" + propertyId) {
                propertyValue = eval("tokenBox" + propertyId).GetValue();
            } else {
                propertyValue = $(this).find("input").val();
            }

            for (var i = 0; i < prop.length; i++) {
                if (prop[i].id == propertyId) {
                    prop[i].value = propertyValue;
                    return;
                }
            }

            if (!propertyId)
                return;

            var property = new ObjectProperties();

            property.id = propertyId;
            property.status = enabled == false ? StatusProperty.Removed : StatusProperty.Existing;
            property.value = propertyValue;
            property.enabled = enabled;
            objectPropertiesRequest.properties.push(property);
        });
        

        $(elemId).val(JSON.stringify(objectPropertiesRequest));
    }

    return {
        SetNewPropertyId: function (id) {
            if(id)
                addId(id, StatusProperty.New);
        },
        SetExistingPropertyId: function (id) {
            if(id)
                addId(id, StatusProperty.Existing);
        },
        SetDeletePropertyId: function(id) {
            if (id)
                removeId(id);
        },
        GetProperties: function () {
            setInputEditProperties();
            return $(elemId).val();
        },
        UpdateGrid: function() {
            $.post("/Flexpage/AddCustomProperty", {
                    propertiesRequest: AddEditCustomPropertyService.GetProperties()
                })
                .done(function(data) {
                    $('#properties .content-grid #BrowserCustomPropertiesGrid').html(data);
                });
        },
        GenerateProperties: function() {
            setInputEditProperties();
        }
    }

}();
var EditCustomProperty = function() {
    var that = this;
    var inputSelect = $("#IsSelect");
    var inputDefault = $("#IsDefault");
    var data = [];
    var currentKey=0;
    var selectId = 0;

    function insertIframe(propertyName, id, objectID, folderID, src) {
        $('<iframe/>',
            {
                id: 'dialog-iframe',
                css: {
                    'position': 'fixed',
                    'top': 0,
                    'left': 0,
                    'width': '100%',
                    'height': '100%',
                    'border': 0,
                    'z-index': 12001
                },
                src: src + '?typeProperty=' + propertyName + '&objectPropertyID=' + id + '&folderID=' + folderID + '&objectID=' + objectID
            }).appendTo($('body'));
    }

    function pushData(input, id) {
        var ids = [];
        
        if (input&&input.val()&&input.val().length > 0)
            ids = input.val().split(',');

        if (ids.length > 0) {
            for (var i = 0; i < ids.length; i++) {
                if (ids[i] == id)
                    return;
            }
        }
        
        ids.push(id);
        input.val(ids.join(','));
    }

    function removeData(input, id) {
        var ids = [];
        
        if (input && input.val()&&input.val().length > 0)
            ids = input.val().split(',');
        
        if (ids.length > 0) {
            for (var i = 0; i < ids.length; i++) {
                if (ids[i] == id) {
                    ids.splice(i, 1);
                    break;
                }
            }
        }

        input.val(ids.join(','));
    }

    function setDataToInputSelect(id) {
        inputSelect = $("#IsSelect");
        pushData(inputSelect, id);
    }

    function removeDataFromInputSelect(id) {
        inputSelect = $("#IsSelect");
        removeData(inputSelect, id);
    }

    function setDataToInputDefault(id) {
        inputDefault = $("#IsDefault");
        pushData(inputDefault, id);
    }

    function removeDataFromInputDefault(id) {
        inputDefault = $("#IsDefault");
        removeData(inputDefault, id);
    }

    function OnGetRowValues(values) {
        
        var newData = {
            Id: values[0],
            EmailNotification: values[1],
            Abbreviation: values[2],
        };
        
        currentKey = newData.Id;

        for (var i = 0; i < data.length; i++) {
            if (data[i].Id == newData.Id) {
                fillControl(data[i]);
                return;
            }
        }
        
        fillControl(newData);

        data.push(newData);
    }

    function addStyleToSelectRoe(visibleIndex) {
        $(".grid-select-row").removeClass("grid-select-row");
        $('#fp_webSite_DXDataRow' + visibleIndex).addClass("grid-select-row");
    }

    function fillControl(data, emailNotification) {
        $('#Abbreviation').val(data.Abbreviation);
        $('#EmailNotification').prop( "checked", false );
        $('#EmailNotification[value=' + data.EmailNotification + ']').prop( "checked", true );
    }

    function getInputSelectArrValue() {
        inputSelect = $("#IsSelect");
        return inputSelect && inputSelect.val()&&inputSelect.val().split(",")||"";
    }
    
    function getInputDefaultArrValue() {
        inputDefault = $("#IsDefault");
        return inputDefault&&inputDefault.val()&& inputDefault.val().split(",")||"";
    }

    function showContextMenu(args) {
        if (typeof args.pageY == "undefined") {
            args.pageY = args.event.htmlEvent.pageY;
        }
        if (typeof args.pageX == "undefined") {
            args.pageX = args.event.htmlEvent.pageX;
        }

        args.$contextMenu = $("#PopupMenuCustomProperties");
        PopupMenuCustomProperties.ShowAtPos(args.pageX,args.pageY);

        return args;
    }

    function disableItemDeletePopup() {
        disableEnableMenuItemCustomProperty("DeleteProperty", false);
    }

    function enableItemDeletePopup() {
        disableEnableMenuItemCustomProperty("DeleteProperty", true);
    }

    function disableItemEditPopup() {
        disableEnableMenuItemCustomProperty("EditProperty", false);
    }

    function enableItemEditPopup() {
        disableEnableMenuItemCustomProperty("EditProperty", true);
    }

    function disableEnableMenuItemCustomProperty(itemName, value) {
        if (typeof (PopupMenuCustomProperties) != "undefined") {
            var item = PopupMenuCustomProperties.GetItemByName(itemName);
            if (item) {
                item.SetEnabled(value);
            }
        }
    }

    function openPopupDialog(parameters) {
        fp_popupControlOpen({
            command: 'edit',
            blockType: 'CustomPropertyAdd',
            blockAlias: 'CustomPropertyAdd',
            blockID: '0',
            OneButtonText: 'OK',
            parameters: parameters
        }, function () {
            disableItemDeletePopup();
            disableItemEditPopup();
            AddEditCustomPropertyService.UpdateGrid();
        });
    }

    function getBaseParameters(ids) {

        for (var i = 0; i < ids.length; i++) {
            AddEditCustomPropertyService.SetExistingPropertyId(ids[i]);
        }

        return AddEditCustomPropertyService.GetProperties();
    }

    return {
        ShowAllAvailableProperties: function (obj, objectID) {
            var visibleEnabled = $(obj.target).is(':checked');

            var properties = [];
            $("#сontactDetailsCustomProperties" + objectID + "_DXMainTable [data-field='Title']").each(function (index, elem) {
                var title = $(elem);
                var propertyId = title.attr("data-idprop");
                var propertyValue = $("#сontactDetailsCustomProperties" + objectID + "_DXMainTable .EditInput[data-id=" + propertyId + "]");
                if ($(propertyValue).children().first().attr("role") == "checkbox") {
                    switch ($(propertyValue).find("input").val()) {
                        case "C":
                            propertyValue = "true";
                            break;
                        case "U":
                            propertyValue = "false";
                            break;
                    }

                } else if ($(propertyValue).find("#html" + propertyId).length > 0) {
                    propertyValue = eval("html" + propertyId).GetHtml();
                    propertyValue = encodeURI(propertyValue);
                } else {
                    propertyValue = $(propertyValue).find("input").val();
                }
                var enabled = $("#сontactDetailsCustomProperties" + objectID + "_DXMainTable [aria-checked]")[index];
                properties.push({ ID: propertyId, Title: $(title).text(), DisplayValue: propertyValue ? propertyValue : "", Enabled: enabled ? enabled.checked : true });
            });
            $.ajax({
                url: '/flexpage/ShowAllAvailableProperties',
                data: {
                    VisibleEnabled: visibleEnabled,
                    model: {
                        ObjectID: objectID,
                        properties: properties
                    }
                },
                method: 'POST',
                success: function (data) {
                    $("#BrowserCustomPropertiesGrid").html(data);
                },
                error: function () {
                }
            });
        },
        showDialog: function(propertyName,id,objectID) {
            window.flexpage = window.flexpage || {};
            window.flexpage.isSaveEven = false;
            window.flexpage.onClose = function () {
                $("#dialog-iframe").remove();
                if (window.flexpage.isSaveEven)
                    location.reload(false);
            };
            insertIframe(propertyName, id,null, objectID,'/flexpage/CustomProperty_ShowEditDialog');
        },
        showDialogHtml: function (propertyName, id, objectID) {
            window.flexpage = window.flexpage || {};
            window.flexpage.isSaveEven = false;
            window.flexpage.onClose = function () {
                $("#dialog-iframe").remove();
                if (window.flexpage.isSaveEven)
                    location.reload(false);
            };
            insertIframe(propertyName, id, objectID, null, '/flexpage/CustomProperty_ShowHtml');
            window.flexpage = window.flexpage || {};

            window.flexpage.fp_afterPreSaveBlock = function (res) {
                var name = "html" + id;
                eval(name).SetValue(res.Value);
            }
        },
        selectRow: function(s, visibleIndex, emailNotification, abbreviation, elementDefault, id) {
            addStyleToSelectRoe(visibleIndex)
            OnGetRowValues([id, emailNotification, abbreviation]);
            if (s.GetCheckState() === 'Checked') {
                setDataToInputSelect(id);
                inputDefault = $("#IsDefault");
                if (!inputDefault || !inputDefault.val()||inputDefault.val().length == 0) {
                    setDataToInputDefault(id);
                    elementDefault.SetChecked(true);
                }
            } else {
                if ($.inArray(id + "", getInputDefaultArrValue()) >= 0) {
                    removeDataFromInputDefault(id);
                    elementDefault.SetChecked(false);
                }
                removeDataFromInputSelect(id);
            }
        },
        defaultRow: function(s, visibleIndex, emailNotification, abbreviation, id) {
            addStyleToSelectRoe(visibleIndex)
            OnGetRowValues([id, emailNotification, abbreviation]);

            if (s.GetCheckState() === 'Checked') {
                inputDefault = $("#IsDefault");
                if (inputDefault && inputDefault.val()&&inputDefault.val().length > 0 || $.inArray(id+"", getInputSelectArrValue()) < 0 )
                    s.SetCheckState("Unchecked");
                else
                    setDataToInputDefault(id);
            } else {
                removeDataFromInputDefault(id);
            }

        },
        loadProperty: function(s, e) {
            addStyleToSelectRoe(e.visibleIndex)
            s.GetRowValues(e.visibleIndex, 'Id;EmailNotification;Abbreviation', OnGetRowValues);
        },
        OnChangeAbbreviation: function(value) {
            if (currentKey < 0)
                return;

            for (var i = 0; i < data.length; i++) {
                if (data[i].Id == currentKey) {
                    data[i].Abbreviation = value;
                    return;
                }
            }
        },
        OnChangeEmailNotification: function(value) {
            if (currentKey < 0)
                return;

            for (var i = 0; i < data.length; i++) {
                if (data[i].Id == currentKey) {
                    data[i].EmailNotification = value;
                    return;
                }
            }
        },
        GetData: function() {
            return JSON.stringify(data);
        },
        SetData: function (dataJson) {
            if (dataJson) {
                data = JSON.parse(dataJson);

                var emailNotification = ['None', 'Warn', 'Push'];

                for (var i = 0; i < data.length; i++) {
                    data[i].EmailNotification = emailNotification[data[i].EmailNotification];
                }
            }
        },
        popupMenuItemClick: function(s, e, grid) {
            var ids = [];
            var items = $(".property-values");
            
            items.each(function (i) {
                ids.push($(this).data("idprop"));
            });

            if (e.item.name == "AddProperty") {
                window.flexpage.SetNewPropertyId = AddEditCustomPropertyService.SetNewPropertyId;
                openPopupDialog(getBaseParameters(ids));
            }
            else if (e.item.name == "DeleteProperty") {
                fp_ConfirmDialog('Delete',
                    'Are you sure want to delete this?',
                    function() {
                        AddEditCustomPropertyService.SetDeletePropertyId(selectId);
                        disableItemDeletePopup();
                        disableItemEditPopup();
                        AddEditCustomPropertyService.UpdateGrid();
                    });
            }
        },
        contextMenu: function(s, e) {
            showContextMenu({ "pageY": e.pageY, "pageX": e.pageX, "event": e });
            return false;
        },
        OnSelectionChanged: function(s, e, id) {
            objectId = id;
            if (e.isSelected) {
                selectId = s.GetRowKey(e.visibleIndex);
                enableItemDeletePopup();
                enableItemEditPopup();
            }
        }
    }
}();