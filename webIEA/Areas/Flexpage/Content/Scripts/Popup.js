
var fp_popupScrollLeftPosition, fp_popupScrollTopPosition;

function fp_PopupTogglePleaseWaitDiv(container, value) {
    if (container !="[object Object]") {
        $(container).first().parents(".modal-dialog").first().children(".fp_popupPleaseWaitDiv").css("display", value);
    }
}

//function fp_PopupToggleButtonsContainer(container, value) {
//    console.log("fp_PopupToggleButtonsContainer");
//    $(container).parents(".modal-content").first().children(".fp_popupButtonsContainer").css("display", value);
//}

function fp_AjaxFormComplete(formContainer) {
    if (formContainer != "[object Object]") {
        fp_PopupTogglePleaseWaitDiv(formContainer, "none");
        // $(formContainer).parents(".modal-dialog").children(".fp_popupPleaseWaitDiv").css("display", "none");
        // console.log("fp_AjaxFormComplete");
        // $("#fp_popupPleaseWaitDiv").css("display", "none");
        $(formContainer).scrollLeft(fp_popupScrollLeftPosition);
        $(formContainer).scrollTop(fp_popupScrollTopPosition);
    }
    fp_reinitTooltip();
}

function fp_PostAjaxForm(formContainer, addFormData, onComplete) {
    if (typeof fp_prePostAjaxForm !== "undefined")
    {
        fp_prePostAjaxForm();	
	}
    fp_PopupTogglePleaseWaitDiv(formContainer, "block");
    // $(formContainer).parents(".modal-dialog").children(".fp_popupPleaseWaitDiv").css("display", "block");
    // console.log("fp_PostAjaxForm");
    // $("#fp_popupPleaseWaitDiv").css("display", "block");
    var form = jQuery(formContainer).find('form').first();
    fp_popupScrollLeftPosition = $(formContainer).scrollLeft();
    fp_popupScrollTopPosition = $(formContainer).scrollTop();
    if (addFormData) {
        for (var prm in addFormData) {
            form.append('<input type="hidden" name="' + prm + '" value="' + addFormData[prm] + '"/>');
        }
    }

    //the name of the function - STRING - not delegate
    var cf = "fp_AjaxFormComplete('" + formContainer + "'); "; 
    if (onComplete)		
        cf += onComplete;
    var onFailure = "fp_processUpdateFailure";
    form.attr('data-ajax-success', cf);    
    form.attr('data-ajax-failure', onFailure);
    form.submit();	
    //$("#fp_popupPleaseWaitDiv").css("display", "none");
    return false;
}

function fp_processUpdateFailure(xhr) {
    //For implementation of other failure results, cycle through xhr.status
    //Model validation errors use status code 422 (Unprocessable Entity), copy the following code there
    var data = xhr.responseJSON;
    var finaltext = "";
    if (data) {
        for (var key in data) {
            var message = "";
            for (var error in data[key].errors) {
                message += " " + data[key].errors[error];
            }
            finaltext += data[key].key + ":" + message + "\n";
        }
    } else if (xhr.responseText&&$(xhr.responseText).text().trim().split(".")[0]) {
        finaltext = $(xhr.responseText).text().trim().split(".")[0];
    } else {
        finaltext = xhr.statusText;
    }

    displayPopupMessage(finaltext);
    $("#fp_popupPleaseWaitDiv").css("display", "none");
}

function displayPopupMessage(text) {
    var area = document.getElementById("popup-message-area");
    var areatext = document.getElementById("popup-message-area-text");
    if (area)
        area.style.display = "block";
    if (areatext)
        areatext.innerHTML = text;
}

function fp_reinitTooltip() {
    jQuery('.modal-body').ready(function () {
        jQuery('[data-toggle="tooltip"]').tooltip({
            placement: 'top'
        });
    });
}
function fp_popupControlEndLoad() {
    
}

function fp_popupControlChangeTitle(title, content) {
    $(content).parents('.modal-dialog').first().find('.modal-title').html(title);
}

