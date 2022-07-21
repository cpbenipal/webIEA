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

