window.previewTimeout = null;
window.enteredObject = null;

function magnifierShow(sender) {
    if (sender == window.enteredObject) {
        return;
    } else {
        enteredObject = sender;
    }

    if (document.getElementById("previewDiv") != null && document.getElementById("previewDiv").style.display == "") {
        return;
    }

    if (document.getElementById("magnifier") != null && document.getElementById("magnifier").style.display == "") {
        return;
    }

    var imgPath = $(sender).data("preview");

    if (document.getElementById("magnifier") == null || document.getElementById("magnifier") == undefined) {

        var div = document.createElement("div");
        div.style.width = "32px";
        div.style.height = "32px";
        div.style.display = "none";
        div.innerHTML = "<img src='/Content/Images/magnifier.png'/>";
        div.id = "magnifier";
        div.style.zIndex = "4";
        div.style.cursor = "pointer";

        sender.appendChild(div);
    }
    $(sender).append($('<div>',
        {
            class: 'tiled-content-hover'
        }));
    $(sender).find(".tiled-content-name").show();
    var magnifier = document.getElementById("magnifier");

    magnifier.onclick = function (e) {
         pdfPreviewShow(imgPath);
    };

    magnifier.style.position = 'absolute';

    magnifier.style.top = 95 + "px";
    magnifier.style.left = 58 + "px";

    magnifier.style.display = "";

}


function magnifierHide(sender, event, justHide) {

    if (justHide) {
        if (document.getElementById("magnifier") != null) {
            var div = document.getElementById("magnifier");
            div.parentNode.removeChild(div);
            enteredObject = null;
            $(".tiled-content-hover").remove();
            $(sender).find(".tiled-content-name").hide();
        }

    } else {
       
        var posY1 = findPosY(sender);
        var posX1 = findPosX(sender);
        var posY2 = posY1 + 190;
        var posX2 = posX1 + 140;

        event = (event || event);
        // Calculate pageX/Y if missing and clientX/Y available
        if (event.pageX == null && event.clientX != null) {
            var doc = document.documentElement, body = document.body;
            event.pageX = event.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc && doc.clientLeft || body && body.clientLeft || 0);
            event.pageY = event.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc && doc.clientTop || body && body.clientTop || 0);
        }

        var mouseX = event.pageX;
        var mouseY = event.pageY;

        

        //we are in preview square
        if (mouseX > posX1 &&
            mouseX < posX2 &&
            mouseY > posY1 &&
            mouseY < posY2)
            return;

        if (sender == window.enteredObject) {
            enteredObject = null;
        }

        if (document.getElementById("magnifier") != null) {
            var div = document.getElementById("magnifier");
            div.parentNode.removeChild(div);
            $(".tiled-content-hover").remove();
            $(sender).find(".tiled-content-name").hide();
        }
    }
}

function pdfPreviewShow(imgPath) {
    if (document.getElementById("previewDiv") == null || document.getElementById("previewDiv") == undefined) {


        var div = document.createElement("div");
        div.style.display = "none";
        div.id = "previewDiv";

        document.body.appendChild(div);
    }

    var previewDiv = document.getElementById("previewDiv");
    previewDiv.innerHTML = "<img src='" + imgPath + "' onclick='pdfPreviewHide();'/>";

    previewDiv.style.position = 'absolute';

    var windowSizes = getWindowSize();
    var scrollingSizes = getScrollXY();

    previewDiv.style.left = windowSizes[0] / 2 - 270 + "px";
    previewDiv.style.top = scrollingSizes[1] + "px";
    previewDiv.style.zIndex = "1001";

    var disablingDiv = document.getElementById("disablingDiv");
    disablingDiv.onclick = function (e) {
        pdfPreviewHide();
    };

    previewDiv.style.display = "";
    showSplash(scrollingSizes[1] + "px");
}

function pdfPreviewHide() {
    if (document.getElementById("previewDiv") != null) {
        var div = document.getElementById("previewDiv");
        div.style.display = "none";
    }
    hideSplash();

    magnifierHide(null, null, true);
}

function findPosY(obj) {
    var curtop = 0;
    if (obj.offsetParent) {
        while (1) {
            curtop += obj.offsetTop;
            if (!obj.offsetParent) {
                break;
            }
            obj = obj.offsetParent;
        }
    } else if (obj.y) {
        curtop += obj.y;
    }
    return curtop;
}

function findPosX(obj) {
    var curleft = 0;
    if (obj.offsetParent) {
        while (1) {
            curleft += obj.offsetLeft;
            if (!obj.offsetParent) {
                break;
            }
            obj = obj.offsetParent;
        }
    } else if (obj.x) {
        curleft += obj.x;
    }
    return curleft;
}

function getWindowSize() {
    var myWidth = 0, myHeight = 0;
    if (typeof (window.innerWidth) == 'number') {
        //Non-IE
        myWidth = window.innerWidth;
        myHeight = window.innerHeight;
    } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
        //IE 6+ in 'standards compliant mode'
        myWidth = document.documentElement.clientWidth;
        myHeight = document.documentElement.clientHeight;
    } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
        //IE 4 compatible
        myWidth = document.body.clientWidth;
        myHeight = document.body.clientHeight;
    }
    return [myWidth, myHeight];
}

function getScrollXY() {
    var scrOfX = 0, scrOfY = 0;
    if (typeof (window.pageYOffset) == 'number') {
        //Netscape compliant
        scrOfY = window.pageYOffset;
        scrOfX = window.pageXOffset;
    } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        //DOM compliant
        scrOfY = document.body.scrollTop;
        scrOfX = document.body.scrollLeft;
    } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        //IE6 standards compliant mode
        scrOfY = document.documentElement.scrollTop;
        scrOfX = document.documentElement.scrollLeft;
    }
    return [scrOfX, scrOfY];
}

function showSplash(position) {
    var disablingDiv = document.getElementById("disablingDiv");
    disablingDiv.style.height = document.body.scrollHeight + "px";
    disablingDiv.style.display = "";
}
function hideSplash() {

    var disablingDiv = document.getElementById("disablingDiv");
    disablingDiv.style.display = "none";
}

function wndsize() {
    var w = 0; var h = 0;
    //IE
    if (!window.innerWidth) {
        if (!(document.documentElement.clientWidth == 0)) {
            //strict mode
            w = document.documentElement.clientWidth; h = document.documentElement.clientHeight;
        } else {
            //quirks mode
            w = document.body.clientWidth; h = document.body.clientHeight;
        }
    } else {
        //w3c
        w = window.innerWidth; h = window.innerHeight;
    }
    return { width: w, height: h };
}
