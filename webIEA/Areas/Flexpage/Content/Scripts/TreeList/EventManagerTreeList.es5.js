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

var EventManagerTreeList = (function (_EventManagerBase) {
    _inherits(EventManagerTreeList, _EventManagerBase);

    function EventManagerTreeList(args) {
        _classCallCheck(this, EventManagerTreeList);

        _get(Object.getPrototypeOf(EventManagerTreeList.prototype), "constructor", this).call(this);
        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.clickNode, this.clickNode);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.showContextMenu, this.showContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.clickContextMenu, this.clickContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.renameApplyFolder, this.renameApplyFolder);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.expandedChangingFolder, this.expandedChangingFolder);
        EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.endCallback, this.endCallback);
        var that = this;
        if (args.allowUpload) {
            EventManager.bus.subscribe(EventManager.settings.Events.folderTreeList.initUpload, this.initUpload);
            EventManager.bus.publish(EventManager.settings.Events.folderTreeList.initUpload, args);
        }
        $(".fp_folderSaveAsContactsSelector.hidden-visbility").removeClass("hidden-visbility");
        $(".fp_folderSaveAsContactsSelector").parent().find(".fp_browser-empty").addClass("hidden");
    }

    /**
     *
     *
     * @param {object} args UI options
     */

    _createClass(EventManagerTreeList, [{
        key: "initUpload",
        value: function initUpload(args) {
            $("html").on("dragover", function (e) {
                e.preventDefault();
                e.stopPropagation();
            });

            $("html").on("drop", function (e) {
                e.preventDefault();
                e.stopPropagation();
            });

            // Drop
            $('.fp_folderTreeList .upload-area').on('drop', function (e) {
                e.stopPropagation();
                e.preventDefault();

                var files = e.originalEvent.dataTransfer.files;
                args.requestParameters = new FormData();

                for (var i = 0; i < files.length; i++) {
                    args.requestParameters.append('file' + i, files[i]);
                }
                for (var i = 0; i < files.length; i++) {
                    args.requestParameters.append('file' + i + "_dateModification", new Date(files[i].lastModifiedDate || files[i].lastModified || new Date()).toISOString());
                }
                args.$focusedNode = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode");
                args.dataId = $(e.target).parent().attr("data-id") || args.$focusedNode.attr("data-id");
                args.rowId = $(e.target).parent().attr("data-rowId") || args.$focusedNode.attr("data-rowid");

                args.typeBlock = 'FolderTreeList';
                args.classBlock = 'fp_folderTreeList';

                //args.requestParameters.append('extendedfolderID', EventManagerTreeList.getExtendedfolderID(args.dataId));
                args.requestParameters.append('folderId', args.rowId);

                args.$block = $(e.target).closest(".flexpage-blockWrapper").first();
                args.ID = args.$block.attr("id");
                args.ID = parseInt(args.ID.replace("b", ""));
                args.requestParameters.append('id', args.ID);
                var selectFolderName = $(e.target).parent().attr("data-path") || args.$focusedNode.attr("data-path");
                args.requestParameters.append("selectFolderName", selectFolderName);

                args.folderId = args.rowId;
                args.requestUrl = '/Flexpage/FolderTreeList_UploadFile';

                args.callbackSucceed = function (data, status, xhr, _args) {
                    $(".flexpage-block-container>.fp_folderContent").each(function (i, elem) {
                        args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                        args.ID = parseInt(args.ID.replace("b", ""));
                        args.classBlock = "fp_folderContent";
                        EventManager.tools.RefreshGrid(_args);
                    });
                };
                args.callbackError = function (html, status, xhr, _args, jsonObj) {
                    fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {
                        if (jsonObj.error.overwrite == true) {
                            _args.requestParameters.append('overwrite', true);
                            EventManager.bus.publish(EventManager.settings.Events.service.uploadFile, _args);
                        }
                    }, function () {
                        _args.callbackSucceed();
                    });
                };
                EventManager.bus.publish(EventManager.settings.Events.service.uploadFile, args);
            });
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "expandedChangingFolder",
        value: function expandedChangingFolder(args) {
            args.typeBlock = "FolderTreeList";
            args.classBlock = "fp_folderTreeList";
            args.requestUrl = "Flexpage/" + args.typeBlock + "_ExpandedChanging";
            args.$elem = $("#" + args.event.node.contentElementID).find(".fp_textTreeListNode");
            args.$block = args.$elem.closest("#b" + args.ID);
            if (!args.$elem.hasClass("fp_textTreeListNodeExpanded")) {
                EventManager.tools.showLoading(args);
                args.level = parseInt(args.$elem.attr("data-level"));
                args.rowId = args.$elem.attr("data-rowId");
                args = EventManagerTreeList.getExtendedfolderID(args);
                args.extendedfolderID += "," + args.rowId;
                args.requestParameters = {
                    "ID": args.ID,
                    "folderId": args.rowId,
                    "level": args.level,
                    "extendedfolderID": args.extendedfolderID
                };
                EventManager.bus.publish(EventManager.settings.Events.service.expandedChanging, args);
            }
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "endCallback",
        value: function endCallback(args) {}

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "clickNode",
        value: function clickNode(args) {
            args.path = $(args.e.htmlEvent.target).closest(".fp_textTreeListNode").attr("data-path") || $(args.e.htmlEvent.target).find(".fp_textTreeListNode").attr("data-path") || $(args.e.htmlEvent.target).attr("data-path");
            args.rowID = $(args.e.htmlEvent.target).closest(".fp_textTreeListNode").attr("data-rowID") || $(args.e.htmlEvent.target).find(".fp_textTreeListNode").attr("data-rowID") || $(args.e.htmlEvent.target).attr("data-rowID");
            args.name = $(args.e.htmlEvent.target).closest(".fp_textTreeListNode").attr("data-name") || $(args.e.htmlEvent.target).find(".fp_textTreeListNode").attr("data-name") || $(args.e.htmlEvent.target).attr("data-name");
            args.saveSelectedKey = $(args.e.htmlEvent.target).closest(".fp_textTreeListNode").attr("data-saveSelectedKey") || $(args.e.htmlEvent.target).find(".fp_textTreeListNode").attr("data-saveSelectedKey") || $(args.e.htmlEvent.target).attr("data-saveSelectedKey");
            EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.updateByPathFolder, {
                "typeBlock": "ContactsEnumeration",
                "name": args.name,
                "classBlock": "fp_contactsEnumeration",
                "path": args.path,
                "rowId": args.rowID,
                "initPerformCallback": false,
                "typeContextMenu": args.typeContextMenu
            });
            EventManager.bus.publish(EventManager.settings.Events.folderContent.updateByPathFolder, {
                "typeBlock": "FolderContent",
                "name": args.name,
                "classBlock": "fp_folderContent",
                "path": args.path,
                "rowId": args.rowID,
                "filterCustomProperties": args.filterCustomProperties,
                "filterExtension": args.filterExtension,
                "typeContextMenu": args.typeContextMenu
            });
            args.typeBlock = "FolderTreeList";
            args.classBlock = "fp_folderTreeList";
            args.requestUrl = "Flexpage/" + args.typeBlock + "_ClickNode";
            args.requestParameters = {
                ID: args.ID, "name": args.s.name, "saveSelectedKey": args.saveSelectedKey, "args": JSON.stringify({ "path": args.path, "rowId": args.rowID, "name": args.s.name })
            };
            args.callbackSucceed = function (data, status, xhr, _args) {};
            args.callbackError = function (html, status, xhr, _args, jsonObj) {};
            EventManager.bus.publish(EventManager.settings.Events.service.treelistClickNode, args);
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "showContextMenu",
        value: function showContextMenu(args) {
            var event = args.event;
            event.type = "click";
            EventManager.tools.applyRename(event);
            args.blockCss = "#fp_FolderTreeList_List" + args.ID;
            if ($(args.blockCss + " .fp_textTreeListNodeInput").length === 0) {
                args.$elemClick = $(args.event.htmlEvent.target).parent();
                args.$block = args.$elemClick.closest("#b" + args.ID);
                if (args.$block.find(".fp_freeze").length === 0) {
                    args.classRow = "fp_textTreeListNode";
                    args = EventManager.tools.showContextMenu_CheckAttrs(args);
                    if (args == null) return;
                    args.path = args.$elemClick.attr("data-path");
                    args.name = args.$elemClick.attr("data-name");
                    args.level = args.$elemClick.attr("data-level");
                    args.perm = parseInt(args.$elemClick.attr("data-perm"));
                    args["export"] = args.$elemClick.attr("data-export");
                    args.showContacts = args.$elemClick.attr("data-showContacts") == "True";
                    args.showNotifications = args.$elemClick.attr("data-showNotifications") == "True";
                    if (isNaN(args.perm)) args.perm = 0;

                    EventManager.tools.showContextMenu_CopyAttrs(args);
                    EventManager.tools.showContextMenu(args);
                    //var items = $('#PopupMenu' + args.ID).find(".dxm-item,.dxm-separator");
                    var popup;
                    popup = eval('PopupMenu' + args.ID);
                    for (var i = 0; i < popup.GetItemCount(); i++) {
                        var item = popup.GetItem(i);
                        if (args.typeContextMenu == "Browser" || args.typeContextMenu == "Block") {
                            switch (item.name) {
                                case 'Create':
                                    item.SetVisible(args.path === '\\' || (args.perm & 1) > 0);
                                    break;
                                case 'UploadFile':
                                    item.SetVisible((args.perm & 1) > 0);
                                    break;
                                case 'Paste':
                                    item.SetVisible((args.perm & 1) > 0);
                                    item.SetEnabled(EventManager.tools.get_cookie("PWBrowserCopyData") != null);
                                    break;
                                case 'PasteShortcut':
                                    item.SetVisible((args.perm & 1) > 0);
                                    var cookieString = EventManager.tools.get_cookie("PWBrowserCopyData");
                                    if (cookieString) {
                                        var cookieInfo = JSON.parse(cookieString);
                                        item.SetEnabled(cookieInfo != null && cookieInfo.action !== "Cut" && cookieInfo.type !== "folder");
                                    } else {
                                        item.SetEnabled(false);
                                    }
                                    break;
                                case 'AddContact':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 1) > 0 && args.showContacts);
                                    break;
                                case 'Rename':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 32) > 0);
                                    break;
                                case 'Delete':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 4) > 0);
                                    break;
                                case 'Copy':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 2) > 0);
                                    break;
                                case 'Cut':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 4) > 0 && (args.perm & 2) > 0);
                                    break;
                                case 'Properties':
                                    item.SetVisible((args.perm & 32) > 0 || args.isAdmin);
                                    break;
                                case 'ResetPublishingOverrides':
                                case 'ResetTimeToLeave':
                                    item.SetVisible(args.path !== '\\' && (args.perm & 32) > 0 && args.showNotifications);
                                    break;
                                case 'ExportContacts':
                                    item.SetVisible(args["export"] === 'True');
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
        }

        /**
          *
          *
          * @param {object} args UI options
          */
    }, {
        key: "clickContextMenu",
        value: function clickContextMenu(args) {
            args.typeBlock = "FolderTreeList";
            args.classBlock = "fp_folderTreeList";
            args.requestUrl = "Flexpage/" + args.typeBlock + "_" + args.event.item.name;

            args = EventManager.tools.clickContextMenu_GetAttrs(args);
            if (args == null) return;

            args = EventManagerTreeList.getExtendedfolderID(args);
            if (args.event.item.name === "Create" || args.event.item.name === "Rename") {
                args.extendedfolderID += args.rowId;
            }
            args = EventManagerTreeList.getLevel(args);
            args.folderID = parseInt(args.rowId);
            args.requestParameters = {
                "ID": args.ID, "extendedfolderID": args.extendedfolderID, "folderId": args.folderID,
                "level": args.level, "maxLevel": args.maxLevel
            };

            if (args.event.item.name === "Create") {
                eval('fp_FolderTreeList_List' + args.ID).StartEditNewNode(args.rowId);
                $(window.document).on("keydown", function (e) {
                    if (e.keyCode === 27) {
                        eval('fp_FolderTreeList_List' + args.ID).CancelEdit();
                        return;
                    }
                    if (e.keyCode === 13) eval('fp_FolderTreeList_List' + args.ID).UpdateEdit();
                });
                /*EventManager.tools.showLoading(args);
                args.callbackSucceed = function (data, status, xhr, _args) {
                    EventManager.tools.UpdateBlock(data, status, xhr, _args);
                    args = EventManagerTreeList.getRow(args);
                    args = EventManagerTreeList.focus(args);
                }
                EventManager.bus.publish(EventManager.settings.Events.service.createFolder, args);*/
            } else if (args.event.item.name === "AddNotification") {
                    var nameFolder = $(".fp_textTreeListNode[data-rowid=" + args.rowId + "] span").text();
                    fp_popupControlOpen({ command: 'notification', blocklistID: '0', blockType: 'FolderNotification', blockAlias: '', action: 'GetFolderNotifications', controller: 'Notifications', blockID: args.rowId, title: 'Folder Notifications ' + nameFolder }, function (save) {});
                } else if (args.event.item.name === "AddContact") {
                    args.alias = args.name;
                    args.command = "add";
                    EventManager.bus.publish(EventManager.settings.Events.contactsEnumeration.addContactToFolder, args);
                } else if (args.event.item.name === "Rename") {
                    eval('fp_FolderTreeList_List' + args.ID).StartEdit(args.rowId);
                    //$(window.document).on("keydown", EventManager.tools.applyRename);

                    $(window.document).on("keydown", function (e) {
                        if (e.keyCode === 27) {
                            eval('fp_FolderTreeList_List' + args.ID).CancelEdit();
                            return;
                        }
                        if (e.keyCode === 13) eval('fp_FolderTreeList_List' + args.ID).UpdateEdit();
                    });
                } else if (args.event.item.name === "Delete") {
                    fp_ConfirmDialog('Delete', 'Are you sure you want to delete folder?', function () {
                        var selectnode = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode").attr("data-path");
                        if (args.path == selectnode) {
                            $("[data-rowId='" + args.parentId + "']").click();
                        }
                        eval('fp_FolderTreeList_List' + args.ID).DeleteNode(args.rowId);
                    });

                    //EventManager.tools.showLoading(args);
                    //EventManager.bus.publish(EventManager.settings.Events.service.deleteFolder, args);
                } else if (args.event.item.name === "UploadFile") {
                        args = EventManagerTreeList.bindUploadFile(args);
                    } else if (args.event.item.name === "Properties") {
                        var nameFolder = $(".fp_textTreeListNode[data-rowid=" + args.rowId + "] span").text();
                        fp_popupControlOpen({ command: 'edit', blocklistID: '0', blockType: 'ObjectProperties', blockAlias: '', blockID: args.rowId, title: 'Properties: ' + nameFolder }, function (save) {
                            if (save) {
                                var fn = window["fp_refreshNode" + args.ID];
                                if (typeof fn === 'function') {
                                    fn();
                                }
                                $(".fp_browser .fp_contactsEnumeration").each(function (index, elem) {
                                    var _args = {};
                                    _args.typeBlock = "ContactsEnumeration";
                                    _args.classBlock = "fp_contactsEnumeration";
                                    _args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                                    _args.ID = parseInt(_args.ID.replace("b", ""));
                                    EventManager.tools.RefreshGrid(_args);
                                });
                                $(".fp_browser .fp_folderContent").each(function (index, elem) {
                                    var _args = {};
                                    _args.typeBlock = "FolderContent";
                                    _args.classBlock = "fp_folderContent";
                                    _args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                                    _args.ID = parseInt(_args.ID.replace("b", ""));
                                    EventManager.tools.RefreshGrid(_args);
                                });
                            }
                        });
                    } else if (args.event.item.name === "Refresh") {
                        var fn = window["fp_refreshNode" + args.ID];
                        if (typeof fn === 'function') {
                            fn();
                        }
                    } else if (args.event.item.name === "Copy") {
                        var folderIDToCopy = args.rowId;
                        var cookieInfo = { type: "folder", selectedIDs: folderIDToCopy, action: "DeepCopy" };
                        EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                    } else if (args.event.item.name === "Cut") {
                        var folderIDToCopy = args.rowId;
                        var cookieInfo = { type: "folder", selectedIDs: folderIDToCopy, action: "Cut" };
                        EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                    } else if (args.event.item.name === 'Paste') {
                        EventManagerTreeList.pasteItem(args, "DeepCopy");
                    } else if (args.event.item.name === 'PasteShortcut') {
                        EventManagerTreeList.pasteItem(args, "CopyShortcut");
                    } else if (args.event.item.name === 'ExportContacts') {
                        window.flexpage = window.flexpage || {};
                        window.flexpage.isSaveEven = false;
                        window.flexpage.onClose = function () {
                            $("#dialog-iframe").remove();
                        };

                        window.flexpage.Download = function (path) {
                            $("#dialog-iframe").remove();
                            location.href = path;
                        };

                        $('<iframe/>', {
                            id: 'dialog-iframe',
                            css: {
                                'position': 'fixed',
                                'top': 0,
                                'left': 0,
                                'width': '100%',
                                'height': '100%',
                                'border': 0,
                                'z-index': 12001
                            },
                            src: '/Export/ShowExportSettingsDialog?folderId=' + args.rowId
                        }).appendTo($('body'));
                    } else if (args.event.item.name === 'ResetPublishingOverrides') {
                        args.requestUrl = "Admin/DeletePublishingContactOverrides";
                        args.folderID = parseInt(args.rowId);
                        args.callbackSucceed = function (data, status, xhr, _args) {};
                        args.requestParameters = {
                            "ID": args.ID, "folderId": args.folderID
                        };
                        EventManager.bus.publish(EventManager.settings.Events.service.resetPublishingOverrides, args);
                    } else if (args.event.item.name === 'ResetTimeToLeave') {
                        args.requestUrl = "Admin/ObjectTimeToLeaveReset";
                        args.folderID = parseInt(args.rowId);
                        args.callbackSucceed = function (data, status, xhr, _args) {}, fp_ConfirmDialog("Reset time to leave", "Do you want to make a recursive reset for all child objects?", function () {
                            args.requestParameters = {
                                "ID": args.ID, "folderId": args.folderID, "recursive": true
                            };
                            EventManager.bus.publish(EventManager.settings.Events.service.resetTimeToLeave, args);
                        }, function () {
                            args.requestParameters = {
                                "ID": args.ID, "folderId": args.folderID, "recursive": false
                            };
                            EventManager.bus.publish(EventManager.settings.Events.service.resetTimeToLeave, args);
                        });
                    } else if (args.event.item.name === 'OpenLocalVersion') {
                        if (args.path) {
                            var folderPath = args.path.replace(/\\/g, '\/');
                            window.open("plurishell://show/" + folderPath);
                        }
                    }
            return false;
        }

        /**
          *
          *
          * @param {object} args UI options
          */
    }, {
        key: "renameApplyFolder",
        value: function renameApplyFolder(args) {
            if (args.event.type === "keydown" && (args.event.which == 13 || args.event.keyCode == 13) || args.event.type === "click" && $(args.event.target).hasClass("fp_textTreeListNodeInput") === false && $(args.event.target).closest(".dxmLite").length === 0) {
                $(".fp_textTreeListNodeInput").each(function (i, input) {
                    args.typeBlock = "FolderTreeList";
                    args.classBlock = "fp_folderTreeList";
                    args.requestUrl = "Flexpage/" + args.typeBlock + "_RenameApply";
                    args.$row = $(input).closest(".fp_textTreeListNode");
                    args.ID = parseInt(args.$row.attr("data-id"));
                    args.rowId = args.$row.attr("data-rowId");
                    args.$block = $(input).closest(".flexpage-blockWrapper#b" + args.ID);
                    if (args.$block.find(".fp_freeze").length === 0) {
                        args.newName = $(input).val();
                        if (args.newName === "") {
                            return;
                        }
                        EventManager.tools.showLoading(args);
                        args = EventManagerTreeList.getExtendedfolderID(args);
                        args = EventManagerTreeList.getLevel(args);
                        args.requestParameters = {
                            "ID": args.ID,
                            "folderId": args.rowId,
                            "newName": args.newName,
                            "extendedfolderID": args.extendedfolderID,
                            "level": args.level,
                            "maxLevel": args.maxLevel
                        };

                        args.callbackSucceed = function (data, status, xhr, _args) {
                            EventManager.tools.UpdateBlock(data, status, xhr, _args);
                            $(window.document).off("keydown", EventManager.tools.applyRename);
                        };
                        EventManager.bus.publish(EventManager.settings.Events.service.renameApplyFolder, args);
                    }
                });
            }
        }

        /**
         *
         *
         * @param {object} args UI options
         * @return {args} args UI options
         */
    }], [{
        key: "bindUploadFile",
        value: function bindUploadFile(args) {
            $('<input type="file" multiple>').on('change', function () {
                var formData = new FormData();

                formData.append('folderId', args.rowId);

                for (var i = 0; i < this.files.length; i++) {
                    formData.append('file' + i, this.files[i]);
                }
                for (var i = 0; i < this.files.length; i++) {
                    formData.append('file' + i + "_dateModification", new Date(this.files[i].lastModifiedDate || this.files[i].lastModified || new Date()).toISOString());
                }
                args.requestParameters = formData;
                if (fp_settings.debug == true) {
                    console.log(args.requestParameters.ID);
                }
                args.$block = $(".fp_textTreeListNode[data-rowid=" + args.rowId + "]").closest(".flexpage-blockWrapper").first();
                args.ID = args.$block.attr("id");
                args.ID = parseInt(args.ID.replace("b", ""));
                args.requestParameters.append('id', args.ID);
                args.requestParameters.append("selectFolderName", args.path);

                args.callbackSucceed = function (data, status, xhr, args) {
                    $(".flexpage-block-container>.fp_folderContent").each(function (i, elem) {
                        args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                        args.ID = parseInt(args.ID.replace("b", ""));

                        args.classBlock = "fp_folderContent";
                        EventManager.tools.RefreshGrid(args);
                    });
                };
                args.callbackError = function (html, status, xhr, _args, jsonObj) {
                    fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {
                        if (jsonObj.error.overwrite == true) {
                            _args.requestParameters.append('overwrite', true);
                            EventManager.bus.publish(EventManager.settings.Events.service.uploadFile, _args);
                        }
                    }, function () {
                        args.callbackSucceed();
                    });
                };
                EventManager.bus.publish(EventManager.settings.Events.service.uploadFile, args);
            }).click();
            return args;
        }
    }, {
        key: "pasteItem",
        value: function pasteItem(args, action) {
            var cookieString = EventManager.tools.get_cookie("PWBrowserCopyData");
            if (cookieString == null) {
                fp_WarningDialog("Error", "You need to copy a file or a folder first");
                return;
            }
            var cookieInfo = JSON.parse(cookieString);
            if (cookieInfo.action == "Cut") {
                EventManager.tools.delete_cookie("PWBrowserCopyData");
            }
            var formData = new FormData();
            formData.append('IDsToCopy', cookieInfo.selectedIDs);
            formData.append('folderId', args.rowId);
            formData.append('Type', cookieInfo.type);
            formData.append('Action', cookieInfo.action == "Cut" ? cookieInfo.action : action);

            var selectFolderName = args.path;

            if (!selectFolderName) return;
            formData.append("selectFolderName", selectFolderName);

            args.typeBlock = "FolderTreeList";
            args.classBlock = "fp_folderTreeList";
            args.requestUrl = "Flexpage/Browser_Paste";
            args.requestParameters = formData;

            EventManager.tools.showLoading(args);
            args.callbackSucceed = function (data, status, xhr, _args) {
                EventManager.tools.hideLoading(args);
                var fn = window["fp_refreshNode" + args.ID];
                if (typeof fn === 'function') {
                    fn();
                }
                var $container = $("#b" + args.ID + " ." + args.classBlock + "");
                $container.removeClass("fp_freeze");
                $(".flexpage-block-container>.fp_folderContent").each(function (i, elem) {
                    args.ID = $(elem).closest(".flexpage-blockWrapper").attr("id");
                    args.ID = parseInt(args.ID.replace("b", ""));

                    args.classBlock = "fp_folderContent";
                    EventManager.tools.RefreshGrid(args);
                });
            };
            EventManager.bus.publish(EventManager.settings.Events.service.paste, args);
        }

        /**
         *
         *
         * @param {object} args UI options
         * @return {args} args UI options
         */
    }, {
        key: "getRow",
        value: function getRow(args) {
            args.$elemClick = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
            args.$row = args.$elemClick.find(".fp_textTreeListNode[data-rowid=" + args.rowId + "]");
            return args;
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "renameName",
        value: function renameName(args) {
            args = EventManagerTreeList.getRow(args);
            $(args.$row).html("<input  class='fp_textTreeListNodeInput' value='" + args.$row.html() + "' />");
            EventManagerTreeList.focus(args);
        }

        /**
         *
         *
         * @param {object} args UI options
         * @return {args} args UI options
         */
    }, {
        key: "focus",
        value: function focus(args) {
            args.$input = $(args.$row).find(".fp_textTreeListNodeInput");
            if (args.$input.length === 0) {
                //find childs treelist
                args.$input = $(args.$row).closest("li").find(".fp_textTreeListNodeInput");
            }
            args = EventManager.tools.focus(args);
            return args;
        }

        /**
         *
         *
         * @param {object} args UI options
         * @return {args} args UI options
         */
    }, {
        key: "getExtendedfolderID",
        value: function getExtendedfolderID(args) {
            var $block = $(".flexpage-blockWrapper#b" + args.ID + " .fp_folderTreeList");
            args.extendedfolderID = "";
            args.maxLevel = 1;
            $block.find(".fp_textTreeListNode:visible").map(function (i, node) {
                if ($(node).closest("li").find("ul .fp_textTreeListNode:visible").length > 0) {
                    var id = $(node).attr("data-rowId");
                    var level = parseInt($(node).attr("data-level"));
                    id = parseInt(id);
                    args.extendedfolderID += id + ",";
                    if (level > args.maxLevel) {
                        args.maxLevel = level;
                    }
                }
            });
            return args;
        }

        /**
         *
         *
         * @param {object} args UI options
         * @return {args} args UI options
         */
    }, {
        key: "getLevel",
        value: function getLevel(args) {
            args = EventManagerTreeList.getRow(args);
            args.level = parseInt($(args.$row).attr("data-level"));
            return args;
        }
    }]);

    return EventManagerTreeList;
})(EventManagerBase);

;

