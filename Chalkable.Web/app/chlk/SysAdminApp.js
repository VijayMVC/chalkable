REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.Dom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controllers.SchoolsController');
REQUIRE('chlk.controllers.AppsController');
REQUIRE('chlk.controllers.AppCategoriesController');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.BackgroundTaskController');
REQUIRE('chlk.controllers.SignUpController');
REQUIRE('chlk.controllers.StorageController');
REQUIRE('chlk.controllers.FundsController');
REQUIRE('chlk.controllers.DepartmentsController');

REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.SelectControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.PhotoContainerControl');

//REQUIRE('~/app/chlk/shared.js');

NAMESPACE('chlk', function (){

    /** @class chlk.SysAdminApp */
    CLASS(
        'SysAdminApp', EXTENDS(ria.mvc.Application), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('schools');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },
            OVERRIDE, ria.async.Future, function onStart_() {

                return BASE()
                    .then(function (data) {
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/SysAdmin.jade')())
                            .appendTo('#main');

                        return data;
                    }).then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/SysAdminSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
