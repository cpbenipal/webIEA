﻿//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
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

"use strict";

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EventManager = EventManager || {};

var EventManagerBrowser = (function (_EventManagerBase) {
    _inherits(EventManagerBrowser, _EventManagerBase);

    function EventManagerBrowser() {
        _classCallCheck(this, EventManagerBrowser);

        _get(Object.getPrototypeOf(EventManagerBrowser.prototype), "constructor", this).call(this);
        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.browser.updateByPathFolder, this.updateByPathFolder);
    }

    /**
     *
     *
     * @param {object} args UI options
     */

    _createClass(EventManagerBrowser, [{
        key: "updateByPathFolder",
        value: function updateByPathFolder(args) {
            var argsC = {
                "typeBlock": "ContactsEnumeration",
                "classBlock": "fp_contactsEnumeration",
                "updateByPathFolder": EventManager.settings.Events.contactsEnumeration.updateByPathFolder,
                "typeContextMenu": args.typeContextMenu,
                "update": args.s.update
            };
            var argsF = {
                "typeBlock": "FolderContent",
                "classBlock": "fp_folderContent",
                "updateByPathFolder": EventManager.settings.Events.folderContent.updateByPathFolder,
                "update": args.s.update,
                "filterCustomProperties": args.filterCustomProperties,
                "typeContextMenu": args.typeContextMenu,
                "filterExtension": args.filterExtension
            };
            argsC.$block = argsF.$block = $(args.s.mainElement).find(".flexpage-blockWrapper:visible");

            var updateByPathFolderPublish = function updateByPathFolderPublish(args) {
                var $browser = $(".fp_browser");
                args.path = $browser.find("[name=" + args.typeBlock + "SelectFolderName]").val();
                if (args.path) {
                    args.ID = args.$block.attr("id").replace("b", "");
                    EventManager.bus.publish(args.updateByPathFolder, args);
                }
            };
            if (args.contacts) {
                updateByPathFolderPublish(argsC);
            }

            updateByPathFolderPublish(argsF);
        }
    }]);

    return EventManagerBrowser;
})(EventManagerBase);

;

