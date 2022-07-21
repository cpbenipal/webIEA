var CalendarActionTypeEnum = {
    "AddEvent":0,
    "EditEvent": 1,
    "CategoryGrid": 2,
    "EventGrid": 3,
    "AddCategory": 4,
    "DeleteCategory": 5,
    "EditCategory": 6,
    "ShowEvents": 7,
    "ShowPopupEvents": 8
};

var flexpageCalendar = {
    calendarContainer: "#calendar-block-edit",
    getCalendarContainerListMode: function() {
        return this.calendarContainer + " .list-mode";
    },
    getCalendarBlockSelector: function(selector) {
        return $("#calendar-block-edit " + selector);
    },
    refreshCategoryGrid: function() {
        if(window.fp_CalendarBlock_Grid)
            fp_CalendarBlock_Grid.Refresh();

        if(window.fp_CalendarEventBlock_Grid)
            fp_CalendarEventBlock_Grid.Refresh();
    },
    fp_Calendar_CustomButtonClick: function(s, e) {
        if (e.buttonID == 'btnDelete')
            fp_ConfirmDialog('Delete',
                'Are you sure you want to delete the event?',
                function() { s.DeleteRow(e.visibleIndex); });
        else if (e.buttonID == 'btnEdit') {
            flexpageCalendar.openDialogAddEvent(s.GetRowKey(e.visibleIndex));
        }
    },
    fp_Calendar_CustomEventButtonClick: function(s, e) {
        var $that = this;
        if (e.buttonID == 'btnDelete')
            fp_ConfirmDialog('Delete',
                'Are you sure you want to delete the category?',
                function() { s.DeleteRow(e.visibleIndex); });
        else if (e.buttonID == 'btnEdit') {
            flexpageCalendar.addEditCategoryToCalendar(s.GetRowKey(e.visibleIndex));
        }

        s.PerformCallback();
    },
    selectListMode: function(elem, isSelect) {
        if (isSelect) {
            elem.css({
                "background-color": "rgb(213, 213, 213)",
                "border-radius": "10px"
            });

            elem.find("img").css({
                "display": "block",
                "margin": "0 auto"
            });

            elem.find(".img-main").hide();

            elem.find(".img-hover").css({
                "margin": "0 auto",
                "display": "block"
            });

            elem.find("a").css("color", "rgb(35, 89, 142)");
        } else {
            elem.css({
                "background-color": "transparent",
                "border-radius": "10px"
            });

            elem.find("img").css({
                "display": "block",
                "margin": "0 auto"
            });

            elem.find(".img-hover").hide();

            elem.find(".img-main").css({
                "margin": "0 auto",
                "display": "block"
            });

            elem.find("a").css("color", "#979797");
        }
    },
    selectDefault: function() {
        var listMode = $(this.getCalendarContainerListMode());
        for (var i = 0; i < listMode.length; i++) {
            this.selectListMode(listMode.eq(i), listMode.eq(i).data("select"));
        }
    },
    openDialogAddEvent: function(id, date) {
        fp_popupControlOpen({ command: 'edit', blockType: 'CalendarEvent', blockID: id, alwaysCallOnClose: true }, function () {});     
    },
    openDialogCategory: function(id) {
        var $that = this;
        $that.getPleaseWaitDiv().show();
        window.flexpage = window.flexpage || {};
        window.flexpage.onClose = function() {
            if(window.fp_CalendarBlock_Grid)
                fp_CalendarEventBlock_Grid.Refresh();
            if (window.fp_CalendarEventBlock_Grid)
                fp_CalendarEventBlock_Grid.Refresh();

            $("#dialog-iframe").remove();
            $that.getPleaseWaitDiv().hide();
        };

        this.openIframe("/CalendarBlock/Categories?command=" + CalendarActionTypeEnum.AddCategory + "&id=" + id);       
    },
    openDialogShowEvents: function (date, id) {
        window.flexpage = window.flexpage || {};
        window.flexpage.isSaveEven = false;
        window.flexpage.onClose = function () {
            $("#dialog-iframe").remove();
            if (window.flexpage.isSaveEven)
                location.reload(false);
        };

        this.openIframe("/CalendarBlock/CalendarEventPopup?command=" + CalendarActionTypeEnum.ShowPopupEvents + "&date=" + date.toISOString() + "&id=" + id);       
    },
    openIframe: function (src) {
        $("<iframe/>",
            {
                id: "dialog-iframe",
                css: {
                    "position": "fixed",
                    "top": 0,
                    "left": 0,
                    "width": "100%",
                    "height": "100%",
                    "border": 0,
                    "z-index": 12001
                },
                src: src
            }).appendTo($("body"));
    },
    getPleaseWaitDiv: function() {
        return $("#pleaseWaitDiv");
    },
    sendAjax: function (action, data, callback, controller) {
        if (!controller) {
            controller = "CalendarBlock";
        }
        var $that = this;
        this.getPleaseWaitDiv().show();
        $.ajax({
            url: '/' + controller + '/' + action,
            type: "POST",
            data: data,
            success: function(result) {
                callback(result);
                $that.getPleaseWaitDiv().hide();
                $that.refreshCategoryGrid();
            }
        });
    },
    getCategoryContent: function() {
        return $("#AddCalendarCategoryContent");
    },
    getSpeed: function() {
        return 1000;
    },
    addEditCategoryToCalendar: function(id) {
        var $that = this;
        $that.getPleaseWaitDiv().show();
        window.flexpage = window.flexpage || {};
        window.flexpage.onClose = function() {
            if(window.fp_CalendarBlock_Grid)
                fp_CalendarEventBlock_Grid.Refresh();

            if (window.fp_CalendarEventBlock_Grid)
                fp_CalendarEventBlock_Grid.Refresh();

            $("#dialog-iframe").remove();
            $that.getPleaseWaitDiv().hide();
        };

        this.openIframe("/CalendarBlock/Categories?command=" + CalendarActionTypeEnum.EditCategory + "&id=" + id);   
    },
    getDataFromCategoryForm: function() {
        return $("#CategoryForm").serialize();
    },
    getCalendarAddEditDayForm: function() {
        return $(".table-calendar #CalendarAddEditDayForm").serialize();
    },
    saveCategory: function() {
        if ($("#Name_CurrentLocalization").val().trim() == "") {
            alert("Event name is empty!");
            return;
        }

        var $that = this;
        this.sendAjax(
            "AddEditCategory",
            $that.getDataFromCategoryForm(),
            function(result) {
                $('#flexpage-popup-control_1').modal("hide");
                setTimeout(function () { window.parent.flexpage.onClose(); }, 1000);
            }
        );
    },
    saveAddNewEvent: function() {
        if (StartDate.GetDate() > EndDate.GetDate()) {
            alert("Invalid time interval");
            return;
        }

        if ($(".table-calendar #CalendarAddEditDayForm #Title_CurrentLocalization").val().trim() == "") {
            alert("Title is empty!");
            return;
        }

        if ($(".table-calendar #CalendarAddEditDayForm #Place_CurrentLocalization").val().trim() == "") {
            alert("Place is empty!");
            return;
        }

        $("#Notes_Texts_" + this.GetSelectedLangCode() + "_").val(fp_HtmlEditor_Notes.GetHtml());
        $("#Notes_CurrentText").val(fp_HtmlEditor_Notes.GetHtml());

        this.sendAjax(
            "CalendarAddEditDay",
            this.getCalendarAddEditDayForm(),
            function () {
                var popup = $('#flexpage-popup-control_' + $(".table-calendar #CalendarAddEditDayForm").data("id"));
                popup.modal("hide");
                setTimeout(function () { window.parent.flexpage.onClose(); }, 1000);
            }
        );
    },
    changeLanguageCategory: function(id, code) {
        $("#Name_Localizations_"+this.GetSelectedLangCode()+"_").val($("#Name_CurrentLocalization").val());
        //$("#HiddenInputCategory #NewLanguageCode").val(code);
        var $that = this;
        var form = this.getDataFromCategoryForm() + "&lang=" + code;

        this.sendAjax(
            "ChangeLanguageCategory",
            form,
            function(result) {
                $that.getCalendarBlockSelector(".modal-body")
                    .html(result);
                $that.getCalendarBlockSelector(".event-colorpicker")
                    .colorpicker();

                $("#Name_CurrentLocalization").val($("#Name_Localizations_"+code+"_").val());
            }
        );
    },
    GetSelectedLangCode: function() {
        return $(".languages .language-selected").data("code");
    },
    changeLanguageDay: function(id, code) {
        $("#Title_Localizations_"+this.GetSelectedLangCode()+"_").val($("#Title_CurrentLocalization").val());
        $("#Place_Localizations_"+this.GetSelectedLangCode()+"_").val($("#Place_CurrentLocalization").val());
        $("#Notes_Texts_" + this.GetSelectedLangCode() + "_").val(fp_HtmlEditor_Notes.GetHtml());
        $("#Notes_CurrentText").val(fp_HtmlEditor_Notes.GetHtml());
        $("#HiddenInputDay #NewLanguageCode").val(code);
        
        this.sendAjax(
            "ChangeLanguageEvent",
            this.getCalendarAddEditDayForm(),
            function(result) {
                $(".table-calendar .modal-body").html(result);
                $("#Title_CurrentLocalization").val($("#Title_Localizations_"+code+"_").val());
                $("#Place_CurrentLocalization").val($("#Place_Localizations_"+code+"_").val());
                $("#Place_CurrentLocalization").val($("#Place_Localizations_"+code+"_").val());
                fp_HtmlEditor_Notes.SetHtml($("#Notes_Texts_" + code + "_").val());
                $('.modal-body').ready(function () {
                    $('[data-toggle="tooltip"]').tooltip({
                        placement: 'top'
                    });
                });
            }
        );
    },
    deleteEvent: function (id, date, blockId) {
        var $that = this;
        fp_ConfirmDialog('Delete',
            'Are you sure you want to delete the event?',
            function () {
                $that.sendAjax(
                    "DeleteEvent",
                    {
                        id: id
                    },
                    function (result) {
                        $that.updateListEvents(date, blockId);
                    },
                    "Events"
                );
            });
    },
    favoriteEvent: function (date, calendarID, eventID) {
        var $that = this;
        fp_ConfirmDialog('Add to favorites',
            'Are you sure you want to add this event to favorites?',
            function () {
                console.log("favoriteEvent");
                var _url = document.referrer;
                if (_url.includes("?"))
                    _url += '&';
                else
                    _url += '?';
                // _url += "eventID=" + eventID;
                _url += "calendarDate=" + date + "&calendarID=" + calendarID;
                $that.sendAjax(
                    "AddEventToFavorites",
                    {
                        date: date,
                        calendarID: calendarID,
                        eventID: eventID,
                        url: _url
                    },
                    function (result) {
                        $(".table-celendar-event-popup #partialEvents").html(result);
                        flexpageCalendar.repositionEventPopup();
                    }
                );
            });
    },
    updateListEvents: function (date, id) {
        this.sendAjax(
            "CalendarEventPopup",
            {
                command: CalendarActionTypeEnum.ShowEvents,
                date: date,
                id: id,
            },
            function (result) {
                $(".table-celendar-event-popup #partialEvents").html(result);
                flexpageCalendar.repositionEventPopup();
            }           
        );
    },
    repositionEventPopup: function() {
        var modal = flexpageCalendar.getCalendarBlockSelector(".modal"),
            dialog = flexpageCalendar.getCalendarBlockSelector(".modal-dialog");
        
        modal.css('display', 'block');
        dialog.css("margin-top", Math.max(0, ($(window).height() - dialog.height()) / 2));
    },
    showMultiMonthMode: function (obj) {
        if (obj.val() === "1") 
            $("#MultiMonth").fadeIn();
        else 
            $("#MultiMonth").fadeOut();
    },
    dateTitiel: {}
}

