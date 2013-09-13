REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){
    "use strict";
    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            OVERRIDE,  ArrayOf(chlk.models.common.ActionLinkModel), function prepareActionLinksData_(){
                var controller = 'teachers';
                var actionLinksData = [];
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'details', 'Now', true));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'info', 'Info'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'schedule', 'Schedule'));
                actionLinksData.push(new chlk.models.common.ActionLinkModel(controller, 'apps', 'Apps'));
                return actionLinksData;
            },


            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.teacherService
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
                result = this.teacherService
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
                //return BASE(personId, chlk.models.common.RoleNamesEnum.TEACHER);
                return this.scheduleByRole(personId, date_, chlk.models.common.RoleNamesEnum.TEACHER.valueOf());
            }
        ])
});
