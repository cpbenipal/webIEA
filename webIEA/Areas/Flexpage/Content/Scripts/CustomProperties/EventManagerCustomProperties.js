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

class EventManagerCustomProperties extends EventManagerBase {
    constructor(args) {
        super();
        EventManager.bus.installTo(this);
    }
    
    static bind(args) {
        $(".fp_customProperties-btn__save").on("click", { args: args }, EventManagerCustomProperties.eventSave);
    }
    static unbind(args) {
        $(".fp_customProperties-btn__save").off("click", EventManagerCustomProperties.eventSave);
    }
    static eventSave(e) {
        var $form = $(e.target).closest("form");
        var ID = $($form).find("[name=ID]").val();
        var objectTypes = [];
        var checkboxs = $($form).find(".checkbox .check-box");
        for (var i = 0; i < checkboxs.length; i++) {
            var value = $(checkboxs[i]).is(":checked");
            if (value == true) {
                objectTypes.push(i+1);
            }
        }
        var args = {
            event: e, requestUrl: "Flexpage/CustomProperties_SaveObjectTypes", ID: ID, requestParameters: { ID: ID, ObjectTypes: objectTypes }, callbackSucceed: function (html, status, xhr, args, jsonObj) {
                publisherLog.Push(new DivLog(jsonObj.message));
            }
        };
        EventManager.bus.publish(EventManager.settings.Events.service.saveCustomProperties, args);
    }
};