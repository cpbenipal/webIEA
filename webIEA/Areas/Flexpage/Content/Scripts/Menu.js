function onetime(node, type, callback) {
	// create event
	node.addEventListener(type, function(e) {
		// remove event
		e.target.removeEventListener(e.type, arguments.callee);
		// call handler
		return callback(e);
	});
}

var FlexpageMenu = {
    create: function(id, _options) {        
        this.container = $('#' + id);        
        if (!this.container)
            return;        
        
        this.options = _options;
        this.hideTimeout = [];
        /*this.options = {
            showEffect: _options.showEffect,
            hideEffect: _options.hideEffect,
            duration: _options.duration ? _options.duration : 0.5,
            topOffset: _options.topOffset ? _options.topOffset : 0,
            topOffsetIE7: _options.topOffsetIE7 ? _options.topOffsetIE7 : 0,
            leftOffset: _options.leftOffset ? _options.leftOffset : 0,
            autocloseTimeout: _options.autocloseTimeout ? _options.autocloseTimeout * 1000 : 100,
            positionSubmenu: _options.positionSubmenu !== undefined ? _options.positionSubmenu : true,
            oneSubmenuVisible: _options.oneSubmenuVisible !== undefined ? _options.oneSubmenuVisible : false,
            mainMenuActive: _options.mainMenuActive !== undefined ? _options.mainMenuActive : false,
            allowForMobile: _options.allowForMobile !== undefined ? _options.allowForMobile : false
        };*/

        var initiallizeClickForTouchEnabled = this.options.click;
        var supportsTouch = false;
        if ('ontouchstart' in window) {
            //iOS & android
            supportsTouch = true;
        } else if (window.navigator && navigator.maxTouchPoints && navigator.maxTouchPoints > 0) {
            supportsTouch = true;
        } else if (window.navigator && navigator.msMaxTouchPoints && navigator.msMaxTouchPoints > 0) {
            supportsTouch = true;
        } else if (window.DocumentTouch && document instanceof DocumentTouch) {
            supportsTouch = true;
        } else if (window.navigator && navigator.userAgent.search(/Touch/i) != -1) {
            supportsTouch = true;
        } else if ((/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) == true) {
            supportsTouch = true;
        } else {
            onetime(window, "touchstart", function (e) {

                if (e.type === 'touchstart')
                    supportsTouch = true;
            });
        }

        // PointerEvents shall not be used as mouse events are also considered as point events

        if (supportsTouch == true) {
            if (this.options.allowForMobile) {
                initiallizeClickForTouchEnabled = true;
            }
        };        

        var menuObj = this;

        var $this = this;
        this.container.bind("click", function(event){ $this.hideAll(event, { menu: menuObj })});
        $(document).bind("click", function(event) { $this.hideAll(event, { menu: menuObj })});
        
        var subMenuIdx = 1;
        this.container.find("li>a,li>span").each(function (idx, el) {
            el = $(el);
            var submenu = el.next();
            submenu.attr('submenu', idx);
            if (submenu && submenu.length == 1) {
                if (initiallizeClickForTouchEnabled == true) {
                    el.bind("click", function(event) {
                        $this.showSubmenu(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                }
                else {
                    el.bind("mouseover", function(event) {
                        $this.showSubmenu(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                    el.bind("mouseout", function(event) {
                        $this.hideSubmenu(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                }
                submenu.css('visibility', 'visible');

                if (initiallizeClickForTouchEnabled == true) {
                    submenu.bind("click", function(event) {
                        $this.showSubmenu(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                }
                else {
                    submenu.bind("mouseover", function(event) {
                        $this.submenuMouseOver(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                    submenu.bind("mouseout",  function(event) {
                        $this.submenuMouseOut(event, { 'menu': menuObj, 'el':el, 'submenu':submenu });
                    });
                }
            }
        }.bind(this));

        $("ul.submenu.level1").each(function (idx, el) {
            $(el).addClass("mnSubmenu" + idx);            
        });
    },

    hideAll: function(event, data) {
        $("ul.submenu").each(function (idx, el) {
            el = $(el);
            if (data.menu) {
                if (data.menu.options.hideEffect)
                    Effect[data.menu.options.hideEffect].apply(Effect, [el, { duration: data.menu.options.duration }]);
                else {
                    if (data.menu.options.mainMenuActive)
                        el.parent().removeClass("activeMenu");
                    el.hide();
                }
            }
        });
    },
    
    showSubmenu: function(event, data) {
        if (event) {
        }
        else {
            event = window.event;
        }
        if (event.type == "click") {
            if (event.stopPropagation) {
                event.stopPropagation();
            }
            else {
                event.cancelBubble = true;
            }
        }
        data.el.toggleClass('fp_menu-item-active');
        if (!data.submenu.is(":visible")) {
            //close all opened menu
            data.menu.container.find("li>ul").each(function (idx, el) {
                el = $(el);
                if (el[0] == data.submenu[0])
                    return;
                var parent = data.submenu.parent().parent();
                while (parent && parent.length > 0) {
                    if (el[0] == parent[0])
                        return;
                    parent = parent.parent().parent();
                }
                setTimeout(data.menu.hideSubmenuEffect({ menu: data.menu, el: el.prev(), submenu: el }), 1);                
            });
            if (data.menu.hideTimeout[data.submenu.attr('submenu')])
                clearTimeout(data.menu.hideTimeout[data.submenu.attr('submenu')]);
            if (data.menu.options.positionSubmenu) {
                var topOffsetValue = (isIE7() && data.menu.options.topOffsetIE7 != 0) ?
                    data.menu.options.topOffsetIE7 : data.menu.options.topOffset;
                
                /*data.submenu.offset({
                    left: data.el.hasClass("topLevel") ? 0 : data.el.outerWidth() + data.menu.options.leftOffset,
                    top: data.el.hasClass("topLevel") ? data.el.outerHeight() + topOffsetValue : 0
                });*/
                data.submenu.css('left', data.el.hasClass("topLevel") ? 0 : data.el.parent().outerWidth() + data.menu.options.leftOffset);
                data.submenu.css('top', data.el.hasClass("topLevel") ? data.el.parent().outerHeight() + topOffsetValue : 0);
            }

            if (data.menu.options.showEffect)
                Effect[data.menu.options.showEffect].apply(Effect, [data.submenu, { duration: data.menu.options.duration }]);
            else {
                if (data.menu.options.mainMenuActive)
                    data.el.addClass("activeMenu");
                data.submenu.show();                
            }
        } else if (this.options.click) {
            this.hideSubmenu(event, data);
        }
    },

    hideSubmenuEffect: function(data) {
        if (data.menu.options.hideEffect)
            Effect[data.menu.options.hideEffect].apply(Effect, [data.submenu, { duration: data.menu.options.duration }]);
        else {
            if (data.menu.options.mainMenuActive) 
                data.el.removeClass("activeMenu");
            data.submenu.hide();
        }
    },

    hideSubmenu: function(event, data) {
        var $this = this;
        data.menu.hideTimeout[data.submenu.attr('submenu')] = setTimeout(function(){ $this.hideSubmenuEffect(data); }, data.menu.options.autocloseTimeout);
    },

    submenuMouseOver: function(event, data) {
        clearTimeout(data.menu.hideTimeout[data.submenu.attr('submenu')]);
    },

    submenuMouseOut: function(event, data) {
        var $this = this;
        data.menu.hideTimeout[data.submenu.attr('submenu')] = setTimeout(function(){ $this.hideSubmenuEffect(data); }, data.menu.options.autocloseTimeout);
    }
}

function isIE7() {
    return (navigator.appVersion.indexOf('MSIE 7.') == -1) ? false : true;
}