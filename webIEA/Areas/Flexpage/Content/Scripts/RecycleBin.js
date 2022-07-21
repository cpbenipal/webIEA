var RecycleBinClass = (function() {

    function showPopupMenu(args){
        if (typeof args.pageY == 'undefined') {
            args.pageY = args.event.htmlEvent.pageY;
        }
        if (typeof args.pageX == 'undefined') {
            args.pageX = args.event.htmlEvent.pageX;
        }
        var top = args.pageY - $('.recycle-grid').offset().top;
        var left = args.pageX - $('.recycle-grid').offset().left;
        $("#RecyclePopupMenu").attr('style', 'top:' + top + 'px; left:' + left + 'px; display:block; position:absolute;z-index: 20000;');
    }

    return {
        PopupMenuClick : function(s, e) {
            $('.dxmLite > div').attr('style', 'display:none;');

            if (e.item == undefined)
                return;

            if (e.item.name === 'Refresh') {
                publisherLog.Push(new DivLog("Table successfully updated"));
            }
            else if (e.item.name === 'Restore') {
                var ids = RecycleBinGrid.GetSelectedKeysOnPage();
                if (ids.length == 0)
                    return;
                $.ajax({
                    type: "POST",
                    url: '/Admin/RecycleBin_RestoreAction',
                    data: {ids:ids},
                    success: function () {
                        $("[class*='dxgvSelectedRow']").remove();
                        publisherLog.Push(new DivLog("Object successfully restored"));
                    }
                });

            }
            else if (e.item.name === 'Delete') {
                fp_ConfirmDialog('Delete', 'Do you really want to delete the selected files?', function () {
                    var ids = RecycleBinGrid.GetSelectedKeysOnPage();
                    if (ids.length == 0)
                        return;
                    $.ajax({
                        type: "POST",
                        url: '/Admin/RecycleBin_DeleteAction',
                        data: { ids: ids },
                        success: function () {
                            $("[class*='dxgvSelectedRow']").remove();
                            publisherLog.Push(new DivLog("Object successfully deleted"));
                        }
                    });
                });
            }
        },
        ContextMenu : function(s, e) {
            showPopupMenu({ 'pageY': e.pageY, 'pageX': e.pageX, 'event': e });
        },
        RowClick: function(s, e) {
            s.SelectRowOnPage(
                e.visibleIndex,
                !s.IsRowSelectedOnPage(e.visibleIndex));            
        }
    }
}()); 

function fp_initPopupMenuRecycle(s, e) {
    $(window).on("click", function () {
        $("#RecyclePopupMenu").css({ display:"none" });
    });
}