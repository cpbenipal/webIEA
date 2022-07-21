function fp_ActivateTab(tabID, event) {
    var tab = $('a[href="#' + tabID + '"]');

    if (tab) {
        tab.tab('show');
        $('html, body').stop().animate({
                scrollTop: tab.offset().top
            },
            2000);

        if (event)
            event.preventDefault();

        return true;
    }

    return false;
}

function initContent() {
    $(".toggle-switch-custom").each(function (i, el) {
        el = $(el);
        el.click(function (e) {
            var el = $(e.target);
            el.parents('.advanced-settings').first().find('.advanced-elements').toggleClass("opacity-field");
        });
        if (el.is(':checked')) {
            el.parents('.advanced-settings').first().find('.advanced-elements').toggleClass("opacity-field");
        }
    });
}

function fp_initTabsSlideshow(elementIDs, delay) {
    var links = elementIDs.map(function (id) { return $("#" + id + "-link") });
    var i = {
        current: 0,
        max: links.length,
        get next() {
            if (++this.current === this.max) {
                this.current = 0;
            }
            return this.current;
        }
    };
    
    setInterval(function() {
        links[i.next].click();
    }, delay);
}
