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
                model.setLocalId(parseInt(form_.localid, 10));
                model.setNcesId(parseInt(form_.ncesid, 10));
                model.setSchoolType(form_.schooltype);
                model.setSchoolUrl(form_.schoolurl);
                model.setSendEmailNotifications(form_.sendemailnotifications != "false");
                model.setTimezoneId(form_.timezoneid);
            }
            return this.ShadeView(chlk.activities.AddSchoolDialog, ria.async.DeferredData(model));
        },
        VOID, function appsAction() {},
        VOID, function settingsAction() {},
        VOID, function signupsAction() {},
        VOID, function fundsAction() {}
    ])
});
