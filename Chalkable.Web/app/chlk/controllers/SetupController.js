REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.activities.setup.TeacherSettingsPage');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.setup.TeacherSettings');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SetupController */
    CLASS(
        'SetupController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [ria.mvc.Inject],
            chlk.services.FinalGradeService, 'finalGradeService',

            [ria.mvc.Inject],
            chlk.services.CalendarService, 'calendarService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

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

            [[Number]],
            function teacherSettingsAction(index){
                var classes = this.classService.getClassesForTopBar();
                var classId = classes[index].getId();

                var result = ria.async.wait([
                    this.finalGradeService.getFinalGrades(classId, false),
                    this.calendarService.getTeacherClassWeek(classId)
                ]).then(function(result){
                    var model = new chlk.models.setup.TeacherSettings();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(true);
                    topModel.setSelectedItemId(classId);
                    model.setTopData(topModel);
                    model.setCalendarInfo(result[1]);
                    var gradesInfo = result[0].getFinalGradeAnnType();
                    gradesInfo.forEach(function(item, index){
                        item.setIndex(index);
                    });
                    gradesInfo.sort(function(a,b){
                        return b.getValue() > a.getValue();
                    });
                    if(index < classes.length - 1)
                        result[0].setNextClassNumber(index++);
                    model.setGradingInfo(result[0]);
                    return model;
                });
                return this.PushView(chlk.activities.setup.TeacherSettingsPage, result);
            },

            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
                this.teacherService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(),
                    model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), model.getFinalGradeAnnouncementTypeIds(),
                    model.getPercents(), model.getDropLowest(), model.getGradingStyleByType(), model.getNeedsTypesForClasses())
                    .then(function(model){
                        var index = model.getNextClassNumber();
                        if(index){
                            this.redirect_('setup', 'teacherSettings', [index]);
                        }

                    })
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
                                return this.redirect_('setup', 'teacherSettings', [0]);
                            }.bind(this));
                    }.bind(this));
                this.StartLoading(chlk.activities.setup.HelloPage);
            }
        ])
});
