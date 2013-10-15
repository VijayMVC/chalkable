REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.SchoolService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.services.GradeLevelService');


REQUIRE('chlk.activities.school.SchoolDetailsPage');
REQUIRE('chlk.activities.school.SchoolPeoplePage');
REQUIRE('chlk.activities.school.ActionButtonsPopup');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.activities.school.SchoolSisPage');
REQUIRE('chlk.activities.school.SchoolsListPage');
REQUIRE('chlk.activities.school.ImportSchoolDialog');

REQUIRE('chlk.models.school.SchoolPeople');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.DistrictId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolImport.ImportTaskData');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SchoolsController */
    CLASS(
        'SchoolsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.SchoolService, 'schoolService',

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
                .getSchoolsForImport(districtId)
                .attach(this.validateResponse_())
                .then(function(data){
                    var schools = data.getItems();
                    return new ria.async.DeferredData(new chlk.models.schoolImport.SchoolImportViewData(districtId, schools));
                });
                return this.ShadeView(chlk.activities.school.ImportSchoolDialog, result);
        },

        [[chlk.models.schoolImport.ImportTaskData]],
        VOID, function importSchoolsAction(model) {
            var result = this.schoolService
                .runSchoolImport(
                    model.getDistrictId(),
                    model.getSisSchoolId(),
                    model.getSisSchoolYearId()
                )
                .attach(this.validateResponse_())
                .then(function(data){
                    if (data)
                        alert('Task was scheduled');
                });
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


        ArrayOf(chlk.models.common.NameId),  function convertRolesToNameIds_(roles){
            var res = [];
            for(var i =0; i < roles.length; i++){
                res.push(new chlk.models.common.NameId(roles[i].getRoleId().valueOf(), roles[i].getRole));
            }
            return res;
        },
        ArrayOf(chlk.models.common.NameId), function convertGradeLevelsToNameIds_(gradeLevels){
            var res = [];
            for(var i = 0; i < gradeLevels.length; i++){
                res.push(new chlk.models.common.NameId(gradeLevels[i].getId().valueOf(), gradeLevels[i].getName()));
            }
            return res;
        },

        //TODO:refactor
        [[chlk.models.id.SchoolId]],
        function peopleAction(id) {
            var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
            var newGradeLevels = this.convertGradeLevelsToNameIds_(this.gradeLevelService.getGradeLevels());
            newGradeLevels.unshift(serializer.deserialize({name: 'All Grades', id: null}, chlk.models.common.NameId));
            var roles =  this.convertRolesToNameIds_(this.accountService.getSchoolRoles());
            roles.push(serializer.deserialize({name: 'All Roles', id: null}, chlk.models.common.NameId));

            var result = ria.async.wait([
                this.schoolService.getPeopleSummary(id),
                this.schoolService.getUsers(id, null, null,0, null, false)
            ]).then(function(result){
                var users = new chlk.models.people.UsersList(result[1], true);
                return new chlk.models.school.SchoolPeople(users, roles, newGradeLevels, result[0]);
            });
            return this.PushView(chlk.activities.school.SchoolPeoplePage, result);
        },



        [[chlk.models.people.UsersList]],
        VOID, function setPeopleFilterAction(model_) {
//            var result = ria.async.wait([
//                this.schoolService.getUsers(
//                    model_.getSchoolId(),
//                    model_.getRoleId() || null,
//                    model_.getGradeLevelId() || null,
//                    model_.isByLastName(),
//                    0
//                )
//            ]).then(function(result){
//                model_.setUsers(result[0]);
//                model_.setSelectedIndex(0);
//                return model_;
//            });
//            return this.UpdateView(chlk.activities.school.SchoolPeoplePage, result);
        },



        //TODO:refactor
        [[chlk.models.id.SchoolId]],
        VOID, function actionButtonsAction(id) {
            var result =  this.schoolService
                .getDetails(id)
                .then(function(data){
                        return new chlk.models.school.ActionButtons(
                            data.getButtons(),
                            data.getEmails(),
                            chlk.controls.getActionLinkControlLastNode()
                        );
                })
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.school.ActionButtonsPopup, result);
        },

        [[Object]],
        function actionLinkAction(form_){
            if(confirm(form_.index + ' ' + form_.email))
                this.context.getDefaultView().getCurrent().close();
        },

        [[chlk.models.id.SchoolId, chlk.models.id.DistrictId]],
        VOID, function deleteAction(id, districtId){
                this.schoolService.del(id)
                    .then(function()
                        {
                            this.ShowMsgBox("School will be deleted", "School delete task is created");
                            this.pageAction(districtId);
                        }.bind(this));
        }
    ])
});
