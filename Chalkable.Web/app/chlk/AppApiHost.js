REQUIRE('ria.mvc.IContext');
REQUIRE('chlk.controllers.AppApiReactorController');
REQUIRE('chlk.models.apps.AppActionTypes');
REQUIRE('chlk.models.apps.AppModes');

NAMESPACE('chlk', function(){
    var singletonInstance = null;

    function chlkGetDomain(url) {
        return (url && url.replace(/(:\d+)\//, '/').split("app").shift().replace(/(http[s]?:\/\/)?(www.)?/i, '').split('/').shift()) || null;
    }

    function zoomLevel() {
        return parseFloat(ria.dom.Dom('html').$.css('zoom') || '1');
    }

    function getVisibleBound(frame) {
        var $wnd = jQuery(window),
            $frame = jQuery(frame),
            zoom = zoomLevel(),
            frm_pos = $frame.offset(),
            wnd_top = $wnd.scrollTop(),
            visible_height = window.innerHeight / zoom,
            top_bound = Math.max(wnd_top, frm_pos.top),
            btm_bound = Math.min(wnd_top + visible_height, frm_pos.top + $frame.height()),
            frm_visible_top = top_bound - frm_pos.top,
            frm_visible_height = btm_bound - top_bound;

        return {
            top: frm_visible_top,
            height: frm_visible_height
        };
    }

    function notifyIframeVisibleArea() {
        ria.dom.Dom('iframe').valueOf()
            .forEach(function (frame) {
                var visible = getVisibleBound(frame);

                if (frame.__visible_top != visible.top || frame.__visible_height != visible.height) {
                    frame.contentWindow.postMessage({
                        action: 'updateVisibleArea',
                        top: visible.top,
                        height: visible.height
                    }, "*");

                    frame.__visible_top = visible.top;
                    frame.__visible_height = visible.height;
                }
            });
    }

    var cbs = {};

    /** @class chlk.AppApiHost*/
    CLASS('AppApiHost', [
        // $$ - singleton instance factory
        function $$(instance, Clazz, ctor, args) {
            return singletonInstance || (singletonInstance = new ria.__API.init(instance, Clazz, ctor, args));
        },

        function $(){
            BASE();
            this.context_ = null;
            this.bodyHeightTimer_ = null;
        },

        [[ria.mvc.IContext]],
        function onStart(context) {
            this.context_ = context;

            var $wnd = jQuery(window)
                .on('message', this.messengerCallback_)
                .on('resize', function () {
                    ria.dom.Dom('iframe').valueOf()
                        .map(function (_) { return _.contentWindow})
                        .forEach(function (wnd) {
                            wnd.postMessage({action: 'updateZoom', zoom: zoomLevel()}, "*");
                        })
                })

                .on("scroll resize", notifyIframeVisibleArea);


            var that = this;
            +function (callback){
                var elm = document.body;
                var lastHeight = elm.clientHeight, newHeight;
                +function run(){
                    newHeight = elm.clientHeight;
                    if( lastHeight != newHeight )
                        callback();
                    lastHeight = newHeight;

                    if( that.bodyHeightTimer_ )
                        clearTimeout(that.bodyHeightTimer_);

                    that.bodyHeightTimer_ = setTimeout(run, 200);
                }();
            }(notifyIframeVisibleArea);
        },

        function onStop() {
            jQuery(window).off('message', this.messengerCallback_);
        },


        [[Object, String, Object]],
        function addApp(rWindow, rURL, data) {
            if (data.simpleApp && data.simpleApp == true){
                if (data.attach == false)
                    this.closeApp(data);
                else
                    this.doCallApiReactor_('simpleAppAttach', data);
            }
            else{
                var res = data || {};
                res.action = 'addYourself';
                rWindow.postMessage(res, rURL);

                this.doCallApiReactor_('addAppBegin', {rWindow: rWindow, rURL: rURL, data: data});
            }
        },

        [[Object, String, Object]],
        function closeApp(data){
            if (data.simpleApp && data.simpleApp == true){
                if (data.attach == false)
                    this.doCallApiReactor_('closeCurrentApp', data);
                else
                    this.doCallApiReactor_('closeMe', data);
            }
            else
                this.doCallApiReactor_('closeMe', data);
        },

        [[Object, String, Object]],
        function isAppReadyForClosing(rWindow, rURL, cb) {
            var res = {};
            res.action = 'isReadyForClosing';
            res.reqId = (new Date()).getTime().toString(36) + Math.random().toString(36) + '-irfc';
            cbs[res.reqId] = cb;

            rWindow.postMessage(res, rURL);
        },

        function messengerCallback_(e){
            e = e.originalEvent;

            var data = e.data,
                source = e.source,
                origin = e.origin,
                iframe = ria.dom.Dom('iframe').valueOf().filter(function (_) {return _.contentWindow === e.source}).shift();

            if (!data || !source || !origin || !data.action || !iframe)
                return;

            var $iframe = ria.dom.Dom(iframe);

            data.__source = source;
            data.__origin = origin;
            data.__iframe = $iframe;

            switch (data.action) {
                case 'requestOrigin':
                    if (data.url && data.url.indexOf(window.location.origin) === 0) {
                        source.postMessage({action: 'updateOrigin', zoom: zoomLevel()}, origin);

                        var visible = getVisibleBound(iframe);
                        source.postMessage({action: 'updateVisibleArea', top: visible.top, height: visible.height}, "*");
                    }

                    break;

                case 'appResized':
                    if (!$iframe.hasClass('fixed-height')) {
                        $iframe.$.height(data.height + 10 + 'px');
                    }

                    break;

                case 'userAction':
                    ria.dom.Dom().trigger(data.firedEvt);

                    break;

                case 'shadeMe':
                case 'popMe':
                    if ($iframe.$.parents('.app-wrapper-page').Dom().exists()) {
                        this.doCallApiReactor_(data.action, data);
                    }

                    break;

                case 'handleResponse':
                    var reqId = data.reqId;
                    if (cbs.hasOwnProperty(reqId)) {
                        cbs[reqId](data.value);
                        delete cbs[reqId];
                    }

                    break;

                default:
                    try {
                        var action = chlk.models.apps.AppActionTypes(data.action);
                        data.isApp && this.doCallApiReactor_(action.valueOf(), data);
                    } catch (ex) {}
            }
        },

        [[String, Object]],
        VOID, function doCallApiReactor_(message, data) {
            var state = this.context_.getState();
            state.setController('appapireactor');
            state.setAction(message);
            state.setParams([data]);
            state.setPublic(false);
            this.context_.stateUpdated();
        }
    ]);
});
