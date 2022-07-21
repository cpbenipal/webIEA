var fp_videoplaylists = {};

function fp_videoPlaylistSelectItem(container, id, sender, mediaType) {
    if (fp_videoplaylists[container] !== undefined)
        fp_videoplaylists[container].removeClass("videoplaylist-view-item-selected");
    var i = $(sender);
    i.addClass("videoplaylist-view-item-selected");
    fp_videoplaylists[container] = i;
    //$(container).css("visibility", "hidden");
    $.ajax({
        url: "Flexpage/UpdateMediaPlaylist",
        data: { 'ID': id, 'command': 'selectitem', 'parameters': '', 'mediaType': mediaType },
        type: "post",
        cache: false,
        success: function (result) {
            $(container).html(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
    return false;
}
