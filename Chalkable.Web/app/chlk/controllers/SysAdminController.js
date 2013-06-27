REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.SchoolsActivity');
REQUIRE('chlk.activities.AddSchoolDialog');


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
                .getSchools(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.SchoolsActivity, result);
        },
        VOID, function addSchoolAction(form_) {

            var model = new chlk.models.School;

            if (form_){
                model.setName(form_.name);
                model.setLocalid(parseInt(form_.localid, 10));
                model.setNcesid(parseInt(form_.ncesid, 10));
                model.setSchooltype(form_.schooltype);
                model.setSchoolurl(form_.schoolurl);
                model.setSendemailnotifications(!!form_.sendemailnotifications);
                model.setTimezoneid(form_.timezoneid);
            }
            return this.ShadeView(chlk.activities.AddSchoolDialog, ria.async.DeferredData(model));
        },
        VOID, function appsAction() {},
        VOID, function settingsAction() {},
        VOID, function signupsAction() {},
        VOID, function fundsAction() {}
    ])
});
