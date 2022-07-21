if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    };
}

Number.isInteger = Number.isInteger || function(value) {
    return typeof value === "number" && 
        isFinite(value) && 
        Math.floor(value) === value;
};

function getBoolean(value) {
    switch (value) {
        case true:
        case "true":
        case "True":
        case "TRUE":
        case 1:
        case "1":
        case "on":
        case "yes":
            return true;
        default:
            return false;
    }
}
function trimLastCommaAndTrailingWhiteSpace(stringData) {
    return stringData.replace(/,\s*$/, "");
}
function trimLastCollonAndTrailingWhiteSpace(stringData) {
    return stringData.replace(/;\s*$/, "");
}
function watermarkImage(elemImage, text) {
    // Create test image to get proper dimensions of the image.
    var testImage = new Image();
    testImage.onload = function () {
        var h = testImage.height, w = testImage.width, img = new Image();
        // Once the image with the SVG of the watermark is loaded...
        img.onload = function () {
            // Make canvas with image and watermark
            var canvas = Object.assign(document.createElement('canvas'), { width: w, height: h });
            var ctx = canvas.getContext('2d');
            ctx.drawImage(testImage, 0, 0);
            ctx.drawImage(img, 0, 0);
            // If PNG can't be retrieved show the error in the console
            try {
                elemImage.src = canvas.toDataURL('image/png');
            }
            catch (e) {
                console.error('Cannot watermark image with text:', { src: elemImage.src, text: text, error: e });
            }
        };
        // SVG image watermark (HTML of text at bottom right)
        img.src = 'data:image/svg+xml;base64,' + window.btoa(
            '<svg xmlns="http://www.w3.org/5000/svg" height="' + h + '" width="' + w + '">' +
            '<foreignObject width="100%" height="100%">' +
            '<div xmlns="http://www.w3.org/1999/xhtml">' +
            '<div style="position: absolute;' +
            'right: 0;' +
            'bottom: 0;' +
            'font-family: Tahoma;' +
            'font-size: 10pt;' +
            'background: #000;' +
            'color: #fff;' +
            'padding: 0.25em;' +
            'border-radius: 0.25em;' +
            'opacity: 0.6;' +
            'margin: 0 0.125em 0.125em 0;' +
            '">' + text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;") + '</div>' +
            '</div>' +
            '</foreignObject>' +
            '</svg>'
        );
    };
    testImage.src = elemImage.src;
}
function mutuallyExclusive(inputItem) {
    var checkedState = jQuery(inputItem).prop("checked")
    jQuery(inputItem)
        .closest('div.mutuallyExclusiveElementsContainer')
        .find('.mutuallyExclusiveElement:checked')
        .prop("checked", false).prop("value", false);

    jQuery(inputItem).prop("checked", checkedState).prop("value", true);
}
function applyMutuallyExclusiveness() {
    jQuery("div.mutuallyExclusiveElementsContainer").find(".mutuallyExclusiveElement").on("click", function () {
        var checkedState = jQuery(this).prop("checked")
        jQuery(this)
            .closest('div.mutuallyExclusiveElementsContainer')
            .find('.mutuallyExclusiveElement:checked')
            .prop("checked", false);

        jQuery(this).prop("checked", checkedState);
    });
}

