/*!
 * jQuery Cookie Plugin
 * https://github.com/carhartl/jquery-cookie
 *
 * Copyright 2011, Klaus Hartl
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.opensource.org/licenses/GPL-2.0
 */ /*!
 * jQuery Cookie Plugin v1.4.1
 * https://github.com/carhartl/jquery-cookie
 *
 * Copyright 2006, 2014 Klaus Hartl
 * Released under the MIT license
 */
(function (factory) {
	if (typeof define === 'function' && define.amd) {
		// AMD (Register as an anonymous module)
		define(['jquery'], factory);
	} else if (typeof exports === 'object') {
		// Node/CommonJS
		module.exports = factory(require('jquery'));
	} else {
		// Browser globals
		factory(jQuery);
	}
}(function ($) {

	var pluses = /\+/g;

	function encode(s) {
		return config.raw ? s : encodeURIComponent(s);
	}

	function decode(s) {
		return config.raw ? s : decodeURIComponent(s);
	}

	function stringifyCookieValue(value) {
		return encode(config.json ? JSON.stringify(value) : String(value));
	}

	function parseCookieValue(s) {
		if (s.indexOf('"') === 0) {
			// This is a quoted cookie as according to RFC2068, unescape...
			s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
		}

		try {
			// Replace server-side written pluses with spaces.
			// If we can't decode the cookie, ignore it, it's unusable.
			// If we can't parse the cookie, ignore it, it's unusable.
			s = decodeURIComponent(s.replace(pluses, ' '));
			return config.json ? JSON.parse(s) : s;
		} catch(e) {}
	}

	function read(s, converter) {
		var value = config.raw ? s : parseCookieValue(s);
		return $.isFunction(converter) ? converter(value) : value;
	}

	var config = $.cookie = function (key, value, options) {

		// Write

		if (arguments.length > 1 && !$.isFunction(value)) {
			options = $.extend({}, config.defaults, options);

			if (typeof options.expires === 'number') {
				var days = options.expires, t = options.expires = new Date();
				t.setMilliseconds(t.getMilliseconds() + days * 864e+5);
			}

			return (document.cookie = [
				encode(key), '=', stringifyCookieValue(value),
				options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
				options.path    ? '; path=' + options.path : '',
				options.domain  ? '; domain=' + options.domain : '',
				options.secure  ? '; secure' : ''
			].join(''));
		}

		// Read

		var result = key ? undefined : {},
			// To prevent the for loop in the first place assign an empty array
			// in case there are no cookies at all. Also prevents odd result when
			// calling $.cookie().
			cookies = document.cookie ? document.cookie.split('; ') : [],
			i = 0,
			l = cookies.length;

		for (; i < l; i++) {
			var parts = cookies[i].split('='),
				name = decode(parts.shift()),
				cookie = parts.join('=');

			if (key === name) {
				// If second argument (value) is a function it's a converter...
				result = read(cookie, value);
				break;
			}

			// Prevent storing a cookie that we couldn't decode.
			if (!key && (cookie = read(cookie)) !== undefined) {
				result[name] = cookie;
			}
		}

		return result;
	};

	config.defaults = {};

	$.removeCookie = function (key, options) {
		// Must not alter options, thus extending a fresh object...
		$.cookie(key, '', $.extend({}, options, { expires: -1 }));
		return !$.cookie(key);
	};

}));

/**
 * Copyright (C) 2012 Chris Wharton (chris@weare2ndfloor.com)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * THIS SOFTWARE AND DOCUMENTATION IS PROVIDED "AS IS," AND COPYRIGHT
 * HOLDERS MAKE NO REPRESENTATIONS OR WARRANTIES, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO, WARRANTIES OF MERCHANTABILITY OR
 * FITNESS FOR ANY PARTICULAR PURPOSE OR THAT THE USE OF THE SOFTWARE
 * OR DOCUMENTATION WILL NOT INFRINGE ANY THIRD PARTY PATENTS,
 * COPYRIGHTS, TRADEMARKS OR OTHER RIGHTS.COPYRIGHT HOLDERS WILL NOT
 * BE LIABLE FOR ANY DIRECT, INDIRECT, SPECIAL OR CONSEQUENTIAL
 * DAMAGES ARISING OUT OF ANY USE OF THE SOFTWARE OR DOCUMENTATION.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://gnu.org/licenses/>.
 
 Documentation available at http://cookiecuttr.com
 
 */

