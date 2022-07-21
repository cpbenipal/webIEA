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

"use strict";

var _createClass = (function () {
    function defineProperties(target, props) {
        for (var i = 0; i < props.length; i++) {
            var descriptor = props[i];descriptor.enumerable = descriptor.enumerable || false;descriptor.configurable = true;if ("value" in descriptor) descriptor.writable = true;Object.defineProperty(target, descriptor.key, descriptor);
        }
    }return function (Constructor, protoProps, staticProps) {
        if (protoProps) defineProperties(Constructor.prototype, protoProps);if (staticProps) defineProperties(Constructor, staticProps);return Constructor;
    };
})();

var _get = function get(_x, _x2, _x3) {
    var _again = true;_function: while (_again) {
        var object = _x,
            property = _x2,
            receiver = _x3;_again = false;if (object === null) object = Function.prototype;var desc = Object.getOwnPropertyDescriptor(object, property);if (desc === undefined) {
            var parent = Object.getPrototypeOf(object);if (parent === null) {
                return undefined;
            } else {
                _x = parent;_x2 = property;_x3 = receiver;_again = true;desc = parent = undefined;continue _function;
            }
        } else if ("value" in desc) {
            return desc.value;
        } else {
            var getter = desc.get;if (getter === undefined) {
                return undefined;
            }return getter.call(receiver);
        }
    }
};

function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
        throw new TypeError("Cannot call a class as a function");
    }
}

function _inherits(subClass, superClass) {
    if (typeof superClass !== "function" && superClass !== null) {
        throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
    }subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } });if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
}

var EventManager = EventManager || {};

