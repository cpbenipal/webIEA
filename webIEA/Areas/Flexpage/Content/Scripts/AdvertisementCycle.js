function AdvertisementCycle(cycleDelay, id, idBlock, tagName) {
        this.id = id;
        this.blockReloadId = "#container-Advertisement-" + idBlock;
        this.speedReload = 'slow';
        this.url = '/Flexpage/GetAdvertisement/';
        this.cycleDelay = cycleDelay;
        this.tagName = tagName;
    }

    AdvertisementCycle.prototype.startReload = function () {
        var that = this;
        var advertisementViews = [];
        var allImagesLoaded;

        var reload = function () {
            if (advertisementViews.length == 0) {
                $.ajax({
                    type: "POST",
                    url: that.url,
                    data: { id: that.id, tagName: that.tagName },
                    error: function (data) {
                        console.log("An error occurred.");
                    },
                    beforeSend: function () {
                    },
                    success: function (data) {
                        advertisementViews = JSON.parse(data.viewArray);
                        allImagesLoaded = data.allImagesLoaded;
                        updateImage();
                    }
                });
            }
            else
            {
                updateImage();
            }
            setTimeout(reload, that.cycleDelay);
        };

        var updateImage = function () {
            fadeOut(advertisementViews[0]);
            if (allImagesLoaded)
                advertisementViews.push(advertisementViews.shift());
            else
                advertisementViews.shift();
        }

        var fadeOut = function (view) {
            $(that.blockReloadId).fadeOut(that.speedReload, function () {
                $(that.blockReloadId).html(view);
                $(that.blockReloadId).fadeIn(that.speedReload, function () {
                });
            });
        };

        reload();
    };