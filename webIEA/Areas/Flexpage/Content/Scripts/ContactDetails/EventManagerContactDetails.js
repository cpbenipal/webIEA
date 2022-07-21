//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
//and https://marketplace.visualstudio.com/items?itemName=MadsKristensen.BundlerMinifier
//and recompile the file in the menu (right click)
//Web:Debug true to disable minimization in AppSettings


//EventManager.bus - realizes subscriptions. Do not change
//EventManager.init - settings, initialization and base class
//EventManager.service - ajax requests
//EventManager.tools - common methods
//_bundleEventManager - bundle Do not write it directly, but only rebuild WebCompiler. Otherwise, the changes will be overwritten.


//in method parameters, do not use a dynamic set of parameters,
//but use args as an object to pass parameters to the method

//args.requestUrl - url for ajax request
//args.ID and args.classBlock is required
//args.requestParameters the arguments for the controller

//do not call methods directly, use subscription technology


var EventManager = EventManager || {};
class EventManagerContactDetails extends EventManagerBase {
    constructor() {
        super();
        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.contactDetails.tabClick, this.update);
        EventManager.bus.subscribe(EventManager.settings.Events.contactDetails.edit, this.update);
        EventManager.bus.subscribe(EventManager.settings.Events.contactDetails.save, this.save);
        EventManager.bus.subscribe(EventManager.settings.Events.contactDetails.cancel, this.update);
        var that = this;
    }
    /**
     *
     *
     * @param {object} args UI options
     */
    save(args) {
        EventManagerContactDetails.bind(args);
    }
    /**
     *
     *
     * @param {object} args UI options
     */
    update(args) {
        args.typeBlock = "ContactDetails";
        args.requestUrl = "ContactDetails/" + args.typeBlock+"_UpdateTab";
        args.classBlock = "fp_contactDetails";
        args= EventManagerContactDetails.getAttrs(args);
        
        args.requestParameters = {
            "blockID": args.ID, 
            "contactID": args.contactID,
            "selectTab": args.selectTab,
            "selectTabIndex": args.selectTabIndex ,
            "contactType": args.contactType,
            "edit": args.edit,
        };
        
        var html = args.$tab.html().trim();
        if (html.length == 0 || args.reload==true) {
            EventManager.tools.showLoading(args);
            args.callbackSucceed = function (data, status, xhr, _args) {
                EventManager.tools.UpdateTab(data, status, xhr, _args);
            }
            EventManager.bus.publish(EventManager.settings.Events.service.updateContactDetails, args);
        }
    }
    static toggleAddressCheckbox(s, customEditors) {
        var checked = s.GetChecked();
        this.toggleAddressFields(!checked, customEditors);
        this.toggleAddressImportField(checked);
    }

    static toggleAddressFields(value, customEditors) {
        var textFields = [StreetTextBox, ZipTextBox, CityTextBox, CountryComboBox, DescriptionTextBox];
        for (let i = 0; i < customEditors; i++) {
            textFields.push(eval("CustomPropertyEditor_" + i));
        }
        textFields.forEach(f => f.SetEnabled(value));
    }

    static toggleAddressImportField(value) {
        if (typeof LinkedAddressSource !== 'undefined') {
            LinkedAddressSource.SetEnabled(value);
        }
    }

    static setAddressValue(source) {
        var ID = LinkedAddressSource.GetValue();
        var value = JSON.parse(source.cpAllAddressViewsJson).filter(el => el.ID === ID)[0];
        StreetTextBox.SetValue(value.Street);
        ZipTextBox.SetValue(value.Zip);
        CityTextBox.SetValue(value.City);
        CountryComboBox.SetValue(value.CountryID);
        DescriptionTextBox.SetValue(value.Description);
        $("#SourceContactShortcutID").val(value.SourceContactShortcutID);
    }
    static getAttrs(args) {
        args.$block = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock + "");
        var $tab = args.event.tab;
        if (!$tab) {
            $tab = $(args.$block).find(".dxtc-activeTab:visible");
            args.selectTab = $tab.text().replace(" ", "");
            args.selectTabIndex = $tab.attr("id").replace("tabControl" + args.ID + "_AT", "");
            args.$tab = $tab;
        } else {
            args.selectTab = $tab.name;
            args.selectTabIndex = $tab.index;
            args.$tab = args.$block.find("#tabControl" + args.ID + "_C" + args.selectTabIndex + " div");
        }
        return args;
    }
    static bind(args) {
        $(".fp_contactDetails-btn__edit").on("click", { args: args }, EventManagerContactDetails.eventEdit);
        $(".fp_contactDetails-btn__save").on("click", { args: args }, EventManagerContactDetails.eventSave);
        $(".fp_contactDetails-btn__cancel").on("click", { args: args }, EventManagerContactDetails.eventCancel);
        $(".fp_contactDetails-btn__generatePassword").on("click", { args: args }, EventManagerContactDetails.eventGeneratePassword);
        $(".fp_contactDetails-btn__removeAuth").on("click", { args: args }, EventManagerContactDetails.eventRemoveAuth);
    }
    static unbind(args) {
        $(".fp_contactDetails-btn__edit").off("click", EventManagerContactDetails.eventEdit);
        $(".fp_contactDetails-btn__save").off("click",  EventManagerContactDetails.eventSave);
        $(".fp_contactDetails-btn__cancel").off("click", EventManagerContactDetails.eventCancel);
        $(".fp_contactDetails-btn__generatePassword").off("click", { args: args }, EventManagerContactDetails.eventGeneratePassword);
        $(".fp_contactDetails-btn__removeAuth").off("click", { args: args }, EventManagerContactDetails.eventRemoveAuth);
    }
    static eventEdit(e) {
       var args = e.data.args;
        EventManager.bus.publish(EventManager.settings.Events.contactDetails.edit,
            { "event": e, "ID": args.ID, "contactID": args.contactID, "contactType": args.contactType, "edit": true,"reload":true });
    }
    static eventSave(e) {
        var args = e.data.args;
        if ($("[name='Administration.Login']").length > 0) {
            $("[name='Administration.Login']").val(Login.GetValue());
            $("[name='Administration.Password']").val(Password.GetValue());
        }
            $(e.target).closest("form").submit();
        
    }
    static eventCancel(e) {
        var args = e.data.args;
        EventManager.bus.publish(EventManager.settings.Events.contactDetails.cancel,
            { "event": e, "ID": args.ID, "contactID": args.contactID, "contactType": args.contactType, "edit": false, "reload": true });
    }
    static eventGeneratePassword(e) {
        var args = e.data.args;
        args.typeBlock = "ContactDetails";
        args.requestUrl = "Admin/ContactDetails_GeneratePassword";
        args.classBlock = "fp_contactDetails";
        args = EventManagerContactDetails.getAttrs(args);
        args.callbackSucceed = function (data, status, xhr, _args) {
            $(args.$block).find("[name='Administration.Password']").val(data);
            $(args.$block).find("[name='Password']").attr("type", "");
            $(args.$block).find("[name='Password']").val(data);
        }
        EventManager.bus.publish(EventManager.settings.Events.service.saveContactDetails, args);
    }
    static eventRemoveAuth(e) {
        var args = e.data.args;
        var remove = confirm("Are you sure you want to delete login and password?");
        if (remove) {
            args = EventManagerContactDetails.getAttrs(args);
            $(args.$block).find("[name='Administration.Login']").val("");
            $(args.$block).find("[name='Administration.Password']").val("");
            $(args.$block).find("[name='Login']").val("");
            $(args.$block).find("[name='Password']").val("");
            $(args.$block).find("#formAdministration").submit();
        }
    }
};

