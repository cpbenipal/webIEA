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

class EventManagerBrowser extends EventManagerBase {
    constructor() {
        super();
        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.browser.updateByPathFolder, this.updateByPathFolder);
    }
    /**
     *
     *
     * @param {object} args UI options
     */
    updateByPathFolder(args) {
        let argsC = {
            "typeBlock": "ContactsEnumeration",
            "classBlock": "fp_contactsEnumeration",
            "updateByPathFolder": EventManager.settings.Events.contactsEnumeration.updateByPathFolder,
            "typeContextMenu": args.typeContextMenu,
            "update": args.s.update
        };
        let argsF = {
            "typeBlock": "FolderContent",
            "classBlock": "fp_folderContent",
            "updateByPathFolder": EventManager.settings.Events.folderContent.updateByPathFolder,
            "update": args.s.update,
            "filterCustomProperties": args.filterCustomProperties,
            "typeContextMenu": args.typeContextMenu,
            "filterExtension": args.filterExtension
        };
        argsC.$block = argsF.$block = $(args.s.mainElement).find(".flexpage-blockWrapper:visible");

        const updateByPathFolderPublish = function (args) {
            var $browser = $(`.fp_browser`);
            args.path = $browser.find(`[name=${args.typeBlock}SelectFolderName]`).val();
            if (args.path) {
                args.ID = args.$block.attr("id").replace("b", "");
                EventManager.bus.publish(args.updateByPathFolder, args);
            }
        }
        if (args.contacts) {
            updateByPathFolderPublish(argsC);
        }
       
        updateByPathFolderPublish(argsF);
    }
};
