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
class EventManagerFAQEdit extends EventManagerBase {
    constructor(args) {
        super();
        var that = this;
        for (var key in EventManager.settings.Events.faqEdit) {
            var func = this[key];
            var value = EventManager.settings.Events.faqEdit[key];
            EventManager.bus.subscribe(value, func);
        }
        this.init(args);
    }
    init(args) {
        
    }
    changeLanguageQuestionEdit(args) {
        var form = $(`#question-edit`);
        args.requestUrl = "FAQBlock/UpdateFAQQuestionEdit";
        args.requestParameters = { "parameters": args.lang, "command": "changelanguage" };
        $.each($("#question-edit input, #question-edit select, #question-edit textarea"), function (i, field) {
            args.requestParameters[field.name] = field.value;
        });
        
        args.callbackSucceed = function (html, status, xhr, _args, jsonObj) {
            form.html(html);
        }
        
        EventManager.bus.publish(EventManager.settings.Events.service.faqQuestionEdit, args);
    }
};

