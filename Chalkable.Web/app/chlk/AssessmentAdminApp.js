REQUIRE('chlk.BaseApp');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.controllers.StandardController');
REQUIRE('chlk.controllers.SettingsController');

NAMESPACE('chlk', function (){

    /** @class chlk.AssessmentAdminApp */
    CLASS(
        'AssessmentAdminApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('apps');
                dispatcher.setDefaultControllerAction('assessment-settings');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.ASSESSMENTADMIN, 'AssessmentAdmin'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/AssessmentAdminSideBar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
