REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SetupController */
    CLASS(
        'SetupController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [[chlk.models.people.User]],
            function prepareProfileData(model){
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
                return model;
            },

            [[chlk.models.id.SchoolPersonId]],
            function helloAction(personId_){
                var result = this.teacherService
                    .getInfo(personId_ || this.getContext().getSession().get('currentPerson').getId())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.prepareProfileData(model);
                    }.bind(this));
                return this.PushView(chlk.activities.setup.HelloPage, result);
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                this.personService.changePassword(model.getId(), model.getPassword())
                    .then(function(changed){
                        this.teacherService
                            .updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(),
                                model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate())
                            .attach(this.validateResponse_())
                            .then(function(model){
                                this.StopLoading(chlk.activities.setup.HelloPage);
                                return this.redirect_('feed', 'list', []);
                            }.bind(this));
                    }.bind(this));
                this.StartLoading(chlk.activities.setup.HelloPage);
            }
        ])
});
