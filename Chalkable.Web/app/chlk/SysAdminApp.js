REQUIRE('chlk.BaseApp');

REQUIRE('chlk.controllers.SchoolsController');
REQUIRE('chlk.controllers.DistrictController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.controllers.AppCategoriesController');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.BackgroundTaskController');
REQUIRE('chlk.controllers.SignUpController');
REQUIRE('chlk.controllers.StorageController');
REQUIRE('chlk.controllers.FundsController');
REQUIRE('chlk.controllers.DepartmentsController');
REQUIRE('chlk.controllers.DbMaintenanceController');

NAMESPACE('chlk', function (){

    /** @class chlk.SysAdminApp */
    CLASS(
        'SysAdminApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('district');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.SYSADMIN, 'SysAdmin'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/SysAdminSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
