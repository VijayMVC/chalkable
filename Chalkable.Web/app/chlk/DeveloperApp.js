REQUIRE('chlk.BaseApp');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.controls.AppsListControl');



NAMESPACE('chlk', function (){

    /** @class chlk.TeacherApp */
    CLASS(
        'DeveloperApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('apps');
                dispatcher.setDefaultControllerAction('details');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                var serializer = new ria.serialize.JsonSerializer();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.DEVELOPER, 'Developer'));
                session.set('currentApp', serializer.deserialize(window.application || {}, chlk.models.apps.Application));
                return session;
            },

            [[chlk.models.apps.Application]],
            function switchApp(app){
                this.getContext().getSession().set('currentApp', app);
            },

            /*ArrayOf(chlk.models.apps.Application), function getApps(){
                var res = this.getContext().getSession();//.get('dev-apps') || [];
                console.log(res);
                return res;
            },*/

            chlk.models.apps.Application, function getCurrentApp(){
                var res = this.getContext().getSession().get('currentApp');
                return res;
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/DeveloperSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    })
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/demofooters/DeveloperDemoFooter.jade')(this))
                            .appendTo('#demo-footer');
                        return data;
                    }, this);
            }
        ]);
});
