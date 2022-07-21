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
class EventManagerFAQ extends EventManagerBase {
    constructor(args) {
        super();
        var that = this;
        for (var key in EventManager.settings.Events.faq) {
            var func = this[key];
            var value = EventManager.settings.Events.faq[key];
            EventManager.bus.subscribe(value, func);
        }
        this.init(args);
    }
    init(args) {
        $(".fp_faq-question-header").on("click", function (e) {
            var $question = $(e.target).closest(".fp_faq-question");
            $question.toggleClass("fp_faq-question_open");
            $question.find(".fp_faq-question-header").toggleClass("fp_faq-question-header_open");
            $question.find(".fp_faq-question-answer").toggleClass("fp_hidden");
            $question.find(".fa-chevron-circle-down").toggleClass("fp_hidden");
            $question.find(".fa-chevron-circle-up").toggleClass("fp_hidden");
        });
    }
    filter(args) {
        $(`#fp_faq-block-${args.ID}-form`).submit();

    }
};

