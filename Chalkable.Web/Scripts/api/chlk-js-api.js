/**
 * Created by Volodymyr on 1/19/2016.
 */

var CHLK_API = function (window, document, $) {

    if (typeof $ === 'undefined') {
        throw Error('CHLK_API requires jQuery')
    }


    function chlkRequestSiteRoot() {
        var result = "";
        var scripts = document.getElementsByTagName('script');

        for (var index = 0; index < scripts.length; index++) {
            var script = scripts[index];

            var url = script.src.toString().split('?')[0],
                regex = /chlk-js-api(\.min)?\.js$/i;

            if (url.match(regex)) {
                result = url.toLowerCase().replace(regex, '');
                break;
            }
        }

        switch (result) {
            case 'https://chalkable.com/':
                return 'https://classroom.chalkable.com/';
            case 'https://qa.chalkable.com/':
                return 'https://classroom.qa.chalkable.com/';
        }

        return result;
    }

    var ChlkActionTypes = {
        ADD_ME: 'addMe',
        CLOSE_ME : 'closeMe',
        SAVE_ME: 'saveMe',
        SHOW_PLUS: 'showPlus',
        APP_ERROR: 'appError',
        ON_BEFORE_CLOSE: 'addYourself',
        UPDATE_ORIGIN: 'updateOrigin',
        REQUEST_ORIGIN: 'requestOrigin',
        ON_CALLBACK: 'handleResponse',
        RESIZED: 'appResized',
        MODAL_ME: 'modalMe',
        UN_MODAL_ME: 'unModalMe',
        STANDARD_PICKER: 'showStandardPicker',
        ON_PAUSED: 'paused',
        ON_RESUME: 'resume',
        ALERT_BOX: 'showAlertBox',
        PROMPT_BOX: 'showPromptBox',
        CONFIRM_BOX: 'showConfirmBox'
    };

    function postMessage(data, rWindow, rURL){
        (rWindow || window.parent).postMessage(data, rURL || parentURL);
    }

    var cbs = {};

    function handleCallback(data) {
        var id = data.reqId;
        if (id && cbs[id] && (typeof cbs[id] === 'function')) {
            cbs[id](data);
            delete cbs[id];
        }
    }

    function postAction(data, action, rURL, cb){
        var res = data || {};
        res.action = action;
        res.isApp = true;
        res.reqId = (new Date()).getTime().toString(36) + Math.random().toString(36);
        cbs[res.reqId] = cb;

        postMessage(res, null, rURL);
    }

    var parentURL = chlkRequestSiteRoot();

    function updateOrigin(origin){
        parentURL = origin;
    }

    function requestOrigin(data){
        postAction(data, ChlkActionTypes.REQUEST_ORIGIN, "*");
    }



    $(function () {
        $(window)
            .on("message", function (e) {
                console.log(e, e.originalEvent);
                e = e.originalEvent;
                switch (e.data.action) {
                    case ChlkActionTypes.UPDATE_ORIGIN:
                        updateOrigin(e.origin);
                        break;
                    case ChlkActionTypes.ON_BEFORE_CLOSE:
                        handleOnBeforeClose(e.data);
                        break;
                    case ChlkActionTypes.ON_CALLBACK:
                        handleCallback(e.data);
                        break;
                }
            });

        requestOrigin({url: parentURL});


        ~function (elm, callback){
            var lastHeight = elm.clientHeight, newHeight;
            ~function run(){
                var notify = lastHeight !== (lastHeight = newHeight = elm.clientHeight);
                setTimeout(run, 200);
                notify && callback(newHeight);
            }();
        }(document.body, function(height) {
            postAction({height: height}, ChlkActionTypes.RESIZED, parentURL);
        });
    });

    var onBeforeAttachHandler = null;

    function handleOnBeforeClose(data) {
        if (onBeforeAttachHandler === null) {
            throw Error('onBeforeAttachHandler should be registered with CHLK_API.onBeforeAttachHandler() call');
        }

        var isAttach = !!data.attach;

        if (typeof onBeforeAttachHandler === 'function') {
            onBeforeAttachHandler(data, function (isReady) {
                data.appReady = isReady;
                postAction(data, isAttach ? ChlkActionTypes.ADD_ME : ChlkActionTypes.SAVE_ME, parentURL);
            });
        } else if (typeof onBeforeAttachHandler === 'boolean') {
            data.appReady = onBeforeAttachHandler;
            postAction(data, isAttach ? ChlkActionTypes.ADD_ME : ChlkActionTypes.SAVE_ME, parentURL);
        }
    }

    var obj = {
        onBeforeClose: function (cb) {
            if (typeof cb !== 'function') {
                throw TypeError('Argument cb is not a function');
            }

            onBeforeAttachHandler = cb;
        },

        onAfterPause: function (cb) {},
        onAfterResume: function (cb) {},

        setReady: function (isReady) {
            onBeforeAttachHandler = isReady;
        },

        closeMe: function (data) {
            postAction(data, ChlkActionTypes.CLOSE_ME, parentURL);
        },

        modalMe: function (data) {
            postAction(data, ChlkActionTypes.MODAL_ME, parentURL);
        },

        unModalMe: function (data) {
            postAction(data, ChlkActionTypes.UN_MODAL_ME, parentURL);
        },

        /* data: {} */
        showStandardPicker: function (data, cb) {
            postAction(data, ChlkActionTypes.STANDARD_PICKER, parentURL, cb);
        },

        /* data: {text: "", header: ""} */
        showAlertBox: function (data, cb) {
            if (typeof data === 'string')
                data = {text: data};

            postAction(data, ChlkActionTypes.ALERT_BOX, parentURL, cb);
        },

        /* data: {text: "", header: "", inputValue: "", inputAttrs: "", inputType: ""} */
        showPromptBox: function (data, cb) {
            if (typeof data === 'string')
                data = {text: data};

            postAction(data, ChlkActionTypes.PROMPT_BOX, parentURL, cb);
        },

        /* data: {text: "", header: "", buttonText: "", buttonClass: ""} */
        showConfirmBox: function (data, cb) {
            if (typeof data === 'string')
                data = {text: data, buttonText: 'OK'};

            postAction(data, ChlkActionTypes.CONFIRM_BOX, parentURL, cb);
        }
    };

    Object.freeze(obj);

    return obj;
}(window, document, jQuery);
