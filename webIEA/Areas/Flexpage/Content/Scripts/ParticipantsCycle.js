function ParticipantsCycle(cycleDelay, id, idBlock, tagName, itemsPerPage, showCountry, showFavorite) {
    this.id = id;
    this.blockReloadId = "#container-Participants-" + idBlock;
    this.speedReload = 'slow';
    this.url = '/ParticipantsBlock/GetParticipants/';
    this.cycleDelay = cycleDelay;
    this.tagName = tagName;
    this.page = 0;
    this.itemsPerPage = itemsPerPage;
    this.totalCount = 0;
    this.countryId = 0;
    this.firstName = '';
    this.lastName = '';
    this.organization = '';
    this.showCountry = showCountry;
    this.showFavorite = showFavorite;
}

ParticipantsCycle.prototype.startReload = function (id) {
    var that = this;

    that.reload(0, id);

    if (that.cycleDelay > 0) {
        setTimeout(new function () {

            that.reload(1, id);
        }, that.cycleDelay);
    }

};

ParticipantsCycle.prototype.setFilter = function (id) {
    this.countryId = $('#fp_participant-country' + id).val();
    this.firstName = $('#fp_participant-firstname' + id).val();
    this.lastName = $('#fp_participant-lastname' + id).val();
    this.organization = $('#fp_participant-organization' + id).val();
    this.page = 0;
    this.reload(0, id);
}

ParticipantsCycle.prototype.reload = function (increment, id) {
    var that = this;
    that.id = id;
    if (increment != 0 && that.totalCount > 0) {
        that.page += increment;
        let items = that.page * that.itemsPerPage;
        that.page = items >= that.totalCount ? 0 : (items < 0 ? (that.totalCount % that.itemsPerPage == 0 ? Math.floor(that.totalCount / that.itemsPerPage) - 1 : Math.floor(that.totalCount / that.itemsPerPage)) : that.page);
    }
    $.ajax({
        type: "POST",
        url: that.url,
        data: { blockID: that.id, page: that.page, itemsCount: that.itemsPerPage, countryId: that.countryId, firstName: that.firstName, lastName: that.lastName, organization: that.organization },
        error: function (data) {
            //debugger;
            //console.log("An error occurred.");
        },
        beforeSend: function () {
        },
        success: function (data) {
            that.totalCount = data.Count;
            var partHtml = '';
            for (i = 0; i < data.Participants.length; i++) {
                var participant = data.Participants[i];
                partHtml += `<div class="col-md-3 fp_participant-wrapper" data-id="${participant.ID}" id="participant${participant.ID}">`
                    + `<div class="fp_participant-name"><p>${participant.FirstName} ${participant.LastName}</p></div>`
                    + (that.showCountry && participant.Country ? '<div class="fp_participant-country"><p>' + participant.Country + "</p></div>" : "")
                    + (participant.CompanyName ? '<div class="fp_participant-company"><p>' + participant.CompanyName + "</p></div>"
                        : '<div class="fp_participant-company" style="display:none;"></div>')
                    + (participant.Position ? '<div class="fp_participant-position"><p>' + participant.Position + "</p></div>"
                        : '<div class="fp_participant-position" style="display:none;"></div>')
                    + (that.showFavorite && participant.Favorites ? '<div class="fp_participant-favorites"><p>' + participant.Favorites + "</p></div>"
                        : '<div class="fp_participant-favorites" style="display:none;"></div>')
                    + `<img src="${participant.ImageUrl ? participant.ImageUrl : '/Areas/Flexpage/Content/Images/participant.svg' }" class="fp_participant-image"/>` 
                    + '</div>';
            }

            $('#participants-ext-items' + that.id).html(partHtml);

            var ids = [];
            $('#participants-ext-items' + that.id + ' .fp_participant-wrapper').each(function () {
                ids.push($(this).data('id'));
            });
            that.extDataLoading(that.id, ids);
            $('#b' + that.id + ' .fp_participants-button input').toggle(that.totalCount > that.itemsPerPage);
        }
    });

};
ParticipantsCycle.prototype.extDataLoading = function (idBlock, ids) {
    $.ajax({
        type: "POST",
        url: '/ParticipantsBlock/GetParticipantsExtraData/',
        data: { ids: ids, blockID: idBlock},
        error: function (data) {
            //debugger;
            //console.log("An error occurred.");
        },
        success: function (data) {
            var block = $('#participants-ext-items' + idBlock);
            if (block) {
                for (i = 0; i < data.Participants.length; i++) {
                    var participant = data.Participants[i];
                    var part = block.children(`#participant${data.Participants[i].ID}`);

                    if (participant.CompanyName) {
                        var pos = $(part.children('.fp_participant-company')[0]);
                        pos.show();
                        pos.html(`<p>${participant.CompanyName}</p>`)
                    }

                    if (participant.Position) {
                        var pos = $(part.children('.fp_participant-position')[0]);
                        pos.show();
                        pos.html(`<p>${participant.Position}</p>`)
                    }

                    if (participant.Favorites) {
                        var pos = $(part.children('.fp_participant-favorites')[0]);
                        pos.show();
                        pos.html(`<p>${participant.Favorites}</p>`)
                    }

                    if (participant.ImageUrl) {
                        $(part.children('img.fp_participant-image')[0]).attr("src", participant.ImageUrl);
                    }
                }
            }
        }
    });
}

