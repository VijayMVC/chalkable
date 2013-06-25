REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.SchoolsActivity');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SysAdmin */
    CLASS(
        'SysAdminController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        function schoolsAction() {
            var result = this.schoolService
                .getSchools()
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.SchoolsActivity, result);
        },
        VOID, function addSchoolAction() {
        },
        VOID, function appsAction() {},
        VOID, function settingsAction() {},
        VOID, function signupsAction() {},
        VOID, function fundsAction() {}
    ])
});