;(function ($) {
    $.cookieCuttr = function (options) {
        var defaults = {
            cookieCutter: false, // you'd like to enable the div/section/span etc. hide feature? change this to true
            cookieCutterDeclineOnly: false, // you'd like the CookieCutter to only hide when someone has clicked declined set this to true
            cookieAnalytics: true, // just using a simple analytics package? change this to true
            cookieDeclineButton: false, // this will disable non essential cookies
            cookieAcceptButton: true, // this will disable non essential cookies
            cookieResetButton: false,
            cookieOverlayEnabled: false, // don't want a discreet toolbar? Fine, set this to true
            cookiePolicyLink: '/privacy-policy/', // if applicable, enter the link to your privacy policy here...
            cookieMessage: 'We use cookies on this website, you can <a href="{{cookiePolicyLink}}" title="read about our cookies">read about them here</a>. To use the website as intended please...',
            cookieAnalyticsMessage: 'We use cookies, just to track visits to our website, we store no personal details.',
            cookieErrorMessage: "We\'re sorry, this feature places cookies in your browser and has been disabled. <br>To continue using this functionality, please",
            cookieWhatAreTheyLink: "http://www.allaboutcookies.org/",
            cookieDisable: '',
            cookieExpires: 365,
            cookieAcceptButtonText: "ACCEPT COOKIES",
            cookieDeclineButtonText: "DECLINE COOKIES",
            cookieResetButtonText: "RESET COOKIES FOR THIS WEBSITE",
            cookieWhatAreLinkText: "What are cookies?",
            cookieNotificationLocationBottom: false, // top or bottom - they are your only options, so true for bottom, false for top            
            cookiePolicyPage: false,
            cookiePolicyPageMessage: 'Please read the information below and then choose from the following options',
            cookieDiscreetLink: false,
            cookieDiscreetReset: false,
            cookieDiscreetLinkText: "Cookies?",
            cookieDiscreetPosition: "topleft", //options: topleft, topright, bottomleft, bottomright         
            cookieNoMessage: false, // change to true hide message from all pages apart from your policy page
            cookieDomain: "localhost",
            cookieWarningContainerStyleClass: "cc-cookies-banner"
        };
        var options = $.extend(defaults, options);
        var message = defaults.cookieMessage.replace('{{cookiePolicyLink}}', defaults.cookiePolicyLink);
        defaults.cookieMessage = 'We use cookies on this website, you can <a href="' + defaults.cookiePolicyLink + '" title="read about our cookies">read about them here</a>. To use the website as intended please...';
        //convert options
        var cookiePolicyLinkIn = options.cookiePolicyLink;
        var cookieCutter = options.cookieCutter;
        var cookieCutterDeclineOnly = options.cookieCutterDeclineOnly;
        var cookieAnalytics = options.cookieAnalytics;
        var cookieDeclineButton = options.cookieDeclineButton;
        var cookieAcceptButton = options.cookieAcceptButton;
        var cookieResetButton = options.cookieResetButton;
        var cookieOverlayEnabled = options.cookieOverlayEnabled;
        var cookiePolicyLink = options.cookiePolicyLink;
        var cookieMessage = message;
        var cookieAnalyticsMessage = options.cookieAnalyticsMessage;
        var cookieErrorMessage = options.cookieErrorMessage;
        var cookieDisable = options.cookieDisable;
        var cookieWhatAreTheyLink = options.cookieWhatAreTheyLink;
        var cookieExpires = options.cookieExpires;
        var cookieAcceptButtonText = options.cookieAcceptButtonText;
        var cookieDeclineButtonText = options.cookieDeclineButtonText;
        var cookieResetButtonText = options.cookieResetButtonText;
        var cookieWhatAreLinkText = options.cookieWhatAreLinkText;
        var cookieNotificationLocationBottom = options.cookieNotificationLocationBottom;
        var cookiePolicyPage = options.cookiePolicyPage;
        var cookiePolicyPageMessage = options.cookiePolicyPageMessage;
        var cookieDiscreetLink = options.cookieDiscreetLink;
        var cookieDiscreetReset = options.cookieDiscreetReset;
        var cookieDiscreetLinkText = options.cookieDiscreetLinkText;
        var cookieDiscreetPosition = options.cookieDiscreetPosition;
        var cookieNoMessage = options.cookieNoMessage;
        var cookieWarningContainerStyleClass = options.cookieWarningContainerStyleClass;
        // cookie identifier
        var $cookieAccepted = $.cookie('cc_cookie_accept') == "cc_cookie_accept";
        $.cookieAccepted = function () {
            return $cookieAccepted;
        };
        var $cookieDeclined = $.cookie('cc_cookie_decline') == "cc_cookie_decline";
        $.cookieDeclined = function () {
            return $cookieDeclined;
        };
        // write cookie accept button
        if (cookieAcceptButton) {
            var cookieAccept = ' <a href="#accept" class="cc-cookie-accept">' + cookieAcceptButtonText + '</a> ';
        } else {
            var cookieAccept = "";
        }
        // write cookie decline button
        if (cookieDeclineButton) {
            var cookieDecline = ' <a href="#decline" class="cc-cookie-decline">' + cookieDeclineButtonText + '</a> ';
        } else {
            var cookieDecline = "";
        }
        // write extra class for overlay
        if (cookieOverlayEnabled) {
            var cookieOverlay = 'cc-overlay';
        } else {
            var cookieOverlay = "";
        }
        // to prepend or append, that is the question?
        if ((cookieNotificationLocationBottom) || (cookieDiscreetPosition == "bottomright") || (cookieDiscreetPosition == "bottomleft")) {
            var appOrPre = true;
        } else {
            var appOrPre = false;
        }
        if (($cookieAccepted) || ($cookieDeclined)) {
            // write cookie reset button
            if ((cookieResetButton) && (cookieDiscreetReset)) {
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' cc-discreet"><a class="cc-cookie-reset" href="#" title="' + cookieResetButtonText + '">' + cookieResetButtonText + '</a></div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' cc-discreet"><a class="cc-cookie-reset" href="#" title="' + cookieResetButtonText + '">' + cookieResetButtonText + '</a></div>');
                }
                //add appropriate CSS depending on position chosen
                if (cookieDiscreetPosition == "topleft") {
                    $('div.cc-cookies').css("top", "0");
                    $('div.cc-cookies').css("left", "0");
                }
                if (cookieDiscreetPosition == "topright") {
                    $('div.cc-cookies').css("top", "0");
                    $('div.cc-cookies').css("right", "0");
                }
                if (cookieDiscreetPosition == "bottomleft") {
                    $('div.cc-cookies').css("bottom", "0");
                    $('div.cc-cookies').css("left", "0");
                }
                if (cookieDiscreetPosition == "bottomright") {
                    $('div.cc-cookies').css("bottom", "0");
                    $('div.cc-cookies').css("right", "0");
                }
            } else if (cookieResetButton) {
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + '"><a href="#" class="cc-cookie-reset">' + cookieResetButtonText + '</a></div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + '"><a href="#" class="cc-cookie-reset">' + cookieResetButtonText + '</a></div>');
                }
            } else {
                var cookieResetButton = "";
            }
        } else {
            // add message to just after opening body tag
            if ((cookieNoMessage) && (!cookiePolicyPage)) {
                // show no link on any pages APART from the policy page
            } else if ((cookieDiscreetLink) && (!cookiePolicyPage)) { // show discreet link
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' cc-discreet"><a href="' + cookiePolicyLinkIn + '" title="' + cookieDiscreetLinkText + '">' + cookieDiscreetLinkText + '</a></div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' cc-discreet"><a href="' + cookiePolicyLinkIn + '" title="' + cookieDiscreetLinkText + '">' + cookieDiscreetLinkText + '</a></div>');
                }
                //add appropriate CSS depending on position chosen
                if (cookieDiscreetPosition == "topleft") {
                    $('div.cc-cookies').css("top", "0");
                    $('div.cc-cookies').css("left", "0");
                }
                if (cookieDiscreetPosition == "topright") {
                    $('div.cc-cookies').css("top", "0");
                    $('div.cc-cookies').css("right", "0");
                }
                if (cookieDiscreetPosition == "bottomleft") {
                    $('div.cc-cookies').css("bottom", "0");
                    $('div.cc-cookies').css("left", "0");
                }
                if (cookieDiscreetPosition == "bottomright") {
                    $('div.cc-cookies').css("bottom", "0");
                    $('div.cc-cookies').css("right", "0");
                }
            } else if (cookieAnalytics) { // show analytics overlay
                cookieAnalyticsMessage = '<span class="cc-cookies-message">' + cookieAnalyticsMessage + '</span>';
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookieAnalyticsMessage + cookieAccept + cookieDecline + '<a href="' + cookieWhatAreTheyLink + '" title="Visit All about cookies (External link)">' + cookieWhatAreLinkText + '</a></div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookieAnalyticsMessage + cookieAccept + cookieDecline + '<a href="' + cookieWhatAreTheyLink + '" title="Visit All about cookies (External link)">' + cookieWhatAreLinkText + '</a></div>');
                }
            }
            if (cookiePolicyPage) { // show policy page overlay
                cookiePolicyPageMessage = '<span class="cc-cookies-message">' + cookiePolicyPageMessage + '</span>';
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookiePolicyPageMessage + " " + ' <a href="#accept" class="cc-cookie-accept">' + cookieAcceptButtonText + '</a> ' + ' <a href="#decline" class="cc-cookie-decline">' + cookieDeclineButtonText + '</a> ' + '</div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookiePolicyPageMessage + " " + ' <a href="#accept" class="cc-cookie-accept">' + cookieAcceptButtonText + '</a> ' + ' <a href="#decline" class="cc-cookie-decline">' + cookieDeclineButtonText + '</a> ' + '</div>');
                }
            } else if ((!cookieAnalytics) && (!cookieDiscreetLink)) { // show privacy policy option
                cookieMessage = '<span class="cc-cookies-message">' + cookieMessage + '</span>';
                if (appOrPre) {
                    $('body').append('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookieMessage + cookieAccept + cookieDecline + '</div>');
                } else {
                    $('body').prepend('<div class="cc-cookies ' + cookieWarningContainerStyleClass + ' ' + cookieOverlay + '">' + cookieMessage + cookieAccept + cookieDecline + '</div>');
                }
            }
        }
        if ((cookieCutter) && (!cookieCutterDeclineOnly) && (($cookieDeclined) || (!$cookieAccepted))) {
            $(cookieDisable).html('<div class="cc-cookies-error">' + cookieErrorMessage + ' <a href="#accept" class="cc-cookie-accept">' + cookieAcceptButtonText + '</a> ' + '</div>');
        }
        if ((cookieCutter) && (cookieCutterDeclineOnly) && ($cookieDeclined)) {
            $(cookieDisable).html('<div class="cc-cookies-error">' + cookieErrorMessage + ' <a href="#accept" class="cc-cookie-accept">' + cookieAcceptButtonText + '</a> ' + '</div>');
        }
        // if bottom is true, switch div to bottom if not in discreet mode
        if ((cookieNotificationLocationBottom) && (!cookieDiscreetLink)) {
            $('div.cc-cookies').css("top", "auto");
            $('div.cc-cookies').css("bottom", "0");
        }
        if ((cookieNotificationLocationBottom) && (cookieDiscreetLink) && (cookiePolicyPage)) {
            $('div.cc-cookies').css("top", "auto");
            $('div.cc-cookies').css("bottom", "0");
        }
        // setting the cookies

        // for top bar
        $('.cc-cookie-accept, .cc-cookie-decline').click(function (e) {
            e.preventDefault();
            if ($(this).is('[href$=\\#decline]')) {
                $.cookie("cc_cookie_accept", null, {
                    path: '/'
                });
                $.cookie("cc_cookie_decline", "cc_cookie_decline", {
                    expires: cookieExpires,
                    path: '/'
                });
                if (options.cookieDomain) {
                    // kill google analytics cookies
                    $.cookie("__utma", null, {
                        domain: '.' + options.cookieDomain,
                        path: '/'
                    });
                    $.cookie("__utmb", null, {
                        domain: '.' + options.cookieDomain,
                        path: '/'
                    });
                    $.cookie("__utmc", null, {
                        domain: '.' + options.cookieDomain,
                        path: '/'
                    });
                    $.cookie("__utmz", null, {
                        domain: '.' + options.cookieDomain,
                        path: '/'
                    });
                }
            } else {
                $.cookie("cc_cookie_decline", null, {
                    path: '/'
                });
                $.cookie("cc_cookie_accept", "cc_cookie_accept", {
                    expires: cookieExpires,
                    path: '/'
                });
            }
            $(".cc-cookies").fadeOut(function () {
                // reload page to activate cookies
                // removed in RM#8431
                // location.reload();
            });
            try {
                jQuery('.cc-cookies').dialog('close');
            } catch(err) {
            }

        });
        //reset cookies
        $('a.cc-cookie-reset').click(function (f) {
            f.preventDefault();
            $.cookie("cc_cookie_accept", null, {
                path: '/'
            });
            $.cookie("cc_cookie_decline", null, {
                path: '/'
            });
            $(".cc-cookies").fadeOut(function () {
                // reload page to activate cookies
                // removed in RM#8431
                // location.reload();
            });
        });
        //cookie error accept
        $('.cc-cookies-error a.cc-cookie-accept').click(function (g) {
            g.preventDefault();
            $.cookie("cc_cookie_accept", "cc_cookie_accept", {
                expires: cookieExpires,
                path: '/'
            });
            $.cookie("cc_cookie_decline", null, {
                path: '/'
            });
            jQuery('.cc-cookies').dialog('close');
            // reload page to activate cookies
            // removed in RM#8431
            // location.reload();
        });

    };
})(jQuery);
