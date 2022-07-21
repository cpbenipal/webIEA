var fp_popup_number = 0;

function fp_reload() {
    location.reload();
}

function fp_popupControlChangeTitle(title, content) {
    alert($(content).parents('.modal-dialog').first().find('.modal-title').html(title));
    $(content).parents('.modal-dialog').first().find('.modal-title').html(title);
}

function GreateIframe(params) {
    console.log('params',params.blockListID);
    var _src = "/Flexpage/GetPopup?blocklistID=" +
        (params.blockListID ? params.blockListID : 0) +
        "&number=" +
        (++fp_popup_number) +
        "&blockAfter=" +
        (params.blockAfter ? params.blockAfter : 0) +
        '&pageUrl=' + (params.url ? params.url : '') +
        '&OneButtonText=' + (params.OneButtonText ? params.OneButtonText : '') +
        "&allowSave=" + (typeof params.allowSave === 'undefined' ? 1 : params.allowSave) +
        "&parameters="+params.parameters;
    return $("<iframe/>",
        {
            id: "dialog-iframe",
            css: {
                "position": "fixed",
                "top": 0,
                "left": 0,
                "width": "100%",
                "height": "100%",
                "border": 0,
                "z-index": 1050,
            },
            src: _src
        });
}
function fp_popupControlOpen(params, onCancel) {
    console.log(params);
    window.flexpage = window.flexpage || {};
    window.flexpage.params = params;
    window.flexpage.onCancel = onCancel;
    window.flexpage.onClose = function () {
        $("#dialog-iframe").remove();
    };
    var ifrm = GreateIframe(params);
    var appnd = ifrm.appendTo("body");
    var err = appnd.ajaxError(function (e) {
        $("#dialog-iframe").remove();
        GreateIframe(params).appendTo("body");
    });
    //var err = appnd.error(function (e) {
    //    $("#dialog-iframe").remove();
    //    GreateIframe(params).appendTo("body");
    //});
}