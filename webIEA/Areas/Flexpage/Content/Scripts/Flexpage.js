//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
//and recompile the file in the menu (right click)
//debug true to disable minimization files EventManager

var fp_settings = fp_settings||{};


function fp_initSetting(setting, value) {
    if (setting)
        fp_settings[setting] = value;
}

function fp_deleteBlock(btn, blockID, blockName, blocklistID) {
    if (!fp_settings.deleteBlockUrl) {
        alert('Delete block url is not configured');
        return;
    }

    fp_ConfirmDialog('Delete', 'Are you sure you want to delete the block?', function() {
        $.ajax({
            url: fp_settings.deleteBlockUrl,
            data: { 'blockID': blockID, 'blockName': blockName, 'blocklistID': blocklistID },
            type: "post",
            cache: false,
            success: function (result) {
                $(btn).parents('.flexpage-blockWrapper').first().detach();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_deleteNewsRecord(btn, blockID) {
    if (!fp_settings.deleteBlockUrl) {
        alert('Delete block url is not configured');
        return;
    }

    fp_ConfirmDialog('Delete', 'Are you sure you want to delete the news?', function() {
        $.ajax({
            url: fp_settings.deleteBlockUrl,
            data: { 'blockID': blockID, 'blockName': 'Event' },
            type: "post",
            cache: false,
            success: function (result) {
                $(btn).parents('.newsRec-container').first().detach();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_moveBlock(id, blocklistID, direction) {
    if (!fp_settings.moveBlockUrl) {
        alert('Url for move block is not configured');
        return;
    }

    fp_ConfirmDialog('Move block','Are you sure want to move the block?', function() {
        $.ajax({
            url: fp_settings.moveBlockUrl,
            data: { 'blockID': id, 'blocklistID': blocklistID,'direction': direction },
            type: "post",
            cache: false,
            success: function (result) {
                location.reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_StoreFromHtmlEditorToHidden(fieldName) {
    eval('$("#' + fieldName + '_CurrentText").val(fp_HtmlEditor_' + fieldName + '.GetHtml());');
}

function fp_ConfirmDialog(title, text, onconfirm,oncancel) {
    $('body > #fp_confirm_dialog').detach();

    var dialog = $('<div class="modal fade" id="fp_confirm_dialog" tabindex="-1" role="dialog" ></div>')
        .append($('<div class="modal-dialog mini"></div>')
            .append($('<div class="modal-content"></div>')
                .append($('<div class="modal-header"></div>')
                    .append($('<h3 class="modal-title"></h3>').html(title)))
                .append($('<div class="modal-body"></div>').append($('<p>').html(text)))
                .append($('<div class="modal-footer"></div>')
                    .append($('<div class="col-md-6 pull-right right"></div>')
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" class="ok_btn">OK</a>'))
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" aria-label="Close">CANCEL</a>'))
                    )
                )
            )
        ).appendTo($('body'));

    $('body > #fp_confirm_dialog .ok_btn').on('click', onconfirm);
    if (oncancel) {
        $('body > #fp_confirm_dialog [aria-label="Close"]').on('click', oncancel);
    }
    $('body > #fp_confirm_dialog').modal();
}

function fp_WarningDialog(title, text, onAction) {
    $('body > #fp_confirm_dialog').detach();

    var dialog = $('<div class="modal fade" id="fp_confirm_dialog" tabindex="-1" role="dialog" ></div>')
        .append($('<div class="modal-dialog mini"></div>')
            .append($('<div class="modal-content"></div>')
                .append($('<div class="modal-header"></div>')
                    .append($('<h3 class="modal-title"></h3>').html(title)))
                .append($('<div class="modal-body"></div>')
                    .append($('<p>').html(text)))
                .append($('<div class="modal-footer"></div>')
                    .append($('<div class="col-md-6 pull-right right"></div>')
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" class="ok_btn">OK</a>'))))))
        .appendTo($('body'));

    $('body > #fp_confirm_dialog .ok_btn').on('click', onAction);

    $('body > #fp_confirm_dialog').modal();
}

function fp_TwoButtonsDialog(title, text, firstbutton, secondbutton, onconfirm) {
    $('body > #fp_twoButtons_dialog').detach();

    var dialog = $('<div class="modal fade" id="fp_twoButtons_dialog" tabindex="-1" role="dialog" ></div>')
        .append($('<div class="modal-dialog mini"></div>')
            .append($('<div class="modal-content"></div>')
                .append($('<div class="modal-header"></div>')
                    .append($('<h3 class="modal-title"></h3>').html(title)))
                .append($('<div class="modal-body"></div>').append($('<p>').html(text)))
                .append($('<div class="modal-footer"></div>')
                    .append($('<div class="col-md-6 pull-right right"></div>')
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" class="first_btn">' + firstbutton + '</a > '))
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" class="second_btn">' + secondbutton + '</a > '))
                        .append($('<a href="javascript:void(0)" data-dismiss="modal" aria-label="Close">CANCEL</a>'))
                    )
                )
            )
        ).appendTo($('body'));

    $('body > #fp_twoButtons_dialog .first_btn').on('click', function () { onconfirm(0); });
    $('body > #fp_twoButtons_dialog .second_btn').on('click', function () { onconfirm(1); });

    $('body > #fp_twoButtons_dialog').modal();
}

function fp_reload() {
    location.reload();
}

function fp_toggleTabVisibility(tabBlock, tabID) {
    if (!fp_settings.toggleTabVisibilityUrl) {
        alert('Url for toggle tab visibility is not configured');
        return;
    }

    $.ajax({
        url: fp_settings.toggleTabVisibilityUrl,
        data: { 'tabID': tabID },
        type: "post",
        cache: false,
        success: function (result) {
            $(tabBlock).children('img').attr("src", result);
        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    });
}


function fp_toggleBlockVisibility(block, id) {
    if (!fp_settings.toggleBlockVisibilityUrl) {
        alert('Url for toggle block visibility is not configured');
        return;
    }
    $.ajax({
        url: fp_settings.toggleBlockVisibilityUrl,
        data: { 'blockID': id },
        type: "post",
        cache: false,
        success: function (result) {
            if (result == "True") {
                $(block).children('img.flexpage-icon-menu__show').show(result);
                $(block).children('img.flexpage-icon-menu__hide').hide(result);
            } else {
                $(block).children('img.flexpage-icon-menu__show').hide(result);
                $(block).children('img.flexpage-icon-menu__hide').show(result);
            }
            //$(block).down().src = result;
        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    });
}

function fp_cutBlock(blockID, blocklistID) {
    if (!fp_settings.cutBlockUrl) {
        alert('Url to cut block is not configured');
        return;
    }
    fp_ConfirmDialog('Cut block', 'Are you sure want to cut the block?', function () {
        $.ajax({
            url: fp_settings.cutBlockUrl,
            data: { 'blockID': blockID, 'blocklistID': blocklistID },
            type: "post",
            cache: false,
            success: function (result) {
             //   alert("Cutted");
                fp_reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_copyBlock(blockID, blocklistID) {
    if (!fp_settings.copyBlockUrl) {
        alert('Url to copy block is not configured');
        return;
    }
    fp_ConfirmDialog('Copy block', 'Are you sure want to copy the block?', function () {
        $.ajax({
            url: fp_settings.copyBlockUrl,
            data: { 'blockID': blockID, 'blocklistID': blocklistID},
            type: "post",
            cache: false,
            success: function (result) {
                fp_reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_PasteBlock(pasteafterID, listblockID) {
    if (!fp_settings.pasteBlockUrl) {
        alert('Url to paste block is not configured');
        return;
    }

    fp_ConfirmDialog('Paste block', 'Are you sure want to paste the block?', function () {
        $.ajax({
            url: fp_settings.pasteBlockUrl,
            data: { 'pasteafterID': pasteafterID, 'targetBlocklistID': listblockID },
            type: "post",
            cache: false,
            success: function (result) {
                fp_reload();
              //  alert("Pasted");
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_addToRss(blockID) {
    if (!fp_settings.addToRssUrl) {
        alert('Url to enable rss is not configured');
        return;
    }

    fp_ConfirmDialog('RSS', 'Are you sure want to change RSS?', function () {
        $.ajax({
            url: fp_settings.addToRssUrl,
            data: { 'blockID': blockID },
            type: "post",
            cache: false,
            success: function (result) {
                fp_reload();
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
    });
}

function fp_sliderCheckBoxClicked(btn) {
    var ch = $(btn).find('input[type=checkbox]');
    ch.prop("checked", !ch.prop("checked"));
}

// mail scrambling

function fp_emailScrambling_encodeElement(strToEncode) {
    return unescape(strToEncode);
}

function fp_emailScrambling_descriptElement(id) {
    var el = document.getElementById(id);
    if (el != null && el.title != '') {
        el.innerHTML = fp_emailScrambling_encodeElement(el.title.replace(/(.{2})/g, '%$1'));
        el.title = '';
    }
}

var fp_loadScripts = function (url,type, afterFnc) {
    if (fp_settings.debug === true && url.substring(url.length - 7, url.length-2)===".min.") {
        url = url.substring(0, url.length - 6) + "js";
    }
    var head = document.getElementsByTagName("head")[0];
    var scripts = [...head.getElementsByTagName("script")];
    scripts = scripts.filter(scr => scr.getAttribute("src") === url);
    if (scripts.length === 0) {
        var e = document.createElement("script");
        e.src = url;
        e.type = "text/javascript";
        document.getElementsByTagName("head")[0].appendChild(e);
    }
    if (type && afterFnc) {
        if (fp_settings.debug == true) {
            //console.log("loading: " + type);
        }
       var typeValue = window[type];
        if (typeof typeValue !== 'undefined') {
            if (fp_settings.debug == true) {
                console.log("loaded: " + type);
            }
           afterFnc();
        } else {
            if (fp_settings.debug == true) {
                console.log("no load: " + type);
            }
            setTimeout(function () { fp_loadScripts(url, type, afterFnc) }, 1500);
        }
    }
}

jQuery(document).ready(function () {
    jQuery("span[fpscrambledmail][id][title]").each(function (index, item) {
        fp_emailScrambling_descriptElement(jQuery(item).prop("id"));
    });
});

function deleteRowInDevexpressGrid(table, id) {
    fp_ConfirmDialog('Delete', 'Are you sure you want to delete?', function () {
        table.DeleteRow(id);
    });
}
