//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
//and https://marketplace.visualstudio.com/items?itemName=MadsKristensen.BundlerMinifier
//and recompile the file in the menu (right click)
//Web:Debug false to disable console.log
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
class EventManagerPendingEvents extends EventManagerBase {
    constructor(args) {
        super();
        EventManager.bus.installTo(this);
        var that = this;
        for (var key in EventManager.settings.Events.pendingEvents) {
            var func = this[key];
            var value = EventManager.settings.Events.pendingEvents[key];
            EventManager.bus.subscribe(value, func);
        }
        
        var refreshEvery = 60000;
        var timerId = setInterval(() => {
            EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
        }, refreshEvery);
        setInterval(() => {
            var val = $("[name='RefreshEvery']input").val();
            val = parseInt(val) * 1000;
            if (val != refreshEvery) {
                refreshEvery = val;
                //console.log(refreshEvery);
                clearInterval(timerId);
                timerId = setInterval(() => {
                    EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
                }, refreshEvery);
            }
        }, 3000);
    }
    start(args) {
        args.requestUrl = "Admin/PendingEvents_Start";
        args.requestParameters = { };
        args.callbackSucceed = function (data, status, xhr, _args) {
            $(".fp_pendingEvents-settings").html(data);
        }
        
        EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
    }
    stop(args) {
        args.requestUrl = "Admin/PendingEvents_Stop";
        args.requestParameters = {  };
        args.callbackSucceed = function (data, status, xhr, _args) {
            $(".fp_pendingEvents-settings").html(data);
        }
        
        EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
    }
    setDelay(args) {
        args.requestUrl = "Admin/PendingEvents_SetDelay";
        var delay = $("#QueueDelay input").val();
        args.requestParameters = { delay: delay};
        args.callbackSucceed = function (data, status, xhr, _args) {
            $(".fp_pendingEvents-settings").html(data);
        }

        EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
    }
    refresh(args) {
        fp_PendingEvents_File_Grid.Refresh();
        fp_PendingEvents_Notification_Grid.Refresh();
    }
    showContextMenu(args) {
        args.blockCss = "#fp_" + args.typeBlock+"_Grid_DXMainTable";
        var event = args.event;
        event.type = "click";

        args.$elemClick = $(args.event.htmlEvent.target);
        args = EventManager.tools.showContextMenu_CheckAttrs(args);
        if (args == null)
            return;
        args = EventManager.tools.showContextMenu_CopyAttrs(args);
        args = EventManager.tools.showContextMenu(args);
    }
    clickContextMenu(args) {
        
        if (!args.event.item)
            return;
        args = EventManager.tools.clickContextMenu_GetAttrs(args);

        args.requestUrl = `Admin/${args.typeBlock}` + args.event.item.name;
        args.requestParameters = { ID: args.rowId, contactID: args.contactID, address: args.address};
        args.callbackSucceed = function (data, status, xhr, _args) {
            EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
        }
        if (args.event.item.name === "Delete") {
            EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
        } 
    }
};

