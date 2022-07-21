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
EventManager.service = (function () {
    var init = function () {
        EventManager.bus.installTo(this);
        for (var key in EventManager.settings.Events.service) {
            var func = this[key];
            var value = EventManager.settings.Events.service[key];
            EventManager.bus.subscribe(value, func);
        }
    }
    var setCallbackSucceed = function (html, status, xhr, args) {
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
    }
    var setCallbackFailed = function (html, status, xhr, args) {
        args.callbackFailed(html, status, xhr, args);
    }
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var get = function (args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "GET",
            cache: true,
            success: function (html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args)
            },
            error: function (html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    }
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var setFile = function (args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: false,
            contentType: false,
            processData: false,
            success: function (html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
                hideUploadFileWaitBlock();
            },
            error: function (html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
                hideUploadFileWaitBlock();
            }
        });
    }
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var setStrict = function (args) {
        $.ajax({
            url: document.location.origin + "/" + args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: false,
            contentType: false,
            processData: false,
            success: function (html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args);
            },
            error: function (html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    }
    /**
     * 
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     */
    var set = function (args) {
        $.ajax({
            url: document.location.origin+"/"+ args.requestUrl,
            data: args.requestParameters,
            type: "POST",
            cache: true,
            async: args.async === "undefined" ? true : args.async,
            success: function (html, status, xhr) {
                setCallbackSucceed(html, status, xhr, args)
            },
            error: function (html, status, xhr) {
                setCallbackFailed(html, status, xhr, args);
            }
        });
    }
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksUpdateBlock = function (args) {
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
    }
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksRefreshGridDialogFailed = function (args) {
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
    }
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksRefreshGrid = function (args) {
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
    }
    /**
     * for Ajax request needs a callback function. This function uses default callback functions
     * file for changes EventManager/EventManager.service.js 
     * read comments
     * Do not change!
     *
     * @param {object} args UI options
     * @return {args} args UI options
     */
    var setCallbacksSuccess = function (args) {
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
    }
     /**
     * Ajax request after clicking on a folder in treelist
     * file for changes EventManager/EventManager.service.js
     * read comments
     *
     * @param {object} args UI options
     */
    var updateByPathFolder = function (args) {
        
        args = setCallbacksUpdateBlock(args);
        get(args);
    }
    /**
     * add Contact To Folder
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var addContactToFolder = function (args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    }
    /**
     * delete Contact To Folder
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteContactToFolder = function (args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    }
    /**
     * Create folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var createFolder = function (args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    }
    
     /**
     * Rename folder
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var renameType = function (args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    }
    /**
     * Apply rename folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var renameApplyFolder = function (args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    }
    
    /**
     * delete folder 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteFolder = function (args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    }
    /**
     * delete file
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var deleteFile = function (args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    }
    /**
     * 
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var uploadFile = function (args) {
        showUploadFileWaitBlock();
        args = setCallbacksUpdateBlock(args);
        args.processData = false;
        setFile(args);
    }

    var intervalShowUpload = false;

    var showUploadFileWaitBlock = function() {
        intervalShowUpload = true;
        var interval = setInterval(function() {
            var bottom = $(".uploadFileWaitBlock").css("bottom");

            $(".uploadFileWaitBlock").css("bottom", parseInt(bottom)+1);
            if (20 <= parseInt(bottom)) {
                clearInterval(interval);
                intervalShowUpload = false;
            }
        }, 5);
    }

    var hideUploadFileWaitBlock = function () {
        $(".uploadFileWaitBlock").show();
        var uploadFileWaitBlock = $(".uploadFileWaitBlock"),
            height = uploadFileWaitBlock.height(),
            padding = parseInt(uploadFileWaitBlock.css("padding-top")) * 2 + 21,
            bottomPos = (height + padding) * -1;

        var interval = setInterval(function() {
            var bottom = $(".uploadFileWaitBlock").css("bottom");
            console.log(">>>", intervalShowUpload);
            if (!intervalShowUpload) {
                $(".uploadFileWaitBlock").css("bottom", parseInt(bottom) - 1);
                if (bottomPos >= parseInt(bottom)) {
                    clearInterval(interval);
                }
            }
        }, 5);
    }

     /**
     * expanded folder in treeList
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var expandedChanging = function(args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    }
     /**
     * update Contact Details
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var updateContactDetails = function (args) {
        args = setCallbacksUpdateBlock(args);
        get(args);
    }
    
    /**
    *
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var set_ResultUpdateBlock_Default = function (args) {
        args = setCallbacksUpdateBlock(args);
        set(args);
    }
    
    /**
    *
    * file for changes EventManager/EventManager.service.js
    * read comments
    * @param {object} args UI options
    */
    var set_ResultRefreshGrid_Default = function (args) {
        args = setCallbacksRefreshGrid(args);
        set(args);
    }
    /**
     *
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var set_ResultJson_Default = function (args) {
        args = setCallbacksSuccess(args);
        set(args);
    }
    /**
     *
     * file for changes EventManager/EventManager.service.js
     * read comments
     * @param {object} args UI options
     */
    var setStrict_ResultJson_Default = function (args) {
        args = setCallbacksSuccess(args);
        setStrict(args);
    }
    var set_FAQQuestionEdit = args => {
        args = setCallbacksSuccess(args);
        set(args);
    }
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
    
}());
