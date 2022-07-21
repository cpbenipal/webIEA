var GalleryFlexpage = function ($itemsPerPage, $modelId) {
    this.itemsPerPage = $itemsPerPage;
    this.glBlockClassName = '.gl-block-' + $modelId;
    this.glBlockLink = this.glBlockClassName + " a";
    this.glBlockImage = this.glBlockClassName + " a img";
    this.glBlock = $(this.glBlockClassName);
};

GalleryFlexpage.prototype.setActualSize = function () {
    if ($(this.glBlock).data('actualsize') === false) {
        var glHeight = 0,
            rows = [];
        for (var i = 0; i < this.glBlock.length; i++) {
            rows.push(this.glBlock[i]);

            if ($(this.glBlock[i]).height() >= glHeight) {
                glHeight = $(this.glBlock[i]).height();
            }

            if (rows.length == this.itemsPerPage) {
                var imgHaight = $(rows[0]).find("img").height();
                if (imgHaight > glHeight) {
                    $(rows).height(imgHaight);
                } else {
                    $(rows).height(glHeight);
                }
                rows = [];
                glHeight = 0;
            }
        }
    }
};

GalleryFlexpage.prototype.GetWidthParentBlock = function () {
    var widthParentBlock = 20;
    for (var i = 0; i < this.itemsPerPage; i++) {
        widthParentBlock += $(this.glBlock[i]).find('img').width();
    }

    return widthParentBlock;
};

GalleryFlexpage.prototype.setCssGlBlock = function ($float, $left, $marginLeft) {
    $(this.glBlock).css({
        'float': $float,
        'left': $left,
        'margin-left': $marginLeft
    });
};

GalleryFlexpage.prototype.resizeGalery = function () {
    var widthParentBlock = this.GetWidthParentBlock();

    if ($(this.glBlock).parents('.galleryInnerContainer').width() <= widthParentBlock) {
        // Removed image resizing in #8367
    } else {
        this.setCssGlBlock('left', 0, 0);

        if ($(this.glBlock).data('actualsize') === false)
            $(this.glBlockClassName).parent().css('min-width', widthParentBlock + 'px');
    }
};

GalleryFlexpage.prototype.EnableZoom = function () {
    var $this = this;
    $(this.glBlockImage).mouseover(function () {
        var zoom = $(this).parents($this.glBlockClassName).data('zoomfactor');
        zoom = zoom > 0 ? zoom : 1;
        $(this).css('transform', 'scale(' + zoom + ')');
    });

    $(this.glBlockImage).mouseout(function () {
        $(this).css('transform', 'scale(1)');
    });
};

GalleryFlexpage.prototype.init = function () {
    var $this = this;
    this.setActualSize();

    if ($(this.glBlock).data('actualsize') === false) {
        this.resizeGalery();

        $(window).resize(function () {
            $this.resizeGalery();
        });
    }
}