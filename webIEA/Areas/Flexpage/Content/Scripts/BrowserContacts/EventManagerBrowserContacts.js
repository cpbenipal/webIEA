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

class EventManagerBrowserContacts extends EventManagerBase {
    constructor() {
        super();
        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.browserContacts.endCallback, this.endCallback);
    }
    
    /**
     *
     *
     * @param {object} args UI options
     */
    endCallback(args) {
        var $input = $("#b" + args.ID + " [id*='fp_ContactsEnumeration_Grid" + args.ID + "_DXDataRow']");
                
        if ($input.length==1) {
            args = EventManagerBrowserContacts.selectContact(args);
        } 
        return args;
    }
    static selectContact(args) {
        var index = 0;
        var $row = $("#b" + args.ID + " [id*=fp_ContactsEnumeration_Grid" + args.ID + "_DXDataRow" + index + "]");
            $row.click();
              
    }
};