function createPagination(targetContiner, pagination) {
        var pagingTotalItems = parseInt(targetContiner.attr("data-totalitems"));
        var pagingTheme = "compact-theme";
        var pagingPrevText = "Prev";
        var pagingNextText = "Next";
        var pagingLabelMap = [];
        var pagingEllipsePageSet = true;
        var pagingDisplayedPages = 5;

        if (targetContiner.attr("data-pagingstyles") == "Bullets") {
            pagingTheme = "bullets-theme";
            pagingPrevText = "";
            pagingNextText = "";
            pagingLabelMap = Array.apply(null, Array(pagingTotalItems)).map(String.prototype.valueOf, "&nbsp;");
            pagingEllipsePageSet = false;
            pagingDisplayedPages = (pagingTotalItems * 2) + 5;
        }
				
        else if (targetContiner.attr("data-pagingstyles") == "Buttons") {
            pagingTheme = "";
            pagingPrevText = "<<";
            pagingNextText = ">>";
            pagingLabelMap = [];
            pagingEllipsePageSet = true;
        }
        else if (targetContiner.attr("data-pagingstyles") == "Arrows") {
            pagingTheme = "arrow-theme";
            pagingPrevText = "<<<";
            pagingNextText = ">>>";
            pagingLabelMap = [];
            pagingEllipsePageSet = true;
        }

        $(pagination).pagination({
            items: pagingTotalItems,
            itemsOnPage: parseInt(targetContiner.attr("data-itemperpage")),
            prevText: pagingPrevText,
            nextText: pagingNextText,
            labelMap: pagingLabelMap,
            ellipsePageSet: pagingEllipsePageSet,
            displayedPages: pagingDisplayedPages,
            hrefTextPrefix: null,
            cssStyle: pagingTheme,// There are custom themes in simplePagination.css,
            onPageClick: function (dataObject, event) {
                var imageTransition = targetContiner.attr("data-imagetransition");
                if (imageTransition == "FadeInFadeOut") {
                    var previousItem = jQuery(targetContiner).find("div[data-pageindex='" + dataObject.previousPage + "']");
                    var currentItem = jQuery(targetContiner).find("div[data-pageindex='" + dataObject.currentPage + "']");

                    currentItem.animate({ opacity: 1, duration: 5000, display: "block", queue: false }, {
                        duration: targetContiner.attr("data-imageschangeinterval"),
                        easing: 'linear',
                        complete: function () {
                            var currentItem = jQuery(this);
                            currentItem.fadeIn();
                            currentItem.css("opacity", 1);
                        }
                    });
                    previousItem.animate({ opacity: 0, display: "none", duration: 5000, queue: false }, {
                        duration: targetContiner.attr("data-imageschangeinterval"),
                        easing: 'linear',
                        complete: function () {
                            previousItem.hide();
                            previousItem.css({ "left": "0px" });
                        },
                        start: function () {
                        }
                    });
                }
                else if (imageTransition == "Slideshow") {
                    var previousItem = jQuery(targetContiner).find("div[data-pageindex='" + dataObject.previousPage + "']");
                    var currentItem = jQuery(targetContiner).find("div[data-pageindex='" + dataObject.currentPage + "']");
                    currentItem.css({ right: ("-=" + currentItem.width() * 2), marginRight: ("-=" + currentItem.width() * 2) });
                    currentItem.show();
                    //updatePageNumbers(targetContiner);//update index for correct management of indexes
                    //jQuery(previousItem).addClass("left").one("transitionend", function () {
                    //    previousItem.hide();
                    //    previousItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                    //    previousItem.detach();
                    //    jQuery(targetContiner).find(".galleryInnerContainer").append(previousItem);
                    //});
                    //jQuery(previousItem).addClass("right").one("transitionend", function () {
                    //    currentItem.show();
                    //    currentItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                    //});
                    currentItem.animate({
                        display: "block",
                        right: "+=0px",
                        marginRight: "+=0px"
                    },
                        {
                            queue: true,
                            duration: targetContiner.attr("data-imageschangeinterval"),
                            easing: 'linear',
                            complete: function () {
                                var currentItem = jQuery(this);
                                currentItem.show();
                                currentItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                            }
                        });

                    previousItem.animate({
                        display: "none",
                        left: ("-=" + previousItem.width()),
                        marginLeft: ("-=" + previousItem.width()),
                        opacity: 0
                    },
                        {
                            queue: true,
                            duration: targetContiner.attr("data-imageschangeinterval"),
                            easing: 'linear',
                            complete: function () {
                                previousItem.hide();
                                previousItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                                previousItem.detach();
                                jQuery(targetContiner).find(".galleryInnerContainer").append(previousItem);
                            },
                            start: function () {
                            }
                        });
                }
                else {
                    jQuery(targetContiner).find("div[data-pageindex='" + dataObject.previousPage + "']").hide();
                    jQuery(targetContiner).find("div[data-pageindex='" + dataObject.currentPage + "']").show();
                }
                if(pagingTheme !== "bullets-theme" && pagingTheme !== "arrow-theme")
                    $(pagination).find('ul:first-child').addClass('pagination pagination-sm');
            }
        });

        if(pagingTheme !== "bullets-theme" && pagingTheme !== "arrow-theme")
            $(pagination).find('ul:first-child').addClass('pagination pagination-sm');
    }

