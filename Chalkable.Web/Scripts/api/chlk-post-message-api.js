//todo: add enums

var chlkRequestSiteRoot = function () {
    var result = "";
    var scripts = document.getElementsByTagName('script');

    for (var index = 0; index < scripts.length; index++) {
        var script = scripts[index];

        if (script.src.toString().match(/chlk-post-message-api.js$/i)) {
            result = script.src.toString().toLowerCase().replace('scripts/api/chlk-post-message-api.js','');
           break;
        }
    }
    return result;
};


var chlkGetDomain = function(url){
    var result = "";
    if (url !== undefined){
        url = url.replace(/(:\d+)\//, '/');
        var parts = url.split("app");
        result = parts[0].replace(/(http[s]?:\/\/)?(www.)?/i, '');
    }
    return result;
}

var CHLK_MESSENGER = (function () {


    var ChlkActionTypes = {
        ADD_ME: 'addMe',
        CLOSE_ME : 'closeMe',
        SAVE_ME: 'saveMe',
        SHOW_PLUS: 'showPlus',
        APP_ERROR: 'appError',
        ADD_YOURSELF: 'addYourself',
        UPDATE_ORIGIN: 'updateOrigin',
        REQUEST_ORIGIN: 'requestOrigin'
    };
    var messenger = {
        parentURL: chlkRequestSiteRoot(),

        updated: false,
        closeMe : function(data){
            this.postAction(data, ChlkActionTypes.CLOSE_ME, this.parentURL);
        },

        requestOrigin: function(data){
            this.postAction(data, ChlkActionTypes.REQUEST_ORIGIN, "*");
        },

        updateOrigin: function(origin){
            this.parentURL = origin;
            this.updated = true;
            var res = {action: ChlkActionTypes.SHOW_PLUS, isApp : true};
            this.postMessage(res, null, this.parentURL);
        },

        postAction : function(data, action, rURL){
            var res = data || {};
            res.action = action;
            res.isApp = true;
            this.postMessage(res, null, rURL);
        },

        addMe : function(data){
            this.postAction(data, ChlkActionTypes.ADD_ME, this.parentURL);
        },

        saveMe : function(data){
            this.postAction(data, ChlkActionTypes.SAVE_ME, this.parentURL);
        },

        appError : function(data){
            this.postAction(data, ChlkActionTypes.APP_ERROR, this.parentURL);
        },

        addApp : function(rWindow, rURL, data){
            var res = data || {};
            res.action = ChlkActionTypes.ADD_YOURSELF;
            this.postMessage(res, rWindow, rURL);
        },

        showPlus : function(data){
        },

        addCallback : function(callback){
            if (document.addEventListener) {
                window.addEventListener("message", callback, false);
            } else if (document.attachEvent) {
                window.attachEvent("onmessage", callback);
            }
        },

        removeCallback : function(callback){
            if (document.removeEventListener) {
                window.removeEventListener("message", callback, false);
            } else if (document.detachEvent) {
                window.detachEvent("onmessage", callback);
            }
        },

        addYourself : function(fn){
            if (!this.updated) {
                this.requestOrigin({url:this.parentURL});
            }
            var that = this;
            function callback(e){
                if (e.data.action === ChlkActionTypes.ADD_YOURSELF) {
                    var attach = !!e.data.attach;
                    var result = fn(e.data);
                    var data = e.data;
                    data.appReady = result;
                    attach === true ? CHLK_MESSENGER.addMe(data)
                                    : CHLK_MESSENGER.saveMe(data);
                }
                if (e.data.action === ChlkActionTypes.UPDATE_ORIGIN){
                    that.updateOrigin(e.origin);
                }
            }

            if (document.addEventListener) {
                window.addEventListener("message", callback, false);
            } else if (document.attachEvent) {
                window.attachEvent("onmessage", callback);
            }
        },

        postMessage : function(data, rWindow, rURL){
            (rWindow || window.parent).postMessage(data, rURL || this.parentURL);
        }
    };

    return messenger;
})();

