REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.activities.profile.SchedulePage');


REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.UsersList');

REQUIRE('chlk.models.common.ActionLinkModel');
REQUIRE('chlk.models.people.UserProfileModel');


NAMESPACE('chlk.controllers', function (){
    "use strict";

    /** @class chlk.controllers.UserController */
    CLASS(
        'UserController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [ria.mvc.Inject],
            chlk.services.CalendarService, 'calendarService',


            [[chlk.models.common.PaginatedList, Number, Boolean, String]],
            chlk.models.people.UsersList, function prepareUsersModel(users, selectedIndex, byLastName, filter_){
                return new chlk.models.people.UsersList(this.prepareUsers(users, null), byLastName, selectedIndex, filter_);
            },

            [[chlk.models.common.PaginatedList, Number]],
            chlk.models.common.PaginatedList, function prepareUsers(usersData, start_){
                var start = start_ || 0;
                usersData.getItems().forEach(function(item, index){
                    item.setIndex(start_ + index);
                }.bind(this));
                return usersData;
            },

            [[chlk.models.people.User]],
            function prepareProfileData(model){
                var bDate = model.getBirthDate();
                var res = '';
                if(bDate){
                    var day = parseInt(bDate.format('d'), 10);
                    var str;
                    switch(day){
                        case 1: str = 'st';break;
                        case 2: str = 'n&#100;';break;
                        case 3: str = 'r&#100;';break;
                        default: str = 'st';
                    }
                    res = 'M d' + str + ' y';
                }

                var phones = model.getPhones();
                var addresses = model.getAddresses();
                var phonesValue = [];
                var addressesValue = [];

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

                var currentPerson = this.getCurrentPerson();
                var roleId = currentPerson.getRole().getId();

                //todo: rewrite
                var isAbleToEdit = (roleId != chlk.models.common.RoleEnum.STUDENT.valueOf() && model.getId() == currentPerson.getId())
                    || roleId == chlk.models.common.RoleEnum.ADMINEDIT.valueOf()
                    || roleId == chlk.models.common.RoleEnum.ADMINGRADE.valueOf();
                model.setAbleEdit(isAbleToEdit);
                if(bDate)
                    model.setBirthDateText(bDate.toString(res).replace(/&#100;/g, 'd'));

                var genderText = model.getGender() ? (model.getGender().toLowerCase() == 'm' ? 'Male' : 'Female') : '';
                model.setGenderFullText(genderText);
                return model;
            },

            ArrayOf(chlk.models.common.ActionLinkModel), function prepareActionLinksData_(user){
                return [];
            },

            chlk.models.people.UserProfileModel, function prepareUserProfileModel_(user){
                return new chlk.models.people.UserProfileModel(this.prepareProfileData(user)
                    , this.prepareActionLinksData_(user));
            },

            Boolean, function isAdminRoleName_(roleName){
                return roleName === chlk.models.common.RoleNamesEnum.ADMINGRADE.valueOf()
                    || roleName === chlk.models.common.RoleNamesEnum.ADMINEDIT.valueOf()
                    || roleName === chlk.models.common.RoleNamesEnum.ADMINVIEW.valueOf();
            },

            [[chlk.models.id.SchoolPersonId, String]],
            function infoByRoleAction(personId, roleName){
                var action = "info",
                    controller = null,
                    loweredRoleName = roleName.toLowerCase();
                if(this.isAdminRoleName_(loweredRoleName))
                    controller = "admins";
                if(loweredRoleName == chlk.models.common.RoleNamesEnum.TEACHER.valueOf())
                    controller = "teachers";
                if(loweredRoleName == chlk.models.common.RoleNamesEnum.STUDENT.valueOf())
                    controller = "students";
                return this.Redirect(controller, action, [personId.valueOf()]);
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String]],
            function scheduleByRole(personId, date_, role){
                var result = ria.async.wait([
                        this.personService.getSchedule(personId),
                        this.calendarService.getDayInfo(date_, personId)
                    ])
                    .attach(this.validateResponse_())
                    .then(function(results){
                        var schedule = results[0];
                        schedule.setRoleName(role);
                        return new chlk.models.people.SchedulePage(schedule, results[1]);
                    }.bind(this));
                return this.PushView(chlk.activities.profile.SchedulePage, result);
            },

            [[chlk.models.id.SchoolPersonId, Object]],
            function uploadPictureAction(personId, files){
                var result = this.personService
                    .uploadPicture(personId, files)
                    .then(function(loaded){
                        var res = this.getContext().getSession().get('userModel');
                        return new chlk.models.people.UserProfileModel(res, this.prepareActionLinksData_());
                    }.bind(this));
                return this.UpdateView(this.getInfoPageClass(), result);
            }
        ])
});