jQuery(document).ready(function () {
    applyMutuallyExclusiveness();
    jQuery(".color-picker").colorpicker();

    var galleries = jQuery(".justified-gallery").justifiedGallery({
        rowHeight: 200,
        mobileRowHeight: 200,
        margins: 5,
        lastRow: "nojustify",
        fixedHeight: false,
        captions: true,
        captionsColor: "#000000",
        captionsOpacity: 0.7,
        randomize: false,
        maxRowHeight: 0,
        target: null,
        //refreshTime: 250,
        cssAnimation: true,
        captionsAnimationDuration: 500,
        imagesAnimationDuration: 300,
        captionsVisibleOpacity: 0.7,
        sizeRangeSuffixes: { 512: '_ltsmall', 1024: '_ltbig' },
    }).on('jg.complete', function () {
        var currentGallery = this;
        if (getBoolean(jQuery(currentGallery).attr("data-initializedonce")) === false) {
            jQuery(currentGallery).attr("data-initializedonce", true)
            var rowHeight = jQuery(currentGallery).attr("data-thumbnailheight");

            rowHeight = parseInt(rowHeight);
            if (Number.isInteger(rowHeight) === false) {
                rowHeight = 200;
            }
            var targetContiner = jQuery(currentGallery);
            var lookup = jQuery(currentGallery).attr("data-lookuptoapplypopupvalue");
            if (typeof (lookup) !== undefined && lookup !== null && lookup.trim() !== "") {
                targetContiner = jQuery(currentGallery).closest(jQuery(currentGallery).attr("data-lookuptoapplypopupvalue"));
            }

            var alignment = jQuery(currentGallery).attr("data-imagealignment");
            var rtl = false;
            var lastRow = "nojustify";
            if (alignment == "LeftToRight") {
                lastRow = "left";
            } else if (alignment == "RightToLeft") {
                lastRow = "right";
                rtl = true;
            } else if (alignment == "Justify" 
                && (parseInt(jQuery(targetContiner).attr("data-itemperpage")) <= 1 || jQuery(targetContiner).attr("data-renderthumbnailactualSize") == 'true')) {
                lastRow = "justify";
            }

            jQuery(currentGallery).justifiedGallery({
                rowHeight: rowHeight,
                mobileRowHeight: rowHeight,
                rtl: rtl,
                lastRow: lastRow
            });
            //if (jQuery(currentGallery).attr("data-enableZoom").toLocaleLowerCase() === 'true') {
            //    var zoomFactor = jQuery(currentGallery).attr("data-zoomfactor").
            //        zoomFactor = parseInt(zoomFactor);
            //    if (Number.isInteger(zoomFactor)) {
            //        if (zoomFactor < 1.01) {
            //            zoomFactor = 1.01;
            //        }
            //    } else {
            //        zoomFactor = 1.01;
            //    }
            //    jQuery(currentGallery).find("img").ezPlus({
            //        responsive: false,
            //        scrollZoom: true,
            //        imageCrossfade:true,
            //        easing: true,
            //        zoomType: "Lens",
            //        tint:true,
            //        minZoomLevel: zoomFactor
            //    });
            //}
        }
    });

    var initMagnificPopup = function (targetContiner) {
        if (getBoolean(targetContiner.attr("data-enableclicktoenlarge")) === true) {
            if (getBoolean(targetContiner.attr("data-popupadded")) === false) {
                targetContiner.attr("data-popupadded", 'true');
                var $images = targetContiner.find('.justified-gallery a,.galleryInnerContainer a');
                $images.magnificPopup({
                    type: 'image',
                    closeOnContentClick: false,
                    closeBtnInside: false,
                    mainClass: 'mfp-with-zoom mfp-img-mobile',

                    gallery: {
                        enabled: true,
                        navigateByImgClick: false,
                        preload: [0, 1], // Will preload 0 - before current, and 1 after the current image
                        arrowMarkup: '<button title="%title%" type="button" class="mfp-arrow mfp-arrow-%dir%"></button>', // markup of an arrow button

                        tPrev: 'Previous (Left arrow key)', // title for left button
                        tNext: 'Next (Right arrow key)', // title for right button
                        tCounter: '<span class="mfp-counter">%curr% of %total%</span>' // markup of counter
                    },
                    image: {
                        tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
                        verticalFit: true,
                        titleSrc: function (item) {
                            return item.el.attr('title') + ' &middot; <a class="image-source-link" href="' + item.el.attr('data-source') + '" target="_blank">image source</a>';
                        }
                    }
                    //Following not seems to work
                    //,
                    //zoom: {
                    //    enabled: jQuery(currentGallery).attr("data-enableZoom"),
                    //    duration: 300, // don't foget to change the duration also in CSS
                    //    easing: 'ease-in-out', // CSS transition easing function
                    //    opener: function (openerElement) {
                    //        // openerElement is the element on which popup was initialized, in this case its <a> tag
                    //        // you don't need to add "opener" option if this code matches your needs, it's defailt one.
                    //        return openerElement.is('img') ? openerElement : openerElement.find('img');
                    //    }
                    //},
                    //retina: {
                    //    ratio: 1, // Increase this number to enable retina image support.
                    //    // Image in popup will be scaled down by this number.
                    //    // Option can also be a function which should return a number (in case you support multiple ratios). For example:
                    //    // ratio: function() { return window.devicePixelRatio === 1.5 ? 1.5 : 2  }


                    //    replaceSrc: function (item, ratio) {
                    //        return item.src.replace(/\.\w+$/, function (m) { return '@2x' + m; });
                    //    } // function that changes image source
                    //}
                });
            }
        }
    }
    jQuery(".gallery-block").each(function (index, currentGallery) {
        var targetContiner = jQuery(currentGallery);
        initMagnificPopup(targetContiner);
    });
    jQuery(galleries).each(function (index, currentGallery) {
        var targetContiner = jQuery(currentGallery);
        var lookup = jQuery(currentGallery).attr("data-lookuptoapplypopupvalue");
        if (typeof (lookup) !== undefined && lookup !== null && lookup.trim() !== "") {
            targetContiner = jQuery(currentGallery).closest(jQuery(currentGallery).attr("data-lookuptoapplypopupvalue"));
        }
        initMagnificPopup(targetContiner);
	
        if (getBoolean(targetContiner.attr("data-zoomapplied")) === false) {
            targetContiner.attr("data-zoomapplied", 'true');
            if (getBoolean(targetContiner.attr("data-enablezoom")) === true) {
                var zoomFactor = jQuery(currentGallery).attr("data-zoomfactor");
				zoomFactor = (parseFloat(zoomFactor.replace(/[,]+/g, '.'))).toFixed(2)
                //zoomFactor = parseFloat(zoomFactor);
                //if (Number.isInteger(zoomFactor) == false) {
                //    zoomFactor = 1;
               // }
                if (zoomFactor<1) { 
                    zoomFactor = 1;
                }
                if (zoomFactor != 1) {
                    //targetContiner.find('img').each(function (imageIndex, imageItem) {
                    //magnify(imageItem, zoomFactor)
                    //});
                    targetContiner.find('img').css({
                        "position": "absolute"
                        //,"width": 100%,
                        //,"height": 100%,
                        , "transform": "translate(0, 0) scale(1)"
                        , "-moz-transition ": "all .5s ease-in-out, transform .60s"
                        , "-ms-transition": "all .5s ease-in-out, transform .60s"
                        , "-webkit-transition": "all .5s ease-in-out, transform .60s"
                        , "-o-transition": "all .5s ease-in-out, transform .60s"
                        , "transition": "all .5s ease-in-out, transform .60s"
                    });
                    targetContiner.find('img').hover(function () {
                        var currentImg = jQuery(this);
                        currentImg.css({
                            "position": "absolute",
                            "transform": "scale(" + zoomFactor + ")" /* CSS3 */
                            , "-moz-transform": "scale(" + zoomFactor + ")" /* Firefox */
                            , "-webkit-transform": "scale(" + zoomFactor + ")" /* Webkit */
                            , "-o-transform": "scale(" + zoomFactor + ")" /* Opera */
                            , "-ms-transform": "scale(" + zoomFactor + ")" /* IE 9 */
                            , "-ms-filter": "progid:DXImageTransform.Microsoft.Matrix(M11=1.5, M12=0, M21=0, M22=1.5, SizingMethod='auto expand')" /* IE8 */
                            , "filter": "progid:DXImageTransform.Microsoft.Matrix(M11 = 1.5, M12 = 0, M21 = 0, M22 = 1.5, SizingMethod = 'auto expand')" /* IE6 and 7 */
                        });
                    }, function () {
                        var currentImg = jQuery(this);
                        currentImg.css({
                            "position": "absolute",
                            "transform": "scale(1)" /* CSS3 */
                            , "-moz-transform": "scale(1)" /* Firefox */
                            , "-webkit-transform": "scale(1)" /* Webkit */
                            , "-o-transform": "scale(1)" /* Opera */
                            , "-ms-transform": "scale(1)" /* IE 9 */
                        });
                    }
                    );
                }
            }
        }

        if (getBoolean(targetContiner.attr("data-pagingenabled")) === true) {
            if (getBoolean(targetContiner.attr("data-pagingapplied")) === false) {
                jQuery(targetContiner).attr("data-pagingapplied", true);
                var paginationId = jQuery(targetContiner).attr("data-paginationId");
                createPagination(targetContiner,$(paginationId));
            }
        }
        if (getBoolean(targetContiner.attr("data-randomimages")) == true) {
            if (getBoolean(targetContiner.attr("data-randomimagesapplied")) == false) {
                jQuery(targetContiner).attr("data-randomimagesapplied", true);
                setInterval(function () { autoPageSwitcher(targetContiner); }, parseInt(targetContiner.attr("data-imageschangeinterval")));
            }
        }

        //if (jQuery(currentGallery).attr("data-enableZoom").toLocaleLowerCase() === 'true') {
        //       var zoomFactor = jQuery(currentGallery).attr("data-zoomfactor").
        //           zoomFactor = parseInt(zoomFactor);
        //       if (Number.isInteger(zoomFactor)) {
        //           if (zoomFactor < 1.01) {
        //               zoomFactor = 1.01;
        //           }
        //       } else {
        //           zoomFactor = 1.01;
        //       }
        //       jQuery(currentGallery).find("img").ezPlus({
        //           responsive: false,
        //           scrollZoom: true,
        //           imageCrossfade:true,
        //           easing: true,
        //           zoomType: "Lens",
        //           tint:true,
        //           minZoomLevel: zoomFactor
        //       });
        //   }

    });
    //ApplyZoom 
    jQuery(".easyzoom").easyZoom({ preventClicks: false });//  does not provide zoom factor. It needs to be tweeked a bit little. It is most simplest.
    jQuery("img.ezPlus").ezPlus({
        responsive: false,
        scrollZoom: true,
        imageCrossfade: true,
        easing: true,
        zoomType: "Lens",
        tint: true,
        minZoomLevel: 1
    });

	var sliders = jQuery(".slider-block .galleryInnerContainer").each(function(idx, el){
		el = $(el);
		var changeInterval = parseInt(el.parent().attr('data-imageschangeinterval'));
		$(el).jGallery({
			autostart: true, autostartAtImage: 1, browserHistory: false, canChangeMode: true, canClose: false, canMinimalizeThumbnails: true, canZoom: true, disabledOnIE8AndOlder: false, draggableZoom: true, hideThumbnailsOnInit: true, mode: 'slider', preloadAll: false,
			slideshow: true, slideshowAutostart: true, slideshowCanRandom: true, slideshowInterval: (isNaN(changeInterval) ? 3 : changeInterval / 1000)+'s', swipeEvents: true, thumbnails: false, thumbnailsHideOnMobile: true, thumbnailsPosition: 'bottom', title: true, titleExpanded: false,
			transition: 'moveToLeft_moveFromRight', transitionBackward: 'auto', transitionCols: 1, transitionDuration: '2s', transitionRows: 1, transitionTimingFunction: 'cubic-bezier(0,1,1,1)', transitionWaveDirection: 'forward', width: '100%', zoomSize: 'fit'
		});
	});
});
function updatePageNumbers(targetContiner) {
    jQuery(targetContiner).find(".justified-gallery").each(function (index, item) {
        jQuery(item).attr("data-pageindex", index + 1);
    });
}
function autoPageSwitcher(targetContiner) {
    var paginationInstance = jQuery(targetContiner).find(".flexpage-pagination");

    var imageTransition = targetContiner.attr("data-imagetransition");
    if (imageTransition == "FadeInFadeOut") {
        var currentPage = paginationInstance.pagination('getCurrentPage');
        var previousPage = currentPage;
        currentPage = currentPage + 1;

        var previousItem = jQuery(targetContiner).find("div[data-pageindex='" + previousPage + "']");
        var currentItem = jQuery(targetContiner).find("div[data-pageindex='" + currentPage + "']");

        currentItem.animate({ opacity: 1, display: "block", duration: "slow", queue: false }, {
            duration: targetContiner.attr("data-imageschangeinterval"),
            easing: 'linear',
            complete: function () {
                var currentItem = jQuery(this);
                currentItem.fadeIn();
                currentItem.css("opacity", 1);
            }
        });
        previousItem.animate({ opacity: 0, display: "none", duration: "slow", queue: false }, {
            duration: targetContiner.attr("data-imageschangeinterval"),
            easing: 'linear',
            complete: function () {
                previousItem.hide();
                previousItem.css({ "left": "0px" });
            },
            start: function () {
            }
        });
        paginationInstance.pagination("selectPage", currentPage);
    }
    else if (imageTransition == "Slideshow") {
        var currentPage = paginationInstance.pagination('getCurrentPage');
        var previousPage = currentPage;
        var pageCount = paginationInstance.pagination('getPagesCount');
        if (currentPage >= pageCount) {
            currentPage = 1;
        }
        else {
            currentPage = currentPage + 1;
        }

        var previousItem = jQuery(targetContiner).find("div[data-pageindex='" + previousPage + "']");
        var currentItem = jQuery(targetContiner).find("div[data-pageindex='" + currentPage + "']");
        currentItem.css({ right: ("-=" + currentItem.width() * 2), marginRight: ("-=" + currentItem.width() * 2) });
        currentItem.show();
        //updatePageNumbers(targetContiner);//update index for correct management of indexes
        //jQuery(previousItem).addClass("left").one("transitionend", function () {
        //    previousItem.hide();
        //    previousItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
        //    previousItem.detach();
        //    jQuery(targetContiner).find(".galleryInnerContainer").append(previousItem);
        //});
        //jQuery(previousItem).addClass("right").one("transitionend", function () {
        //    currentItem.show();
        //    currentItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
        //});
        currentItem.animate({
            display: "block",
            right: "+=0px",
            marginRight: "+=0px"
        },
            {
                queue: true,
                duration: targetContiner.attr("data-imageschangeinterval"),
                easing: 'linear',
                complete: function () {
                    var currentItem = jQuery(this);
                    currentItem.show();
                    currentItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                }
            });

        previousItem.animate({
            display: "none",
            left: ("-=" + previousItem.width()),
            marginLeft: ("-=" + previousItem.width()),
            opacity: 0
        },
            {
                queue: true,
                duration: targetContiner.attr("data-imageschangeinterval"),
                easing: 'linear',
                complete: function () {
                    previousItem.hide();
                    previousItem.css({ "right": "0px", marginTop: "0px", marginRight: "0px", "left": "0px", marginLeft: "0px", opacity: 1 });
                    previousItem.detach();
                    jQuery(targetContiner).find(".galleryInnerContainer").append(previousItem);
                },
                start: function () {
                }
            });
        paginationInstance.pagination("selectPage", currentPage);
    }
}
/*
 * Required by zoom
 * * {box-sizing: border-box;}

.img-zoom-container {
  position: relative;
}

.img-zoom-lens {
  position: absolute;
  border: 1px solid #d4d4d4; 
width: 40px;
height: 40px;
}

.img-zoom-result {
    border: 1px solid #d4d4d4;    
    width: 300px;
    height: 300px;
}
<a class='img-zoom-container'><img/></a>
 <div id="myresult" class="img-zoom-result"></div>
 */

