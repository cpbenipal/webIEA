function fp_webFormSucessSubmit(result) {
    // console.log("fp_webFormSucessSubmit: " + JSON.stringify(result));
    if (result.url) {
        window.location.href = result.url;
    }
}

function fp_FrontendPostAjaxForm(formContainer, addFormData) {
    var form = jQuery(formContainer).find('form').first();
    if (addFormData) {
        for (var prm in addFormData) {
            form.append('<input type="hidden" name="' + prm + '" value="' + addFormData[prm] + '"/>');
        }
    }

    //the name of the function - STRING - not delegate
    //var cf = "fp_AjaxFormComplete('" + formContainer + "'); ";
    //if (onComplete)
    //    cf += onComplete;
    //form.attr('data-ajax-success', cf);
    form.submit();
    //$("#fp_popupPleaseWaitDiv").css("display", "none");
    return false;
}



function fp_webFormFileUploaded(sender, event) {
    var container = $("input[name='" + sender.name+ "']").parents(".webform-container").first();
    console.log("fp_webFormFileUploaded:");
    $("#" + sender.name + "Hidden").val(event.callbackData);
    fp_FrontendPostAjaxForm('#' + container.attr('id'), { command: 'fileuploaded', parameters: event.callbackData }, 'fp_TrackInitialize(' + true + ')');
}

function fp_webFormInputFileChanged(sender) {
    $(sender).next().css("visibility", "visible");
}

function fp_webFormClearInputFile(sender) {
    var fn = $("#" + sender).val();
    $("#" + sender).val("");
    var container = $("#" + sender).parents(".webform-container").first();
    fp_FrontendPostAjaxForm('#' + container.attr('id'), { command: 'removefile', parameters: fn }, 'fp_TrackInitialize(' + true + ')');
    return false;
}

//function fp_webFormSubmit(formContainer, addFormData, onComplete) {

//    console.log("fp_webFormSubmit: formContainer = " + formContainer);
//    var form = jQuery(formContainer).find('form').first();
//    if (addFormData) {
//        for (var prm in addFormData) {
//            form.append('<input type="hidden" name="' + prm + '" value="' + addFormData[prm] + '"/>');
//        }
//    }
//    //the name of the function - STRING - not delegate
//    var cf = "fp_webFormSubmitComplete(); ";
//    if (onComplete)
//        cf += onComplete;
//    form.attr('data-ajax-success', cf);
//    form.submit();
//    return false;
//}
