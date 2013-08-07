REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.activities.profile.InfoViewPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [[chlk.models.people.User]],
            function prepareProfileData(model){
                var bDate = model.getBirthDate(),res='';
                if(bDate){
                    var day = parseInt(bDate.format('d'), 10), str;
                    switch(day){
                        case 1: str = 'st';break;
                        case 2: str = 'nd';break;
                        case 3: str = 'rd';break;
                        default: str = 'st';
                    }
                    res = 'M d' + str + ' y';
                }

                var phones = model.getPhones(), addresses = model.getAddresses() ,phonesValue=[], addressesValue=[];
                phones.forEach(function(item){
                    /*var values = [];
                    values.push(item.getId().valueOf());
                    values.push(item.getType());
                    values.push(item.isIsPrimary());
                    values.push(item.getValue());
                    phonesValue.push(values.join())*/
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
                model.setAbleEdit(roleName != roles.STUDENT.valueOf() || roleName == roles.ADMINEDIT.valueOf() || roleName == roles.ADMINGRADE.valueOf());
                model.setBirthDateText(res);
                var gt = model.getGender() ? (model.getGender().toLowerCase() == 'm' ? 'Male' : 'Female') : '';
                model.setGenderFullText(gt);
                return model;
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.teacherService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.prepareProfileData(model);
                    }.bind(this));
                return this.PushView(chlk.activities.profile.InfoViewPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                console.info(model);
                /*var result;
                if(personId_){
                    result = this.teacherService
                        .getInfo(personId_)
                        .attach(this.validateResponse_())
                        .then(function(model){
                            return this.prepareProfileData(model);
                        }.bind(this));
                }else{
                    result = new ria.async.DeferredData(model_);
                }
                return this.PushView(chlk.activities.profile.InfoEditPage, result);*/
            },

            [[chlk.models.id.SchoolPersonId, Object]],
            function uploadPictureAction(model){

            }
        ])
});