function imageZoom(imgItem, resultID) {
    var img, lens, result, cx, cy;
    img = jQuery(imgItem)[0];// document.getElementById(imgID);
    result = document.getElementById(resultID);
    /*create lens:*/
    lens = document.createElement("div");
    lens.setAttribute("class", "img-zoom-lens");
    /*insert lens:*/
    img.parentElement.insertBefore(lens, img);
    /*calculate the ratio between result DIV and lens:*/
    cx = result.offsetWidth / lens.offsetWidth;
    cy = result.offsetHeight / lens.offsetHeight;
    /*set background properties for the result DIV:*/
    result.style.backgroundImage = "url('" + img.src + "')";
    result.style.backgroundSize = (img.width * cx) + "px " + (img.height * cy) + "px";
    /*execute a function when someone moves the cursor over the image, or the lens:*/
    lens.addEventListener("mousemove", moveLens);
    img.addEventListener("mousemove", moveLens);
    img.addEventListener("mouseout", removelens);
    /*and also for touch screens:*/
    lens.addEventListener("touchmove", moveLens);
    img.addEventListener("touchmove", moveLens);
    function removelens(e) {
        lens.parentNode.removeChild(lens);
    }
    function moveLens(e) {
        var pos, x, y;
        /*prevent any other actions that may occur when moving over the image:*/
        e.preventDefault();
        /*get the cursor's x and y positions:*/
        pos = getCursorPos(e);
        /*calculate the position of the lens:*/
        x = pos.x - (lens.offsetWidth / 2);
        y = pos.y - (lens.offsetHeight / 2);
        /*prevent the lens from being positioned outside the image:*/
        if (x > img.width - lens.offsetWidth) { x = img.width - lens.offsetWidth; }
        if (x < 0) { x = 0; }
        if (y > img.height - lens.offsetHeight) { y = img.height - lens.offsetHeight; }
        if (y < 0) { y = 0; }
        /*set the position of the lens:*/
        lens.style.left = x + "px";
        lens.style.top = y + "px";
        /*display what the lens "sees":*/
        result.style.backgroundPosition = "-" + (x * cx) + "px -" + (y * cy) + "px";
    }
    function getCursorPos(e) {
        var a, x = 0, y = 0;
        e = e || window.event;
        /*get the x and y positions of the image:*/
        a = img.getBoundingClientRect();
        /*calculate the cursor's x and y coordinates, relative to the image:*/
        x = e.pageX - a.left;
        y = e.pageY - a.top;
        /*consider any page scrolling:*/
        x = x - window.pageXOffset;
        y = y - window.pageYOffset;
        return { x: x, y: y };
    }
}
/*
 .img-magnifier-container {
  position:relative;
}

.img-magnifier-glass {
  position: absolute;
  border: 3px solid #000;
  border-radius: 50%;
  cursor: none;
  /*Set the size of the magnifier glass:*/
