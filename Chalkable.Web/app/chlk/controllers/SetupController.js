REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PreferenceService');

REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.activities.setup.VideoPage');
REQUIRE('chlk.activities.setup.StartPage');
REQUIRE('chlk.activities.setup.TeacherSettingsPage');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.setup.TeacherSettings');
REQUIRE('chlk.models.grading.Final');
REQUIRE('chlk.models.settings.Preference');

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

            [ria.mvc.Inject],
            chlk.services.PreferenceService, 'preferenceService',

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

            [[chlk.models.id.SchoolPersonId]],
            function videoAction(personId_){
                var result = this.preferenceService
                    .getPublic(chlk.models.settings.PreferenceEnum.VIDEO_GETTING_INFO_CHALKABLE.valueOf())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        model = model || new chlk.models.settings.Preference();
                        var classes = this.classService.getClassesForTopBar();
                        model.setType(classes.length);
                        return model;
                    }.bind(this));
                return this.PushView(chlk.activities.setup.VideoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function startAction(personId_){
                var model = chlk.models.settings.Preference();
                var classes = this.classService.getClassesForTopBar();
                model.setType(classes.length);
                var result =  ria.async.DeferredData(model);
                return this.PushView(chlk.activities.setup.StartPage, result);
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
                    var gradesInfo = result[0].getFinalGradeAnnType(), sum=0;
                    gradesInfo.forEach(function(item, index){
                        item.setIndex(index);
                        sum+=(item.getValue() || 0);
                    });
                    gradesInfo.sort(function(a,b){
                        return b.getValue() > a.getValue();
                    });
                    result[0].setNextClassNumber(++index);
                    this.getContext().getSession().set('settingsModel', result[0]);
                    sum+=(result[0].getAttendance() || 0);
                    sum+=(result[0].getParticipation() || 0);
                    sum+=(result[0].getDiscipline() || 0);
                    model.setPercentsSum(sum);
                    model.setGradingInfo(result[0]);
                    return model;
                    }.bind(this));
                return this.PushView(chlk.activities.setup.TeacherSettingsPage, result);
            },

            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
                this.finalGradeService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(),
                    model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), model.getFinalGradeAnnouncementTypeIds(),
                    model.getPercents(), model.getDropLowest(), model.getGradingStyleByType(), model.isNeedsTypesForClasses())
                    .then(function(model){
                        var index = model.getNextClassNumber();
                        if(index){
                            this.redirect_('setup', 'teacherSettings', [index]);
                        }

                    });
                this.StartLoading(chlk.activities.setup.TeacherSettingsPage);
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
                                return this.redirect_('setup', 'video', [model.getId().valueOf()]);
                            }.bind(this));
                    }.bind(this));
                this.StartLoading(chlk.activities.setup.HelloPage);
            }
        ])
});
