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
        [[Number, Number]],
        function updateListAction(districtId_, start_) {
            var result = this.schoolService
                .getSchools(districtId_, start_ || 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.school.SchoolsListPage, result);
        },

        [chlk.controllers.SidebarButton('schools')],
        [[Number, Number]],
        function listAction(districtId, pageIndex_) {
            var result = this.schoolService
                .getSchools(districtId, pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.school.SchoolsListPage, result);
        },

        [[chlk.models.school.School]],
        VOID, function importAction(model_) {
            var result = this.schoolService
                .getTimezones()
                .then(function(data){
                    var model = model_ || new chlk.models.school.School;
                    model.setTimezones(data.getItems());
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.school.SchoolsListPage, result);
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
                this.adminService.getUsers(id,null,null,null,0),
                this.accountService.getRoles()
            ]).then(function(result){
                var serializer = new ria.serialize.JsonSerializer();
                var model = new chlk.models.school.SchoolPeople();
                model.setSchoolInfo(result[0]);
                var usersModel = new chlk.models.school.SchoolPeoplePart();
                usersModel.setSelectedIndex(0);
                usersModel.setByLastName(true);
                usersModel.setUsers(result[1]);
                usersModel.setSchoolId(result[0].getId());
                model.setUsersPart(usersModel);
                var roles = result[2];
                newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.common.NameId));
                roles.unshift(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.common.NameId));
                model.setGradeLevels(newGradeLevels);
                model.setRoles(roles);
                return model;
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        },

        /*[[Number, Boolean]],
        function updatePeopleAction(id, byLastName) {
            var serializer = new ria.serialize.JsonSerializer();
            var model = new chlk.models.school.SchoolPeoplePart();
            model.setByLastName(byLastName);
            model.setSchoolId(id);
            this.forward_('schools', 'set-people-filter', [model]);
        },

        [[Number, Number, Number, Boolean, Boolean]],
        function setPeopleFilterAction(schoolId, roleId, gradeLevelId, byLastName,blockSelects){
            var result = ria.async.wait([
                this.adminService.getUsers(schoolId,roleId, gradeLevelId, byLastName,0)
            ]).then(function(result){
                var serializer = new ria.serialize.JsonSerializer();
                var model = new chlk.models.school.SchoolPeoplePart();
                model.setUsers(result[0]);
                model.setByLastName(byLastName);
                model.setSelectedIndex(0);
                model.setSchoolId(schoolId);
                return model;
            });
            return this.UpdateView(chlk.activities.school.SchoolPeoplePage, result);
        },*/

        [[chlk.models.school.SchoolPeoplePart]],
        VOID, function setPeopleFilterAction(model_) {
            var result = ria.async.wait([
                this.adminService.getUsers(model_.getSchoolId(),model_.getRoleId() || null, model_.getGradeLevelId() || null, model_.isByLastName(),0)
            ]).then(function(result){
                model_.setUsers(result[0]);
                model_.setSelectedIndex(0);
                return model_;
            });
            return this.UpdateView(chlk.activities.school.SchoolPeoplePage, result);
        },

        [[Number]],
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

        [[Object]],
        function actionLinkAction(form_){
            if(confirm(form_.index + ' ' + form_.email))
                this.context.getDefaultView().getCurrent().close();
        },

        [[Number]],
        VOID, function deleteAction(id){

        }
    ])
});