var EventManagerContent = (function (_EventManagerBase) {
    _inherits(EventManagerContent, _EventManagerBase);

    function EventManagerContent() {
        _classCallCheck(this, EventManagerContent);

        _get(Object.getPrototypeOf(EventManagerContent.prototype), "constructor", this).call(this);
    }

    /**
     * update By Path Folder for folderContent and ContactsEnumeration
     * change function in Content/EventManagerContent.js 
     * read comments
     *
     * @param {object} args UI options
     */

    _createClass(EventManagerContent, [{
        key: "updateByPathFolder",
        value: function updateByPathFolder(args, item) {
            args = EventManager.tools.getBrowser(args);
            var $selectFolderName = args.$browser.find("[name=" + args.typeBlock + "SelectFolderName]");
            var $selectFolderNameLoad = args.$browser.find("[name=" + args.typeBlock + "SelectFolderNameLoad]");
            $selectFolderName.val(args.path);
            args.$block = $(item).closest(".flexpage-blockWrapper");
            var strId = args.$block.attr("id").replace("b", "");
            args.ID = parseInt(strId);
            args.theme = args.$block.find("[name=DevExpressTheme]").val();

            //if (args.$block.find(`[data-alias='${args.name}']:visible`).length > 0) {
            var loadPath = $selectFolderNameLoad.val();
            //get id

            if (loadPath != args.path || args.update == true || args.initPerformCallback == false) {

                // try to find block
                var grid = args.$block.find("table[id$=_Grid" + args.ID + "]");
                var pwbrowser = grid && grid.length > 0 ? eval(grid.attr('pwbrowser')) : false;

                EventManager.tools.showLoading(args);
                args.callbackSucceed = function (data, status, xhr, _args) {
                    $selectFolderName = args.$browser.find("[name=" + args.typeBlock + "SelectFolderName]").val();
                    if ($selectFolderName == args.path) {
                        if (args.initPerformCallback != false) {
                            $selectFolderNameLoad.val(args.path);
                        }
                        EventManager.tools.UpdateBlock(data, status, xhr, args);
                        EventManager.bus.publish(EventManager.settings.Events.folderContent.initUpload, args);
                    }
                    EventManager.tools.hideLoading(args);
                };
                args.requestParameters = {
                    "ID": args.ID,
                    "selectFolderName": args.path,
                    "theme": args.theme,
                    "TypeContextMenu": args.typeContextMenu,
                    "PWBrowser": pwbrowser,
                    "initPerformCallback": args.initPerformCallback,
                    "filterCustomProperties": args.filterCustomProperties,
                    "filterExtension": args.filterExtension
                };
                args.publish = true;
                return args;
            }
            //}
            args.publish = false;
            return args;
        }

        /**
         * detail Toggle Button for folderContent and ContactsEnumeration
         * change function in Content/EventManagerContent.js 
         * read comments
         * @param {object} args UI options
         */
    }, {
        key: "detailToggleButton",
        value: function detailToggleButton(args) {
            var row = $(args.event).attr("class");
            var $dataRow = $(args.event).closest("[class*=' dxgvDataRow'],[class^='dxgvDataRow']");

            var classSplit = $dataRow.attr("class").split(" ").find(function (s) {
                return s.search("dxgvDataRow") != -1;
            }).split("dxgvDataRow");
            var theme = classSplit[classSplit.length - 1];
            if ($(args.event).hasClass("allowOnlyOneDetailRow") === true && $(args.event).hasClass("dxgvDetailButtonExpanded") === false) {
                var $details = $(".dxgvDetailButtonExpanded");
                for (var i = 0; i < $details.length; i++) {
                    $($details[i]).click();
                }
            }
            $(args.event).toggleClass('dxgvDetailButtonExpanded');

            var id = $dataRow.attr("id").replace("ata", "");
            if ($(args.event).hasClass("dxgvDetailButtonExpanded") === true) {
                $("#" + id).css({ "display": "table-row" });
            } else {
                $("#" + id).css({ "display": "none" });
            }
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "rowClick",
        value: function rowClick(args) {}

        /**
         * get Row for folderContent and ContactsEnumeration
         * change function in Content/EventManagerContent.js 
         * read comments
         * @param {object} args UI options
         */
    }], [{
        key: "getRow",
        value: function getRow(args) {
            args.$elemClick = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
            args.$row = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]");
            return args;
        }

        /**
         * rename Name for folderContent and ContactsEnumeration
         * change function in Content/EventManagerContent.js read comments
         * @param {object} args UI options
         */
    }, {
        key: "renameName",
        value: function renameName(args) {
            EventManager.tools.showLoading(args);
            args = EventManagerContent.getRow(args);
            $(args.$row[0]).html("<input  class='fp_gridNameValueInput' value='" + args.$row.html() + "' />");

            if (args.stopDownloadLink === true) {
                var $gvDownloadLink = $(args.$row).closest(".gvDownloadLink");
                if ($gvDownloadLink.length > 0) {
                    $gvDownloadLink = $gvDownloadLink.first();
                    $($gvDownloadLink).replaceWith($(args.$row));
                }
            }
            EventManagerContent.focus(args);
            EventManager.tools.hideLoading(args);
        }

        /**
         * focus for folderContent and ContactsEnumeration
         * change function in Content/EventManagerContent.js 
         * read comments
         *
         * @param {object} args UI options
         */
    }, {
        key: "focus",
        value: function focus(args) {
            args.$input = $(args.$row).find(".fp_gridNameValueInput");
            EventManager.tools.focus(args);
            return args;
        }
    }]);

    return EventManagerContent;
})(EventManagerBase);

;

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

"use strict";

var _createClass = (function () {
    function defineProperties(target, props) {
        for (var i = 0; i < props.length; i++) {
            var descriptor = props[i];descriptor.enumerable = descriptor.enumerable || false;descriptor.configurable = true;if ("value" in descriptor) descriptor.writable = true;Object.defineProperty(target, descriptor.key, descriptor);
        }
    }return function (Constructor, protoProps, staticProps) {
        if (protoProps) defineProperties(Constructor.prototype, protoProps);if (staticProps) defineProperties(Constructor, staticProps);return Constructor;
    };
})();

var _get2 = function get(_x, _x2, _x3) {
    var _again = true;_function: while (_again) {
        var object = _x,
            property = _x2,
            receiver = _x3;_again = false;if (object === null) object = Function.prototype;var desc = Object.getOwnPropertyDescriptor(object, property);if (desc === undefined) {
            var parent = Object.getPrototypeOf(object);if (parent === null) {
                return undefined;
            } else {
                _x = parent;_x2 = property;_x3 = receiver;_again = true;desc = parent = undefined;continue _function;
            }
        } else if ("value" in desc) {
            return desc.value;
        } else {
            var getter = desc.get;if (getter === undefined) {
                return undefined;
            }return getter.call(receiver);
        }
    }
};

function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
        throw new TypeError("Cannot call a class as a function");
    }
}

function _inherits(subClass, superClass) {
    if (typeof superClass !== "function" && superClass !== null) {
        throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
    }subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } });if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
}

