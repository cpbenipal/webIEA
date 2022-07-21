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
EventManager.bus = (function () {

    var channels = {};
    /**
    * creates an event subscription
    * Do not change!
    *  @param {string} channel name event
    *  @param {Function} fn function called after the event
    *  @returns {Function} subscribe context
    */
    var subscribe = function subscribe(channel, fn) {
        var _this = this;

        if (!channels[channel]) channels[channel] = [];
        if (fp_settings.debug == true) {
            //console.log("subscribe: " + channel);
        }
        if (!channels[channel].some(function (channel) {
            return channel.callback === fn && channel.context === _this;
        })) {
            channels[channel].push({
                context: this,
                callback: fn
            });
        }
        return this;
    },

    /**
    * creating an event
    * Do not change!
    * @param {string} channel name event
    * @returns {Function} subscribe context
    */
    publish = function publish(channel) {
        if (!channels[channel]) return false;
        var args = Array.prototype.slice.call(arguments, 1);
        for (var i = 0, l = channels[channel].length; i < l; i++) {
            var subscription = channels[channel][i];
            try {
                if (fp_settings.debug == true) {
                    console.log("publish: " + channel);
                }
                subscription.callback.apply(subscription.context, args);
            } catch (err) {
                EventManager.tools.FailedMessage("Event " + channel + " error: " + err, channels);
            }
        }
        return this;
    };

    return {
        publish: publish,
        subscribe: subscribe,
        installTo: function installTo(obj) {
            obj.subscribe = subscribe;
            obj.publish = publish;
        }
    };
})();


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
EventManager.service = (function () {
    var init = function init() {
        EventManager.bus.installTo(this);
        for (var key in EventManager.settings.Events.service) {
            var func = this[key];
            var value = EventManager.settings.Events.service[key];
            EventManager.bus.subscribe(value, func);
        }
    };
    var setCallbackSucceed = function setCallbackSucceed(html, status, xhr, args) {
        var response = {};
        try {
            response = JSON.parse(html);
        } catch (e) {
            args.callbackSucceed(html, status, xhr, args, response);
            return;
        }
        if (response.error) {
            args.callbackError(html, status, xhr, args, response);
        } else {
            args.callbackSucceed(html, status, xhr, args, response);
        }
    };
    var setCallbackFailed = function setCallbackFailed(html, status, xhr, args) {
        args.callbackFailed(html, status, xhr, args);
    };
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var get = function get(args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "GET",
            cache: true,
            success: function success(html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
            },
            error: function error(html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    };
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var setFile = function setFile(args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: false,
            contentType: false,
            processData: false,
            success: function success(html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
                hideUploadFileWaitBlock();
            },
            error: function error(html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
                hideUploadFileWaitBlock();
            }
        });
    };
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var setStrict = function setStrict(args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: false,
            contentType: false,
            processData: false,
            success: function success(html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
            },
            error: function error(html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    };
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var set = function set(args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: true,
            async: args.async === "undefined" ? true : args.async,
            success: function success(html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
            },
            error: function error(html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    };
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksUpdateBlock = function setCallbacksUpdateBlock(args) {
        if (typeof args.callbackSucceed === "undefined") {
            args.callbackSucceed = EventManager.tools.UpdateBlock;
        }
        if (typeof args.callbackFailed === "undefined") {
            args.callbackFailed = EventManager.tools.CallbackFailed;
        }
        if (typeof args.callbackError === "undefined") {
            args.callbackError = EventManager.tools.CallbackError;
        }
        return args;
    };
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksRefreshGridDialogFailed = function setCallbacksRefreshGridDialogFailed(args) {
        if (typeof args.callbackSucceed === "undefined") {
            args.callbackSucceed = EventManager.tools.RefreshGrid;
        }
        if (typeof args.callbackFailed === "undefined") {
            args.callbackFailed = EventManager.tools.CallbackFailedDialogFailed;
        }
        if (typeof args.callbackError === "undefined") {
            args.callbackError = EventManager.tools.CallbackFailedDialogFailed;
        }
        return args;
    };
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksRefreshGrid = function setCallbacksRefreshGrid(args) {
        if (typeof args.callbackSucceed === "undefined") {
            args.callbackSucceed = EventManager.tools.RefreshGrid;
        }
        if (typeof args.callbackFailed === "undefined") {
            args.callbackFailed = EventManager.tools.CallbackFailed;
        }
        if (typeof args.callbackError === "undefined") {
            args.callbackError = EventManager.tools.CallbackError;
        }
        return args;
    };
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksSuccess = function setCallbacksSuccess(args) {
        if (typeof args.callbackSucceed === "undefined") {
            args.callbackSucceed = EventManager.tools.CallbackSuccess;
        }
        if (typeof args.callbackFailed === "undefined") {
            args.callbackFailed = EventManager.tools.CallbackFailed;
        }
        if (typeof args.callbackError === "undefined") {
            args.callbackError = EventManager.tools.CallbackError;
        }
        return args;
    };
    /**
    * Ajax request after clicking on a folder in treelist
    * file for changes EventManager/EventManager.service.js
    * read comments
    *
    * @param {object} args UI options
    */
    var updateByPathFolder = function updateByPathFolder(args) {

        args = setCallbacksUpdateBlock(args);
        get(args);
    };
    /**
     * add Contact To Folder
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var addContactToFolder = function addContactToFolder(args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    };
    /**
     * delete Contact To Folder
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteContactToFolder = function deleteContactToFolder(args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    };
    /**
     * Create folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var createFolder = function createFolder(args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    };

    /**
    * Rename folder
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var renameType = function renameType(args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    };
    /**
     * Apply rename folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var renameApplyFolder = function renameApplyFolder(args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    };

    /**
     * delete folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteFolder = function deleteFolder(args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    };
    /**
     * delete file
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteFile = function deleteFile(args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    };
    /**
     * 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var uploadFile = function uploadFile(args) {
        showUploadFileWaitBlock();
        args = setCallbacksUpdateBlock(args);
        args.processData = false;
        setFile(args);
    };

    var intervalShowUpload = false;

    var showUploadFileWaitBlock = function showUploadFileWaitBlock() {
        intervalShowUpload = true;
        var interval = setInterval(function () {
            var bottom = $(".uploadFileWaitBlock").css("bottom");

            $(".uploadFileWaitBlock").css("bottom", parseInt(bottom) + 1);
            if (20 <= parseInt(bottom)) {
                clearInterval(interval);
                intervalShowUpload = false;
            }
        }, 5);
    };

    var hideUploadFileWaitBlock = function hideUploadFileWaitBlock() {
        $(".uploadFileWaitBlock").show();
        var uploadFileWaitBlock = $(".uploadFileWaitBlock"),
            height = uploadFileWaitBlock.height(),
            padding = parseInt(uploadFileWaitBlock.css("padding-top")) * 2 + 21,
            bottomPos = (height + padding) * -1;

        var interval = setInterval(function () {
            var bottom = $(".uploadFileWaitBlock").css("bottom");
            console.log(">>>", intervalShowUpload);
            if (!intervalShowUpload) {
                $(".uploadFileWaitBlock").css("bottom", parseInt(bottom) - 1);
                if (bottomPos >= parseInt(bottom)) {
                    clearInterval(interval);
                }
            }
        }, 5);
    };

    /**
    * expanded folder in treeList
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var expandedChanging = function expandedChanging(args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    };
    /**
    * update Contact Details
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var updateContactDetails = function updateContactDetails(args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    };

    /**
    *
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var set_ResultUpdateBlock_Default = function set_ResultUpdateBlock_Default(args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    };

    /**
    *
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var set_ResultRefreshGrid_Default = function set_ResultRefreshGrid_Default(args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    };
    /**
     *
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var set_ResultJson_Default = function set_ResultJson_Default(args) {
        args = setCallbacksSuccess(args);
        set(args);
    };
    /**
     *
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var setStrict_ResultJson_Default = function setStrict_ResultJson_Default(args) {
        args = setCallbacksSuccess(args);
        setStrict(args);
    };
    var set_FAQQuestionEdit = function set_FAQQuestionEdit(args) {
        args = setCallbacksSuccess(args);
        set(args);
    };
    return {
        init: init,
        updateByPathFolder: updateByPathFolder,
        addContactToFolder: addContactToFolder,
        deleteContactToFolder: deleteContactToFolder,
        createFolder: createFolder,
        renameApplyFolder: renameApplyFolder,
        renameApplyFile: set_ResultRefreshGrid_Default,
        deleteFolder: deleteFolder,
        deleteFile: deleteFile,
        uploadFile: uploadFile,
        expandedChanging: expandedChanging,
        updateContactDetails: updateContactDetails,
        saveContactDetails: set_ResultUpdateBlock_Default,
        saveCustomProperties: set_ResultJson_Default,
        getCustomPropertyShowContact: set_ResultJson_Default,
        setContactsAdvancedSearch: set_ResultJson_Default,
        setPendingEvents: set_ResultJson_Default,
        addNewContact: set_ResultJson_Default,
        resetPublishingOverrides: set_ResultJson_Default,
        resetTimeToLeave: set_ResultJson_Default,
        paste: setStrict_ResultJson_Default,
        treelistClickNode: set_ResultJson_Default,
        deleteContact: set_ResultRefreshGrid_Default,
        faqQuestionEdit: set_FAQQuestionEdit
    };
})();


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

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var EventManager = EventManager || {};
EventManager.settings = {
    debug: fp_settings.debug,
    PathJs: {
        treeList: "/Areas/Flexpage/Content/Scripts/TreeList/EventManagerTreeList.es5.min.js",
        contactsEnumeration: "/Areas/Flexpage/Content/Scripts/ContactsEnumeration/_bundleContactsEnumeration.min.js",
        folderContent: "/Areas/Flexpage/Content/Scripts/FolderContent/_bundleFolderContent.min.js",
        browser: "/Areas/Flexpage/Content/Scripts/Browser/EventManagerBrowser.es5.min.js",
        browserContacts: "/Areas/Flexpage/Content/Scripts/BrowserContacts/EventManagerBrowserContacts.es5.min.js",
        contactDetails: "/Areas/Flexpage/Content/Scripts/ContactDetails/EventManagerContactDetails.es5.min.js",
        customProperties: "/Areas/Flexpage/Content/Scripts/CustomProperties/EventManagerCustomProperties.es5.min.js",
        contactsAdvancedSearch: "/Areas/Flexpage/Content/Scripts/ContactsAdvancedSearch/EventManagerContactsAdvancedSearch.es5.min.js",
        pendingEvents: "/Areas/Flexpage/Content/Scripts/PendingEvents/EventManagerPendingEvents.es5.min.js",
        enums: "/Areas/Flexpage/Content/Scripts/Enums/EventManagerEnums.es5.min.js",
        browserSearch: "/Areas/Flexpage/Content/Scripts/BrowserSearch/EventManagerBrowserSearch.es5.min.js",
        faq: "/Areas/Flexpage/Content/Scripts/FAQ/EventManagerFAQ.es5.min.js",
        faqEdit: "/Areas/Flexpage/Content/Scripts/FAQ/EventManagerFAQEdit.es5.min.js"
    },
    Events: {
        hideAllContextMenu: "hideAllContextMenu",
        folderTreeList: {
            initUpload: "folderTreeList_InitUpload",
            showContextMenu: "folderTreeList_ShowContextMenu",
            clickNode: "folderTreeList_ClickNode",
            clickContextMenu: "folderTreeList_ClickContextMenu",
            renameApplyFolder: "renameApplyFolder",
            expandedChangingFolder: "folderTreeList_ExpandedChangingFolder",
            selectionChanged: "folderTreeList_SelectionChanged",
            endCallback: "folderTreeList_endCallback"
        },
        contactsEnumeration: {
            updateByPathFolder: "contactsEnumeration_UpdateByPathFolder",
            detailToggleButton: "contactsEnumeration_DetailToggleButton",
            rowClick: "contactsEnumeration_RowClick",
            showContextMenu: "contactsEnumeration_ShowContextMenu",
            clickContextMenu: "contactsEnumeration_ClickContextMenu",
            updateGeneralInfo: "contactsEnumeration_UpdateGeneralInfo",
            updateTelecoms: "contactsEnumeration_UpdateTelecoms",
            addContactToFolder: "contactsEnumeration_AddContactToFolder",
            addPerson: "contactsEnumeration_AddPerson",
            addCompany: "contactsEnumeration_AddCompany",
            deleteContactToFolder: "contactsEnumeration_DeleteContactToFolder",
            updateCustomProperties: "contactsEnumeration_UpdateCustomProperties",
            addNewContact: "contactsEnumeration_AddNewContact",
            deleteContact: "contactsEnumeration_deleteContact",
            endCallback: "contactsEnumeration_endCallback"
        },
        contactDetails: {
            tabClick: "contactDetails_TabClick",
            edit: "contactDetails_Edit",
            save: "contactDetails_Save",
            cancel: "contactDetails_Cancel"
        },
        browser: {
            updateByPathFolder: "browser_UpdateByPathFolder",
            endCallback: "browser_EndCallback"
        },
        browserSearch: {
            showContextMenu: "browserSearch_ShowContextMenu",
            clickContextMenu: "browserSearch_ClickContextMenu",
            downloadZip: "browserSearch_downloadZip"
        },
        browserContacts: {
            endCallback: "browserContacts_EndCallback"
        },
        folderContent: {
            initUpload: "folderContent_InitUpload",
            updateByPathFolder: "folderContent_UpdateByPathFolder",
            showContextMenu: "folderContent_ShowContextMenu",
            clickContextMenu: "folderContent_clickContextMenu",
            renameApplyFile: "renameApplyFile",
            uploadFile: "uploadFile",
            detailToggleButton: "folderContent_detailToggleButton",
            downloadZip: "folderContent_downloadZip",
            endCallback: "folderContent_endCallback"
        },
        contactsAdvancedSearch: {
            search: "contactsAdvancedSearch_search",
            exportToFile: "contactsAdvancedSearch_exportToFile",
            importToFile: "contactsAdvancedSearch_importToFile",
            openToBrowser: "contactsAdvancedSearch_openToBrowser",
            openFile: "contactsAdvancedSearch_openFile",
            saveToFolder: "contactsAdvancedSearch_saveToFolder",
            linkFolder: "contactsAdvancedSearch_linkFolder",
            saveXmlChanges: "contactsAdvancedSearch_saveXmlChanges",
            setHistoryStep: "contactsAdvancedSearch_setHistoryStep",
            newFile: "contactsAdvancedSearch_newFile"
        },
        pendingEvents: {
            start: "pendingEvents_start",
            stop: "pendingEvents_stop",
            setDelay: "pendingEvents_setDelay",
            refresh: "pendingEvents_refresh",
            showContextMenu: "pendingEvents_showContextMenu",
            clickContextMenu: "pendingEvents_clickContextMenu"
        },
        customProperties: {},
        faqEdit: {
            changeLanguageQuestionEdit: "faq_changeLanguageQuestionEdit"
        },
        faq: {
            filter: "faq_filter"
        },
        service: {
            updateByPathFolder: "service_updateByPathFolder",
            createFolder: "service_createFolder",
            renameApplyFolder: "service_renameApplyFolder",
            deleteFolder: "service_deleteFolder",
            renameApplyFile: "service_renameApplyFile",
            deleteFile: "service_deleteFile",
            uploadFile: "service_uploadFile",
            expandedChanging: "service_expandedChanging",
            updateContactDetails: "service_updateContactDetails",
            saveContactDetails: "service_saveContactDetails",
            addContactToFolder: "service_addContactToFolder",
            deleteContactToFolder: "service_deleteContactToFolder",
            saveCustomProperties: "service_saveCustomProperties",
            getCustomPropertyShowContact: "service_getCustomPropertyShowContact",
            setContactsAdvancedSearch: "service_setContactsAdvancedSearch",
            setPendingEvents: "service_setPendingEvents",
            addNewContact: "service_addNewContact",
            resetPublishingOverrides: "service_resetPublishingOverrides",
            resetTimeToLeave: "service_resetTimeToLeave",
            paste: "service_paste",
            treelistClickNode: "service_treelistClickNode",
            deleteContact: "service_deleteContact",
            faqQuestionEdit: "service_faqQuestionEdit"
        }
    }
};

var EventManagerBase = (function () {
    function EventManagerBase() {
        _classCallCheck(this, EventManagerBase);

        EventManager.bus.installTo(this);
        EventManager.bus.subscribe(EventManager.settings.Events.hideAllContextMenu, this.hideAllContextMenu);
    }

    /**
     *
     */

    _createClass(EventManagerBase, [{
        key: "hideAllContextMenu",
        value: function hideAllContextMenu() {
            $("[class*='dxmLite'] > div").attr("style", "display:none;");
        }
    }]);

    return EventManagerBase;
})();

var eventManagerBase = new EventManagerBase();

EventManager.init = function (options) {
    EventManager.service.init();
};
EventManager.init();

