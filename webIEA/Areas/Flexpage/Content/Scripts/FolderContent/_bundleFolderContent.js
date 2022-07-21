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

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get2 = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var EventManager = EventManager || {};

var EventManagerFolderContent = (function (_EventManagerContent) {
    _inherits(EventManagerFolderContent, _EventManagerContent);

    function EventManagerFolderContent(args) {
        _classCallCheck(this, EventManagerFolderContent);

        _get2(Object.getPrototypeOf(EventManagerFolderContent.prototype), "constructor", this).call(this);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.updateByPathFolder, this.updateByPathFolder);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.showContextMenu, this.showContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.clickContextMenu, this.clickContextMenu);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.renameApplyFile, this.renameApplyFile);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.uploadFile, this.uploadFile);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.detailToggleButton, this.detailToggleButton);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.downloadZip, this.downloadZip);
        EventManager.bus.subscribe(EventManager.settings.Events.folderContent.endCallback, this.endCallback);

        if (args.allowUpload) {
            EventManager.bus.subscribe(EventManager.settings.Events.folderContent.initUpload, this.initUpload);
            EventManager.bus.publish(EventManager.settings.Events.folderContent.initUpload, args);
        }

        setTimeout(function () {
            EventManager.bus.publish(EventManager.settings.Events.folderContent.endCallback, args);
        }, 4000);
    }

    _createClass(EventManagerFolderContent, [{
        key: "endCallback",
        value: function endCallback(args) {
            args.rowId = $("[class*='dxtlFocusedNode'] .fp_textTreeListNode").attr("data-rowid");
            args.publish = false;

            args = EventManager.tools.getBrowser(args);
            if (args.$browser.length > 0 && args.rowId) {
                var items = args.$browser.find(".flexpage-blockWrapper .fp_folderContent");
                args = _get(Object.getPrototypeOf(EventManagerFolderContent.prototype), "updateByPathFolder", this).call(this, args, items[0]);
            }
            if (args.refreshFiles) {
                EventManagerFolderContent.refreshContent(args);
            }
        }
    }, {
        key: "initUpload",

        /**
        *
        *
        * @param {object} args UI options
        */
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
            $('.fp_folderContent .upload-area').on('drop', function (e) {
                e.stopPropagation();
                e.preventDefault();

                var files = e.originalEvent.dataTransfer.files;
                args.requestParameters = new FormData();

                for (var i = 0; i < files.length; i++) {
                    args.requestParameters.append('file' + i, files[i]);
                }
                for (i = 0; i < files.length; i++) {
                    args.requestParameters.append('file' + i + "_dateModification", new Date(files[i].lastModifiedDate || files[i].lastModified || new Date()).toISOString());
                }
                args.typeBlock = 'FolderContent';
                args.classBlock = 'fp_folderContent';
                args.$block = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
                var SelectFolderName = $(args.$block).find("#SelectFolderName").val() || $(args.$block).find("#PWFolderName").val();
                if (!SelectFolderName) return;
                args.requestParameters.append('id', args.ID);
                args.dataID = args.ID;
                args.requestParameters.append('selectFolderName', SelectFolderName);

                //args.requestParameters.append('extendedfolderID', EventManagerTreeList.getExtendedfolderID(args.dataId));
                args.requestParameters.append('folderId', args.rowId);
                args.callbackSucceed = function (data, status, xhr, _args) {
                    EventManager.tools.RefreshGrid(_args);
                };
                args.folderId = args.rowId;

                args.requestUrl = '/Flexpage/FolderContent_UploadFile';

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
            });
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "updateByPathFolder",
        value: function updateByPathFolder(args) {
            args.requestUrl = "Flexpage/" + args.typeBlock + "_Update";
            args.classBlock = "fp_folderContent";
            var items = $(".flexpage-blockWrapper ." + args.classBlock);
            if (items.length == 0 && fp_settings.debug == true) {
                console.log("No " + args.classBlock + ". maybe error updateByPathFolder");
            }
            for (var i = 0; i < items.length; i++) {
                args = _get2(Object.getPrototypeOf(EventManagerFolderContent.prototype), "updateByPathFolder", this).call(this, args, items[i]);
                if (args.publish == true) {
                    EventManager.bus.publish(EventManager.settings.Events.service.updateByPathFolder, args);
                }
            }
        }

        /**
         * show Context Menu for folderContent 
         * change function in FolderContent/EventManagerFolderContent.js
         * read comments
         *
         * @param {object} args UI options
         */
    }, {
        key: "showContextMenu",
        value: function showContextMenu(args) {
            args.blockCss = "#fp_FolderContent_Grid" + args.ID + "_DXMainTable";
            var event = args.event;
            event.type = "click";
            EventManager.tools.applyRename(event);

            if ($(args.blockCss + " .fp_gridNameValueInput").length === 0) {
                args.$elemClick = $(args.event.htmlEvent.target);
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
                    for (var i = 0; i < popup.GetItemCount(); i++) {
                        var item = popup.GetItem(i);
                        if (args.typeContextMenu == "Browser" || args.typeContextMenu == "Block") {
                            switch (item.name) {
                                case 'Create':
                                    item.SetVisible((args.perm & 1) > 0 && _args != null);
                                    break;
                                case 'UploadFile':
                                    item.SetVisible((args.folderPerm & 1) > 0);
                                    break;
                                case 'AddContact':
                                    item.SetVisible((args.perm & 1) > 0 && _args != null);
                                    break;
                                case 'Rename':
                                    item.SetVisible((args.perm & 1) > 0 && _args != null);
                                    break;
                                case 'Delete':
                                    item.SetVisible((args.perm & 4) > 0 && _args != null);
                                    break;
                                case 'Properties':
                                    item.SetVisible(_args != null);
                                    break;
                                case 'VersionHistory':
                                    item.SetVisible(_args != null);
                                    break;
                                case 'Download':
                                    item.SetVisible(_args != null);
                                    break;
                                case 'Copy':
                                    item.SetVisible(_args != null && (args.perm & 2) > 0);
                                    break;
                                case 'Cut':
                                    item.SetVisible(_args != null && (args.perm & 4) > 0 && (args.perm & 2) > 0);
                                    break;
                                case 'Paste':
                                    item.SetVisible(args.path !== '\\' && (args.folderPerm & 1) > 0);
                                    //if (typeof get_cookie === "undefined") {
                                    //    var get_cookie = function () { };
                                    //}
                                    item.SetEnabled(EventManager.tools.get_cookie("PWBrowserCopyData") != null);
                                    break;
                                case 'PasteShortcut':
                                    item.SetVisible(args.path !== '\\' && (args.folderPerm & 1) > 0);
                                    //if (typeof get_cookie === "undefined") {
                                    //    var get_cookie = function () { };
                                    //}
                                    var cookieString = EventManager.tools.get_cookie("PWBrowserCopyData");
                                    if (cookieString) {
                                        var cookieInfo = JSON.parse(cookieString);
                                        item.SetEnabled(cookieInfo != null && cookieInfo.action !== "Cut" && cookieInfo.type !== "folder");
                                    } else {
                                        item.SetEnabled(false);
                                    }
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

                    EventManager.tools.showContextMenu(args);
                }
            }
        }

        /**
         * click Context Menu for folderContent 
         * change function in FolderContent/EventManagerFolderContent.js
         * read comments
         *
         * @param {object} args UI options
         */
    }, {
        key: "clickContextMenu",
        value: function clickContextMenu(args) {
            args.typeBlock = "FolderContent";
            args.classBlock = "fp_folderContent";
            args.stopDownloadLink = true;
            args.requestUrl = "Flexpage/" + args.typeBlock + "_" + args.event.item.name;
            var _args = EventManager.tools.clickContextMenu_GetAttrs(args);
            if (_args == null) {} else {
                args = _args;
            }
            args.$elemClick = args.$block;
            args.BlockID = args.ID;

            var PWFolderName = args.$elemClick.find("#PWFolderName").val();
            var SelectFolderName = args.$elemClick.find("#SelectFolderName").val();
            args.selectFolderName = SelectFolderName == '' ? PWFolderName : SelectFolderName;

            args.requestParameters = { "ID": args.ID, "fileId": args.rowId, "selectFolderName": args.selectFolderName };
            var $content = $($(args.$elemClick).closest(".fp_folderContent[data-alias]"));

            if (args.event.item.name === "Properties") {
                if (_args == null) {
                    return;
                }
                fp_popupControlOpen({ command: 'edit', blocklistID: '0', blockType: 'ObjectProperties', blockAlias: '', blockID: args.rowId, parameters: "file" }, function (save) {
                    if (save) {
                        EventManager.tools.RefreshGrid(args);
                    }
                });
            } else if (args.event.item.name === "Rename") {
                if (_args == null) return;
                EventManagerContent.renameName(args);

                var func = function func(e) {
                    if (e.keyCode === 13) {
                        EventManager.tools.showLoading(args);
                        EventManager.tools.applyRename(e);
                        $(window.document).off("keydown", func);
                    }
                    if (e.keyCode === 27) {
                        EventManager.tools.RefreshGrid(args);
                        $(window.document).off("keydown", func);
                        return;
                    }
                };
                $(window.document).on("keydown", func);
            } else if (args.event.item.name === "Delete") {
                if (_args == null) return;
                var $rows = $(args.$block).find("[class*='dxgvSelectedRow']");
                if (fp_settings.debug == true) {
                    console.log("$rows.length:" + $rows.length);
                }
                if ($rows.length > 0) {
                    fp_ConfirmDialog('Delete', 'Do you really want to delete the selected files?', function () {
                        var length = $rows.length;
                        var indexCallback = 0;
                        $rows.each(function (index, elem) {
                            EventManager.tools.showLoading(args);
                            args.$row = $(elem);
                            args.rowId = $(args.$row).find(".fp_gridNameValue").attr("data-rowId");
                            if (fp_settings.debug == true) {
                                console.log("rowId:" + args.rowId);
                            }
                            args.callbackSucceed = function (data, status, xhr, _args) {
                                indexCallback++;
                                if (fp_settings.debug == true) {
                                    console.log("callbackSucceed indexCallback:" + indexCallback);
                                }
                                if (indexCallback == length) {
                                    if (fp_settings.debug == true) {
                                        console.log("indexCallback == length");
                                    }
                                    EventManager.tools.RefreshGrid(args);
                                    EventManager.tools.hideLoading(args);
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
                        EventManager.tools.showLoading(args);
                        args.$row = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]").closest(".dxgvDataRow");

                        args.callbackSucceed = function (data, status, xhr, _args) {
                            EventManager.tools.RefreshGrid(_args);
                            EventManager.tools.hideLoading(args);
                        };
                        EventManager.bus.publish(EventManager.settings.Events.service.deleteFile, args);
                    });
                }
            } else if (args.event.item.name === "UploadFile") {
                //if (_args == null)
                //    return;
                $('<input type="file" multiple>').on('change', function () {
                    var formData = new FormData();

                    formData.append('id', args.requestParameters.ID);

                    for (var i = 0; i < this.files.length; i++) {
                        formData.append('file' + i, this.files[i]);
                    }
                    for (i = 0; i < this.files.length; i++) {
                        formData.append('file' + i + "_dateModification", new Date(this.files[i].lastModifiedDate || this.files[i].lastModified || new Date()).toISOString());
                    }
                    args.requestParameters = formData;

                    args.$block = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
                    var selectFolderName = $(args.$block).find("#SelectFolderName").val() || $(args.$block).find("#PWFolderName").val();
                    args.requestParameters.append("selectFolderName", selectFolderName);

                    args.callbackSucceed = function (data, status, xhr, _args) {
                        EventManager.tools.RefreshGrid(_args);
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
            } else if (args.event.item.name === "VersionHistory") {
                if (_args == null) return;
                var column = $("td[data-rowid=" + args.rowId + "] span");
                var fileName = column.text() + column.data("ext");
                fp_popupControlOpen({ command: 'edit', blocklistID: '0', blockType: 'FileHistory', blockAlias: '', blockID: args.rowId, title: 'Versions of ' + fileName, parameters: "file" });
            } else if (args.event.item.name === "Download") {
                if (_args == null) return;
                var $rows = $(args.$block).find("[class*='dxgvSelectedRow']");
                if ($rows.length > 1) {
                    EventManager.bus.publish(EventManager.settings.Events.folderContent.downloadZip, {
                        ID: args.ID
                    });
                } else {
                    window.location.href = '/Flexpage/DownloadFile?id=' + args.rowId + '&revisionID=0';
                }
            } else if (args.event.item.name === "Refresh") {
                EventManagerFolderContent.refreshContent(args);
            } else if (args.event.item.name === "Copy") {
                eval('fp_FolderContent_Grid' + args.ID).GetSelectedFieldValues("ID", function (values) {
                    if (!values || values.length === 0) {
                        values = [args.rowId];
                    }
                    var cookieInfo = { type: "files", selectedIDs: values.join(','), action: "DeepCopy" };
                    EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                });
            } else if (args.event.item.name === "Cut") {
                eval('fp_FolderContent_Grid' + args.ID).GetSelectedFieldValues("ID", function (values) {
                    if (!values || values.length === 0) {
                        values = [args.rowId];
                    }
                    var cookieInfo = { type: "files", selectedIDs: values.join(','), action: "Cut" };
                    EventManager.tools.set_cookie("PWBrowserCopyData", JSON.stringify(cookieInfo), 30 * 60);
                });
            } else if (args.event.item.name === 'Paste') {
                EventManagerFolderContent.pasteItem(args, "DeepCopy");
            } else if (args.event.item.name === 'PasteShortcut') {
                EventManagerFolderContent.pasteItem(args, "CopyShortcut");
            } else if (args.event.item.name === 'OpenLocalVersion') {
                if (args.path) {
                    var folderPath = args.path.replace(/\\/g, '\/');
                    var _fileName = $("span[data-blockid=" + args.ID + "][data-id=" + args.rowId + "]").html();
                    var fileExt = $("span[data-blockid=" + args.ID + "][data-id=" + args.rowId + "]").attr('data-ext');
                    window.open("plurishell://show/" + folderPath + "/" + _fileName + fileExt);
                }
            }
            return false;
        }

        /**
        * rename Apply File for folderContent and ContactsEnumeration
         * change function in Content/EventManagerFolderContent.js
         * read comments
         * @param {object} args UI options
         */
    }, {
        key: "renameApplyFile",
        value: function renameApplyFile(args) {
            args.typeBlock = "FolderContent";
            args.classBlock = "fp_folderContent";
            args.classRow = "fp_gridNameValue";
            args.stopDownloadLink = true;
            if (args.event.type === "keydown" && (args.event.which == 13 || args.event.keyCode == 13) || args.event.type === "click" && $(args.event.target).hasClass("fp_gridNameValueInput") === false && $(args.event.target).closest(".dxmLite").length === 0) {
                $(".fp_gridNameValueInput").each(function (i, input) {
                    args.requestUrl = "Flexpage/" + args.typeBlock + "_RenameApply";
                    args.$row = $(input).closest("." + args.classRow);
                    args.ID = parseInt(args.$row.attr("data-id"));
                    args.BlockID = parseInt(args.$row.attr("data-blockid"));
                    args.rowId = args.$row.attr("data-rowId");
                    args.newName = $(input).val();
                    if (args.newName === "") {
                        return;
                    } else {
                        if (args.newName[args.newName.length - 1] === '.') {
                            alert('Error in file name. The file name cannot end with "."');
                            return;
                        }
                    }

                    EventManager.tools.showLoading(args);
                    var $content = $($(args.$row).closest(".fp_folderContent[data-alias]"));
                    args.selectFolderName = $content.find("#SelectFolderName").val();
                    args.requestParameters = { "ID": args.ID, "fileId": args.rowId, "newName": args.newName, "selectFolderName": args.selectFolderName, "BlockID": args.BlockID };
                    args.callbackSucceed = function (data, status, xhr, _args) {
                        args.$row.html($(input).val());
                        _args.ID = args.BlockID;
                        EventManager.tools.RefreshGrid(_args);
                    };
                    args.callbackError = function (html, status, xhr, _args, jsonObj) {
                        _args.ID = args.BlockID;
                        EventManager.tools.RefreshGrid(_args);
                        EventManager.tools.CallbackError(html, status, xhr, args, jsonObj);
                    };

                    EventManager.bus.publish(EventManager.settings.Events.service.renameApplyFile, args);
                    event.stopPropagation();
                });
                return false;
            }
        }

        /**
         *
         *
         * @param {object} args UI options
         */
    }, {
        key: "uploadFile",
        value: function uploadFile(args) {
            args.typeBlock = "FolderContent";
            args.classBlock = "fp_folderContent";
            args.stopDownloadLink = true;
            _get2(Object.getPrototypeOf(EventManagerFolderContent.prototype), "uploadFile", this).call(this, args);
        }
    }, {
        key: "downloadZip",
        value: function downloadZip(args) {
            eval("fp_FolderContent_Grid" + args.ID).GetSelectedFieldValues('ID', function (values) {
                if (!values || values.length === 0) {
                    fp_ConfirmDialog('Warning', 'Please select files to upload', function () {});
                    return;
                }

                var selectedIDs = values.join(',');
                window.location.href = '/flexpage/FolderContentDownloadZip?ids=' + selectedIDs;
            });
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
            formData.append('Type', cookieInfo.type);
            formData.append('Action', cookieInfo.action == "Cut" ? cookieInfo.action : action);
            var name = $(".flexpage-blockWrapper#b" + args.ID).find(".fp_folderTreeList[data-name]").attr("data-name");
            if (fp_settings.debug == true) {
                console.log(name);
            }
            args.typeBlock = 'FolderContent';
            args.classBlock = 'fp_folderContent';
            args.$block = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
            var selectFolderName = $(args.$block).find("#SelectFolderName").val() || $(args.$block).find("#PWFolderName").val();
            if (!selectFolderName) return;
            formData.append("selectFolderName", selectFolderName);

            args.requestUrl = "Flexpage/Browser_Paste";
            args.requestParameters = formData;
            EventManager.tools.showLoading(args);
            args.callbackSucceed = function (data, status, xhr, _args) {
                EventManager.tools.hideLoading(args);
                EventManager.tools.RefreshGrid(args);
            };
            EventManager.bus.publish(EventManager.settings.Events.service.paste, args);
        }
    }]);

    return EventManagerFolderContent;
})(EventManagerContent);

;

