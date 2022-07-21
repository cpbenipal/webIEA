//to compile needed https://marketplace.visualstudio.com/items?itemName=MadsKristensen.WebCompiler
//and https://marketplace.visualstudio.com/items?itemName=MadsKristensen.BundlerMinifier
//and recompile the file in the menu (right click)
//Web:Debug true to disable minimization in AppSettings

//EventManager.bus - realizes subscriptions. Do not change
//EventManager.init - settings, initialization and base class
//EventManager.service - ajax requests
//EventManager.tools - common methods
//_bundleEventManager - bundle Do not write it directly, but only rebuild WebCompiler. Otherwise, the changes will be overwritten.

//in method parameters, do not use a dynamic set of parameters,
//but use args as an object to pass parameters to the method

//args.requestUrl - url for ajax request
//args.ID and args.classBlock is required
//args.requestParameters the arguments for the controller

//do not call methods directly, use subscription technology

"use strict";

var EventManager = EventManager || {};
EventManager.bus = (function () {

    var channels = {};
    /**
    * creates an event subscription
    * Do not change!
    *  @param {string} channel name event
    *  @param {Function} fn function called after the event
    *  @returns {Function} subscribe context
    */
    var subscribe = function subscribe(channel, fn) {
        var _this = this;

        if (!channels[channel]) channels[channel] = [];
        if (fp_settings.debug == true) {
            //console.log("subscribe: " + channel);
        }
        if (!channels[channel].some(function (channel) {
            return channel.callback === fn && channel.context === _this;
        })) {
            channels[channel].push({
                context: this,
                callback: fn
            });
        }
        return this;
    },

    /**
    * creating an event
    * Do not change!
    * @param {string} channel name event
    * @returns {Function} subscribe context
    */
    publish = function publish(channel) {
        if (!channels[channel]) return false;
        var args = Array.prototype.slice.call(arguments, 1);
        for (var i = 0, l = channels[channel].length; i < l; i++) {
            var subscription = channels[channel][i];
            try {
                if (fp_settings.debug == true) {
                    console.log("publish: " + channel);
                }
                subscription.callback.apply(subscription.context, args);
            } catch (err) {
                EventManager.tools.FailedMessage("Event " + channel + " error: " + err, channels);
            }
        }
        return this;
    };

    return {
        publish: publish,
        subscribe: subscribe,
        installTo: function installTo(obj) {
            obj.subscribe = subscribe;
            obj.publish = publish;
        }
    };
})();

