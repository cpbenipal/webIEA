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

class EventManagerContent extends EventManagerBase {
    constructor() {
        super();
    }
    /**
     * update By Path Folder for folderContent and ContactsEnumeration
     * change function in Content/EventManagerContent.js 
     * read comments
     *
     * @param {object} args UI options
     */
    updateByPathFolder(args, item) {
        args = EventManager.tools.getBrowser(args);
        let $selectFolderName = args.$browser.find(`[name=${args.typeBlock}SelectFolderName]`);
        let $selectFolderNameLoad = args.$browser.find(`[name=${args.typeBlock}SelectFolderNameLoad]`);
        $selectFolderName.val(args.path);
        args.$block = $(item).closest(".flexpage-blockWrapper");
        const strId = args.$block.attr("id").replace("b", "");
        args.ID = parseInt(strId);
        args.theme = args.$block.find("[name=DevExpressTheme]").val();

        //if (args.$block.find(`[data-alias='${args.name}']:visible`).length > 0) {
        var loadPath = $selectFolderNameLoad.val();
        //get id

        if (loadPath != args.path || args.update == true || args.initPerformCallback==false) {

            // try to find block
            const grid = args.$block.find(`table[id$=_Grid${args.ID}]`);
            const pwbrowser = grid && grid.length > 0 ? eval(grid.attr('pwbrowser')) : false;

            EventManager.tools.showLoading(args);
            args.callbackSucceed = function (data, status, xhr, _args) {
                $selectFolderName = args.$browser.find(`[name=${args.typeBlock}SelectFolderName]`).val();
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
    detailToggleButton(args) {
        var row = $(args.event).attr("class")
        var $dataRow = $(args.event).closest("[class*=' dxgvDataRow'],[class^='dxgvDataRow']");

        var classSplit = $dataRow.attr("class").split(" ").find(s => s.search("dxgvDataRow") != -1).split("dxgvDataRow");
        var theme = classSplit[classSplit.length - 1];
        if ($(args.event).hasClass("allowOnlyOneDetailRow") === true
            && $(args.event).hasClass("dxgvDetailButtonExpanded") === false) {
            var $details = $(".dxgvDetailButtonExpanded");
            for (var i = 0; i < $details.length; i++) {
                ($($details[i]).click());
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
    rowClick(args) {

    }
    /**
     * get Row for folderContent and ContactsEnumeration
     * change function in Content/EventManagerContent.js 
     * read comments
     * @param {object} args UI options
     */
    static getRow(args) {
        args.$elemClick = $(".flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock);
        args.$row = args.$elemClick.find(".fp_gridNameValue[data-rowid=" + args.rowId + "]");
        return args;
    }
    /**
     * rename Name for folderContent and ContactsEnumeration
     * change function in Content/EventManagerContent.js read comments
     * @param {object} args UI options
     */
    static renameName(args) {
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
    static focus(args) {
        args.$input = $(args.$row).find(".fp_gridNameValueInput");
        EventManager.tools.focus(args);
        return args;
    }
};
