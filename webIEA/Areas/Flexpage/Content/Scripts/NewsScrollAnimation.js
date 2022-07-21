var NewsFlexpage = function ($modelId, $newsPerPage, $totalPages) {
    this.mainWrapperID = `#newsFrame_${$modelId}`;
    this.blockID = $modelId;
    this.newsPerPage = $newsPerPage;
    this.currentPage = 1;
    this.totalPages = $totalPages;

    this.visibleBlockID = `#visibleNews_${$modelId}`;
    this.uploadedBlockID = `#uploadedNews_${$modelId}`;
    this.nextButton = `#nextLinkWrapper${$modelId}`;
    this.prevButton = `#prevLinkWrapper${$modelId}`
    this.buttonsWrapper = `#pagerWrapper_${$modelId}`
    this.animationLocked = false;

};

NewsFlexpage.prototype.init = function () {
    var $this = this;
    $(`#prevLink${this.blockID}`).click(function () {
        if (!$this.animationLocked) {
            $this.animationLocked = true;
            $this.ScrollBack();
        }
    });

    $(`#nextLink${this.blockID}`).click(function () {
        if (!$this.animationLocked) {
            $this.animationLocked = true;
            $this.ScrollForward();
        }
    });

    $(`${this.buttonsWrapper} ul li[pageno]`).click(function () {
        if (!$this.animationLocked) {
            let pageno = parseInt($(this).attr('pageno'));
            if (pageno && pageno != $this.currentPage) {
                $this.animationLocked = true;
                if ($this.currentPage < pageno && $this.currentPage < $this.totalPages) {
                    $this.currentPage = pageno;
                    $this.UploadNews("forward");
                }
                else if ($this.currentPage > pageno && $this.currentPage > 0) {
                    $this.currentPage = pageno;
                    $this.UploadNews("back");
                }
            }
        }
    });
}

NewsFlexpage.prototype.ScrollForward = function () {
    if (this.currentPage < this.totalPages) {
        this.currentPage++;
        this.UploadNews("forward");
    }
}

NewsFlexpage.prototype.ScrollBack = function () {
    if (this.currentPage > 0) {
        this.currentPage--;
        this.UploadNews("back");
    }
}

NewsFlexpage.prototype.UploadNews = function ($direction) {
    var $this = this;
    $.ajax({
        url: '/News/UploadNews?pageNo=' + this.currentPage,
        data: { 'blockID': this.blockID, 'direction': $direction },
        type: "post",
        cache: false,
        success: function (result) {
            $this.ScrollNews(result, $direction);
        },
        error: function (xhr, ajaxOptions, thrownError) {

        }
    });
}

NewsFlexpage.prototype.ScrollNews = function ($result, $direction) {
    $this = this;
    let animationCounter = 0;
    if ($result) {
        if ($direction === "forward") {
            $(this.mainWrapperID).append($result);
            $(this.mainWrapperID + ' div.forAnimate').animate({ 'left': '-=' + 100 + '%' }, 1000,
                function () {
                    animationCounter++;
                    if ($(this.mainWrapperID + ' div.forAnimate:animated').length === 0 && animationCounter === 2) {
                        animationCounter = 0;
                        $this.AfterScroll($direction);
                    }
                });
        }
        else {
            $(this.mainWrapperID).prepend($result);
            $(this.mainWrapperID + ' div.forAnimate').each(function (index, value) {
                $(this).animate({ 'left': '+=' + 100 + '%' }, 1000,
                    function () {
                        animationCounter++;
                        if ($(this.mainWrapperID + ' div.forAnimate:animated').length === 0 && animationCounter === 2) {
                            animationCounter = 0;
                            $this.AfterScroll($direction);
                        }
                    });
            });
        }
    }
}

NewsFlexpage.prototype.AfterScroll = function ($direction) {
    $(this.visibleBlockID).remove();
    if ($direction === 'forward') {
        $(this.uploadedBlockID).addClass('visibleNews').removeClass('uploadedNews_forward');
    }
    else {
        $(this.uploadedBlockID).addClass('visibleNews').removeClass('uploadedNews_back');
    }

    $(this.uploadedBlockID).attr('style', '');
    $(this.uploadedBlockID).attr('id', `visibleNews_${this.blockID}`);
    if ($(`${this.prevButton}:hidden`) && this.currentPage > 1) {
        $(this.prevButton).show();
    }
    else if (this.currentPage <= 1 && $(`${this.prevButton}:visible`)) {
        $(this.prevButton).hide();
    }

    if ($(`${this.nextButton}:hidden`) && this.currentPage < this.totalPages) {
        $(this.nextButton).show();
    }
    else if (this.currentPage === this.totalPages && $(`${this.nextButton}:visible`)) {
        $(this.nextButton).hide();
    }

    $(`${this.buttonsWrapper} ul li.news-pager-item.active`).removeClass('active');
    $(`${this.buttonsWrapper} ul li[pageno=${this.currentPage}]`).addClass('active');

    this.animationLocked = false;
}