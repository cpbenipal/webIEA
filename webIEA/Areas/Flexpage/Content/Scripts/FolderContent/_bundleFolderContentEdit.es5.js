//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
//and recompile the file in the menu (right click)
//Web:Debug true to disable minimization in AppSettings
'use strict';

function initContent() {
    $('.sortable-folder-content').sortable({
        handle: '.dnd-action',
        cancel: ''
    }).disableSelection();

    $(".toggle-switch-custom").each(function (i, el) {
        el = $(el);
        el.click(function (e) {
            var el = $(e.target);
            el.parents('.form-fields').first().next().toggleClass("opacity-field");
        });
        if (el.is(':checked')) {
            el.parents('.form-fields').first().next().toggleClass("opacity-field");
        }
    });

    $(".toggle-switch").click(function () {
        $(this).addClass('tracker-dirty');
    });

    $('select[name="PagingMode"]').on('change', function (event, elem) {
        var value = $(elem).val();
        if (value == 2) {
            $('[name="PagingSize"]').prop('disabled', true);
        } else {
            $('[name="PagingSize"]').prop('disabled', false);
        }
    });
    $("[name='SourceType']").on("click", function (e) {
        var val = $(e.target).val();
        if (fp_settings.debug == true) {
            console.log(val);
        }
        $(".sourceType").removeClass("dissable");
        $(".sourceType:not([for='SourceType" + val + "'])").addClass("dissable");
    });
    $("[name='WithParameter']").on("click", function (e) {
        var val = $(e.target).val() == "true";
        if (fp_settings.debug == true) {
            console.log(val);
        }
        $(e.target).val(!val);
    });
    if (fp_settings.debug == true) {
        console.log("initContent");
    }
    if (window["initFolderContent"]) {
        initFolderContent();
    }
    if (window["initContactEnumeration"]) {
        initContactEnumeration();
    }
    setTimeout(function () {
        fp_SetSourceBlockAlias(null);
    }, 200);
}
jQuery(function ($) {
    initContent();
});

function fp_SetSourceBlockAlias(alias) {
    $(window.parent.document).find(".fp_folderTreeList").each(function (i, item) {
        var name = $(item).find("[name='Name']").val();
        $('select[name="SourceBlockAlias"]').append($('<option>', {
            value: name,
            text: name
        }));
    });
    if (alias == null) {
        alias = $('input[name="SourceBlockAliasValue"]').first().attr("value");
        if (alias === "" && $('select[name="SourceBlockAlias"] option').length > 0) {
            alias = $('select[name="SourceBlockAlias"] option').first().attr("value");
        }
    }
    $('select[name="SourceBlockAlias"]').val(alias);
    $('input[name="SourceBlockAliasValue"]').val(alias);
    $('select[name="SourceBlockAlias"] option[value="' + alias + '"]').attr('selected', 'selected');
}

function fp_folderContent_setOrders(btn) {
    $("#" + fp_blockEditorContainer + ' .fp_fc_column').each(function (i, el) {
        $(el).find('input[name$="].Order"]').val(i);
    });
}

function fp_preSaveBlock() {
    fp_folderContent_setOrders();
}

function fp_folderContent_chooseFolder() {
    fp_popupControlOpen({ command: 'choose', blockType: 'FolderSelector', alwaysCallOnClose: true }, function () {});
}
var fp_blockEditorContainer = fp_blockEditorContainer || {};

function fp_folderContent_deleteColumn(btn) {
    var isDirty = IsFormDirty(fp_blockEditorContainer);
    fp_folderContent_setOrders();
    fp_PostAjaxForm("#" + fp_blockEditorContainer, { command: 'remove_column', parameters: $(btn).parents('.fp_fc_column').first().find('input[name$="].Order"]').val() }, 'fp_folderContent_afterChangeColumns(' + isDirty + ')');
    return false;
}

function fp_folderContent_addColumn(btn) {
    var alias = $('select[name="SourceBlockAlias"]').val();
    var isDirty = IsFormDirty(fp_blockEditorContainer);
    fp_folderContent_setOrders();
    fp_PostAjaxForm("#" + fp_blockEditorContainer, { command: 'add_column' }, 'fp_folderContent_afterChangeColumns(' + isDirty + ',"' + alias + '")');
}

function fp_folderContent_afterChangeColumns(isFormDirty, alias) {
    fp_TrackInitialize(isFormDirty);
    $("#" + fp_blockEditorContainer + ' .tabs-navigation li.active').removeClass('active');
    $("#" + fp_blockEditorContainer + ' .tab-pane.active').removeClass('active');

    $("#" + fp_blockEditorContainer + ' .tabs-navigation li a[href="#columns"]').parent().addClass('active');
    $("#" + fp_blockEditorContainer + ' .tab-pane[id="columns"]').addClass('in active');
    fp_SetSourceBlockAlias(alias);
    fp_folderContent_setOrders();
    initContent();
}
window.flexpage = window.flexpage || {};

window.flexpage.fp_afterObjectSelected = function (obj) {
    $('input[name=PWFolderName]').val(obj.name);
};

//the code in the file /Areas/Flexpage/Content/Scripts/Content/contentEdit.js and it is common to folderContent and ContactsEnumeration
"use strict";

function initFolderContent() {
    if (fp_settings.debug == true) {
        console.log("initFolderContent");
    }
}

