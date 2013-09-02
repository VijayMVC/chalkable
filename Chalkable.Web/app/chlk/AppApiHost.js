REQUIRE('ria.mvc.IContext');
REQUIRE('chlk.controllers.AppApiReactorController');
REQUIRE('chlk.models.apps.AppActionTypes');
REQUIRE('chlk.models.apps.AppModes');

NAMESPACE('chlk', function(){
    var singletonInstance = null;

    /** @class chlk.AppApiHost*/
    CLASS('AppApiHost', [
        // $$ - singleton instance factory
        function $$(instance, Clazz, ctor, args) {
            return singletonInstance || (singletonInstance = new ria.__API.init(instance, Clazz, ctor, args));
        },

        [[ria.mvc.IContext]],
        function onStart(context) {
            this.context_ = context;
            CHLK_MESSENGER.addCallback(this.messengerCallback_);
        },

        function onStop() {
            CHLK_MESSENGER.removeCallback(this.messengerCallback_);
        },

        function messengerCallback_(e){
            if (e.data && e.data.url){
                var domain = chlkGetDomain(e.data.url);
                var rDomain = chlkGetDomain(WEB_SITE_ROOT);
                if (domain == rDomain && e.data.action == 'requestOrigin'){
                    e.source.postMessage({action: 'updateOrigin'}, e.origin);
                }
            }

            // TODO: check if following is secure
            if (e.data.isApp) {
                if (e.data.action) {
                    switch (e.data.action) {
                        case ACTION_TYPES.ADD_ME :
                        case ACTION_TYPES.CLOSE_ME :
                        case ACTION_TYPES.SAVE_ME :
                        case ACTION_TYPES.SHOW_PLUS :
                            this.doCallApiReactor_(e.data.action, e.data);
                            break;
                    }
                }
            }
        },

        [[String, Object]],
        VOID, function doCallApiReactor_(message, data) {
            var state = this.context_.getState();
            state.setController('app-api-reactor');
            state.setAction(message);
            state.setParams([data]);
            state.setPublic(false);
            this.context_.stateUpdated();
        }
    ]);
});