var EventManager = EventManager || {};

var EventManagerContactsEnumeration = (function (_EventManagerContent) {
    _inherits(EventManagerContactsEnumeration, _EventManagerContent);

    function EventManagerContactsEnumeration(args) {
        _classCallCheck(this, EventManagerContactsEnumeration);

        _get2(Object.getPrototypeOf(EventManagerContactsEnumeration.prototype), "constructor", this).call(this);
        EventManager.bus.installTo(this);
        for (var key in EventManager.settings.Events.contactsEnumeration) {
            var func = this[key];
            var value = EventManager.settings.Events.contactsEnumeration[key];
            EventManager.bus.subscribe(value, func);
        }
        setTimeout(function () {
            EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.endCallback, args);
        }, 4000);
    }

    /**
     *
     *
     * @param {object} args UI options
     */

    _createClass(EventManagerContactsEnumeration, [{
        key: "endCallback",
        value: function endCallback(args) {
            args.rowId = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode").attr("data-rowid");
            args.publish = false;
            EventManagerContactsEnumeration.toogleContactTab(args);
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "updateByPathFolder",
        value: function updateByPathFolder(args) {
            args = EventManager.tools.getBrowser(args);
            args.requestUrl = "ContactsEnumeration/" + args.typeBlock + "_Update";
            args.classBlock = "fp_contactsEnumeration";
            var items = $(".flexpage-blockWrapper ." + args.classBlock);
            if (items.length == 0 && fp_settings.debug == true) {
                console.log("No " + args.classBlock + ". maybe error updateByPathFolder");
            }
            for (var i = 0; i < items.length; i++) {
                args = _get2(Object.getPrototypeOf(EventManagerContactsEnumeration.prototype), "updateByPathFolder", this).call(this, args, items[i]);
                if (!args.initPerformCallback) {
                    args = EventManagerContactsEnumeration.toogleContactTab(args);
                    if (args.publish == true) {
                        EventManager.bus.publish(EventManager.settings.Events.service.updateByPathFolder, args);
                    }
                }
            }
        }
    }, {
        key: "showContextMenu",

        /**
         * show Context Menu for  ContactsEnumeration
         * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
         * read comments
         *
         * @param {object} args UI options
         */
        value: function showContextMenu(args) {

            args.blockCss = "#fp_ContactsEnumeration_Grid" + args.ID + "_DXMainTable";
            var event = args.event;
            event.type = "click";
            EventManager.tools.applyRename(event);

            args.$elemClick = $(args.event.htmlEvent.target);
            args.$block = args.$elemClick.closest("#b" + args.ID);

            if (args.$block.find(".fp_freeze").length === 0) {
                args = EventManager.tools.getBrowser(args);
                args.classRow = "fp_contactValue";
                var _args = EventManager.tools.showContextMenu_CheckAttrs(args);
                if (_args == null) {} else {
                    args = _args;
                }
                args.path = args.$block.find("[name='SelectFolderName']").val() || args.$block.find("[name='PWFolderName']").val();
                if (!args.path) {
                    args.path = '\\';
                }
                args.$focusedNode = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode");
                args.showNotifications = args.$focusedNode.attr("data-showNotifications") == "True";
                args.showSelectionBoxes = fp_settings["showSelectionBoxes" + args.ID] == "True";
                args.perm = parseInt(args.$focusedNode.attr("data-perm"));
                args = EventManager.tools.showContextMenu_CopyAttrs(args);
                args = EventManager.tools.showContextMenu(args);

                var popup = eval('PopupMenu' + args.ID);
                for (var i = 0; i < popup.GetItemCount(); i++) {
                    var item = popup.GetItem(i);
                    if (args.typeContextMenu == "Block") {
                        switch (item.name) {
                            case 'Refresh':
                                break;
                            case 'Navigate':
                                item.SetVisible(_args != null);
                                break;
                            case 'AddPerson':
                            case 'AddCompany':
                                item.SetVisible(fp_settings.isContactsAdmin);
                                break;
                            case 'AddContact':
                                item.SetVisible(args.path !== '\\' && fp_settings.isContactsAdmin);
                                break;
                            case 'DeleteContact':
                                item.SetVisible(_args != null && args.path !== '\\' && fp_settings.isContactsAdmin);
                                break;
                            default:
                                item.SetVisible(false);
                                break;
                        }
                    } else if (args.typeContextMenu == "Contacts") {
                        switch (item.name) {
                            case 'Refresh':
                                break;
                            case 'Navigate':
                                item.SetVisible(_args != null);
                                break;
                            case 'AddPerson':
                            case 'AddCompany':
                                item.SetVisible(fp_settings.isContactsAdmin);
                                break;
                            case 'Delete':
                                item.SetVisible(_args != null && fp_settings.isContactsAdmin);
                                break;
                            default:
                                item.SetVisible(false);
                                break;
                        }
                    } else if (args.typeContextMenu == "Browser") {
                        switch (item.name) {
                            case 'Refresh':
                                break;
                            case 'Navigate':
                                item.SetVisible(_args != null);
                                break;
                            case 'AddContact':
                                item.SetVisible(args.path !== '\\' && (args.perm & 1) > 0);
                                break;
                            case 'DeleteContact':
                                item.SetVisible(_args != null && args.path !== '\\' && (args.perm & 1) > 0);
                                break;
                            case 'EmailOverriding':
                                item.SetVisible(_args != null && !args.showNotifications && !args.showSelectionBoxes && (args.perm & 32) > 0);
                                break;
                            case 'Notifications':
                                item.SetVisible(_args != null && args.showNotifications && !args.showSelectionBoxes && (args.perm & 32) > 0);
                                break;
                            case 'Properties':
                                item.SetVisible(_args != null);
                                break;
                            default:
                                item.SetVisible(false);
                                break;
                        }
                    } else {
                        switch (item.name) {
                            case 'Refresh':
                                break;
                            default:
                                item.SetVisible(false);
                                break;
                        }
                    }
                }
            }
        }

        /**
        * click Context Menu for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "clickContextMenu",
        value: function clickContextMenu(args) {
            args.typeBlock = "ContactsEnumeration";
            args.classBlock = "fp_contactsEnumeration";
            args.requestUrl = "ContactsEnumeration/" + args.typeBlock;
            if (!args.event.item) return;

            var _args = EventManager.tools.clickContextMenu_GetAttrs(args);
            if (_args == null) {} else {
                args = _args;
            }
            args.path = args.$block.find("[name='SelectFolderName']").val() || args.$block.find("[name='PWFolderName']").val();
            args.$focusedNode = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode");
            args.folderId = parseInt(args.$focusedNode.attr("data-rowid"));

            if (args.event.item.name === "Properties") {
                if (_args == null) {
                    return;
                }
                var column = $("div.fp_contactValue[data-rowid=" + args.rowId + "]")[0];
                var contactName = column.innerText;

                fp_popupControlOpen({ command: 'edit', blocklistID: '0', blockType: 'ObjectProperties', blockAlias: '', blockID: args.shortcutId, title: 'Properties: ' + contactName, parameters: "contact" }, function (save) {
                    if (save) {
                        EventManager.tools.RefreshGrid(args);
                    }
                });
            } else if (args.event.item.name === "AddContact") {
                args.command = "add";
                EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.addContactToFolder, args);
            } else if (args.event.item.name === "AddPerson") {
                if (_args == null) {
                    return;
                }
                EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.addPerson, args);
            } else if (args.event.item.name === "AddCompany") {
                if (_args == null) {
                    return;
                }
                EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.addCompany, args);
            } else if (args.event.item.name === "DeleteContact") {
                if (_args == null) {
                    return;
                }
                args.command = "remove";
                EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.deleteContactToFolder, args);
            } else if (args.event.item.name === "Delete") {
                if (_args == null) {
                    return;
                }
                args.command = "delete";
                EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.deleteContact, args);
            } else if (args.event.item.name === "Refresh") {
                EventManagerContactsEnumeration.refreshContent(args);
            } else if (args.event.item.name === "Navigate") {
                window.open('/Home/Contacts/' + args.rowId + '?type=' + (args.contactType == "1" ? "Person" : "Company"));
            } else if (args.event.item.name === "Notifications") {
                fp_popupControlOpen({ command: 'edit', blockType: 'Notifications', parameters: "{\"folderID\":" + args.folderId + ", \"ContactID\":" + args.rowId + ", \"ContactShortcutID\":" + args.shortcutId + "}", controller: 'Admin', action: 'Notifications' + (args.contactType == "1" ? "Person" : "Company") + 'Popup' });
            } else if (args.event.item.name === "EmailOverriding") {
                fp_popupControlOpen({ command: 'edit', blockType: 'EmailOverriding', parameters: "{\"folderID\":" + args.folderId + ", \"ContactID\":" + args.rowId + ", \"ContactShortcutID\":" + args.shortcutId + "}", controller: 'Admin', action: 'EmailOverriding' + (args.contactType == "1" ? "Person" : "Company") + 'Popup' });
            }
            return false;
        }

        /**
        * add Contact To Folder for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "addContactToFolder",
        value: function addContactToFolder(args) {
            args.typeBlock = "ContactsEnumeration";
            args.classBlock = "fp_contactsEnumeration";
            args.requestUrl = "ContactsEnumeration/UpdateContactsEnumerationFolder";
            fp_popupControlOpen({ command: 'choose', blockType: 'ContactsEnumerationSelector', alwaysCallOnClose: true, title: args.path, controller: "Flexpage" }, function () {});
            window.flexpage.fp_afterObjectSelected = function (objs) {
                args.requestParameters = [];
                $(objs).each(function (i, obj) {
                    if (obj.contactID) {
                        args.contactID = parseInt(obj.contactID);
                        args.callbackSucceed = function () {};
                        args.requestParameters.push({ "contactID": args.contactID, "path": args.path, "command": args.command });
                    }
                });
                args.requestParameters = { "parameters": JSON.stringify(args.requestParameters) };
                EventManager.bus.publish(EventManager.settings.Events.service.addContactToFolder, args);

                $(".fp_contactsEnumeration").each(function (i, elem) {
                    EventManager.tools.showLoading(args);
                    args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                    args.ID = parseInt(args.ID.replace("b", ""));
                    setTimeout(function () {
                        EventManager.tools.RefreshGrid(args);
                    }, 1000);
                });
            };
        }

        /**
        * add Contact To Folder for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "deleteContact",
        value: function deleteContact(args) {
            var $rows = $(args.$block).find("[class*='dxgvSelectedRow'],[class*='dxtlFocusedNode']");
            if (fp_settings.debug == true) {
                console.log("$rows.length:" + $rows.length);
            }
            args.typeBlock = "ContactsEnumeration";
            args.classBlock = "fp_contactsEnumeration";
            args.requestUrl = "ContactsEnumeration/UpdateContactsEnumerationFolder";
            fp_ConfirmDialog('Delete', "Do you really want to delete selected contacts? They can't be restored afterwards.", function () {
                if ($rows.length > 0) {
                    var length = $rows.length;
                    var indexCallback = 0;
                    $rows.each(function (index, elem) {
                        args.$row = $(elem);
                        args.rowId = $(elem).find(".fp_contactValue").attr("data-rowid");
                        args.contactID = parseInt(args.rowId);
                        args.contactType = $(elem).find(".fp_contactValue").attr("data-contactType");
                        args.requestParameters = {
                            "parameters": JSON.stringify([{ "contactID": args.contactID, "contactType": args.contactType, "command": args.command }])
                        };
                        args.callbackSucceed = function (html, status, xhr, _args, jsonObj) {
                            indexCallback++;
                            if (jsonObj.success) {
                                if (fp_settings.debug == true) {
                                    console.log("callbackSucceed indexCallback:" + indexCallback);
                                }
                                $(args.$row).remove();
                                if (indexCallback == length) {
                                    EventManager.tools.RefreshGrid(args);
                                    if (fp_settings.debug == true) {
                                        console.log("indexCallback == length");
                                    }
                                }
                            } else {
                                fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {
                                    if (indexCallback == length) {
                                        EventManager.tools.RefreshGrid(args);
                                        if (fp_settings.debug == true) {
                                            console.log("indexCallback == length");
                                        }
                                    }
                                }, function () {
                                    if (indexCallback == length) {
                                        EventManager.tools.RefreshGrid(args);
                                        if (fp_settings.debug == true) {
                                            console.log("indexCallback == length");
                                        }
                                    }
                                });
                            }
                        };
                        args.callbackError = function (html, status, xhr, _args, jsonObj) {
                            fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {}, function () {});
                        };
                        EventManager.bus.publish(EventManager.settings.Events.service.deleteContact, args);
                    });
                } else {
                    args.contactID = parseInt(args.rowId);
                    args.callbackSucceed = function (html, status, xhr, _args, jsonObj) {
                        if (jsonObj.success) {
                            EventManager.tools.RefreshGrid(args);
                        } else {
                            fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {}, function () {});
                        }
                    };
                    args.callbackError = function (html, status, xhr, _args, jsonObj) {
                        fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {}, function () {});
                    };
                    args.requestParameters = {
                        "parameters": JSON.stringify([{ "contactID": args.contactID, "contactType": args.contactType, "command": args.command }])
                    };
                    EventManager.bus.publish(EventManager.settings.Events.service.deleteContact, args);
                }
            });
        }

        /**
        * add Contact To Folder for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "deleteContactToFolder",
        value: function deleteContactToFolder(args) {
            var $rows = $(args.$block).find("[class*='dxgvSelectedRow']");
            if (fp_settings.debug == true) {
                console.log("$rows.length:" + $rows.length);
            }
            if ($rows.length > 0) {
                fp_ConfirmDialog('Delete', 'Do you really want to delete the selected contacts?', function () {
                    var length = $rows.length;
                    var indexCallback = 0;
                    $rows.each(function (index, elem) {
                        EventManager.tools.showLoading(args);
                        args.$row = $(elem);
                        args.rowId = $(elem).find(".fp_contactValue").attr("data-rowid");
                        if (fp_settings.debug == true) {
                            console.log("rowId:" + args.rowId);
                        }
                        args.typeBlock = "ContactsEnumeration";
                        args.classBlock = "fp_contactsEnumeration";
                        args.requestUrl = "ContactsEnumeration/UpdateContactsEnumerationFolder";
                        args.contactID = parseInt(args.rowId);
                        args.callbackSucceed = function () {
                            indexCallback++;
                            if (fp_settings.debug == true) {
                                console.log("callbackSucceed indexCallback:" + indexCallback);
                            }
                            $(args.$row).remove();
                            if (indexCallback == length) {
                                if (fp_settings.debug == true) {
                                    console.log("indexCallback == length");
                                }
                                EventManager.tools.RefreshGrid(args);
                                EventManager.tools.hideLoading(args);
                            }
                        };
                        args.requestParameters = { "parameters": JSON.stringify([{ "contactID": args.contactID, "contactType": args.contactType, "path": args.path, "command": args.command }]) };
                        EventManager.bus.publish(EventManager.settings.Events.service.deleteContactToFolder, args);
                    });
                });
            } else {
                args.typeBlock = "ContactsEnumeration";
                args.classBlock = "fp_contactsEnumeration";
                args.requestUrl = "ContactsEnumeration/UpdateContactsEnumerationFolder";
                EventManager.tools.showLoading(args);
                args.contactID = parseInt(args.rowId);
                args.callbackSucceed = function () {
                    EventManager.tools.RefreshGrid(args);
                    EventManager.tools.hideLoading(args);
                };
                args.requestParameters = {
                    "parameters": JSON.stringify([{ "contactID": args.contactID, "contactType": args.contactType, "path": args.path, "command": args.command }])
                };
                EventManager.bus.publish(EventManager.settings.Events.service.deleteContactToFolder, args);
            }
        }

        /**
        * addPerson for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "addPerson",
        value: function addPerson(args) {
            fp_popupControlOpen({ command: 'addContact', blocklistID: '0', blockType: 'ContactAdd', blockAlias: '', title: 'Person', controller: 'Flexpage', OneButtonText: 'ADD' }, function () {});
        }

        /**
        * addCompany for ContactsEnumeration
        * change function in ContactsEnumeration/EventManagerContactsEnumeration.js
        * read comments
        *
        * @param {object} args UI options
        */
    }, {
        key: "addCompany",
        value: function addCompany(args) {
            fp_popupControlOpen({ command: 'addContact', blocklistID: '0', blockType: 'ContactAdd', blockAlias: '', title: 'Company', controller: 'Flexpage', OneButtonText: 'ADD' }, function () {});
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "rowClick",
        value: function rowClick(args) {
            args.typeBlock = "ContactsEnumeration";
            args.requestUrl = "ContactsEnumeration/" + args.typeBlock + "_ContactDetails";
            args.classBlock = "fp_contactsEnumeration";
            _get2(Object.getPrototypeOf(EventManagerContactsEnumeration.prototype), "rowClick", this).call(this, args);
            var $block = $(".flexpage-blockWrapper#b" + args.ID);
            var $container = $block.find("." + args.classBlock);
            var name = $container.attr("data-name");
            args.contactID = null;
            args.contactType = null;
            var rowClick = args.event.htmlEvent ? true : false;
            if (rowClick) {
                if ($(args.event.htmlEvent.target).hasClass(".fp_contactValue")) {
                    args.$elem = $(args.event.htmlEvent.target);
                    args.contactID = $(args.$elem).attr("data-rowId");
                }
                if (!args.contactID) {
                    args.$elem = $(args.event.htmlEvent.target).find(".fp_contactValue");
                    args.contactID = $(args.$elem).attr("data-rowId");
                }
                if (!args.contactID) {
                    args.$elem = $(args.event.htmlEvent.target).closest(".fp_contactValue");
                    args.contactID = $(args.$elem).attr("data-rowId");
                }
            }
            if (!args.contactID) {
                args.$elem = $($block).find("[class*='dxgvSelectedRow'] .fp_contactValue");
                args.contactID = $(args.$elem).attr("data-rowId");
            }
            if (name) {
                args.contactType = $(args.$elem).attr("data-contactType");
                $(".fp_contactDetails[data-alias=" + name + "]").each(function (index, cd) {
                    var _args = {};
                    _args.typeBlock = "ContactDetails";
                    var emptyRequest = false;
                    if (args.contactID) {
                        _args.requestUrl = "ContactDetails/" + _args.typeBlock;
                    } else {
                        _args.requestUrl = "ContactDetails/" + _args.typeBlock + "Empty";
                        emptyRequest = true;
                    }
                    _args.classBlock = "fp_contactDetails";

                    _args.$block = $(cd).closest(".flexpage-blockWrapper");
                    _args.ID = _args.$block.attr("id");
                    if (_args.ID) {
                        _args.ID = parseInt(_args.ID.replace("b", ""));
                        _args.contactID = args.contactID;
                        _args.contactType = args.contactType;
                        var oldContactID = _args.$block.find("[data-contactID]").attr("data-contactID");
                        if (args.contactID && oldContactID && args.contactID == oldContactID && !rowClick) {
                            return;
                        }
                        var activeTab = _args.$block.find(".dxtc-activeTab:visible");
                        if (activeTab.length > 0) {
                            _args.selectTabIndex = activeTab.attr("id");
                            if (_args.selectTabIndex) {
                                _args.selectTabIndex = _args.selectTabIndex.replace("tabControl" + _args.ID + "_AT", "");
                                _args.selectTab = activeTab.text();
                                if (_args.selectTab) {
                                    _args.selectTab = _args.selectTab.replace(" ", "");
                                }
                            }
                        }
                        if (args.contactID) {
                            _args.requestParameters = {
                                "ID": _args.ID,
                                "selectTabIndex": _args.selectTabIndex,
                                "selectTab": _args.selectTab,
                                "contactID": args.contactID,
                                "contactType": args.contactType,
                                "edit": false
                            };
                        } else {
                            _args.requestParameters = {
                                "ID": _args.ID
                            };
                            if (activeTab.length == 0) {
                                return;
                            }
                        }

                        if (oldContactID != args.contactID) {
                            EventManager.tools.showLoading(_args);
                            _args.callbackSucceed = function (data, status, xhr, __args) {
                                EventManager.tools.UpdateBlock(data, status, xhr, _args);
                                _args.requestUrl = "ContactDetails/" + _args.typeBlock + "_UpdateTab";
                                EventManagerContactDetails.bind(_args);
                            };
                            EventManager.bus.publish(EventManager.settings.Events.service.updateContactDetails, _args);
                        }
                    }
                });
            }
        }
    }, {
        key: "updateGeneralInfo",
        value: function updateGeneralInfo(args) {
            if (args.edit == false) {
                args.$row = $(" .fieldName-PersonCompanyName .fp_contactValue[data-rowId=" + args.contactID + "][data-contactType=" + (args.contactType == "Person" ? 1 : 2) + "]");
                $(args.$row).text(args.name);
            }
            return args;
        }
    }, {
        key: "updateTelecoms",
        value: function updateTelecoms(args) {
            if ($(args.s.mainElement).find("[class*='dxgvEditForm']").length == 0) {
                args.$row = $(" .fieldName-PersonCompanyEmail .fp_contactValue[data-rowId=" + args.contactID + "][data-contactType=" + (args.contactType == "Person" ? 1 : 2) + "]");
                var $rows = $(args.s.mainElement).find("[class*='dxgvDataRow']");
                var rows = [];
                var rowsDefault = [];
                for (var i = 0; i < $rows.length; i++) {
                    var isDefault = $($rows[i]).find("[data-field='IsDefault']").attr("data-text");
                    var typeID = $($rows[i]).find("[data-field='TypeID']").text();
                    var value = $($rows[i]).find("[data-field='Value']").text();
                    if (isDefault == "Checked") {
                        if (typeID == "E-Mail" || typeID == "E-mail") {
                            rowsDefault.push(value);
                        }
                    } else {
                        if (typeID == "E-Mail" || typeID == "E-mail") {
                            rows.push(value);
                        }
                    }
                }
                var email = rowsDefault[0];
                if (email) {
                    $(args.$row).text(email);
                } else {
                    email = rows[0];
                    if (email) {
                        $(args.$row).text(email);
                    }
                }
            }
            return args;
        }
    }, {
        key: "updateCustomProperties",
        value: function updateCustomProperties(args) {
            //if ($(args.s.mainElement).find(".dxgvEditForm").length == 0) {
            //    args.$row = $("[class*='fieldName-CustomProperty .fp_contactValue[data-rowId=" + args.contactID + "]");
            //    for (var j = 0; j < args.$row.length; j++) {
            //        var $row = args.$row[j];
            //        var prop = $($row).closest("[class*='fieldName-CustomProperty']").attr("class").split(' ')[0];
            //        prop = prop.replace("fieldName-CustomProperty", "");
            //        var $rows = $(args.s.mainElement).find(".dxgvDataRow");
            //        var rows = [];
            //        for (var i = 0; i < $rows.length; i++) {
            //            var propertyID = $($rows[i]).find("[data-field='PropertyID']").text();
            //            var value = $($rows[i]).find("[data-field='Value']").text();
            //            if (propertyID == prop) {
            //                rows.push(value);
            //            }
            //        }
            //        var value = rows.join(", ");
            //        if (value) {
            //            $($row).text(value);
            //        }
            //    }
            //}
            //return args;
        }
    }, {
        key: "addNewContact",
        value: function addNewContact(args) {
            args.requestUrl = "ContactsEnumeration/AddContact";
            args.requestParameters = { id: "addContactForm", contact: args.contact };
            args.callbackSucceed = function (data, status, xhr, _args, res) {
                window.open('/Home/Contacts/' + res.id + '?type=' + res.type);
            };
            args.async = false;
            EventManager.bus.publish(EventManager.settings.Events.service.addNewContact, args);
        }
    }], [{
        key: "refreshContent",
        value: function refreshContent(args) {
            var fn = window["fp_refresh" + args.ID];
            if (typeof fn === 'function') {
                fn();
            }
        }
    }, {
        key: "toogleContactTab",
        value: function toogleContactTab(args) {
            args = EventManager.tools.getBrowser(args);
            if (args.$browser.length > 0 && args.rowId) {
                args.$row = $("[data-rowid=" + args.rowId + "]");
                var showContacts = args.$row.attr("data-showContacts") == "True";
                args = EventManager.tools.getBrowser(args);
                if (showContacts) {
                    args.$browser.find("#tabControl" + args.$browser.ID + "_T1").removeClass("hidden");
                } else {
                    args.$browser.find("#tabControl" + args.$browser.ID + "_T0").click();
                    args.$browser.find("#tabControl" + args.$browser.ID + "_T1").addClass("hidden");
                }
                args = EventManager.tools.getBrowser(args);
                var items = args.$browser.find(".flexpage-blockWrapper .fp_contactsEnumeration");
                args = _get(Object.getPrototypeOf(EventManagerContactsEnumeration.prototype), "updateByPathFolder", this).call(this, args, items[0]);
                var grid = eval('fp_ContactsEnumeration_Grid' + args.ID);
                if (fp_settings.debug == true) {
                    console.log("callback:" + grid.callback);
                }
                if (args.$browser.find("#tabControl" + args.$browser.ID + "_AT1:visible").length != 0 && !args.initPerformCallback && !grid.callback) {
                    if (args.publish == true) {
                        args.initPerformCallback = true;
                        EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.updateByPathFolder, args);
                    }
                    if (args.refreshContacts) {
                        EventManagerContactsEnumeration.refreshContent(args);
                    }
                }
            }
            return args;
        }
    }]);

    return EventManagerContactsEnumeration;
})(EventManagerContent);

;

