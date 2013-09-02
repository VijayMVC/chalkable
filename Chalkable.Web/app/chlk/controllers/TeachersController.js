REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.activities.profile.InfoViewPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.UserController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.teacherService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var res = this.prepareProfileData(model);
                        this.getContext().getSession().set('userModel', res);
                        return res;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.InfoViewPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                var result;
                result = this.teacherService
                    .updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(),
                        model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.prepareProfileData(model);
                    }.bind(this));
                return this.UpdateView(chlk.activities.profile.InfoViewPage, result);
            },

            [[chlk.models.id.SchoolPersonId, Object]],
            function uploadPictureAction(personId, files){
                var result = this.personService
                    .uploadPicture(personId, files)
                    .then(function(loaded){
                        return this.getContext().getSession().get('userModel');
                    }.bind(this));
                return this.UpdateView(chlk.activities.profile.InfoViewPage, result);
            }
        ])
});
