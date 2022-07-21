function PublisherLog() {
    var that = this;
    var position = 40;

    this.subscribers = [];
    this.isShow = false;

    this.Show = function () {
        var tmpArr = Object.assign({}, that.subscribers);
        var tmpPosition = position;

        for (var i = 0; i < that.subscribers.length; i++) {
            tmpPosition += 30;
            that.subscribers[i].showBlock(tmpPosition, i);
        }
    }

    this.Push = function (obj) {

        that.subscribers.unshift(obj);
        this.Show();
    }

    this.Remove = function (obj) {
        that.subscribers.splice(obj.index, 1);
    }
}

var publisherLog = new PublisherLog();

function DivLog(msg) {
    var _msg = msg;
    var that = this;
    var idBlock = 'log-';

    this.isShow = false;
    this.index = -1;
    this.isRemoved = false;

    function createBlock(id) {
        idBlock += id
        return $('<div/>',
            {
                'class': idBlock
            }).css({
                'position': 'fixed',
                'min-height': '20px',
                'min-width': '250px',
                'background-color': '#00FA9A',
                'right': '20px',
                'border-radius': '30px',
                'opacity': '0.7',
                'text-align': 'center',
                'color': '#000',
                'padding': '3px',
                'display': 'none'
            }).text(_msg);
    }

    var block = {};

    this.showBlock = function (top, index) {
        if (this.isShow) {
            $(block).css('top', top + 'px');
            return;
        }

        block = createBlock(new Date().getUTCMilliseconds());

        $(block).css('top', top + 'px').appendTo('body').fadeIn(1000, function () {
            setTimeout(function () {
                $(block).fadeOut(1000,
                    function () {
                        $(block).remove();
                        that.isRemoved = true;
                    });
            },
                5000);

        });

        this.isShow = true;
        this.index = index;
    }
}
