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

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EventManager = EventManager || {};

var EventManagerBrowserSearch = (function (_EventManagerBase) {
    _inherits(EventManagerBrowserSearch, _EventManagerBase);

    function EventManagerBrowserSearch(args) {
        _classCallCheck(this, EventManagerBrowserSearch);

        _get(Object.getPrototypeOf(EventManagerBrowserSearch.prototype), "constructor", this).call(this);
        EventManager.bus.subscribe(EventManager.settings.Events.browserSearch.showContextMenu, this.showContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.browserSearch.clickContextMenu, this.clickContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.browserSearch.downloadZip, this.downloadZip);
    }

    /**
     * show Context Menu for browserSearch 
     * change function in BrowserSearch/EventManagerBrowserSearch.js
     * read comments
     *
     * @param {object} args UI options
     */

    _createClass(EventManagerBrowserSearch, [{
        key: "showContextMenu",
        value: function showContextMenu(args) {
            //args.blockCss = "#fp_BrowserSearchResult_Grid" + args.tabIndex + "_DXMainTable";
            var event = args.event;
            event.type = "click";
            EventManager.tools.applyRename(event);

            if ($(args.blockCss + " .fp_gridNameValueInput").length === 0) {
                args.$elemClick = $(args.event.htmlEvent.target);
                args.blockCss = args.$elemClick.closest('table');
                args.$block = args.$elemClick.closest("#b" + args.ID);
                if (args.$block.find(".fp_freeze").length === 0) {
                    args.classRow = "fp_gridNameValue";

                    args.folderPerm = args.$block.find("#FolderPermissions").val();
                    var _args = EventManager.tools.showContextMenu_CheckAttrs(args);
                    if (_args == null) {} else {
                        args = _args;
                    }
                    args.path = args.$block.find("[name='SelectFolderName']").val() || args.$block.find("[name='PWFolderName']").val();
                    if (!args.path) {
                        args.path = '\\';
                    }
                    args.perm = parseInt(args.$elemClick.closest(".main_row").attr("data-perm"));
                    if (isNaN(args.perm)) args.perm = 0;

                    EventManager.tools.showContextMenu_CopyAttrs(args);

                    var popup = eval('PopupMenu' + args.ID);
                    args.$contextMenu = popup;
                    for (var i = 0; i < popup.GetItemCount(); i++) {
                        var item = popup.GetItem(i);
                        switch (item.name) {
                            case 'Delete':
                                item.SetVisible((args.perm & 4) > 0 && _args != null);
                                break;
                            case 'Copy':
                                item.SetVisible(_args != null && (args.perm & 2) > 0);
                                break;
                            case 'Cut':
                                item.SetVisible(_args != null && (args.perm & 4) > 0 && (args.perm & 2) > 0);
                                break;
                            case 'Download':
                                item.SetVisible(_args != null);
                                break;
                            case 'OpenInBrowser':
                                item.SetVisible(_args != null);
                                break;
                        }
                    }
                }

                EventManager.tools.showContextMenu(args);
            }
        }

        /**
         * click Context Menu for browserSearch
         * change function in BrowserSearch/EventManagerBrowserSearch.js
         * read comments
         *
         * @param {object} args UI options
         */
    }, {
        key: "clickContextMenu",
        value: function clickContextMenu(args) {
            args.block = EventManagerBrowserSearch.getActiveTab(args.ID);
            args.stopDownloadLink = true;
            args.requestUrl = "Browser/" + args.typeBlock + "_" + args.event.item.name;
            var _args = EventManager.tools.clickContextMenu_GetAttrs(args);
            if (_args == null) {} else {
                args = _args;
            }
            args.$elemClick = $("#" + args.block.uniqueID);
            //args.BlockID = args.ID;

            args.requestParameters = { "ID": args.ID, "fileId": args.rowId, "selectFolderName": args.selectFolderName };

            if (args.event.item.name === "Copy") {
                args.block.GetSelectedFieldValues("ID", function (values) {
                    if (!values || values.length === 0) {
                        values = [args.rowId];
                    }
                    var cookieInfo = { type: "files", selectedIDs: values.join(','), action: "DeepCopy" };
                    EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                });
            } else if (args.event.item.name === "Cut") {
                args.block.GetSelectedFieldValues("ID", function (values) {
                    if (!values || values.length === 0) {
                        values = [args.rowId];
                    }
                    var cookieInfo = { type: "files", selectedIDs: values.join(','), action: "Cut" };
                    EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                });
            } else if (args.event.item.name === "Download") {
                args.block.GetSelectedFieldValues("ID", function (values) {
                    if (!values || values.length === 0) {
                        values = [args.rowId];
                    } else if (values.length > 1) {
                        EventManager.bus.publish(EventManager.settings.Events.browserSearch.downloadZip, {
                            values: values
                        });
                        return;
                    }
                    window.location.href = '/Flexpage/DownloadFile?id=' + values + '&revisionID=0';
                });
            } else if (args.event.item.name === "Delete") {
                if (_args == null) return;
                args.requestUrl = 'Flexpage/FolderContent_Delete';
                args.block.GetSelectedFieldValues("ID", function (values) {
                    //todo: switch to find("[class*='dxgvSelectedRow']")
                    if (values.length > 0) {
                        fp_ConfirmDialog('Delete', 'Do you really want to delete the selected files?', function () {
                            var length = values.length;
                            var indexCallback = 0;
                            values.forEach(function (elem) {
                                //EventManager.tools.showLoading(args);
                                // todo
                                args.rowId = elem;
                                args.$row = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]").closest(".main_row");

                                if (fp_settings.debug == true) {
                                    console.log("rowId:" + args.rowId);
                                }
                                args.callbackSucceed = function (data, status, xhr, _args) {
                                    indexCallback++;
                                    if (fp_settings.debug == true) {
                                        console.log("callbackSucceed indexCallback:" + indexCallback);
                                    }
                                    //$(args.$row).remove();
                                    if (indexCallback == length) {
                                        if (fp_settings.debug == true) {
                                            console.log("indexCallback == length");
                                        }
                                        EventManagerBrowserSearch.getActiveTab(args.ID).Refresh();
                                    }
                                };
                                args.requestParameters = {
                                    "ID": args.ID,
                                    "fileId": args.rowId,
                                    "selectFolderName": args.selectFolderName
                                };
                                EventManager.bus.publish(EventManager.settings.Events.service.deleteFile, args);
                            });
                        });
                    } else {
                        fp_ConfirmDialog('Delete', 'Are you sure you want to delete file?', function () {
                            // EventManager.tools.showLoading(args);
                            args.$row = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]").closest(".main_row");

                            args.callbackSucceed = function (data, status, xhr, _args) {
                                var grid = EventManagerBrowserSearch.getActiveTab(args.ID);
                                grid.Refresh();
                                //$(args.$row).remove();
                                //   EventManager.tools.RefreshGrid(_args);
                                //   EventManager.tools.hideLoading(args);
                            };
                            EventManager.bus.publish(EventManager.settings.Events.service.deleteFile, args);
                        });
                    }
                });
            } else if (args.event.item.name === "OpenInBrowser") {
                args.path = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]").attr('data-path');
                $("div.fp_textTreeListNode").filter(function (index, element) {
                    return $(element).attr('data-path') == args.path;
                }).first().click();
                //var selector = "#fp_FolderTreeList_List2243_R-" + args.rowId;
                //$(function () {
                //    $(selector).first().click();
                //});
                //args.typeBlock = "FolderContent";         
                //args.name = args.$elemClick.closest("#fp_browser" + args.ID).find(".fp_folderTreeList").attr('data-name');
                eval("fp_browserSearchTabs" + args.ID + ".SetActiveTabIndex(0)");
                //EventManager.bus.publish(EventManager.settings.Events.folderContent.updateByPathFolder, args);          
            }
        }
    }, {
        key: "downloadZip",
        value: function downloadZip(values) {
            values = values.values;
            if (!values || values.length === 0) {
                fp_ConfirmDialog('Warning', 'Please select files to upload', function () {});
                return;
            }

            var selectedIDs = values.join(',');
            window.location.href = '/flexpage/FolderContentDownloadZip?ids=' + selectedIDs;
        }
    }], [{
        key: "getActiveTab",
        value: function getActiveTab(blockID) {
            var index = eval('fp_browserSearchTabs' + blockID + '.GetActiveTabIndex()');
            return eval('fp_BrowserSearchResult_Grid' + blockID + '_' + index);
        }
    }]);

    return EventManagerBrowserSearch;
})(EventManagerBase);