/*width: 100px;
height: 100px;
}
 */
function magnify(imgItem, zoom) {
    var img, glass, w, h, bw;
    img = jQuery(imgItem)[0];//document.getElementById(imgID);
    /*create magnifier glass:*/
    glass = document.createElement("DIV");
    glass.setAttribute("class", "img-magnifier-glass");
    /*insert magnifier glass:*/
    img.parentElement.insertBefore(glass, img);
    /*set background properties for the magnifier glass:*/
    glass.style.backgroundImage = "url('" + jQuery(img).attr("data-zoom-image") + "')";
    glass.style.backgroundRepeat = "no-repeat";
    glass.style.backgroundSize = (img.width * zoom) + "px " + (img.height * zoom) + "px";
    bw = 3;
    w = glass.offsetWidth / 2;
    h = glass.offsetHeight / 2;
    /*execute a function when someone moves the magnifier glass over the image:*/
    glass.addEventListener("mousemove", moveMagnifier);
    img.addEventListener("mousemove", moveMagnifier);
    img.addEventListener("mouseout", removeMagnifier);
    glass.addEventListener("mouseout", removeMagnifier);
    /*and also for touch screens:*/
    glass.addEventListener("touchmove", moveMagnifier);
    img.addEventListener("touchmove", moveMagnifier);
    function removeMagnifier(e) {
        jQuery(glass).hide();
    }
    function moveMagnifier(e) {
        jQuery(glass).show();
        var pos, x, y;
        /*prevent any other actions that may occur when moving over the image*/
        e.preventDefault();
        /*get the cursor's x and y positions:*/
        pos = getCursorPos(e);
        x = pos.x;
        y = pos.y;
        /*prevent the magnifier glass from being positioned outside the image:*/
        if (x > img.width - (w / zoom)) { x = img.width - (w / zoom); }
        if (x < w / zoom) { x = w / zoom; }
        if (y > img.height - (h / zoom)) { y = img.height - (h / zoom); }
        if (y < h / zoom) { y = h / zoom; }
        /*set the position of the magnifier glass:*/
        glass.style.left = (x - w - img.style.marginleft) + "px";
        glass.style.top = (y - h - img.style.margintop) + "px";
        /*display what the magnifier glass "sees":*/
        glass.style.backgroundPosition = "-" + ((x * zoom) - w + bw) + "px -" + ((y * zoom) - h + bw) + "px";
    }
    function getCursorPos(e) {
        var a, x = 0, y = 0;
        e = e || window.event;
        /*get the x and y positions of the image:*/
        a = img.getBoundingClientRect();
        /*calculate the cursor's x and y coordinates, relative to the image:*/
        x = e.pageX - a.left;
        y = e.pageY - a.top;
        /*consider any page scrolling:*/
        x = x - window.pageXOffset;
        y = y - window.pageYOffset;
        return { x: x, y: y };
    }
}
