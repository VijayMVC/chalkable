REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');

REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.models.district.District');

REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.activities.school.ActionButtonsPopup');
REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.activities.school.SchoolSisPage');
REQUIRE('chlk.activities.school.SchoolsListPage');
REQUIRE('chlk.activities.school.ImportSchoolDialog');
REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.id.SchoolId');

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
        [[chlk.models.id.DistrictId, Number]],
        function pageAction(districtId_, start_) {
            var result = this.schoolService
                .getSchools(districtId_, start_ || 0)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new ria.async.DeferredData(new chlk.models.school.SchoolListViewData(districtId, data));
                });
            return this.UpdateView(chlk.activities.school.SchoolsListPage, result);
        },

        [chlk.controllers.SidebarButton('schools')],
        [[chlk.models.id.DistrictId, Number]],
        function listAction(districtId, pageIndex_) {
            var result = this.schoolService
                .getSchools(districtId, pageIndex_ | 0,  false, false)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new ria.async.DeferredData(new chlk.models.school.SchoolListViewData(districtId, data));
                });
            return this.PushView(chlk.activities.school.SchoolsListPage, result);
        },

        [[chlk.models.id.DistrictId]],
        VOID, function importAction(districtId) {
            var result = this.schoolService
                .getSchools(districtId)
                .attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.school.ImportSchoolDialog, result);
        },

        [[chlk.models.id.SchoolId]],
        function detailsAction(id) {
            var result = this.schoolService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolDetailsPage, result);
        },
        [[chlk.models.id.SchoolId]],
        function sisInfoAction(id) {
            var result = this.schoolService
                .getSisInfo(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.school.SchoolSisPage, result);
        },

        [[chlk.models.id.SchoolId]],
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

        [[chlk.models.id.DistrictId]],
        VOID, function deleteAction(id){

        }
    ])
});