function fp_popupControlRefresh(elem, params, onCancel) {
    var popup = $(elem).parents('.modal-dialog').first().parent();
    if (params.command == "save") {
        fp_popupControlRefreshSave(popup, params, onCancel);
    } else {
        _fp_popupControlCommand(popup, params, onCancel);
    }
}
function fp_popupControlRefreshSave(popup, params, onCancel) {
    var idPostfix = popup.find('input[type="hidden"][name="IDPostfix"]').val();
    var blocklistID = popup.find('input[type="hidden"][name="BlocklistID"]').val();
    var blockAfter = popup.find('input[type="hidden"][name="BlockAfter"]').val();
    $.ajax({
        url: "/Admin/Update" + params.blockType,
        data: { command: 'save', BlockType: params.blockType, BlocklistID: blocklistID, BlockAfter: blockAfter, BlockAlias: "#create new#"},
        type: "post",
        cache: false,
        success: function (result) {
            window.parent.location.reload();
        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    });
}
function _fp_popupControlCommand(popup, params, onCancel) {
    //clear content
    popup.find('div[name="fp_PopupContent"]').html('');
    //remove footer
    popup.find('.modal-footer').html('');

    popup.find('input[type="hidden"][name="Command"]').val(params.command);
    popup.find('input[type="hidden"][name="BlockType"]').val(params.blockType);
    if (params.blockID === undefined)
        params.blockID = -1;
    if (isNaN(parseInt(params.blockID)))
        params.blockID = -1;
    popup.find('input[type="hidden"][name="ID"]').val(params.blockID);
    popup.find('input[type="hidden"][name="BlockAlias"]').val(params.blockAlias);
    popup.find('input[type="hidden"][name="Parameters"]').val(params.parameters);
    popup.find('input[type="hidden"][name="Title"]').val(params.title);

    var hf_blocklistID = popup.find('input[type="hidden"][name="BlocklistID"]');
    if ('blocklistID' in params)
        hf_blocklistID.val(params.blocklistID);
    popup.find('input[type="hidden"][name="Controller"]').val(params.controller);
    popup.find('input[type="hidden"][name="Action"]').val(params.action);
    popup.find('input[type="hidden"][name="Url"]').val(params.url);
    popup.find('input[type="hidden"][name="AllowSave"]').val(params.allowSave);

    if (params.alwaysCallOnClose)
        popup.attr('fp_prop_alwaysOnClose', '1');

    //remove on close
    popup.unbind('hide.bs.modal')
		.on('hide.bs.modal', function (e) {
			var dlg = $(e.target);
			if(!dlg.hasClass('not-removable')) {
				dlg.detach();
			}
            if (onCancel !== undefined)
                onCancel();            
        });
    fp_PostAjaxForm('#' + popup.attr('id') + ' .modal-body', {}, 'fp_popupControlEndLoad()');
}

function fp_blockEventAfterClose(elem) {
 /*   var popup = $(elem).parents('.modal-dialog').first().parent();
    if (!popup.attr('fp_prop_alwaysOnClose') || popup.attr('fp_prop_alwaysOnClose') !== '1')
        popup.unbind('hidden.bs.modal');*/
    if (window.parent.flexpage.onCancel) {
        window.parent.flexpage.onCancel(true);
    }
    window.parent.flexpage.onClose();
}

function fp_PopupContent_Complete(result) {
    if (result && result.responseText && result.responseText[0] == '{') {
        var json = JSON.parse(result.responseText);
        if (json.redirectTo) {
            window.location.href = json.redirectTo;
        }
    }
}

var SaveSarvice = (function() {
    var needShowConfirm = false;
    var message = 'Do you want to Save?';

    return {
        show: function(callback) {
            if (needShowConfirm) {
                fp_ConfirmDialog('Save',
                    message,
                    function() {
                        callback();
                    });
            } else {
                callback();
            }
        },
        needShow: function(value, msg) {
            needShowConfirm = value;
            if(msg)
                message = msg;
        },
        setMessage: function(msg) {
            message = msg;
        }
    };
}());

function fp_saveBlock(idPostfix, blocklistID, blockAfter, parameters) {
    if (parameters.search("dirtyConfirm") != -1) {
        fp_checkDirty(idPostfix, save, function () {
        });
    } else {
        save();
    }
    function save() {
        if ($('#fp_BlockEditorSaveBtn_' + idPostfix).hasClass('disabled')) {
            return;
        }
        else {
            $('#fp_BlockEditorSaveBtn_' + idPostfix).addClass('disabled');
        }

        $("#fp_popupPleaseWaitDiv").css("display", "block");
        var res = {};
        if (typeof fp_preSaveBlock !== "undefined")
            res = fp_preSaveBlock();
        if (window.parent.flexpage.fp_afterPreSaveBlock !== undefined)
            window.parent.flexpage.fp_afterPreSaveBlock(res);

        SaveSarvice.show(function () {
            fp_PostAjaxForm('#fp_BlockEditorContainer_' + idPostfix,
                { command: 'save', BlocklistID: blocklistID, BlockAfter: blockAfter, Parameters: parameters },
                'fp_AfterSave("' + idPostfix + '")');
        });
    }
}

function fp_AfterSave(idPostfix) {
    if (window.parent) {
        if(window.parent.flexpage.onCancel) {
            window.parent.flexpage.onCancel(true);
            window.parent.flexpage.onClose();
        }
        else
            window.parent.location.reload();
    } else {
        $("#flexpage-popup-control_" + idPostfix).modal('toggle');
    }

}

function fp_checkDirty(idpostfix,save,cancel) {
    let isDirty = false;
    $('#fp_PopupContent_' + idpostfix).find('[fortraking]').each(function () {
        let formID = $(this).attr('id');
        if (formID) {
            isDirty = IsFormDirty(formID);
        }
    });

    if (isDirty) {
        fp_ConfirmDialog('Close popup', 'Unsaved changes will be lost.</br>Are you sure you want to close the window ?', function () {
            save();
        }, cancel);
    } else {
        save();
    }
}

function fp_beforeClosePopUp(element, idpostfix) {
    fp_checkDirty(idpostfix, function () {
        fp_closePopUp(element);
    });
    // $("#dialog-iframe").remove();
    return false;
}

function IsFormDirty(formID) {
    let isDirty = false;
    if ($('#' + formID + ' .tracker-dirty').length !== 0) {
        isDirty = true;
    }
    return isDirty;
}

function fp_closePopUp(element) {
    $("[id^='flexpage-popup-control']").modal('hide');
    $("[id^='flexpage-popup-control']").collapse();
    fp_blockEventAfterClose(element);
}

function fp_ControlsForTracking(controlsID) {
    controlsID.forEach(function (elemID) {
        $(elemID).addClass('tracked');
        //if (elemID.startsWith('#')) {
        //    $(elemID).addClass('tracked');
        //}
        //else {
        //    $('#' + elemID).addClass('tracked');
        //}
    });
}

$(function() {
    $('.fp_insertBlockLine').click(function() {
        $(this).css('visibility','hidden');
        $(this).parent().children('.fp_insertBlock').slideDown('slow');
    });
    $('.fp_insertBlock a').click(function() {
        var elem = $(this); 
        $(this).parent().parent().children('.fp_insertBlock').fadeOut('slow',function() {
            elem.parent().parent().children('.fp_insertBlockLine').css('visibility','visible');
        });
    });
})