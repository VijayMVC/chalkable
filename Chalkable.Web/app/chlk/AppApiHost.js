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

        function $(){
            BASE();
            this.context_ = null;
        },

        [[ria.mvc.IContext]],
        function onStart(context) {
            this.context_ = context;
            CHLK_MESSENGER.addCallback(this.messengerCallback_);
        },

        function onStop() {
            CHLK_MESSENGER.removeCallback(this.messengerCallback_);
        },


        [[Object, String, Object]],
        function addApp(rWindow, rURL, data){
            CHLK_MESSENGER.addApp(rWindow, rURL, data);
        },

        [[Object, String, Object]],
        function closeApp(data){
            CHLK_MESSENGER.closeMe(data);
        },

        function messengerCallback_(e){
            if (e.data && e.data.url){
                var domain = chlkGetDomain(e.data.url);
                var rDomain = chlkGetDomain(WEB_SITE_ROOT);
                if (domain == rDomain){
                    if (e.data.action == 'requestOrigin'){
                        e.source.postMessage({action: 'updateOrigin'}, e.origin);
                    }
                }
            }
                    // TODO: check if following is secure
                    if (e.data.isApp) {
                        if (e.data.action) {
                            switch (e.data.action) {
                                case chlk.models.apps.AppActionTypes.ADD_ME.valueOf():
                                case chlk.models.apps.AppActionTypes.CLOSE_ME.valueOf():
                                case chlk.models.apps.AppActionTypes.SAVE_ME.valueOf():
                                case chlk.models.apps.AppActionTypes.SHOW_PLUS.valueOf():{
                                    this.doCallApiReactor_(e.data.action, e.data);
                                    break;
                                }

                            }
                        }
                    }
                    /*else{
                    throw new Exception('post-message-api domains differ. Please check your include path.');
                }*/
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