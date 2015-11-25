/**
 * Created by Volodymyr on 11/19/2015.
 */

REQUIRE('ria.ajax.Task');

NAMESPACE('chlk.lib.ajax', function () {

    var document = _GLOBAL.document || null;

    function getCookie( name ) {
        var parts = document.cookie.split(name + "=");
        if (parts.length == 2) return parts.pop().split(";").shift();
    }

    /** @class chlk.lib.ajax.IframeGetTask */
    CLASS(
        'IframeGetTask', EXTENDS(ria.async.Task), [
            [[String, Object]],
            function $(url, params_) {
                BASE();

                this._url = url;
                this._params = params_ || {};
                this._checkReadyCookie = null;

                this._iframe = document.createElement("IFRAME");

                this._iframe.addEventListener("load", this.transferComplete_, false);
                this._iframe.addEventListener("error", this.transferFailed_, false);
                this._iframe.addEventListener("abort", this.transferCanceled_, false);
            },

            OVERRIDE, VOID, function cancel() {
                this._iframe.src = '';
            },

            [[Object]],
            SELF, function params(obj) {
                var p = this._params;
                for(var key in obj) if (obj.hasOwnProperty(key) && (obj[key] != undefined) && (obj[key] != null)) {
                    p[key] = obj[key];
                }
                return this;
            },

            [[String]],
            SELF, function disableCache(paramName_) {
                this._params[paramName_ || '_'] = Math.random().toString(36).substr(2) + (new Date).getTime().toString(36);
                return this;
            },

            [[String]],
            SELF, function checkReadyCookie(name_) {
                this._checkReadyCookie = name_ || null;
                return this;
            },

            [[Number]],
            SELF, function timeout(duration) {
                this._requestTimeout = duration;
                return this;
            },

            FINAL, String, function getParamsAsQueryString_() {
                var p = this._params, r = [];
                for(var key in p) if (p.hasOwnProperty(key)) {
                    r.push([key, p[key]].map(encodeURIComponent).join('='));
                }
                return r.join('&');
            },

            VOID, function updateProgress_(oEvent) {
                this._completer.progress(oEvent);
            },

            VOID, function transferComplete_(evt) {
                if (this._iframe.src ) {
                    var content = null, json = null;
                    try {
                        content = ria.dom.Dom(this._iframe).$.contents();
                        content = content[0].body.innerText;
                        json = JSON.parse(content);
                    } catch (e) {}
                    this._completer.complete(json || content);
                    ria.dom.Dom(this._iframe).removeSelf();
                }
            },

            VOID, function transferFailed_(evt) {
                this._completer.completeError(ria.ajax.ConnectionException());
                ria.dom.Dom(this._iframe).removeSelf();
            },

            VOID, function transferCanceled_(evt) {
                this._completer.cancel();
                ria.dom.Dom(this._iframe).removeSelf();
            },

            String, function getUrl_() {
                return this._url + ((/\?/).test(this._url) ? "&" : "?") + this.getParamsAsQueryString_();
            },

            // todo : was final
            OVERRIDE, VOID, function do_() {
                try {
                    BASE();

                    ria.dom.Dom(this._iframe).appendTo('body');

                    this._iframe.src = this.getUrl_();

                    if (this._checkReadyCookie) {
                        var currentValue = getCookie(this._checkReadyCookie);

                        ria.async.Timer(150, function (timer, lag) {
                            if (!this._completer.isCompleted() && (currentValue != getCookie(this._checkReadyCookie))) {
                                timer.cancel();
                                this._completer.complete({success: true, data: null});
                                ria.dom.Dom(this._iframe).removeSelf();
                            }
                        }, this);
                    }

                } catch (e) {
                    this._completer.completeError(e);
                }

                this._requestTimeout && new ria.async.Timer.$once(this._requestTimeout, this.timeoutHandler_);
            },

            [[ria.async.Timer, Number]],
            VOID, function timeoutHandler_(timer, lag) {
                this._completer.isCompleted() || this.cancel();
            }
        ]);
});
