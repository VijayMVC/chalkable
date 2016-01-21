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

    /** @class chlk.AppApiHost*/
    CLASS('AppApiHost', [
        // $$ - singleton instance factory
        function $$(instance, Clazz, ctor, args) {
            return singletonInstance || (singletonInstance = new ria.__API.init(instance, Clazz, ctor, args));
        },

        function $(){
            BASE();
            this.context_ = null;
        },

        [[ria.mvc.IContext]],
        function onStart(context) {
            this.context_ = context;

            jQuery(window)
                .on('message', this.messengerCallback_)
                .on('resize', function () {
                    ria.dom.Dom('iframe').valueOf()
                        .map(function (_) { return _.contentWindow})
                        .forEach(function (wnd) {
                            wnd.postMessage({action: 'updateZoom', zoom: zoomLevel()}, "*");
                        })
                });
        },

        function onStop() {
            jQuery(window).off('message', this.messengerCallback_);
        },


        [[Object, String, Object]],
        function addApp(rWindow, rURL, data){
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

        function messengerCallback_(e){
            e = e.originalEvent;

            var data = e.data,
                source = e.source,
                origin = e.origin;

            if (!data || !source || !origin || !data.action)
                return;

            switch (data.action) {
                case 'requestOrigin':
                    if (data.url && data.url.indexOf(window.location.origin) === 0) {
                        source.postMessage({action: 'updateOrigin', zoom: zoomLevel()}, origin);
                    }

                    break;

                case 'appResized':
                    var iframe = ria.dom.Dom('iframe').valueOf().filter(function (_) {return _.contentWindow === e.source}).shift();
                    if (iframe) {
                        var $iframe = ria.dom.Dom(iframe);
                        if (!$iframe.hasClass('fixed-height'))
                            $iframe.$.height(data.height + 10 + 'px');
                    }

                    break;

                default:
                    try {
                        var action = chlk.models.apps.AppActionTypes(data.action);
                        data.__origin = origin;
                        data.__source = source;
                        data.isApp && this.doCallApiReactor_(action.valueOf(), data);
                    } catch (ex) {}

                    break;
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
