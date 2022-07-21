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

"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EventManager = EventManager || {};

var EventManagerPendingEvents = (function (_EventManagerBase) {
    _inherits(EventManagerPendingEvents, _EventManagerBase);

    function EventManagerPendingEvents(args) {
        _classCallCheck(this, EventManagerPendingEvents);

        _get(Object.getPrototypeOf(EventManagerPendingEvents.prototype), "constructor", this).call(this);
        EventManager.bus.installTo(this);
        var that = this;
        for (var key in EventManager.settings.Events.pendingEvents) {
            var func = this[key];
            var value = EventManager.settings.Events.pendingEvents[key];
            EventManager.bus.subscribe(value, func);
        }

        var refreshEvery = 60000;
        var timerId = setInterval(function () {
            EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
        }, refreshEvery);
        setInterval(function () {
            var val = $("[name='RefreshEvery']input").val();
            val = parseInt(val) * 1000;
            if (val != refreshEvery) {
                refreshEvery = val;
                //console.log(refreshEvery);
                clearInterval(timerId);
                timerId = setInterval(function () {
                    EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
                }, refreshEvery);
            }
        }, 3000);
    }

    _createClass(EventManagerPendingEvents, [{
        key: "start",
        value: function start(args) {
            args.requestUrl = "Admin/PendingEvents_Start";
            args.requestParameters = {};
            args.callbackSucceed = function (data, status, xhr, _args) {
                $(".fp_pendingEvents-settings").html(data);
            };

            EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
        }
    }, {
        key: "stop",
        value: function stop(args) {
            args.requestUrl = "Admin/PendingEvents_Stop";
            args.requestParameters = {};
            args.callbackSucceed = function (data, status, xhr, _args) {
                $(".fp_pendingEvents-settings").html(data);
            };

            EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
        }
    }, {
        key: "setDelay",
        value: function setDelay(args) {
            args.requestUrl = "Admin/PendingEvents_SetDelay";
            var delay = $("#QueueDelay input").val();
            args.requestParameters = { delay: delay };
            args.callbackSucceed = function (data, status, xhr, _args) {
                $(".fp_pendingEvents-settings").html(data);
            };

            EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
        }
    }, {
        key: "refresh",
        value: function refresh(args) {
            fp_PendingEvents_File_Grid.Refresh();
            fp_PendingEvents_Notification_Grid.Refresh();
        }
    }, {
        key: "showContextMenu",
        value: function showContextMenu(args) {
            args.blockCss = "#fp_" + args.typeBlock + "_Grid_DXMainTable";
            var event = args.event;
            event.type = "click";

            args.$elemClick = $(args.event.htmlEvent.target);
            args = EventManager.tools.showContextMenu_CheckAttrs(args);
            if (args == null) return;
            args = EventManager.tools.showContextMenu_CopyAttrs(args);
            args = EventManager.tools.showContextMenu(args);
        }
    }, {
        key: "clickContextMenu",
        value: function clickContextMenu(args) {

            if (!args.event.item) return;
            args = EventManager.tools.clickContextMenu_GetAttrs(args);

            args.requestUrl = "Admin/" + args.typeBlock + args.event.item.name;
            args.requestParameters = { ID: args.rowId, contactID: args.contactID, address: args.address };
            args.callbackSucceed = function (data, status, xhr, _args) {
                EventManager.bus.publish(EventManager.settings.Events.pendingEvents.refresh, {});
            };
            if (args.event.item.name === "Delete") {
                EventManager.bus.publish(EventManager.settings.Events.service.setPendingEvents, args);
            }
        }
    }]);

    return EventManagerPendingEvents;
})(EventManagerBase);

;

