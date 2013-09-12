REQUIRE('chlk.controllers.UserController');

REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AccountService');

REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.activities.admin.PeoplePage');


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


            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function prepareActionLinksData_(){
                var controller = 'admins';
                var actionLinksData = [];
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'details', 'Now', true));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'info', 'Info'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'apps', 'Apps'));
                return actionLinksData;
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
                    .getUsers(null, null, true, 0, null)
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
                    .getUsers(model.getRoleId(), model.getGradeLevelsIds(), model.isByLastName(), start, model.getFilter())
                    .then(function(usersData){
                        if(isScroll)  return this.prepareUsers(usersData, start);
                        return this.prepareUsersModel(usersData, 0, model.isByLastName(), model.getFilter());
                }.bind(this));
                return this.UpdateView(chlk.activities.admin.PeoplePage, res, isScroll ? window.noLoadingMsg : '');
            },


            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.adminService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                    var res = this.prepareUserProfileModel_(model);
                    this.getContext().getSession().set('userModel', res.getUser());
                    return res;
                }.bind(this));
                return this.PushView(chlk.activities.profile.SchoolPersonInfoPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                var result;
                result = this.adminService
                    .updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(),
                    model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate())
                    .attach(this.validateResponse_())
                    .then(function(model){
                    return this.prepareUserProfileModel_(model);
                }.bind(this));
                return this.UpdateView(chlk.activities.profile.SchoolPersonInfoPage, result);
            },

            [[chlk.models.id.SchoolPersonId, Object]],
            function uploadPictureAction(personId, files){
                var result = this.personService
                    .uploadPicture(personId, files)
                    .then(function(loaded){
                    var res = this.getContext().getSession().get('userModel');
                    return new chlk.models.people.UserProfileModel(res, this.prepareActionLinksData_());
                }.bind(this));
                return this.UpdateView(chlk.activities.profile.SchoolPersonInfoPage, result);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            function scheduleAction(personId, date_){
                return this.scheduleByRole(personId, date_, chlk.models.common.RoleNamesEnum.ADMINGRADE.valueOf());
            }
        ])
});
