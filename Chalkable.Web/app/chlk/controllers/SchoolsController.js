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
        [[chlk.models.district.DistrictId, Number]],
        function pageAction(districtId_, start_) {
            var result = this.schoolService
                .getSchools(districtId_, start_ || 0)
                .attach(this.validateResponse_())
                .then(function(data){
                    var districtId = null;
                    var items = data.getItems();
                    if (items && items.length > 0){
                        districtId = items[0].getDistrictId();
                    }
                    var schoolListViewData = new chlk.models.school.SchoolListViewData();
                    schoolListViewData.setDistrictId(districtId);
                    schoolListViewData.setItems(data);

                    return new ria.async.DeferredData(schoolListViewData);
                });
            return this.UpdateView(chlk.activities.school.SchoolsListPage, result);
        },

        [chlk.controllers.SidebarButton('schools')],
        [[chlk.models.district.DistrictId, Number]],
        function listAction(districtId, pageIndex_) {
            var result = this.schoolService
                .getSchools(districtId, pageIndex_ | 0,  false, false)
                .attach(this.validateResponse_())
                .then(function(data){
                    var districtId = null;
                    var items = data.getItems();
                    if (items && items.length > 0){
                        districtId = items[0].getDistrictId();
                    }
                    var schoolListViewData = new chlk.models.school.SchoolListViewData();
                    schoolListViewData.setDistrictId(districtId);
                    schoolListViewData.setItems(data);

                    return new ria.async.DeferredData(schoolListViewData);
                });
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.school.SchoolsListPage, result);
        },

        [[chlk.models.district.DistrictId]],
        VOID, function importAction(districtId) {
            var result = this.schoolService
                .getSchools(districtId)
                .attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.school.ImportSchoolDialog, result);
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
                model.setByLastName(true);
                model.setUsers(result[1]);
                var roles = result[2];
                newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.common.NameId));
                roles.unshift(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.common.NameId));
                model.setGradeLevels(newGradeLevels);
                model.setRoles(roles);
                model.setSelectedIndex(0);
                return model;
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        },

        [[Number, Boolean]],
        function updatePeopleAction(id, byLastName) {
            var result = ria.async.wait([
                this.adminService.getUsers(id,0)
            ]).then(function(result){
                var serializer = new ria.serialize.JsonSerializer();
                var model = new chlk.models.school.SchoolPeople();
                model.setUsers(result[1]);
                model.setByLastName(byLastName);
                return model;
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
