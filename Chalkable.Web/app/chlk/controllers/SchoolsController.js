REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');

REQUIRE('chlk.models.school.SchoolPeople');

REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.activities.school.ActionButtonsPopup');
REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.activities.school.SchoolSisPage');
REQUIRE('chlk.activities.school.SchoolsListPage');
REQUIRE('chlk.activities.school.AddSchoolDialog');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SchoolsController */
    CLASS(
        'SchoolsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

        [ria.mvc.Inject],
        chlk.services.AdminService, 'adminService',

        [ria.mvc.Inject],
        chlk.services.AccountService, 'accountService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',



        [chlk.controllers.SidebarButton('schools')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.schoolService
                .getSchools(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.school.SchoolsListPage, result);
        },

        VOID, function addSchoolAction(form_) {
            var result = ria.async.wait([
                this.schoolService.getTimezones()
            ]).
            then(function(data){
                var model = new chlk.models.school.School;
                if (form_){
                    model.setName(form_.name);
                    model.setLocalId(parseInt(form_.localid, 10));
                    model.setNcesId(parseInt(form_.ncesid, 10));
                    model.setSchoolType(form_.schooltype);
                    model.setSchoolUrl(form_.schoolurl);
                    model.setSendEmailNotifications(form_.sendemailnotifications != "false");
                    model.setTimezoneId(form_.timezoneId);
                }
                model.setTimezones(data[0].getItems());
                return model;
            });

            return this.ShadeView(chlk.activities.school.AddSchoolDialog, result);
        },

        [[Number]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        },
        [[Number]],
        function sisInfoAction(id) {
            var result = this.schoolService
                .getSisInfo(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolSisPage, result);
        },

        [[Number]],
        function peopleAction(id) {
            var newGradeLevels = this.gradeLevelService.getGradeLevels().slice();

            var result = ria.async.wait([
                this.schoolService.getPeopleSummary(id),
                this.adminService.getUsers(id,0),
                this.accountService.getRoles()
            ]).then(function(result){
                var serializer = new ria.serialize.JsonSerializer();
                var model = new chlk.models.school.SchoolPeople();
                model.setSchoolInfo(result[0]);
                model.setUsers(result[1].getItems());
                var roles = result[2];
                newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.common.NameId));
                roles.unshift(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.common.NameId));
                model.setGradeLevels(newGradeLevels);
                model.setRoles(roles);
                return model;
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        },

        VOID, function actionButtonsAction(id) {
            var result = ria.async.wait([
                this.schoolService.getDetails(id)
            ]).then(function(result){
                var model = new chlk.models.school.ActionButtons();
                model.setTarget(chlk.controls.getActionLinkControlLastNode());
                model.setButtons(result[0].getButtons());
                model.setEmails(result[0].getEmails());
                return model;
            });

            return this.ShadeView(chlk.activities.school.ActionButtonsPopup, result);
        },

        [[Number, String]],
        function actionLinkAction(index, email){

        },

        [[Number]],
        VOID, function deleteAction(id){

        }
    ])
});
