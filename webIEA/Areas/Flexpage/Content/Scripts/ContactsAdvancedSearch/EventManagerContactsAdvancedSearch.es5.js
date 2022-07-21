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

var EventManagerContactsAdvancedSearch = (function (_EventManagerBase) {
    _inherits(EventManagerContactsAdvancedSearch, _EventManagerBase);

    function EventManagerContactsAdvancedSearch(args) {
        _classCallCheck(this, EventManagerContactsAdvancedSearch);

        _get(Object.getPrototypeOf(EventManagerContactsAdvancedSearch.prototype), "constructor", this).call(this);
        EventManager.bus.installTo(this);
        var that = this;
        for (var key in EventManager.settings.Events.contactsAdvancedSearch) {
            var func = this[key];
            var value = EventManager.settings.Events.contactsAdvancedSearch[key];
            EventManager.bus.subscribe(value, func);
        }
        args.nameHistory = $("#HistoryName").val();
        args.historyStart = $("#HistoryStart").val();
        args.userName = $("#UserName").val();
        args.decode = true;

        args = EventManagerContactsAdvancedSearch.getStartXml(args);
        args.xml = args.StartXML;
        args = EventManagerContactsAdvancedSearch.setStartXml(args);
        args = EventManagerContactsAdvancedSearch.getHistoryBPMNxmls(args);
        args = EventManagerContactsAdvancedSearch.setHistoryStepArrows(args);
    }

    _createClass(EventManagerContactsAdvancedSearch, [{
        key: "search",
        value: function search(args) {
            args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_Result";
            args.requestParameters = { xml: args.xml };
            args.callbackSucceed = function (data, status, xhr, _args, response) {
                $("#fp_resultAdvancedSearch").html(data);
                var fn = window["fp_changeActiveTab"];
                fn();
                EventManagerContactsAdvancedSearch.unbind(args);
                EventManagerContactsAdvancedSearch.bind(args);
            };
            if (fp_settings.debug == true) {
                console.log(args.requestParameters.xml);
            }
            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "exportToFile",
        value: function exportToFile(args) {
            args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_Export";
            args.requestParameters = { xml: args.xml, path: args.path, folderId: args.folderId, name: args.name };
            var callbackSucceed = args.callbackSucceed;
            args.callbackSucceed = function (data, status, xhr, _args, response) {
                if (fp_settings.debug == true) {
                    console.log("export");
                }
                callbackSucceed(data, status, xhr, _args, response);
            };
            if (fp_settings.debug == true) {
                console.log(args.requestParameters.xml);
            }
            args.callbackError = function (html, status, xhr, _args, jsonObj) {
                fp_ConfirmDialog(jsonObj.error.title, jsonObj.error.message, function () {
                    if (jsonObj.error.overwrite == true) {
                        args.requestParameters.append('overwrite', true);
                        EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
                    }
                }, function () {});
            };
            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "importToFile",
        value: function importToFile(args) {
            args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_Import";
            args.requestParameters = { xml: args.xml, bpmnXml: args.bpmnXml };
            var callbackSucceed = args.callbackSucceed;
            args.callbackSucceed = function (data, status, xhr, _args, response) {
                if (response.result) {
                    if (fp_settings.debug == true) {
                        console.log("import");
                    }
                    callbackSucceed(data, status, xhr, _args, response);
                    var _args = { ID: args.ID };
                    _args.xml = response.result;
                    _args.StartXML = response.result;
                    _args = EventManagerContactsAdvancedSearch.removeDiagramHistory(_args);
                }
            };

            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "openToBrowser",
        value: function openToBrowser(args) {
            fp_popupControlOpen({ command: 'browserchoose', blockType: 'BrowserSelector', action: 'BrowserSelector', controller: 'FlexPage', alwaysCallOnClose: true }, function () {});
        }
    }, {
        key: "openFile",
        value: function openFile(args) {
            args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_Open";
            args.requestParameters = { fileID: args.fileID };
            var callbackSucceed = args.callbackSucceed;
            args.callbackSucceed = function (data, status, xhr, _args, response) {
                if (response.result) {
                    if (fp_settings.debug == true) {
                        console.log("open");
                    }
                    callbackSucceed(data, status, xhr, _args, response);
                    var _args = { ID: args.ID };
                    _args.xml = response.result;
                    _args.StartXML = response.result;
                    _args = EventManagerContactsAdvancedSearch.removeDiagramHistory(_args);
                }
            };
            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "saveToFolder",
        value: function saveToFolder(args) {
            fp_popupControlOpen({ command: 'browserchoose', blockType: 'FolderSaveAsContactsSelector', action: 'FolderSaveAsContactsSelector', controller: 'FlexPage', alwaysCallOnClose: true }, function () {});
        }
    }, {
        key: "linkFolder",
        value: function linkFolder(args) {
            args.requestUrl = "Admin/ContactsAdvancedSearch_CheckFolderDL";
            args.requestParameters = { path: args.path };
            args.callbackSucceed = function (data, status, xhr, _args, response) {
                if (response.result == true) {
                    EventManagerContactsAdvancedSearch.exportToFolder(args);
                } else if (response.result == false) {
                    fp_ConfirmDialog('Covert', 'The selected folder is not distribution list. Do you want to convert it to distribution list?', function () {
                        args.requestUrl = "Admin/ContactsAdvancedSearch_ConvertFoldertoDL";
                        args.requestParameters = { path: args.path };
                        args.callbackSucceed = function (data, status, xhr, _args, response) {
                            if (response.result == true) {
                                EventManagerContactsAdvancedSearch.exportToFolder(args);
                            } else if (response.result == false) {
                                fp_ConfirmDialog('Error', 'Folder not converted', function () {});
                            }
                        };
                        EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
                    });
                }
            };
            if (fp_settings.debug == true) {
                console.log(args.requestParameters.path);
            }
            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "newFile",
        value: function newFile(args) {
            args.xml = "";
            args.StartXML = "";
            args = EventManagerContactsAdvancedSearch.removeDiagramHistory(args);
        }
    }, {
        key: "setHistoryStep",
        value: function setHistoryStep(args) {
            args.nameHistory = $("#HistoryName").val();
            args.historyStart = $("#HistoryStart").val();
            args.userName = $("#UserName").val();
            if (fp_settings.debug == true) {
                console.log("args.step:" + args.step);
            }
            args = EventManagerContactsAdvancedSearch.getHistoryBPMNxmls(args);
            args = EventManagerContactsAdvancedSearch.setHistoryStepArrows(args);

            if (args.nowStep + args.step >= 0 && args.nowStep + args.step < args.BPMNxmls.length) {
                args.nowStep = args.nowStep + args.step;

                if (args.BPMNxmls[args.nowStep]) {
                    if (fp_settings.debug == true) {
                        console.log("set step:" + args.nowStep + ", localStorage step:" + (args.BPMNxmls.length - args.nowStep));
                    }
                    $("#HistoryStep").val(args.nowStep);
                    args.callbackSucceed(args.BPMNxmls[args.nowStep].xml);
                    //to server
                    if (args.BPMNxmls[args.nowStep].xml) {
                        args.requestParameters = { xml: args.BPMNxmls[args.nowStep].xml };
                        args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_SaveXmlChanges";
                        args.callbackSucceed = function (data, status, xhr, _args, response) {};
                        EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
                    }
                }
            }
        }
    }, {
        key: "saveXmlChanges",
        value: function saveXmlChanges(args) {
            //to client
            args.nameHistory = $("#HistoryName").val();
            args.historyStart = $("#HistoryStart").val();
            args.userName = $("#UserName").val();
            args = EventManagerContactsAdvancedSearch.getHistoryBPMNxmls(args);
            args.temlBPMNXml = { xml: args.xml, datetime: Date.now() };
            args.BPMNxmls.unshift(args.temlBPMNXml);
            args.temlBPMNXml = JSON.stringify(args.temlBPMNXml);
            localStorage.setItem(args.nameHistory + "-" + args.userName + "-" + args.BPMNxmls.length, args.temlBPMNXml, 300000);

            //to server
            if (args.xml) {
                args.requestParameters = { xml: args.xml };
                args.requestUrl = "AdvancedSearch/ContactsAdvancedSearch_SaveXmlChanges";
                args.callbackSucceed = function (data, status, xhr, _args, response) {};
                EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
            }
            args = EventManagerContactsAdvancedSearch.getHistoryBPMNxmls(args);
            args = EventManagerContactsAdvancedSearch.setHistoryStepArrows(args);
        }
    }], [{
        key: "setStartXml",
        value: function setStartXml(args) {
            args.temlBPMNXml = JSON.stringify(args.temlBPMNXml);
            localStorage.setItem(args.nameHistory + "-" + args.userName + "-" + args.historyStart, args.temlBPMNXml, 300000);
            if (!localStorage.getItem(args.nameHistory + "-" + args.userName + "-1")) {
                EventManager.bus.publish(EventManager.settings.Events.contactsAdvancedSearch.saveXmlChanges, args);
            }
            return args;
        }
    }, {
        key: "getStartXml",
        value: function getStartXml(args) {
            if (args.decode) {
                args.StartXML = $("<div></div>").html(args.StartXML).text();
                if (args.StartXML[0] == '"') {
                    args.StartXML = args.StartXML.substr(1, args.StartXML.length - 2);
                }
            }
            args.temlBPMNXml = { xml: args.StartXML, datetime: Date.now() };

            return args;
        }
    }, {
        key: "removeDiagramHistory",
        value: function removeDiagramHistory(args) {
            args.nameHistory = $("#HistoryName").val();
            args.historyStart = $("#HistoryStart").val();
            args.userName = $("#UserName").val();
            var step = 0;
            args.temlBPMNXml = null;

            do {
                step++;
                args.temlBPMNXml = localStorage.getItem(args.nameHistory + "-" + args.userName + "-" + step);
                if (args.temlBPMNXml) {
                    localStorage.removeItem(args.nameHistory + "-" + args.userName + "-" + step);
                }
            } while (args.temlBPMNXml);
            $("#HistoryStep").val(0);
            args = EventManagerContactsAdvancedSearch.getStartXml(args);
            args = EventManagerContactsAdvancedSearch.setStartXml(args);
            return args;
        }
    }, {
        key: "getHistoryBPMNxmls",
        value: function getHistoryBPMNxmls(args) {
            args.BPMNxmls = [];
            var step = 0;
            args.temlBPMNXml = null;
            do {
                step++;
                args.temlBPMNXml = localStorage.getItem(args.nameHistory + "-" + args.userName + "-" + step);
                if (args.temlBPMNXml) {
                    args.BPMNxmls.unshift(JSON.parse(args.temlBPMNXml));
                }
            } while (args.temlBPMNXml);
            return args;
        }
    }, {
        key: "setHistoryStepArrows",
        value: function setHistoryStepArrows(args) {
            args.nowStep = parseInt($("#HistoryStep").val());
            if (!args.step) {
                args.step = 0;
            }
            if (args.BPMNxmls.length > 1 && args.nowStep + args.step < args.BPMNxmls.length - 1) {
                $(".fp_toolbar-buttons__button_undo").removeClass("fp_toolbar-buttons__button_inactive");
            } else {
                $(".fp_toolbar-buttons__button_undo").addClass("fp_toolbar-buttons__button_inactive");
            }
            if (args.BPMNxmls.length > 1 && args.nowStep + args.step > 0) {
                $(".fp_toolbar-buttons__button_redo").removeClass("fp_toolbar-buttons__button_inactive");
            } else {
                $(".fp_toolbar-buttons__button_redo").addClass("fp_toolbar-buttons__button_inactive");
            }
            return args;
        }
    }, {
        key: "exportToFolder",
        value: function exportToFolder(args) {
            var guid = $("#fp_resultAdvancedSearchGUID").val();
            args.requestUrl = "Admin/ContactsAdvancedSearch_Export";
            args.requestParameters = { xml: guid, path: args.path };

            EventManager.bus.publish(EventManager.settings.Events.service.setContactsAdvancedSearch, args);
        }
    }, {
        key: "bind",
        value: function bind(args) {
            $(".fp_advancedSearch-btn__export").on("click", { args: args }, EventManagerContactsAdvancedSearch["export"]);
            $(".fp_advancedSearch-btn__export_to_xls").on("click", { args: args }, EventManagerContactsAdvancedSearch.exportToXls);
        }
    }, {
        key: "unbind",
        value: function unbind(args) {
            $(".fp_advancedSearch-btn__export").off("click", EventManagerContactsAdvancedSearch["export"]);
            $(".fp_advancedSearch-btn__export_to_xls").off("click", EventManagerContactsAdvancedSearch.exportToXls);
        }
    }, {
        key: "export",
        value: function _export(e) {
            var args = e.data.args;
            fp_popupControlOpen({ command: 'choose', blockType: 'FolderSaveContactsSelector', action: 'GetPopupChooseObjectContent', controller: 'FlexPage', alwaysCallOnClose: true, title: 'Export results' }, function () {});
        }
    }, {
        key: "exportToXls",
        value: function exportToXls(e) {

            window.flexpage = window.flexpage || {};
            window.parent.flexpage.ExportXls = $("#SearchXml").val();
            window.flexpage.isSaveEven = false;
            var requestParameters = '?folderId=' + $("#SelectFolderId").val();
            if ($("#fp_resultAdvancedSearchGUID").val()) {
                var guid = $("#fp_resultAdvancedSearchGUID").val();
                requestParameters += '&xml=' + guid;
            }
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
                src: '/Export/ShowExportSettingsDialog' + requestParameters
            }).appendTo($('body'));
        }
    }]);

    return EventManagerContactsAdvancedSearch;
})(EventManagerBase);

;

