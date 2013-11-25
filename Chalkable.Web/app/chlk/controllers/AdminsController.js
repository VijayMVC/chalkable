REQUIRE('chlk.controllers.UserController');

REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AccountService');

REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.activities.admin.PeoplePage');
REQUIRE('chlk.activities.profile.PersonProfileSummaryPage');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){
    "use strict";
    /** @class chlk.controllers.AdminController */
    CLASS(
        'AdminsController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.AdminService, 'adminService',

            [ria.mvc.Inject],
            chlk.services.GradeLevelService, 'gradeLevelService',

            [ria.mvc.Inject],
            chlk.services.AccountService, 'accountService',

            function getInfoPageClass(){
                return chlk.activities.profile.SchoolPersonInfoPage;
            },

            [chlk.controllers.SidebarButton('people')],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.ADMINGRADE,
                chlk.models.common.RoleEnum.ADMINEDIT,
                chlk.models.common.RoleEnum.ADMINVIEW
            ])],

            function peopleAction(){
                var gradeLevels = this.getContext().getSession().get('gradeLevels');
                var roles = this.accountService.getSchoolRoles();
                var res = this.adminService
                    .getUsers(null, null, null, true, 0)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var users = this.prepareUsersModel(data, 0, true);
                        return new chlk.models.admin.PersonsForAdmin(users, roles, gradeLevels);
                    }, this);
                return this.PushView(chlk.activities.admin.PeoplePage, res);
            },

            [[chlk.models.people.UsersList]],
            function updateListAction(model){
                var isScroll = model.isScroll(), start = model.getStart();
                var res = this.adminService
                    .getUsers(model.getFilter(), model.getRoleId(), model.getGradeLevelsIds(), model.isByLastName(), start)
                    .then(function(usersData){
                        if(isScroll)  return this.prepareUsers(usersData, start);
                        return this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter());
                }.bind(this));
                return this.UpdateView(chlk.activities.admin.PeoplePage, res, isScroll ? chlk.activities.lib.DontShowLoader() : '');
            },


            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var res = this.adminService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return new chlk.models.people.UserProfileSummaryViewData(this.getCurrentRole(), model);
                    }, this);
                return this.PushView(chlk.activities.profile.PersonProfileSummaryPage, res);
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.adminService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var userData = this.prepareProfileData(model);
                        var res = new chlk.models.people.UserProfileInfoViewData(this.getCurrentRole(), userData);
                        this.setUserToSession(res);
                        return res;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.SchoolPersonInfoPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                var res = this.infoEdit_(model, chlk.models.people.UserProfileInfoViewData);
                return this.UpdateView(chlk.activities.profile.SchoolPersonInfoPage, res);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function scheduleAction(personId, date_){
                return this.scheduleByRole(personId, date_, chlk.models.common.RoleNamesEnum.ADMINGRADE.valueOf());
            }
        ])
});
