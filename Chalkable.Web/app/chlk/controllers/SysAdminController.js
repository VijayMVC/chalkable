REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.SchoolsActivity');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SysAdminController */
    CLASS(
        'SysAdminController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        [chlk.controllers.SidebarButton('people')],
        [[Number]],
        function schoolsAction(pageIndex_) {
            var result = this.schoolService
                .getSchools(pageIndex_|0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.SchoolsActivity, result);
        },
        VOID, function addSchoolAction() {
            //return this.ShadeView(chlk.activities.BaseDialogActivity, {});
        },
        VOID, function appsAction() {},
        VOID, function settingsAction() {},
        VOID, function signupsAction() {},
        VOID, function fundsAction() {}
    ])
});
