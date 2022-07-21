$(document).ready(function () {
    function hideSubmenu(submenu) {
        if (submenu.parent().find(":hover").length > 0) {           
            return;
        }
        submenu.hide();
    }
    $(".breadcrumbs-link").hover(
        function () {
            linkId = $(this).attr('id');
            submenu = $("#" + linkId + "-submenu");
            submenu.show();
        },
        function () {
            linkId = $(this).attr('id');
            submenu = $("#" + linkId + "-submenu");
            setTimeout(function () {hideSubmenu(submenu) }, 1000);
        });
    $(".breadcrumbs-submenu").hover(
        null,
        function () {
            submenu = $(this).attr('id');
            submenu = $("#" + submenu);
            setTimeout(function () { hideSubmenu(submenu) }, 1000);
        });

    $(".breadcrumbsArrow").on("click", function () {
        let Id = $(this).attr('id').replace("breadcrumbsArrow", "");
        let submenu = $("#breadcrumbs-link-" + Id + "-submenu");
        if ($(submenu).is(":visible")) {
            submenu.hide();
        } else {
            submenu.show();
        }
        
    });
});
