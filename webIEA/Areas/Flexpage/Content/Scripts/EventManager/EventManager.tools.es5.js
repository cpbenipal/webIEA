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

var EventManager = EventManager || {};
EventManager.tools = {};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.CallbackFailed = function (data, status, xhr, args) {
    EventManager.tools.hideLoading(args);
    EventManager.tools.FailedMessage("error ID = " + args.ID + " error : " + data.statusText);
    return null;
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.CallbackFailedDialogFailed = function (data, status, xhr, args) {
    EventManager.tools.hideLoading(args);
    fp_ConfirmDialog('Error', 'An error has occurred, contact your system administrator.', function () {});
    EventManager.tools.FailedMessage("error ID = " + args.ID + " error : " + data.statusText);
    return null;
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.FailedMessage = function (message, obj) {
    if (fp_settings.debug == true) {
        console.log(message);
        console.log(obj);
    }
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.CallbackError = function (html, status, xhr, args, jsonObj, onconfirm, oncancel) {
    fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, onconfirm ? onconfirm : function () {}, oncancel ? oncancel : function () {});
    if (fp_settings.debug == true) {
        console.log(jsonObj.error.title);
        console.log(jsonObj.error.message);
    }
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.CallbackSuccess = function (html, status, xhr, args, jsonObj) {
    fp_ConfirmDialog(jsonObj.title, jsonObj.message, function () {});
};
/**
 * updates block after request
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.UpdateBlock = function (data, status, xhr, args) {
    var $container = $("#b" + args.ID + " ." + args.classBlock + "");
    $container.removeClass("fp_freeze");
    if ($container.length > 0) {
        $($container.first()).html(data);
        if (fp_settings.debug == true) {
            console.log("UpdateBlock " + args.ID);
        }
    } else {
        EventManager.tools.FailedMessage("no update " + args.classBlock);
    }
    EventManager.tools.hideLoading(args);
};
/**
 * Refresh Grid after request
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} args UI options
 */
EventManager.tools.RefreshGrid = function (args) {
    var $container = $("#b" + args.ID + " ." + args.classBlock + "");
    var fn = window["fp_refresh" + args.ID];
    $container.removeClass("fp_freeze");
    if ($container.length > 0 && typeof fn === 'function') {
        fn();
        if (fp_settings.debug == true) {
            console.log("RefreshGrid " + args.ID);
        }
    } else {
        EventManager.tools.FailedMessage("no update " + args.classBlock);
    }
    EventManager.tools.hideLoading(args);
};
/**
 * updates tab after request
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} data parameters transmitted to the controller
 * @param {object} args UI options
 */
EventManager.tools.UpdateTab = function (data, status, xhr, args) {
    var $container = $("#b" + args.ID + " ." + args.classBlock + "");
    $container.removeClass("fp_freeze");
    $container = $container.find("#tabControl" + args.ID + "_C" + args.selectTabIndex + " div");
    if ($container.length > 0) {
        EventManagerContactDetails.unbind(args);
        $($container.first()).html(data);
        EventManagerContactDetails.bind(args);
        if (fp_settings.debug == true) {
            console.log("UpdateTab " + args.ID);
        }
    } else {
        EventManager.tools.FailedMessage("no update tab " + args.classBlock);
    }
    EventManager.tools.hideLoading(args);
};
/**
 * show Loading
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} args UI options
 */
EventManager.tools.showLoading = function (args) {
    EventManager.tools.hideLoading(args);
    var $block = $("#b" + args.ID + " ." + args.classBlock);
    $block.addClass("fp_freeze");
    var $loadingPanel = $("." + args.classBlock + "LoadingPanel #LoadingPanel" + args.ID);
    var left = args.loadingLeft;
    if (typeof args.loadingLeft == "undefined") {
        left = ($block.width() - $loadingPanel.width()) / 2;
    }
    var top = args.loadingTop;
    if (typeof args.loadingTop == "undefined") {
        top = ($block.height() - $loadingPanel.height()) / 2;
        top = Math.min(top, 100);
    }
    $loadingPanel.css({ "left": left, "top": top });
    $loadingPanel.show();
    if (fp_settings.debug == true) {
        // console.log("showLoading " + args.ID);
    }
};
/**
 * hide Loading
 * file for changes EventManager/EventManager.tools.js
 * read comments
 * Do not change!
 *
 * @param {object} args UI options
 */
EventManager.tools.hideLoading = function (args) {
    var loadingPanel = $("." + args.classBlock + "LoadingPanel #LoadingPanel" + args.ID);
    loadingPanel.hide();
    if (fp_settings.debug == true) {
        //console.log("hideLoading " + args.ID);
    }
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.getItemsBlocks = function (typeBlock, name) {
    if (fp_settings.debug == true) {
        console.log($(".fp" + typeBlock + " [data-alias='" + name + "']"));
    }
    return $(".fp" + typeBlock + " [data-alias='" + name + "']");
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.showContextMenu = function (args) {
    if (typeof args.pageY == "undefined") {
        args.pageY = args.event.htmlEvent.pageY;
    }
    if (typeof args.pageX == "undefined") {
        args.pageX = args.event.htmlEvent.pageX;
    }
    var top = args.pageY - $(args.blockCss).offset().top;
    var left = args.pageX - $(args.blockCss).offset().left;
    args.$contextMenu = $("#PopupMenu" + args.ID);
    eval("PopupMenu" + args.ID + ".ShowAtPos(" + args.pageX + ", " + args.pageY + ")");
    return args;
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.showContextMenu_CopyAttrs = function (args) {
    if (args.rowId) {
        $("#PopupMenu" + args.ID).attr("data-rowId", args.rowId);
    }
    if (args.parentId) {
        $("#PopupMenu" + args.ID).attr("data-parentId", args.parentId);
    }
    if (args.path) {
        $("#PopupMenu" + args.ID).attr("data-path", args.path);
    }
    if (args.contactType) {
        $("#PopupMenu" + args.ID).attr("data-contactType", args.contactType);
    }
    if (args.name) {
        $("#PopupMenu" + args.ID).attr("data-name", args.name);
    }
    if (args.level) {
        $("#PopupMenu" + args.ID).attr("data-level", args.level);
    }
    if (args.typeBlock) {
        $("#PopupMenu" + args.ID).attr("data-typeBlock", args.typeBlock);
    }
    if (args.contactID) {
        $("#PopupMenu" + args.ID).attr("data-contactID", args.contactID);
    }
    if (args.shortcutId) {
        $("#PopupMenu" + args.ID).attr("data-shortcut-id", args.shortcutId);
    }
    if (args.address) {
        $("#PopupMenu" + args.ID).attr("data-address", args.address);
    }
    return args;
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.showContextMenu_CheckAttrs = function (args) {
    args.rowId = $(args.$elemClick).attr("data-rowId");
    if (typeof args.rowId === "undefined") {
        args.$elemClick = $(args.$elemClick).find("." + args.classRow);
        args.rowId = args.$elemClick.attr("data-rowId");
    }
    if (typeof args.rowId === "undefined") {
        args.$elemClick = $(args.$elemClick).closest("." + args.classRow);
        args.rowId = args.$elemClick.attr("data-rowId");
    }
    if (typeof args.rowId === "undefined") {
        if (fp_settings.debug == true) {
            console.log("rowId = undefined");
            console.log(args.$elemClick);
        }
        return null;
    }
    args.contactType = args.$elemClick.attr("data-contactType");
    args.parentId = args.$elemClick.attr("data-parentId");
    args.contactID = args.$elemClick.attr("data-contactID");
    args.shortcutId = args.$elemClick.attr("data-shortcut-id");
    args.address = args.$elemClick.attr("data-address");
    return args;
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.clickContextMenu_GetAttrs = function (args) {
    args.$popup = $("#PopupMenu" + args.ID);

    args.rowId = args.$popup.attr("data-rowId");
    args.path = args.$popup.attr("data-path");
    args.parentId = args.$popup.attr("data-parentId");
    args.contactType = args.$popup.attr("data-contactType");
    if (!args.typeBlock) {
        args.typeBlock = args.$popup.attr("data-typeBlock");
    }
    args.contactID = args.$popup.attr("data-contactID");
    args.shortcutId = args.$popup.attr("data-shortcut-id");
    args.address = args.$popup.attr("data-address");
    args.$block = $("#b" + args.ID + " ." + args.classBlock + "[data-name],.flexpage-blockWrapper#b" + args.ID + " ." + args.classBlock + "[data-alias]");
    args.name = args.$block.attr("data-name");
    args.alias = args.$block.attr("data-alias");

    if (typeof args.rowId === "undefined") {
        if (fp_settings.debug == true) {
            console.log("rowId = undefined");
            console.log(args.$popup);
        }
        return null;
    }
    return args;
};
/**
 * apply Rename after click
 * file for changes EventManager/EventManager.tools.js
 * read comments
 *
 * @param {object} args UI options
 */
EventManager.tools.applyRename = function (e) {
    if (typeof e.type === "undefined") {
        e.type = "click";
    }
    EventManager.bus.publish(EventManager.settings.Events.hideAllContextMenu, { "event": e });
    EventManager.bus.publish(EventManager.settings.Events.folderTreeList.renameApplyFolder, { "event": e });
    EventManager.bus.publish(EventManager.settings.Events.folderContent.renameApplyFile, { "event": e });
};
/**
 * file for changes EventManager/EventManager.tools.js
 * read comments
 */
EventManager.tools.focus = function (args) {
    var value = args.$input.val();
    args.$input.val("");
    args.$input.focus();
    args.$input.val(value);
    return args;
};
EventManager.tools.getBrowser = function (args) {
    args.$browser = $(".fp_browser,.fpBrowserContacts");
    if (args.$browser.length > 0) {
        args.$browser.ID = args.$browser.attr("id");
        if (args.$browser.ID) {
            args.$browser.ID = args.$browser.ID.replace("fp_browser", "").replace("b", "");
            args.$browser.ID = parseInt(args.$browser.ID);
        }
    }
    return args;
};

EventManager.tools.get_cookie = function (name) {
    //http://stackoverflow.com/questions/10730362/get-cookie-by-name
    var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) return decodeURIComponent(match[2]);
};

EventManager.tools.set_cookie = function (cookie_name, cookie_value, lifespan_in_seconds, valid_domain) {
    setTimeout(function () {
        // http://www.thesitewizard.com/javascripts/cookies.shtml
        var domain_string = valid_domain ? "; domain=" + valid_domain : '';
        document.cookie = cookie_name + "=" + encodeURIComponent(cookie_value) + "; max-age=" + lifespan_in_seconds + "; path=/" + domain_string;
    }, 0);
};

EventManager.tools.delete_cookie = function (cookie_name, valid_domain) {
    // http://www.thesitewizard.com/javascripts/cookies.shtml
    var domain_string = valid_domain ? "; domain=" + valid_domain : '';
    document.cookie = cookie_name + "=; max-age=0; path=/" + domain_string;
};

