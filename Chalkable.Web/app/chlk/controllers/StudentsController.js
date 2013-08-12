REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.StudentsController */
    CLASS(
        'StudentsController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [[chlk.models.id.ClassId]],
            function my(classId_){
                this.studentService.getStudents(classId_, null, false, sortType_, start_, count_);
            },

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
