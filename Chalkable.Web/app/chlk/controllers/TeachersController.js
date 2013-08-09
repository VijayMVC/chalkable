REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.activities.profile.InfoViewPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [[chlk.models.people.User]],
            function prepareProfileData(model){
                var bDate = model.getBirthDate(),res='';
                if(bDate){
                    var day = parseInt(bDate.format('d'), 10), str;
                    switch(day){
                        case 1: str = 'st';break;
                        case 2: str = 'n&#100;';break;
                        case 3: str = 'r&#100;';break;
                        default: str = 'st';
                    }
                    res = 'M d' + str + ' y';
                }

                var phones = model.getPhones(), addresses = model.getAddresses() ,phonesValue=[], addressesValue=[];
                phones.forEach(function(item){
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        isPrimary: item.isIsPrimary(),
                        value: item.getValue()
                    }
                    phonesValue.push(values);
                    if(item.isIsPrimary() && !model.getPrimaryPhone()){
                        model.setPrimaryPhone(item);
                    }else{
                        if(!model.getHomePhone())
                            model.setHomePhone(item);
                    }
                });

                addresses.forEach(function(item){
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        value: item.getValue()
                    }
                    addressesValue.push(values);
                });

                model.setPhonesValue(JSON.stringify(phonesValue));
                model.setAddressesValue(JSON.stringify(addressesValue));
                var roleName = model.getRole().getName(), roles = chlk.models.common.RoleEnum;
                var currentPerson = this.getContext().getSession().get('currentPerson');
                model.setAbleEdit((roleName != roles.STUDENT.valueOf() && model.getId() == currentPerson.getId()) || roleName == roles.ADMINEDIT.valueOf() || roleName == roles.ADMINGRADE.valueOf());
                bDate && model.setBirthDateText(bDate.toString(res).replace(/&#100;/g, 'd'));
                var gt = model.getGender() ? (model.getGender().toLowerCase() == 'm' ? 'Male' : 'Female') : '';
                model.setGenderFullText(gt);
                model.setPictureUrl(this.personService.getPictureURL(model.getId(), 128));
                return model;
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
                        console.info(loaded);
                        return this.getContext().getSession().get('userModel');
                    }.bind(this));
                return this.UpdateView(chlk.activities.profile.InfoViewPage, result);
            }
        ])
});
