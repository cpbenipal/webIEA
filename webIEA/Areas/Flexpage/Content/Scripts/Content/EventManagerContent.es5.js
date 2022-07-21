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

