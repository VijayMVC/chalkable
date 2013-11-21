REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.activities.profile.PersonProfileSummaryPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.UserProfileInfoViewData');

NAMESPACE('chlk.controllers', function (){
    "use strict";
    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            function getInfoPageClass(){
                return chlk.activities.profile.SchoolPersonInfoPage;
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var res = this.teacherService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return new chlk.models.people.UserProfileSummaryViewData(this.getCurrentRole(), model);
                    }, this);
                return this.PushView(chlk.activities.profile.PersonProfileSummaryPage, res);
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.teacherService
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
                //return BASE(personId, chlk.models.common.RoleNamesEnum.TEACHER);
                return this.scheduleByRole(personId, date_, chlk.models.common.RoleNamesEnum.TEACHER.valueOf());
            }
        ])
});
