REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.activities.profile.SchedulePage');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.UserController */
    CLASS(
        'UserController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [ria.mvc.Inject],
            chlk.services.CalendarService, 'calendarService',

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
                return model;
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String]],
            //function scheduleAction(personId, role){
            function scheduleByRole(personId, date_, role){
                var result = ria.async.wait([
                    this.personService.getSchedule(personId),
                    this.calendarService.getDayInfo(date_, personId)
                ]).attach(this.validateResponse_())
                    .then(function(results){
                        var schedule = results[0];
                        schedule.setRoleName(role);
                        var model = new chlk.models.people.SchedulePage(schedule, results[1]);
                        return model;
                    }.bind(this));
                return this.PushView(chlk.activities.profile.SchedulePage, result);
            }
        ])
});