$(function () {
   
    $("body").on("click", flexpageCalendar.calendarContainer+" #btnAddNewEvent", function() {
        flexpageCalendar.openDialogAddEvent(-1);
    });

    $("body").on("click", flexpageCalendar.calendarContainer + " #AddNewCategoryToCalendar", function () {
        flexpageCalendar.openDialogCategory(-1);
    });

    $("body").on("click", flexpageCalendar.calendarContainer + " #saveCategory", function () {
        flexpageCalendar.saveCategory();
    });

    $("body").on("click", ".table-calendar #saveAddNewEvent", function () {
        window.parent.flexpage.isSaveEven = true;
        flexpageCalendar.saveAddNewEvent();
    });

    $("body").on("mouseover", flexpageCalendar.getCalendarContainerListMode(), function() {
        flexpageCalendar.selectListMode($(this), true);
    });

    $("body").on("mouseout", flexpageCalendar.getCalendarContainerListMode(), function() {
        if($(this).data("select") === false)
            flexpageCalendar.selectListMode($(this), false);
    });

    $("body").on("click", flexpageCalendar.getCalendarContainerListMode(), function() {
        flexpageCalendar.selectListMode($(flexpageCalendar.getCalendarContainerListMode()), false);
        flexpageCalendar.selectListMode($(this), true);

        $(flexpageCalendar.getCalendarContainerListMode()).data("select", false);
        $(this).data("select", true);
        $("#hiddenListMode").val($(this).find("a[data-target]").data("target"));
    });

    $("body").on("change", flexpageCalendar.calendarContainer +" #ddlMultiMonthViewMode", function() {
        flexpageCalendar.showMultiMonthMode($(this));
    });
});

$(document).ready(function () {
    if (typeof queryString === 'undefined')
        return;
    // var params = document.URL.slice(document.URL.indexOf('?') + 1).split('&');
    var params = queryString.split('&');
    var calendarDate = undefined;
    var calendarId = undefined;
    var key = "";
    var value = "";
    for (var i = 0; i < params.length; i++) {
        chunks = params[i].split('=');
        key = chunks[0].trim().toLowerCase();
        value = chunks[1];
        if (key === 'calendarid') {
            calendarId = value;
        }
        else
            if (key === 'calendardate') {
                calendarDate = new Date(value);
            }
    }
    if ((calendarId !== undefined) && (calendarDate !== undefined))
    {
        flexpageCalendar.openDialogShowEvents(calendarDate, calendarId);
    }   
});

