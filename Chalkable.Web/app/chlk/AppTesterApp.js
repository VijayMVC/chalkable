REQUIRE('chlk.BaseApp');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.controllers.StandardController');

NAMESPACE('chlk', function (){

    /** @class chlk.AppTesterApp */
    CLASS(
        'AppTesterApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('apps');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.APPTESTER, 'AppTester'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/AppTesterSideBar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
