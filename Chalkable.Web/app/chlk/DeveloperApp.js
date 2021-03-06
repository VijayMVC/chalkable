REQUIRE('chlk.BaseApp');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.DeveloperController');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.controllers.StandardController');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.controls.developer.AppsListControl');
REQUIRE('chlk.controls.developer.ApiCallListControl');

NAMESPACE('chlk', function (){

    /** @class chlk.DeveloperApp */
    CLASS(
        'DeveloperApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('apps');
                dispatcher.setDefaultControllerAction('general');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.DEVELOPER, 'Developer'));
                this.saveInSession(session, ChlkSessionConstants.CURRENT_CHLK_PERSON, chlk.models.developer.DeveloperInfo, ChlkSessionConstants.CURRENT_DEVELOPER);
                this.saveInSession(session, 'application', chlk.models.apps.Application, 'currentApp');


                var appId = session.get('currentApp').getId();
                session.set('currentAppId', appId);
                this.saveInSession(session, 'applications', ArrayOf(chlk.models.apps.Application), 'dev-apps');
                return session;
            },


            OVERRIDE, ria.async.Future, function onStart_() {


                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/DeveloperSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
