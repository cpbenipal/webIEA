var fp_htmleditor_fullscreen = false;

function fp_HtmlEditor_Init(s, e) {
    fp_htmleditor_fullscreen = false;
}

function fp_HtmlEditor_Command(s, e) {
    if (e.commandName == 'fullscreen') {
        if (!fp_htmleditor_fullscreen) {
            var editorID = s.name;
            var w = $('[id^="flexpage-popup-control"] .modal-dialog').css('margin-left');
            var h = $('[id^="flexpage-popup-control"] .modal-dialog').css('margin-top');
            //change position
            $('[id^="flexpage-popup-control"]').find('div.modal-body').find('div.dxhe-coverDiv').css('display', 'none');
            var editor = $('[id^="flexpage-popup-control"]').find('div.modal-body').find('#' + editorID);
            $(editor).css('top', '-' + h);
            $(editor).css('left', '-' + w);   
        }
        fp_htmleditor_fullscreen = !fp_htmleditor_fullscreen;
    }
}

    $(document).ready(function () {
        function refreshData() {      //to get dropzone in the right place      
            try {                        
                var getid = document.querySelector('[id$="FileManager_Splitter_Upload_InlineDropZone"]').id;
                $("#" + getid).css("top", "");
                $("#" + getid).css("left", "");              
            }
            catch(err){ }
            setTimeout(refreshData, 250);
        }
        refreshData(); // execute function
    });